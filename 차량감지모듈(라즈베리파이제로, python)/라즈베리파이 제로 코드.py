import paho.mqtt.client as mqtt
import time
import RPi.GPIO as GPIO
import base64
import json
from picamera import PiCamera

trig_pin1 = 23
echo_pin1 = 24
trig_pin2 = 27
echo_pin2 = 17
trig_pin3 = 12
echo_pin3 = 16
led_blue1 = 5
led_blue2 = 6
led_blue3 = 13
led_red1 = 19
led_red2 = 26
led_red3 = 21
#GPIO.cleanup()
GPIO.setmode(GPIO.BCM)
#sonic sensor
GPIO.setup(trig_pin1, GPIO.OUT)
GPIO.setup(echo_pin1, GPIO.IN)

GPIO.setup(trig_pin2, GPIO.OUT)
GPIO.setup(echo_pin2, GPIO.IN)

GPIO.setup(trig_pin3, GPIO.OUT)
GPIO.setup(echo_pin3, GPIO.IN)
#led sensor
GPIO.setup(led_blue1, GPIO.OUT)
GPIO.setup(led_blue2, GPIO.OUT)
GPIO.setup(led_blue3, GPIO.OUT)
GPIO.setup(led_red1, GPIO.OUT)
GPIO.setup(led_red2, GPIO.OUT)
GPIO.setup(led_red3, GPIO.OUT)
#global variable
count = 0
count3 = 0
count2 = 0
count1 = 0
camera = PiCamera()
stack = 0

def convertImageToBase64(Image):
	with open(Image, "rb") as image_file:
		encoded = base64.b64encode(image_file.read())
		return encoded
		
import random, string

def randomword(length):
	return ''.join(random.choice(string.lowercase) for i in range(length))

import math

packet_size=3000
def publishEncodedImage(encoded):
	end = packet_size
	start = 0
	length = len(encoded)
	picId = randomword(8)
	pos = 0
	no_of_packets = math.ceil(length/packet_size)
	
	while start <= len(encoded):
		data = {"data": encoded[start:end], "pic_id":picId, "pos": pos, "size": no_of_packets}
		mqttc.publish("Image-Data",json.JSONEncoder().encode(data))
		end += packet_size
		start += packet_size
		pos = pos + 1
		
mqttc = mqtt.Client()
mqttc.connect("192.168.0.2")
mqttc.loop_start()		 
try:
	
	while True:
			# sonic distance1	
			GPIO.output(trig_pin3, False)
			time.sleep(0.5)
			GPIO.output(trig_pin3, True)
			time.sleep(0.00001)
			GPIO.output(trig_pin3, False)
			while GPIO.input(echo_pin3) == 0:
				pulse_start = time.time()

			while GPIO.input(echo_pin3) == 1:
				pulse_end = time.time()

			pulse_duration = pulse_end - pulse_start
			distance = pulse_duration * 34000 / 2
			distance = round(distance, 2)
			time.sleep(0.5)
			
				
			# parking check
			if distance > 10:
				GPIO.output(led_blue3,True)
				GPIO.output(led_red3,False)
				count1 = 0
				count = 0
				print("1 No Parking")
				mqttc.publish("parkinglot/1/status", "off",0)
						
			elif distance < 10:
				count = 1
			if count1 == 1:
				print("1 Parking")	
			
			if count >= 1 and count1 == 0 :
				GPIO.output(led_blue3,False)
				GPIO.output(led_red3,True)
				print("Car in 1 space")
				time.sleep(2)
			
				camera.start_preview()
				time.sleep(3)
				camera.capture("/home/pi/send1.jpg")
				camera.stop_preview()
				file = open("/home/pi/send1.jpg", "rb")         # open the file, note r = read, b = binary
				imagestring = file.read()                                            # read the file
				byteArray = bytes(imagestring)                                       # convert to byte string
				mqttc.publish(topic="parkinglot/image", payload= byteArray ,qos=0)
				count1 = 1
				count = 0
				
			# sonic distance2
			GPIO.output(trig_pin2, False)
			time.sleep(0.5)
			GPIO.output(trig_pin2, True)
			time.sleep(0.00001)
			GPIO.output(trig_pin2, False)
			while GPIO.input(echo_pin2) == 0:
				pulse_start = time.time()

			while GPIO.input(echo_pin2) == 1:
				pulse_end = time.time()

			pulse_duration = pulse_end - pulse_start
			distance = pulse_duration * 34000 / 2
			distance = round(distance, 2)
			time.sleep(0.5)
			# parking check
			if distance > 10:
				GPIO.output(led_blue2,True)
				GPIO.output(led_red2,False)
				count2 = 0
				count = 0
				print("2 No Parking")
				mqttc.publish("parkinglot/2/status", "off",0)
					
			elif distance < 10:
				count = 1
			if count2 == 1:
				print("2 Parking")	
			
			if count >= 1 and count2 == 0 :
				GPIO.output(led_blue2,False)
				GPIO.output(led_red2,True)
				print("Car in 2 space")
				time.sleep(2)
			
				camera.start_preview()
				time.sleep(3)
				camera.capture("/home/pi/send2.jpg")
				camera.stop_preview()
				file = open("/home/pi/send2.jpg", "rb")         # open the file, note r = read, b = binary
				imagestring = file.read()                                            # read the file
				byteArray = bytes(imagestring)                                       # convert to byte string
				mqttc.publish(topic="parkinglot/image", payload= byteArray ,qos=0)
				count2 = 1
				count = 0
				
			# sonic distance3	
			GPIO.output(trig_pin1, False)
			time.sleep(0.5)
			GPIO.output(trig_pin1, True)
			time.sleep(0.00001)
			GPIO.output(trig_pin1, False)
			while GPIO.input(echo_pin1) == 0:
				pulse_start = time.time()

			while GPIO.input(echo_pin1) == 1:
				pulse_end = time.time()

			pulse_duration = pulse_end - pulse_start
			distance = pulse_duration * 34000 / 2
			distance = round(distance, 2)
			time.sleep(0.5)
			# parking check
			if distance > 10:
				GPIO.output(led_blue1,True)
				GPIO.output(led_red1,False)
				count3 = 0
				count = 0
				print("3 No Parking")
				mqttc.publish("parkinglot/3/status", "off", 0)
						
			elif distance < 10:
				count = 1
			if count3 == 1:
				print("3 Parking")	
				
			if count >= 1 and count3 == 0 :
				GPIO.output(led_blue1,False)
				GPIO.output(led_red1,True)
				print("Car in 3 space")
				time.sleep(2)
			
				camera.start_preview()
				time.sleep(3)
				camera.capture("/home/pi/send3.jpg")
				camera.stop_preview()
				file = open("/home/pi/send3.jpg", "rb")         # open the file, note r = read, b = binary
				imagestring = file.read()                                            # read the file
				byteArray = bytes(imagestring)                                       # convert to byte string
				mqttc.publish(topic="parkinglot/image", payload= byteArray ,qos=0)
				count3 = 1
				count = 0
				
			
			
		
		
		

		
except KeyboardInterrupt:
	GPIO.cleanup()
mqttc.loop_stop()
mqttc.disconnect()

