#include "BNOAbstraction.h"

void BNOAbstraction::displaySensorOffsets(const adafruit_bno055_offsets_t &calibData)
{
  Serial.print("Accelerometer: ");
  Serial.print(calibData.accel_offset_x); Serial.print(" ");
  Serial.print(calibData.accel_offset_y); Serial.print(" ");
  Serial.print(calibData.accel_offset_z); Serial.print(" ");

  Serial.print("\nGyro: ");
  Serial.print(calibData.gyro_offset_x); Serial.print(" ");
  Serial.print(calibData.gyro_offset_y); Serial.print(" ");
  Serial.print(calibData.gyro_offset_z); Serial.print(" ");

  Serial.print("\nMag: ");
  Serial.print(calibData.mag_offset_x); Serial.print(" ");
  Serial.print(calibData.mag_offset_y); Serial.print(" ");
  Serial.print(calibData.mag_offset_z); Serial.print(" ");

  Serial.print("\nAccel Radius: ");
  Serial.print(calibData.accel_radius);

  Serial.print("\nMag Radius: ");
  Serial.print(calibData.mag_radius);
}

void BNOAbstraction::displayCalStatus(void)
{
  /* Get the four calibration values (0..3) */
  /* Any sensor data reporting 0 should be ignored, */
  /* 3 means 'fully calibrated" */
  uint8_t system, gyro, accel, mag;
  system = gyro = accel = mag = 0;
  bno.getCalibration(&system, &gyro, &accel, &mag);

  /* The data should be ignored until the system calibration is > 0 */
  Serial.print("\t");
  if (!system)
  {
      Serial.print("! ");
  }

  /* Display the individual values */
  Serial.print("Sys:");
  Serial.print(system, DEC);
  Serial.print(" G:");
  Serial.print(gyro, DEC);
  Serial.print(" A:");
  Serial.print(accel, DEC);
  Serial.print(" M:");
  Serial.print(mag, DEC);
}

bool BNOAbstraction::restoreCalibrationValues()
{
  file.open(FILENAME, FS_ACCESS_READ);
  if(!file.exists())
    return false;

  Serial.println(FILENAME " file exists");

  

  uint8_t data[22] = { 0 };
  
  file.read( data , sizeof(adafruit_bno055_offsets_t));
  
  adafruit_bno055_offsets_t calibrationData = * ((adafruit_bno055_offsets_t*) &data);
  displaySensorOffsets(calibrationData);
  Serial.println("\n\nRestoring Calibration data to the BNO055...");
  bno.setSensorOffsets(calibrationData);
  Serial.println("\n\nCalibration data loaded into BNO055");

}

void BNOAbstraction::writeCalibrationDataToFile(adafruit_bno055_offsets_t newCalib)
{
  Serial.print("Open " FILENAME " file to write ... ");

  if( file.open(FILENAME, FS_ACCESS_WRITE) )
  {
    Serial.println("OK");
    file.write((const uint8_t* ) &newCalib, sizeof(adafruit_bno055_offsets_t));
    Serial.println("Calibration values stored");
    file.close();
  }else
  {
    Serial.println("Failed (hint: path must start with '/') ");
    Serial.print("errnum = ");
    Serial.println(file.errnum);
  }
}

bool BNOAbstraction::begin()
{
  /* Initialize the sensor */
  if (!bno.begin())
  {
      /* There was a problem detecting the BNO055 ... check your connections */
      Serial.print("Ooops, no BNO055 detected ... Check your wiring or I2C ADDR!");
      while (1);
  }

  // begin using file system... must be called after Bluefruit.begin();
  Nffs.begin();

  // check if and then restore calibration values
  if(restoreCalibrationValues())
  {
    calibrationRestored = true;
  }

  bno.setExtCrystalUse(false);

  if(!calibrationRestored)
  {
    Serial.println("Please Calibrate Sensor: ");
    while (!bno.isFullyCalibrated())
    {
      bno.getEvent(&event);

      Serial.print("X: ");
      Serial.print(event.orientation.x, 4);
      Serial.print("\tY: ");
      Serial.print(event.orientation.y, 4);
      Serial.print("\tZ: ");
      Serial.print(event.orientation.z, 4);

      /* Optional: Display calibration status */
      displayCalStatus();

      /* New line for the next sample */
      Serial.println("");

      /* Wait the specified delay before requesting new data */
      delay(BNO055_SAMPLERATE_DELAY_MS);
    }
    Serial.println("\nFully calibrated!");
    Serial.println("--------------------------------");
    Serial.println("Calibration Results: ");
    adafruit_bno055_offsets_t newCalib;
    bno.getSensorOffsets(newCalib);
    displaySensorOffsets(newCalib);

    Serial.println("\n\nStoring calibration data to NFFS...");

    writeCalibrationDataToFile(newCalib);
  }

}

bool BNOAbstraction::getEvent(sensors_event_t *event)
  {
    bno.getEvent(event);
  }
