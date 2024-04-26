#include "WiFi.h"
const char* ssid      = "DESKTOP-VPV22OR 2244";
const char* password  = "17088(jE";
WiFiClient client;
void setup() {
  Serial.begin(115200);
  WiFi.begin(ssid, password);
  while (WiFi.status() != WL_CONNECTED) {
    delay(500);
    Serial.println("Connecting to WiFi..");
  }
    Serial.println("");
    Serial.println("WiFi connected");
    Serial.println("IP address: ");
    Serial.println(WiFi.localIP());
}

void SendRequest(){
  const uint16_t port = 63388; // port TCP server
    const char * host = "192.168.101.71"; // ip or dns
    Serial.print("Connecting to ");
    Serial.println(host);

    if (!client.connect(host, port)) {
        Serial.println("Connection failed.");
        Serial.println("Waiting 5 seconds before retrying...");
        delay(5000);
        return;
    }
    client.print("hello server im from esp");
  int maxloops = 0;
  
  while (!client.available() && maxloops < 1000)
  {
    maxloops++;
    delay(1); //delay 1 msec
  }
  if (client.available() > 0)
  {
    //read back one line from the server
    String line = client.readStringUntil('\r');
    Serial.println(line);
  }
  else
  {
    Serial.println("client.available() timed out ");
  }


    Serial.println("Waiting 5 seconds before restarting...");
    delay(5000);

}

void loop()
{
   SendRequest();
}