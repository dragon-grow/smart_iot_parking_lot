using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Tesseract;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.CPlusPlus;
using Alturos.Yolo;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Iot_Test
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private MqttClient client;              //mqtt 연결을 위한 클라이언트 객체
        private string clientId;                //mqtt 연결 아이디
        private YoloWrapper yoloWrapper;        //YOLO 객체
        private double min, max;                //영상을 이진화 할때 사용하는 변수
        private bool Lot1, Lot2, Lot3, yoloLodingStatus, brokerConnectedStatus;     //각 주차영역과 YOLO로딩 상태, 브로커 연결상태를 저장하는 변수
        private Thread yoloLodingThread;        //YOLO가 로딩되는 동안 프로그램을 사용하기 위해서 별도의 스레드로 동작하도록 하기 위한 스레드 변수

        public MainWindow()
        {
            InitializeComponent();

            Lot1 = false;       // 주차영역 상태를 초기화 합니다.
            Lot2 = false;
            Lot3 = false;
            yoloLodingStatus = false;       // 모듈들의 상태를 초기화 합니다.
            brokerConnectedStatus = false;
        }
        //=================================================================================================//
        //=======================================MQTT 관련 함수입니다.=====================================//
        //=================================================================================================//
        // 프로그램이 종료되면 MQTT 연결 해제
        protected override void OnClosed(EventArgs e)   
        {
            client.Disconnect();
            base.OnClosed(e);
            App.Current.Shutdown();
        }
        //=================================================================================================//
        // 구독 버튼을 눌렀을 때, 구독 요청을 전송
        private void btnSubscribe_Click(object sender, RoutedEventArgs e)       
        {
            if (!brokerConnectedStatus)
            {
                System.Windows.MessageBox.Show("Broker가 아직 연결되지 않았네요!");
                return;
            }

            client.Subscribe(new string[] { "parkinglot/image" }, new byte[] { 0 });

            if (txtTopicSubscribe.Text != "")       // 구독 토픽 텍스트박스에 내용이 있다면, 해당 토픽도 구독 요청한다.
            {                                   
                client.Subscribe(new string[] { txtTopicSubscribe.Text }, new byte[] { 2 });
                System.Windows.MessageBox.Show("[parkinglot/image], [" + txtTopicSubscribe.Text + "]가 구독요청 되었어요!");
            }
            else
            {
                System.Windows.MessageBox.Show("[parkinglot/image]가 구독요청 되었어요!");
            }

        }
        //=================================================================================================//
        private void btnPublish_Click(object sender, RoutedEventArgs e)         // 발행 버튼을 눌렀을 경우, 토픽과 내용을 브로커에게 전달한다.
        {
            if (!brokerConnectedStatus)
            {
                System.Windows.MessageBox.Show("Broker가 아직 연결되지 않았네요!");
                return;
            }
            if (txtTopicPublish.Text != "")
            {
                string Topic = txtTopicPublish.Text;
                
                lbPubLog.Items.Insert(0, string.Format("[{0}] {1}", DateTime.Now.ToString(), txtPublish.Text));                 // 발행한 내용을 리스트에 저장하여 UI에서 출력합니다.
                client.Publish(Topic, Encoding.UTF8.GetBytes(txtPublish.Text), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);       //qos 레벨은 2번.
            }
            else
                System.Windows.MessageBox.Show("You have to enter a topic to publish!");
        }
        //=================================================================================================//
        void client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)        // 구독요청한 내용이 도착하면 실행되는 함수
        {
            Dispatcher.Invoke(delegate
            {
                // 영상 이진화 min, max값, OCR라이브러리인 테서렉트가 텍스트를 잘 인식하기 위해 사용한 이진화 기법의 파라미터값을 텍스트박스로부터 가져온다.
                min = Convert.ToDouble(textBox.Text);       max = Convert.ToDouble(textBox1.Text);

                // 주차상태 라벨 상태 초기화
                recLot1.Content = "";                       recLot2.Content = "";                           recLot3.Content = "";
                recLot1.Background = new SolidColorBrush(Colors.GreenYellow);
                recLot2.Background = new SolidColorBrush(Colors.GreenYellow);
                recLot3.Background = new SolidColorBrush(Colors.GreenYellow);
            });
            Thread.Sleep(200);  //200ms 동안 정지.. UI 스레드의 동작이 완료되기를 기다리기 위함입니다.


            // 주차장 이미지가 들어왔을 경우의 동작입니다.
            else if (e.Topic == "parkinglot/image")
            {
                int moduleDelayTime = 200;
                long yoloTime = 0, tesserTime = 0;
                
                Stopwatch sw = new Stopwatch(), yoloTimeSw = new Stopwatch(), tesserTimeSw = new Stopwatch();
                sw.Start();
                Mat recive = Cv2.ImDecode(e.Message, LoadMode.Color);                                   // 해당 이미지를 OpenCV Mat 형식으로 디코딩해서 변수에 저장합니다.
                Mat recive2 = recive;
                
                recive.Rectangle(new OpenCvSharp.CPlusPlus.Rect(0, 0, 640, 1080), Scalar.Blue, 2);      // 받은 이미지에 주차장 구역을 그립니다. 입력받은 영상에서 가로로 3등분을 그림니다.
                recive.Rectangle(new OpenCvSharp.CPlusPlus.Rect(641, 0, 640, 1080), Scalar.Blue, 2);
                recive.Rectangle(new OpenCvSharp.CPlusPlus.Rect(1280, 0, 640, 1080), Scalar.Blue, 2);
                Cv2.ImWrite("pizero_color.jpg", recive);                                                      // 주차 구역이 그려진 영상을 폴더에 저장합니다. 향후 이용되지 않음. 디버깅용.
                
                Dispatcher.Invoke(delegate{ image.Source = ConvertMatToBitmap(recive); });              // 주차 구역이 그려진 영상을 표시합니다.

                //아래 3줄이 라즈베리파이제로에게서 받은 이미지를 3등분 하는 부분입니다.
                Mat lot_1 = new Mat(recive, new OpenCvSharp.CPlusPlus.Rect(0, 0, 640, 1080));           // 처음에 입력받은 recive영상을 3등분 하여 각 변수에 저장합니다. 왼쪽영상
                Mat lot_2 = new Mat(recive, new OpenCvSharp.CPlusPlus.Rect(641, 0, 640, 1080));         // 가운데 영상
                Mat lot_3 = new Mat(recive, new OpenCvSharp.CPlusPlus.Rect(1280, 0, 640, 1080));        // 오른쪽 영상

                //이미지에 텍스트를 출력합니다.
                recive.PutText("parkingLot 1", new OpenCvSharp.CPlusPlus.Point(40, 70), FontFace.HersheyComplex, 2, Scalar.Blue, 4, LineType.Link4, false);
                recive.PutText("parkingLot 2", new OpenCvSharp.CPlusPlus.Point(681, 70), FontFace.HersheyComplex, 2, Scalar.Blue, 4, LineType.Link4, false);
                recive.PutText("parkingLot 3", new OpenCvSharp.CPlusPlus.Point(1320, 70), FontFace.HersheyComplex, 2, Scalar.Blue, 4, LineType.Link4, false);
                
                
                Cv2.ImWrite("pizero.jpg", recive2);
                Cv2.CvtColor(recive2, recive2, ColorConversion.BgrToGray);
                //Cv2.Threshold(recive2, recive2, min, max, ThresholdType.Binary);
                Dispatcher.Invoke(delegate { image.Source = ConvertMatToBitmap(recive2); });              // 주차 구역이 그려진 영상을 표시합니다.

                yoloTimeSw.Start();
                //아래의 6줄은 YOLO가 번호판(번호x)의 영역을 검출합니다. 
                var items1 = yoloWrapper.Detect(lot_1.ToBytes());   // 주차 구역 왼쪽의 영상에서 번호판을 검출합니다.
                Console.WriteLine("1열 검출완료");
                
                var items2 = yoloWrapper.Detect(lot_2.ToBytes());   // 주차 구역 중앙의 영상에서 번호판을 검출합니다.
                Console.WriteLine("2열 검출완료");
                
                var items3 = yoloWrapper.Detect(lot_3.ToBytes());   // 주차 구역 오른쪽의 영상에서 번호판을 검출합니다.
                Console.WriteLine("3열 검출완료");
                yoloTimeSw.Stop();
                yoloTime = yoloTimeSw.ElapsedMilliseconds;

                //이미지를 바탕으로 차량이 있는지 없는지 확실하게 하기 위해서 해당 영역들을 초기화 합니다.
                client.Publish("parkinglot/1/status", Encoding.UTF8.GetBytes("off"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);       //qos 레벨은 2번.
                client.Publish("parkinglot/2/status", Encoding.UTF8.GetBytes("off"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);       //qos 레벨은 2번.
                client.Publish("parkinglot/3/status", Encoding.UTF8.GetBytes("off"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);       //qos 레벨은 2번.


                foreach (var item1 in items1)   // 주차 구역 왼쪽 영상에서 추출된 번호판에 대해 OCR 프로세싱 작업 수행
                {
                    Mat detect1 = new Mat(lot_1, new OpenCvSharp.CPlusPlus.Rect(item1.X, item1.Y, item1.Width, item1.Height));
                    Cv2.CvtColor(detect1, detect1, ColorConversion.BgrToGray);      // 문자를 인식하기 쉽도록 그레이 영상으로 변환합니다. 
                    Cv2.ImWrite("pizero_gray.jpg", detect1);
                    recive.Rectangle(new OpenCvSharp.CPlusPlus.Rect(item1.X, item1.Y, item1.Width, item1.Height), Scalar.Red, 3);   //번호판 주위에 사각형을 그립니다.
                    Dispatcher.Invoke(delegate
                    {
                        tesserTimeSw.Start();
                        var ocr = new TesseractEngine("./tessdata", "kor", EngineMode.Default);     // 테서렉트 로딩, 한국어 검출 버전
                        Cv2.Threshold(detect1, detect1, min, max, ThresholdType.Binary);            // 문자를 인식하기 쉽도록 영상 이진화
                        Cv2.ImWrite("pizero_threshold.jpg", detect1);
                        Page texts = ocr.Process(BitmapConverter.ToBitmap(detect1));                // 이진화된 영상을 테서렉트에게 전달 (실제 검출작업)
                        Console.WriteLine("1열 테서렉트 완료");

                        String num = texts.GetText();
                        recLot1.Content = num;

                        lbPubLog.Items.Insert(0, string.Format("[{0}] {1}", DateTime.Now.ToString(), num));
                        client.Publish("parkinglot/1/num", Encoding.UTF8.GetBytes(num), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);       //qos 레벨은 2번.
                        client.Publish("parkinglot/1/status", Encoding.UTF8.GetBytes("on"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);       //qos 레벨은 2번.
                        recLot1.Background = new SolidColorBrush(Colors.Red);                       // 라벨 색상을 빨강으로 변경
                        tesserTimeSw.Stop();
                        tesserTime += tesserTimeSw.ElapsedMilliseconds;
                    });
                }
                Thread.Sleep(moduleDelayTime);
                Dispatcher.Invoke(delegate { image.Source = ConvertMatToBitmap(recive); });
                foreach (var item2 in items2)   // 주차 구역 중앙 영상에서 추출된 번호판에 대해 OCR 프로세싱 작업 수행
                {
                    Mat detect2 = new Mat(lot_2, new OpenCvSharp.CPlusPlus.Rect(item2.X, item2.Y, item2.Width, item2.Height));
                    Cv2.CvtColor(detect2, detect2, ColorConversion.BgrToGray);      // 문자를 인식하기 쉽도록 그레이 영상으로 변환합니다. 

                    recive.Rectangle(new OpenCvSharp.CPlusPlus.Rect(item2.X+641, item2.Y, item2.Width, item2.Height), Scalar.Red, 3);
                    Dispatcher.Invoke(delegate
                    {
                        tesserTimeSw.Start();
                        var ocr = new TesseractEngine("./tessdata", "kor", EngineMode.Default);     // 테서렉트 로딩, 한국어 검출 버전
                        Cv2.Threshold(detect2, detect2, min, max, ThresholdType.Binary);            // 문자를 인식하기 쉽도록 영상 이진화
                        Page texts = ocr.Process(BitmapConverter.ToBitmap(detect2));                // 이진화된 영상을 테서렉트에게 전달 (실제 검출작업)
                        Console.WriteLine("2열 테서렉트 완료");

                        String num = texts.GetText();
                        recLot2.Content = num;

                        lbPubLog.Items.Insert(0, string.Format("[{0}] {1}", DateTime.Now.ToString(), num));
                        client.Publish("parkinglot/2/num", Encoding.UTF8.GetBytes(num), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);       //qos 레벨은 2번.
                        client.Publish("parkinglot/2/status", Encoding.UTF8.GetBytes("on"), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, true);       //qos 레벨은 2번.

                        recLot2.Background = new SolidColorBrush(Colors.Red);                       // 라벨 색상을 빨강으로 변경
                        tesserTimeSw.Stop();
                        tesserTime += tesserTimeSw.ElapsedMilliseconds;
                    });
                }
                Thread.Sleep(moduleDelayTime);
                Dispatcher.Invoke(delegate { image.Source = ConvertMatToBitmap(recive); });

                foreach (var item3 in items3)   // 주차 구역 오른쪽 영상에서 추출된 번호판에 대해 OCR 프로세싱 작업 수행
                {
                    Mat detect3 = new Mat(lot_3, new OpenCvSharp.CPlusPlus.Rect(item3.X, item3.Y, item3.Width, item3.Height));
                    Cv2.CvtColor(detect3, detect3, ColorConversion.BgrToGray);      // 문자를 인식하기 쉽도록 그레이 영상으로 변환합니다. 

                    recive.Rectangle(new OpenCvSharp.CPlusPlus.Rect(item3.X+1280, item3.Y, item3.Width, item3.Height), Scalar.Red, 3);
                    Dispatcher.Invoke(delegate
                    {
                        tesserTimeSw.Start();
                        var ocr = new TesseractEngine("./tessdata", "kor", EngineMode.Default);     // 테서렉트 로딩, 한국어 검출 버전
                        Cv2.Threshold(detect3, detect3, min, max, ThresholdType.Binary);            // 문자를 인식하기 쉽도록 영상 이진화
                        Page texts = ocr.Process(BitmapConverter.ToBitmap(detect3));                // 이진화된 영상을 테서렉트에게 전달 (실제 검출작업)
                        Console.WriteLine("3열 테서렉트 완료");

                        String num = texts.GetText();
                        recLot3.Content = num;

                        lbPubLog.Items.Insert(0, string.Format("[{0}] {1}", DateTime.Now.ToString(), num));
                        client.Publish("parkinglot/3/num", Encoding.UTF8.GetBytes(num), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);       //qos 레벨은 2번.
                        client.Publish("parkinglot/3/status", Encoding.UTF8.GetBytes("on"), MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, true);       //qos 레벨은 2번.

                        recLot3.Background = new SolidColorBrush(Colors.Red);                       // 라벨 색상을 빨강으로 변경
                        tesserTimeSw.Stop();
                        tesserTime += tesserTimeSw.ElapsedMilliseconds;
                    });
                }
                Thread.Sleep(moduleDelayTime);
                Dispatcher.Invoke(delegate { image.Source = ConvertMatToBitmap(recive); });
                sw.Stop();
                Console.WriteLine("검출하는데 총 " + sw.ElapsedMilliseconds + " ms 소요되었어요!");
                Console.WriteLine("그 중 YOLO는 총 " + yoloTime + " ms 소요되었어요!");
                Console.WriteLine("그 중 tesser는 총 " + tesserTime + " ms 소요되었어요!");
            }
        }
        //=================================================================================================//
        private void btnBrokerConnect_Click(object sender, RoutedEventArgs e)       // 브로커 연결 요청 버튼
        {
            if (!yoloLodingStatus)
            {
                System.Windows.MessageBox.Show("YOLO가 먼저 로딩되면 눌러주세요!");
                return;
            }
            string BrokerAddress = txtBrokerAddress.Text;

            client = new MqttClient(BrokerAddress);
            client.MqttMsgPublishReceived += client_MqttMsgPublishReceived;     // 구독요청한 토픽이 도착했을 때 실행될 함수 등록
            
            clientId = Guid.NewGuid().ToString();
            client.Connect(clientId);
            if(client.IsConnected)
                Dispatcher.Invoke(delegate {              // 브로커 연결된것이 확인되면 브로커 연결 상태 라벨 상태를 변경한다.
                    label_broker_state.Content = "Broker :  " + BrokerAddress;
                    label_broker_state.Background = new SolidColorBrush(Colors.YellowGreen);
                    brokerConnectedStatus = true;
                });

        }
        //=====================================================================================================//
        //=======================================MQTT END======================================================//
        //=====================================================================================================//

        //===================================IMAGE PROCESSING==================================================//
        public static System.Drawing.Bitmap BitmapFromWriteableBitmap(WriteableBitmap writeBmp)
        {
            System.Drawing.Bitmap bmp;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create((BitmapSource)writeBmp));
                enc.Save(outStream);
                bmp = new System.Drawing.Bitmap(outStream);
            }
            return bmp;
        }
        public static BitmapSource ConvertMatToBitmap(Mat image)    // mat형식을 비트맵 형식으로 변경해주는 함수
        {
            var bitmap = BitmapConverter.ToBitmap(image);
            IntPtr hBitmap = bitmap.GetHbitmap();

            System.Windows.Media.Imaging.BitmapSource bs =
                System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                  hBitmap, IntPtr.Zero, System.Windows.Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            //DeleteObject(hBitmap);
            bitmap.Dispose();
            return bs;
        }
        //=====================================================================================================//
        //=======================================IMAGE PROCESSING END==========================================//
        //=====================================================================================================//

        //===================================기타==============================================================//
        private void button_yolo_load_Click(object sender, RoutedEventArgs e)       // 욜로 로딩합니다....
        {
            Dispatcher.Invoke(delegate {
                label_yolo_state.Content = "YOLO   :  " + "Loading...";
                label_yolo_state.Background = new SolidColorBrush(Colors.LightBlue);
            });
            yoloLodingThread = new Thread(new ThreadStart(yoloLoding));
            yoloLodingThread.IsBackground = true;
            yoloLodingThread.Start();
        }
        private void yoloLoding()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            yoloWrapper = new YoloWrapper("plate.cfg", "plate_600.weights", "plate.names");     // 해당 파일이 번호판을 학습한 웨이트 파일입니다. bin/debug에 있습니다. 260mb 크기

            sw.Stop();
            Console.WriteLine("총 " + sw.ElapsedMilliseconds + " ms 소요되었어요!");
        }
        //=======================================기타 END================================================//
    }
}
