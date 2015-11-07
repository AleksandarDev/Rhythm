#include <DigiFi.h>
#include <string.h>

DigiFi wifi;
char *p, *i;
String path;
byte rgbIndex;
int rIndex;
int gIndex;
int bIndex;

void setup()
{
  Serial2.begin(9600);
  Serial.begin(115200); 
  wifi.begin(115200);

  //DigiX trick - since we are on serial over USB wait for character to be entered in serial terminal
//  while(!Serial.available()){
//    Serial.println("Enter any key to begin");
//    delay(1000);
//  }

  Serial.println("Starting");

  while (wifi.ready() != 1)
  {
    Serial.println("Error connecting to network");
    delay(15000);
  }  

  Serial.println("Connected to wifi!");
  Serial.print("Server running at: ");
  String address = wifi.server(8080);//sets up server and returns IP
  Serial.println(address); 

  

//  wifi.close();
}

void loop()
{

  if ( wifi.serverRequest()){
      Serial.print("Request for: ");
     Serial.println(wifi.serverRequestPath());
     path = wifi.serverRequestPath();
     if(path!="/")
     {
       // Red
       rIndex = path.indexOf('r') + 2;
       rIndex = path.substring(rIndex, rIndex + 3).toInt();
       //Serial.print("R=");
       //Serial.println(rIndex, HEX);
       // Green
       gIndex = path.indexOf('g') + 2;
       gIndex = path.substring(gIndex, gIndex + 3).toInt();
       //Serial.print("G=");
       //Serial.println(gIndex, HEX);
       // Blue
       bIndex = path.indexOf('b') + 2;
       bIndex = path.substring(bIndex, bIndex + 3).toInt();
       //Serial.print("B=");
       //Serial.println(bIndex, HEX);

       Serial2.write(rIndex);
       Serial2.write(gIndex);
       Serial2.write(bIndex);
       
       wifi.serverResponse("");
     }
     else
       wifi.serverResponse("<html><body><h1>Visit /rgb?r=rrr&g=ggg&b=bbb to set RGB color.</h1><div>rrr/ggg/bbb are DEC values of given color that can range from 0-255 inclusively.</div></body></html>"); //defaults to 200
  }

  delay(10);  
}

void processPost(const char* data) {
  // find where the parameters start
  const char * paramsPos = strchr (data, '?');
  if (paramsPos == NULL) return; // no parameters
}

