#include <ESP8266WiFi.h>
#include <ESP8266HTTPClient.h>
#include <WiFiClientSecureBearSSL.h>

const char* ssid = "Praya Labs";              // Replace with your Wi-Fi SSID
const char* password = "7kwe6f5cut";          // Replace with your Wi-Fi password

const char* apiKey = "ZIFUOQPNB64ON641";      // Replace with your ThingSpeak Write API Key
const char* readApiKey = "P6D2G7IILPE3934A";  // Replace with your ThingSpeak Read API Key
const char* server = "api.thingspeak.com";

int channelID = 2430285;                      // Replace with your ThingSpeak channel ID

// GPIO pin assignments for the ESP8266
#define D0_PIN D0
#define D1_PIN D1
#define D2_PIN D2

void setup() {
  Serial.begin(9600);

  // Set GPIO pins as OUTPUT
  pinMode(D0_PIN, OUTPUT);
  pinMode(D1_PIN, OUTPUT);
  pinMode(D2_PIN, OUTPUT);

  // Initialize pins to LOW
  digitalWrite(D0_PIN, LOW);
  digitalWrite(D1_PIN, LOW);
  digitalWrite(D2_PIN, LOW);

  // Connect to WiFi
  connectWiFi();
}

void loop() {
  // Read field1 value from ThingSpeak
  int field1Value = readThingSpeakField(1);

  Serial.print("Field1 Value from API: ");
  Serial.println(field1Value);

  // Perform actions based on field1 value
  if (field1Value == 1) {
    // D0 HIGH, others LOW
    Serial.println("D0 is HIGH, D1 and D2 are LOW");
    digitalWrite(D0_PIN, HIGH);
    digitalWrite(D1_PIN, LOW);
    digitalWrite(D2_PIN, LOW);
    updateThingSpeakStatus(1);  // Update ThingSpeak status
    delay(20000);               // Wait for 20 seconds

    // D2 HIGH, others LOW
    Serial.println("D2 is HIGH, D0 and D1 are LOW");
    digitalWrite(D0_PIN, LOW);
    digitalWrite(D1_PIN, LOW);
    digitalWrite(D2_PIN, HIGH);
    updateThingSpeakStatus(2);  // Update ThingSpeak status
    delay(20000);               // Wait for 20 seconds

    // D1 HIGH, others LOW
    Serial.println("D1 is HIGH, D0 and D2 are LOW");
    digitalWrite(D0_PIN, LOW);
    digitalWrite(D1_PIN, HIGH);
    digitalWrite(D2_PIN, LOW);
    updateThingSpeakStatus(3);  // Update ThingSpeak status
    delay(20000);               // Wait for 20 seconds
  } else {
    // If field1 is 0, turn off all the pins
    Serial.println("Field1 is 0, all pins are LOW");
    digitalWrite(D0_PIN, LOW);
    digitalWrite(D1_PIN, LOW);
    digitalWrite(D2_PIN, LOW);
    updateThingSpeakStatus(0);  // Update status to reflect all off
  }

  delay(5000); // Repeat every second
}

// Connect to the Wi-Fi network
void connectWiFi() {
  Serial.print("Connecting to WiFi...");
  WiFi.begin(ssid, password);

  while (WiFi.status() != WL_CONNECTED) {
    delay(1000);
    Serial.print(".");
  }
  Serial.println("\nConnected to WiFi");
}

// Read data from a specific ThingSpeak field
// Read data from a specific ThingSpeak field
int readThingSpeakField(int fieldNumber) {
  WiFiClient client;
  HTTPClient http;

  // Construct the URL to read data from ThingSpeak
  String url = "http://" + String(server) + "/channels/" + String(channelID) + "/fields/" + String(fieldNumber) + "/last.json?api_key=" + String(readApiKey);
  http.begin(client, url);

  // Send GET request to ThingSpeak
  int httpCode = http.GET();

  if (httpCode > 0) {  // Check for a successful response
    String payload = http.getString();
    Serial.println("Received payload: " + payload);

    // Find the field value
    String searchKey = "\"field" + String(fieldNumber) + "\":\"";
    int fieldValueIndex = payload.indexOf(searchKey) + searchKey.length();
    int endIndex = payload.indexOf("\"", fieldValueIndex);
    String fieldValue = payload.substring(fieldValueIndex, endIndex);

    Serial.print("Field ");
    Serial.print(fieldNumber);
    Serial.print(" Value: ");
    Serial.println(fieldValue);

    http.end();
    return fieldValue.toInt();  // Convert the field value to integer
  } else {
    // If the request fails, print the error
    Serial.print("Error reading ThingSpeak: ");
    Serial.println(http.errorToString(httpCode));
    http.end();
    return 0;  // Return 0 if the request fails
  }
}

// Update ThingSpeak with the current field values
void updateThingSpeakStatus(int currentState) {
  WiFiClient client;
  HTTPClient http;

  // Construct the URL to update ThingSpeak
  String url = "http://" + String(server) + "/update?api_key=" + String(apiKey) + "&field1=1&field2=" + String(currentState == 1 ? 1 : 0) + "&field3=" + String(currentState == 3 ? 1 : 0) + "&field4=" + String(currentState == 2 ? 1 : 0);
  Serial.println("Updating ThingSpeak with URL: ");
  Serial.println(url);

  http.begin(client, url);

  // Send GET request to update ThingSpeak
  int httpCode = http.GET();

  if (httpCode > 0) {
    Serial.println("ThingSpeak update successful");
  } else {
    Serial.print("Error updating ThingSpeak: ");
    Serial.println(http.errorToString(httpCode));
  }

  http.end();
}
