#ifndef BNOAB
#define BNOAB

#include <bluefruit.h>
#include <Nffs.h>
#include <Adafruit_BNO055.h>
#include <Adafruit_Sensor.h>
#include <utility/imumaths.h>



class BNOAbstraction
{
  private:

  //TODO: Decide if needed
  #define BNO055_SAMPLERATE_DELAY_MS (100)
  #define FILENAME    "/calibration.txt"

  Adafruit_BNO055 bno = Adafruit_BNO055(55);
  NffsFile file;
  sensors_event_t event;

  bool calibrationRestored = false;

  void displaySensorOffsets(const adafruit_bno055_offsets_t &calibData);

  void displayCalStatus(void);

  bool restoreCalibrationValues();

  void writeCalibrationDataToFile(adafruit_bno055_offsets_t newCalib);
  
  public:

  bool begin();

  bool getEvent(sensors_event_t *event);


  
};

#endif 