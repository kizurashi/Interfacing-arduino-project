#include <LiquidCrystal.h>

#include <dht.h>// DBT11 library

dht DHT;
#define DHT11_PIN 6
int xxx = 0;
int percentValue = 0;
// initialize the library with the numbers of the interface pins
LiquidCrystal lcd(12, 11, 5, 4, 3, 2); // Lcd pins
void setup() {

  Serial.begin( 9600 );
  pinMode(7,OUTPUT);
  lcd.begin(16, 2);
  

}

void loop() {
  int xx = DHT.read11(DHT11_PIN);  
  // Print a temp and humidity to the LCD.



  delay(1000);
  percentValue = analogRead(A0);
  int percent = convertToPercent(percentValue);
  lcd.print("Moisture = " );
  lcd.print(String(percent)+ "%");
  
  if( percent != 0  ) {
     Serial.println( String(percent) +","+ String(DHT.temperature) +","+String(DHT.humidity) );
  }
  //or (percent >= 35 AND percent <= 30)
   if ( percent == 0 ) {
     delay(500);
     Serial.println( String(percent) +","+ String(DHT.temperature) +","+String(DHT.humidity) );
     digitalWrite(7,HIGH);
     delay(500);// half second

     digitalWrite(7,LOW);
     delay(10000);// 10 seconds motor on
     digitalWrite(7,HIGH);

  } 

}

int convertToPercent ( int value ) {
    int percent = map ( value, 1023, 350, 0, 100 );
    if ( percent > 100 ) {
      percent = 100; 
    }
    return percent;
}
