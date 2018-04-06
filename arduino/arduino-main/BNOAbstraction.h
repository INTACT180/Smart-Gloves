#ifndef BNOAB
#define BNOAB

#include <bluefruit.h>
#include <Nffs.h>
#include <Adafruit_BNO055.h>
#include <Adafruit_Sensor.h>
#include <utility/imumaths.h>
#include <Arduino.h>

#define PERIOD 1.5f/100

enum exercise
{
  BICEP_CURL = 1,
  BENCH_PRESS = 2
};

struct xyz
{
  float x,y,z;
};

struct hpr
{
  float h,p,r;
};

struct xyz_int
{
  int x,y,z;
};

//High pass butterworth filter order=5 alpha1=0.0072 
class  MovingAvg
{
  public:
    MovingAvg()
    {
      for(int i=0; i <= 5; i++)
        v[i]=0.0;
    }
  private:
    float v[6];
  public:
    float step(float x) //class II 
    {
      v[0] = v[1];
      v[1] = v[2];
      v[2] = v[3];
      v[3] = v[4];
      v[4] = v[5];
      v[5] = x;
      return (v[5] + v[4] + v[3] + v[2] + v[1] + v[0])/6.0f;
    }
};

//High pass butterworth filter order=2 alpha1=0.72 
class  FilterBuHp2
{

  #define NZEROS 5
  #define NPOLES 5
  #define GAIN   1.037278059e+00
  public:
    FilterBuHp2()
    {
      for(int i =0; i< NZEROS+1; i++)
      {
        xv[i] = yv[i] = 0;
      }
    }
  private:
    float xv[NZEROS+1], yv[NPOLES+1];
  public:
    float step(float x) //class II 
    {
      xv[0] = xv[1]; xv[1] = xv[2]; xv[2] = xv[3]; xv[3] = xv[4]; xv[4] = xv[5]; 
        xv[5] = x/ GAIN;
        yv[0] = yv[1]; yv[1] = yv[2]; yv[2] = yv[3]; yv[3] = yv[4]; yv[4] = yv[5]; 
        yv[5] =   (xv[5] - xv[0]) + 5 * (xv[1] - xv[4]) + 10 * (xv[3] - xv[2])
                     + (  0.9294148707 * yv[0]) + ( -4.7151053641 * yv[1])
                     + (  9.5687692520 * yv[2]) + ( -9.7098810864 * yv[3])
                     + (  4.9268023221 * yv[4]);
        return yv[5];
    }
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
  hpr currentOrientation;

  xyz accelerationData;
  

  xyz preAccelerationData;
  xyz postAccelerationData;
  xyz preVelocityData;
  xyz postVelocityData;
  xyz prePositionData;
  xyz postPositionData;
  xyz_int counter;
  
  xyz preDeltaOrientationData;
  xyz postDeltaOrientationData;
  
  xyz preOrientationData;
  xyz postOrientationData;
  
  xyz orientationData;
  xyz accelerationTransferData;
  
  imu::Vector<3> dataSample;
  
  MovingAvg filterAcc_X =MovingAvg();
  MovingAvg filterAcc_Y =MovingAvg();
  MovingAvg filterAcc_Z =MovingAvg();

  FilterBuHp2 filterVel_X =FilterBuHp2();
  FilterBuHp2 filterVel_Y =FilterBuHp2();
  FilterBuHp2 filterVel_Z =FilterBuHp2();

  int iteration = 0;
  
  int repCount = -1;
  int rawRep = -1;
  bool movementDirection = 0; // 0 = down, 1 = up
  
  uint32_t ts0;
  uint32_t totalRunTime = 0;
  float period = 1.0f/100;

  bool calibrationRestored = false;

  void displaySensorOffsets(const adafruit_bno055_offsets_t &calibData);

  void displayCalStatus(void);

  bool restoreCalibrationValues();

  void writeCalibrationDataToFile(adafruit_bno055_offsets_t newCalib);

  xyz* calculatePosition();

  void calculateOrientation();
  
  public:

  bool begin();

  void update();

  void getEvent(sensors_event_t *evt);

  void getCurrentPosition(xyz *pos);

  void getOrientation(xyz *orientation);

  void getCurrentOrientation(hpr *orient);

  void getAcceleration(xyz *acc);

  void matLabDataOutput();

  void setupBuzzer();

  void loopBuzzer();

  void bicepCurl(hpr orientation, xyz acceleration, uint32_t runTime);

  void benchPress(hpr orientation, xyz acceleration, uint32_t runTime);
  
};

#endif 
