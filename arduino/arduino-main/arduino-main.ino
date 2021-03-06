/*********************************************************************
 This is an example for our nRF52 based Bluefruit LE modules

 Pick one up today in the adafruit shop!

 Adafruit invests time and resources providing this open source code,
 please support Adafruit and open-source hardware by purchasing
 products from Adafruit!

 MIT license, check LICENSE for more information
 All text above, and the splash screen below must be included in
 any redistribution
*********************************************************************/
#include <bluefruit.h>
#include "BNOAbstraction.h"

// BLE Service
BLEDis  bledis;
BLEUart bleuart;
BLEBas  blebas;

BNOAbstraction bno;
//sensors_event_t event;
xyz orEvent;
xyz posEvent;
xyz accEvent;
uint8_t *ort[3];
uint8_t *acc[3];

uint16_t pastCheck;
int beep_timer = -1;

bool beeper_status = false;

// Software Timer for blinking RED LED
SoftwareTimer blinkTimer;

void setup()
{
  Serial.begin(115200);
  Serial.println("Bluefruit52 BLEUART Example");
  Serial.println("---------------------------\n");
  // Initialize blinkTimer for 1000 ms and start it
  blinkTimer.begin(1000, blink_timer_callback);
  blinkTimer.start();

  // Setup the BLE LED to be enabled on CONNECT
  // Note: This is actually the default behaviour, but provided
  // here in case you want to control this LED manually via PIN 19
  Bluefruit.autoConnLed(true);

  Bluefruit.begin();
  bno.begin();

  HwPWM0.addPin(A2);

  //while(true){beep();}
  
  Scheduler.startLoop(bnoThread);
  Scheduler.startLoop(beep);
  
  // Set max power. Accepted values are: -40, -30, -20, -16, -12, -8, -4, 0, 4
  Bluefruit.setTxPower(4);
  Bluefruit.setName("Bluefruit52");
  //Bluefruit.setName(getMcuUniqueID()); // useful testing with multiple central connections
  Bluefruit.setConnectCallback(connect_callback);
  Bluefruit.setDisconnectCallback(disconnect_callback);

  // Configure and Start Device Information Service
  bledis.setManufacturer("Adafruit Industries");
  bledis.setModel("Bluefruit Feather52");
  bledis.begin();

  // Configure and Start BLE Uart Service
  bleuart.begin();

  // Start BLE Battery Service
  blebas.begin();
  blebas.write(100);

  // Set up and start advertising
  startAdv();

  Serial.println("Please use Adafruit's Bluefruit LE app to connect in UART mode");
  Serial.println("Once connected, enter character(s) that you wish to send");
}

void startAdv(void)
{
  // Advertising packet
  Bluefruit.Advertising.addFlags(BLE_GAP_ADV_FLAGS_LE_ONLY_GENERAL_DISC_MODE);
  Bluefruit.Advertising.addTxPower();

  // Include bleuart 128-bit uuid
  Bluefruit.Advertising.addService(bleuart);

  // Secondary Scan Response packet (optional)
  // Since there is no room for 'Name' in Advertising packet
  Bluefruit.ScanResponse.addName();
  
  /* Start Advertising
   * - Enable auto advertising if disconnected
   * - Interval:  fast mode = 20 ms, slow mode = 152.5 ms
   * - Timeout for fast mode is 30 seconds
   * - Start(timeout) with timeout = 0 will advertise forever (until connected)
   * 
   * For recommended advertising interval
   * https://developer.apple.com/library/content/qa/qa1931/_index.html   
   */
  Bluefruit.Advertising.restartOnDisconnect(true);
  Bluefruit.Advertising.setInterval(32, 244);    // in unit of 0.625 ms
  Bluefruit.Advertising.setFastTimeout(30);      // number of seconds in fast mode
  Bluefruit.Advertising.start(0);                // 0 = Don't stop advertising after n seconds  
}

void loop()
{
  // Forward data from HW Serial to BLEUART

  // Delay to wait for enough input, since we have a limited transmission buffer
  delay(2);

  
  uint8_t buf[64];
  int count = 1;
  buf[0] = 'O';

  bno.getOrientation(&orEvent);
  bno.getCurrentPosition(&posEvent);
  bno.getAcceleration(&accEvent);
  

  uint16_t orientCast[3];

  orientCast[0] = (uint16_t)(orEvent.x * 100.0f);
  orientCast[1] = (uint16_t)(orEvent.y * 100.0f);
  orientCast[2] = (uint16_t)(orEvent.z * 100.0f);
  
  ort[0] = (uint8_t*) (&orientCast[0]);
  ort[1] = (uint8_t*) (&orientCast[1]);
  ort[2] = (uint8_t*) (&orientCast[2]);

  acc[0] = (uint8_t*) (&accEvent.x);
  acc[1] = (uint8_t*) (&accEvent.y);
  acc[2] = (uint8_t*) (&accEvent.z);
  
  for(int i=0; i< 3; i++)
  {
    for(int j=0; j<2;j++)
    {
      buf[count++] = ort[i][j];
    }
  }

//  buf[0] = 'P';
//
  for(int i=0; i< 3; i++)
  {
    for(int j=0; j<4;j++)
    {
      buf[count++] = acc[i][j];
    }
  }

//  Serial.print(buf[count-1]);
//
//  //int count = Serial.readBytes(buf, sizeof(buf));
//  Serial.print(" ");
//  Serial.println(count);

  bleuart.write( buf, count );

  // Forward from BLEUART to HW Serial
  while ( bleuart.available() )
  {
    uint8_t ch;
    ch = (uint8_t) bleuart.read();
    if( ch  == 'B')
      beep_timer = 300;
    //Serial.println(ch);
  }
//  Serial.println("bluetooth");
  // Request CPU to enter low-power mode until an event/interrupt occurs
  waitForEvent();
}

void connect_callback(uint16_t conn_handle)
{
  char central_name[32] = { 0 };
  Bluefruit.Gap.getPeerName(conn_handle, central_name, sizeof(central_name));

  Serial.print("Connected to ");
  Serial.println(central_name);
}

void disconnect_callback(uint16_t conn_handle, uint8_t reason)
{
  (void) conn_handle;
  (void) reason;

  Serial.println();
  Serial.println("Disconnected");
}

void bnoThread()
{
  bno.update();

   delay(8);
}

/**
 * Software Timer callback is invoked via a built-in FreeRTOS thread with
 * minimal stack size. Therefore it should be as simple as possible. If
 * a periodically heavy task is needed, please use Scheduler.startLoop() to
 * create a dedicated task for it.
 * 
 * More information http://www.freertos.org/RTOS-software-timer.html
 */
void blink_timer_callback(TimerHandle_t xTimerID)
{
  (void) xTimerID;
  digitalToggle(LED_RED);
}

/**
 * RTOS Idle callback is automatically invoked by FreeRTOS
 * when there are no active threads. E.g when loop() calls delay() and
 * there is no bluetooth or hw event. This is the ideal place to handle
 * background data.
 * 
 * NOTE: FreeRTOS is configured as tickless idle mode. After this callback
 * is executed, if there is time, freeRTOS kernel will go into low power mode.
 * Therefore waitForEvent() should not be called in this callback.
 * http://www.freertos.org/low-power-tickless-rtos.html
 * 
 * WARNING: This function MUST NOT call any blocking FreeRTOS API 
 * such as delay(), xSemaphoreTake() etc ... for more information
 * http://www.freertos.org/a00016.html
 */
void rtos_idle_callback(void)
{
  
}

void beep()
{
  uint16_t currentCheck = millis();
  uint16_t timeDelta = currentCheck - pastCheck;
  pastCheck = currentCheck;
  
  if(beep_timer > 0)
  {
    if(!beeper_status)
    {
      startBuzzer();
      beeper_status = true;
    }
    else
      beep_timer -= timeDelta;
    Serial.println(beep_timer);
  }

  if(beep_timer<=0)
  {
    if(beeper_status)
    {
      stopBuzzer();
      beeper_status = false;
    }
  }

  delay(50);
  
}

void startBuzzer()
{
  Serial.println("Starting Buzzer");
  HwPWM0.begin();
  HwPWM0.setResolution(15);
  HwPWM0.setClockDiv(PWM_PRESCALER_PRESCALER_DIV_1); // default : freq = 16Mhz
  HwPWM0.writePin( A2, 4000, false);
  
}

void stopBuzzer()
{
  Serial.println("Stopping Buzzer");
  HwPWM0.stop();
}


