   M S B A M L   `   `   `  ���� C  ?Iot_Test, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null#clr-namespace:Iot_TestIot_Test      Iot_Test.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     (localclr-namespace:Iot_Test       � Title$  
MainWindow��$	��520��$	��990��.��� .+��� -    ��Name$ groupBox��$��Parking Image��$
��Left=�$��	10,83,0,0q�$	��Top=�$	��388��$	��642��.���� -    ��Name$ image��$
��Left=�$	��353��$��4,10,0,0q�$	��Top=�$	��620���� -    ��Name$ 
tabControl��$
��Left=�$	��388��$��
657,83,0,0q�$	��Top=�$	��309��.@��� $��주차 상태��.��� ���	#FFE5E5E5.+��� ���	#FFF4F4F5$
��Left=�$	��110��$��	10,10,0,0q�
���   �$	��Top=�$	��283���� ���	#FFF4F4F5$
��Left=�$	��110��$��
10,127,0,0q�
���   �$	��Top=�$	��283���� ���	#FFF4F4F5$
��Left=�$	��110��$��
10,242,0,0q�
���   �$	��Top=�$	��283���� -    ��Name$ recLot1��$
��Null��$
��Left=�$��	10,10,0,0q�$	��Top=�$	��110��$	��283��$��KBIZgo M)�$��36(�$��Center=�$��Center=��� -   $ recLot2��$
��Null��$
��Left=�$��
10,127,0,0q�$	��Top=�$	��110��$	��283��$��Center=�$��Center=�$��36(�$��KBIZgo M)��� -   $ recLot3��$
��Null��$
��Left=�$��
10,242,0,0q�$	��Top=�$	��110��$	��283��$��Center=�$��Center=�$��KBIZgo M)�$��36(��� $��실행 설정��.��� ���	#FFE5E5E5.+��� -   $ label��$��구독 토픽��$
��Left=�$��	10,10,0,0q�$	��Top=��� -    ��Name$ txtTopicSubscribe��$
��Left=�$��26��$��	77,10,0,0q� �� TextWrapping$
 Wrap=�$�� ��$	��Top=�$��95���� -	    ��Name$ btnSubscribe��$��구독 버튼��$
��Left=�$��
177,10,0,0q�$	��Top=�$��74��$��26���� -
   $ label2��$��발행 토픽��$
��Left=�$��	10,41,0,0q�$	��Top=�$��62���� -   $ txtTopicPublish��$
��Left=�$��26��$��	77,41,0,0q�$
 Wrap=�$�� ��$	��Top=�$��95���� -   $ label3��$��발행 내용��$
��Left=�$��	10,72,0,0q�$	��Top=�$��62���� -   $ 
txtPublish��$
��Left=�$��26��$��	77,72,0,0q�$
 Wrap=�$�� ��$	��Top=�$��95���� -   $ 
btnPublish��$��발행 버튼��$
��Left=�$��
177,41,0,0q�$	��Top=�$��74��$��57���� -   $ label4��$��서버 주소��$
��Left=�$��
10,103,0,0q�$	��Top=�$��62���� -   $ txtBrokerAddress��$
��Left=�$��26��$��
77,103,0,0q�$
 Wrap=�$��192.168.0.33��$	��Top=�$��95��$��12(� �� TextAlignment$ Center=�$��Center=�$��Center=��� -   $ btnBrokerConnect��$��연결 버튼��$
��Left=�$��177,103,0,0q�$	��Top=�$��75��$��26���� -   $ button_yolo_load��$��	YOLO LOAD��$
��Left=�$��176,158,0,0q�$	��Top=�$��75���� -   $ textBox��$
��Left=�$��23��$��
30,158,0,0q�$
 Wrap=�$��70��$	��Top=�$��59��	 J� RenderTransformOrigin$	 0.5,0.5&��� -   $ textBox1��$
��Left=�$��23��$��103,158,0,0q�$
 Wrap=�$	��255��$	��Top=�$��58���� $��발행 기록��.��� ���	#FFE5E5E5.+��� -   
 ��Name$
 lbPubLog��$
��Left=�$	��360��$	��Top=�$	��303���� $��구독 기록��.��� ���	#FFE5E5E5.+��� -   $
 lbSubLog��$
��Left=�$	��360��$	��Top=�$	��303���� -   $ image_Title��$
��Left=�$��68��$��	10,10,0,0q�$	��Top=�$��66��$��icon.jpg���� -   $ label_Title��$(��"IoT Parking Lot Management Program��$
��Left=�$��	81,10,0,0q�$	��Top=�$��68��$	��475��$ ��NanumSquareRound ExtraBold)�$��24(�$��Center=�$��Center=��� -   $ label_broker_state��$��Broker :  not Connected��$
��Left=�$��
782,10,0,0q�$	��Top=�$	��184��$��KBIZgo M)����	#FFFFA9A9�� -   $ label_yolo_state��$��YOLO   :  not Loaded��$
��Left=�$��
782,42,0,0q�$	��Top=�$	��184��$��KBIZgo M)����	#FFFFA9A9