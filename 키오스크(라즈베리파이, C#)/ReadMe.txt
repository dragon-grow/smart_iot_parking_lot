구동 HW = 라즈베리파이

번호판 검출 프로그램이 전송해준 검출결과를 바탕으로, 사용자가 차량번호를 검색하면 차량의 위치를 표시해줍니다.
입력한 번호와 일치하는 차량이 없다면, 없다고 메시지박스가 출력됩니다.

통신 = MQTT, WiFi
사용기술 = C#, mqtt
실행방법 = piform.csproj를 통해 프로젝트를 오픈합니다.(pc에서 동착시킬경우), 라즈베리파이에서는 mono라는 프로그램이 있어야 C#기반 프로그램을 실행할 수 있습니다.
작성코드 = Form1.cs,Form2.cs
구독 토픽 = "parkinglot/1/status", "parkinglot/2/status", "parkinglot/3/status", "parkinglot/1/num", "parkinglot/2/num", "parkinglot/3/num"
발행 토픽 = 없음

동작

pc의 번호검출 프로그램이 전송한 값을 저장합니다.
사용자가 자량 번호를 입력하면, 해당 차량이 있는지 확인 후, 차량이 위치한 영역을 표시합니다.
차량이 없다면, 메시지박스로 없다고 표시합니다.

"parkinglot/1/status", "parkinglot/2/status", "parkinglot/3/status" 토픽은 라즈베리파이제로에서도 발행하기 때문에,
라즈베리파이제로에서 차량이 없다고 하면, 해당 차량의 객체는 초기화됩니다.