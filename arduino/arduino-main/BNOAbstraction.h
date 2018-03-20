#ifndef BNOAB
#define BNOAB

#include <bluefruit.h>
#include <Nffs.h>
#include <Adafruit_BNO055.h>
#include <Adafruit_Sensor.h>
#include <utility/imumaths.h>


#define PERIOD 1.5f/100

struct xyz
{
  float x,y,z;
};

struct xyz_int
{
  int x,y,z;
};



class BNOAbstraction
{
  private:

  //TODO: Decide if needed
  #define BNO055_SAMPLERATE_DELAY_MS (100)
  #define FILENAME    "/calibration.txt"

  Adafruit_BNO055 bno = Adafruit_BNO055(55);
  NffsFile file;
  sensors_event_t event;
  sensors_event_t eventTemp;

  xyz currentPosition;

  xyz preAccelerationData;
  xyz postAccelerationData;
  xyz preVelocityData;
  xyz postVelocityData;
  xyz prePositionData;
  xyz postPositionData;
  xyz_int counter;
  imu::Vector<3> dataSample;

  int iteration = 0;
  uint32_t ts0;
  float period = 1.0f/100;

  bool calibrationRestored = false;

  void displaySensorOffsets(const adafruit_bno055_offsets_t &calibData);

  void displayCalStatus(void);

  bool restoreCalibrationValues();

  void writeCalibrationDataToFile(adafruit_bno055_offsets_t newCalib);

  xyz* calculatePosition();
  
  public:

  bool begin();

  void update();

  void getEvent(sensors_event_t *evt);

  void getCurrentPosition(xyz *pos);


  
};

#endif 
