// 서버 구동에 필요한 리소스를 임포트 합니다.
var http = require('http');     
var express = require('express');
var app = express();
var path = require('path');
var static = require('serve-static');
var bodyParser = require('body-parser');
var router = express.Router();
var moment = require('moment');
const mqtt = require('mqtt')

const client = mqtt.connect('mqtt://192.168.0.2')       // MQTT 브로커와 연결합니다.

var state_lot1 = "off";                                 // 해당 구역의 주차 상태를 저장합니다. "on", "off"
var state_lot2 = "off";
var state_lot3 = "off";


client.on('connect', () => {                            // 브로커와 연결되면 실행되는 함수입니다.
  console.log('MQTT 서버에 연결합니다');
})
client.subscribe('parkinglot/1/status')                 // 해당 토픽을 구독합니다.
client.subscribe('parkinglot/2/status')
client.subscribe('parkinglot/3/status')

client.on('message', (topic, message) => {              // 메시지를 수신했을때 함수힙니다.
console.log('['+moment().format('YYYY-MM-DD HH:mm:ss')+'] -- ' +'MQTT 수신 : ' + topic + ', ' + message.toString());  //콘솔 로그에 받은 내용을 출력합니다.
  if(topic == 'parkinglot/1/status') {
    state_lot1 = message.toString();                    // 받은 내용을 상태변수에 저장합니다. "on", "off"
  }
  else if(topic == 'parkinglot/2/status') {
    state_lot2 = message.toString();
  }
  else if(topic == 'parkinglot/3/status') {
    state_lot3 = message.toString();
  }
})


app.set('port', process.env.PORT || 3000);              // 서버 접속 포트를 설정합니다.

router.route('/parkingMap').get(function(req, res){     // 해당 주소로 접속하면 html문서를 작성하여 보내줍니다.
    res.writeHead('200', {'Content-Type' : 'text/html;charset=utf8'});
    res.write('<h1>IoT Smart Parking Map!!</h1>');

    if(state_lot1 == "off"){    //해당영역에 차가 없다면, 그린박스를, 있다면 빨간 박스를 그려줍니다.
        res.write('<p style="border:1px solid; padding:10px; background:dodgerblue; float:left;"> ParkingLot 1 Empty </p>');
    }
    else {
        res.write('<p style="border:1px solid; padding:10px; background:red; float:left;"> ParkingLot 1 Use </p>');
    }
    
    if(state_lot2 == "off"){
        res.write('<p style="border:1px solid; padding:10px; background:dodgerblue; float:left;"> ParkingLot 2 Empty </p>');
    }
    else {
        res.write('<p style="border:1px solid; padding:10px; background:red; float:left;"> ParkingLot 2 Use </p>');
    }
    if(state_lot3 == "off"){
        res.write('<p style="border:1px solid; padding:10px; background:dodgerblue; float:left;"> ParkingLot 3 Empty </p>');
    }
    else {
        res.write('<p style="border:1px solid; padding:10px; background:red; float:left;"> ParkingLot 3 Use </p>');
    }
     

    res.end();
})

app.use('/',router);        
http.createServer(app).listen(app.get('port'), function() {             //서버 실행 함수입니다.
    console.log('익스프레스 서버를 시작했습니다 : ' + app.get('port'));
}); 



 
