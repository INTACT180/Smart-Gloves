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

  Serial.print("\t");
  /* Display the individual values */
  Serial.print("Sys:");
  Serial.print(system, DEC);
  Serial.print(" Gyro:");
  Serial.print(gyro, DEC);
  Serial.print(" Accel:");
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

void BNOAbstraction::getCurrentPosition(xyz *pos)
{
  *pos = currentPosition;
}


void BNOAbstraction::matLabDataOutput()
{

    if(iteration++ >=100)
    {
      iteration=0;
      uint32_t ts1 = millis();

      period = ((float) (ts1 - ts0))/1000.0f/100.0f;
      
      Serial.print(ts1-ts0);
      Serial.print(" ");

      ts0=ts1;
      Serial.print(period,7);
      Serial.print( " period \n");
    }

    ts0 = millis()-10;
    uint32_t ts1;
    while(true)
    {
      ts1 = millis();
      //bno.getEvent(&event);

      imu::Quaternion quat = bno.getQuat();

      Serial.print(ts1-ts0);
      Serial.print("\tW: ");
      Serial.print(quat.w(), 6);
      Serial.print("\tX/H: ");
      Serial.print(quat.x(), 6);
      Serial.print("\tY/P: ");
      Serial.print(quat.y(), 6);
      Serial.print("\tZ/R: ");
      Serial.print(quat.z(), 6);
      
      dataSample = bno.getVector(Adafruit_BNO055::VECTOR_LINEARACCEL);
      Serial.print("\tX_A: ");
      Serial.print(dataSample.x(), 6);
      Serial.print("\tY_A: ");
      Serial.print(dataSample.y(), 6);
      Serial.print("\tZ_A: ");
      Serial.println(dataSample.z(), 6);

      ts0=ts1;
      delay(8);
      
    }
  
}


xyz* BNOAbstraction::calculatePosition()
{
//
//    Serial.print("Time Delta: ");
//    Serial.print(period, 7);
//    Serial.print("\t\t");

  //uint32_t ts0=millis();
//  int it=0;
  
// while(true)
  {
    dataSample = bno.getVector(Adafruit_BNO055::VECTOR_LINEARACCEL);
    postAccelerationData.x = dataSample.x();
    postAccelerationData.y = dataSample.y();
    postAccelerationData.z = dataSample.z();

//    Serial.print("Acceleration X: ");
//    Serial.print(postAccelerationData.x,4);
//    Serial.print("\t\t");
//
//    Serial.print("Acceleration Y: ");
//    Serial.print(postAccelerationData.y,4);
//    Serial.print("\t\t");
//
//    Serial.print("Acceleration Z: ");
//    Serial.print(postAccelerationData.z,4);
////    Serial.print("\t\t");
//
//    postAccelerationData.x = filterAcc_X.step(postAccelerationData.x);
//
//    postAccelerationData.y = filterAcc_Y.step(postAccelerationData.y);
//
//    postAccelerationData.z = filterAcc_Z.step(postAccelerationData.z);
    
    // X Axis Deadzone
    if((postAccelerationData.x <= 0.1) && (postAccelerationData.x >= -0.1))
      postAccelerationData.x = 0;

    // Y Axis Deadzone
    if((postAccelerationData.y <= 0.12) && (postAccelerationData.y >= -0.12))
      postAccelerationData.y = 0;

    // Z Axis Deadzone
    if((postAccelerationData.z <= 0.14) && (postAccelerationData.z >= -0.14))
      postAccelerationData.z = 0;

    
    // X Axis First Integration
    postVelocityData.x = preVelocityData.x + ((postAccelerationData.x + preAccelerationData.x)/2.0f*period);

    // Y Axis First Integration
    postVelocityData.y = preVelocityData.y + ((postAccelerationData.y + preAccelerationData.y)/2.0f*period);

    // Z Axis First Integration
    postVelocityData.z = preVelocityData.z + ((postAccelerationData.z + preAccelerationData.z)/2.0f*period);

//    // Filter Velocity Data
//    postVelocityData.x = filterVel_X.step(postVelocityData.x);
//
//    postVelocityData.y = filterVel_Y.step(postVelocityData.y);
//
//    postVelocityData.z = filterVel_Z.step(postVelocityData.z);

    
   
    // X Axis Second Integration
    postPositionData.x = prePositionData.x + ((postVelocityData.x + preVelocityData.x)/2.0f*period);

    // Y Axis Second Integration
    postPositionData.y = prePositionData.y + ((postVelocityData.y + preVelocityData.y)/2.0f*period);

    // Z Axis Second Integration
    postPositionData.z = prePositionData.z + ((postVelocityData.z + preVelocityData.z)/2.0f*period);
//
//    Serial.print("Acceleration X: ");
//    Serial.print(postAccelerationData.x,4);
//    Serial.print("\t\t");
//
//    Serial.print("Acceleration Y: ");
//    Serial.print(postAccelerationData.y,4);
//    Serial.print("\t\t");
//
//    Serial.print("Acceleration Z: ");
//    Serial.print(postAccelerationData.z,4);
//    Serial.print("\t\t");
//
//        Serial.print("Velocity X: ");
//    Serial.print(postVelocityData.x,4);
//    Serial.print("\t\t");
//
//    Serial.print("Velocity Y: ");
//    Serial.print(postVelocityData.y,4);
//    Serial.print("\t\t");
//
//    Serial.print("Velocity Z: ");
//    Serial.print(postVelocityData.z,4);
//    Serial.print("\t\t");
//
//    Serial.print("Position X: ");
//    Serial.print(postPositionData.x,4);
//    Serial.print("\t\t");
//
//    Serial.print("Position Y: ");
//    Serial.print(postPositionData.y,4);
//    Serial.print("\t\t");
//
//    Serial.print("Position Z: ");
//    Serial.print(postPositionData.z,4);
//    Serial.println("\t\t");
////    
    preAccelerationData = postAccelerationData;
    preVelocityData = postVelocityData;

    // X Axis movement end check
    if(postAccelerationData.x == 0)
      counter.x++;
      
    else
      counter.x = 0;

    if(counter.x >= 25)
    {
      postVelocityData.x = 0;
      preVelocityData.x = 0;
    }

    // Y Axis movement end check
    if(postAccelerationData.y == 0)
      counter.y++;
      
    else
      counter.y = 0;

    if(counter.y >= 25)
    {
      postVelocityData.y = 0;
      preVelocityData.y = 0;
    }

    //Z Axis movement end check
    if(postAccelerationData.z == 0)
      counter.z++;
      
    else
      counter.z = 0;

    if(counter.z >= 25)
    {
      postVelocityData.z = 0;
      preVelocityData.z = 0;
    }
    
    prePositionData = postPositionData;
//
//    if(it++ >=100)
//    {
//      it=0;
//      uint32_t ts1 = millis() -ts0;
//
//      Serial.println(ts1);
//     // ts0=millis();
//    }
  }

   return &postPositionData;
}

void BNOAbstraction::update()
{
    
    if(iteration++ >=100)
    {
      iteration=0;
      uint32_t ts1 = millis();

      period = ((float) (ts1 - ts0))/1000.0f/100.0f;
      
      Serial.print(ts1-ts0);
      Serial.print(" ");

      ts0=ts1;
      Serial.print(period,7);
      Serial.print( " period \n");
    }
//    Serial.print (iteration);
//    Serial.print ("\t ");


  xyz temp;
  imu::Quaternion q = bno.getQuat();
  q.normalize();
  float tempr = q.x();  q.x() = -q.y();  q.y() = tempr;
  q.z() = -q.z();
  imu::Vector<3> euler = q.toEuler();
  Serial.print(F("Orientation: "));
  Serial.print(-180/M_PI * euler.x());  // heading, nose-right is positive, z-axis points up
  Serial.print(F(" "));
  Serial.print(-180/M_PI * euler.y());  // roll, rightwing-up is positive, y-axis points forward
  Serial.print(F(" "));
  Serial.print(-180/M_PI * euler.z());  // pitch, nose-down is positive, x-axis points right
  Serial.println(F(""));

  temp.x = -180/M_PI * euler.x() +180.0f;

  temp.y = -180/M_PI * euler.y() +180.0f; 

  temp.z = -180/M_PI * euler.z() +180.0f;
  

  orientationData = temp;
  
  //bno.getEvent(&eventTemp);
  //event = eventTemp;
  
  xyz *tempXYZ = calculatePosition();
  currentPosition = *tempXYZ;

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
  //Nffs.format();

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

      Serial.print("X/H: ");
      Serial.print(event.orientation.x, 4);
      Serial.print("\tY/P: ");
      Serial.print(event.orientation.y, 4);
      Serial.print("\tZ/R: ");
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

  /// Setting of Update values
    preAccelerationData.x = 0;
    preAccelerationData.y = 0;
    preAccelerationData.z = 0;
  
    postAccelerationData.x = 0;
    postAccelerationData.y = 0;
    postAccelerationData.z = 0;
  
    preVelocityData.x = 0;
    preVelocityData.y = 0;
    preVelocityData.z = 0;
  
    postVelocityData.x = 0;
    postVelocityData.y = 0;
    postVelocityData.z = 0;
  
    prePositionData.x = 0;
    prePositionData.y = 0;
    prePositionData.z = 0;
  
    postPositionData.x = 0;
    postPositionData.y = 0;
    postPositionData.z = 0;
    
    postPositionData.x = 0;
    postPositionData.y = 0;
    postPositionData.z = 0;

    counter.x = 0;
    counter.y = 0;
    counter.z = 0;
    
  }

  bno.setMode(Adafruit_BNO055::OPERATION_MODE_ACCGYRO);

}

void BNOAbstraction::getOrientation(xyz *OR)
  {
   *OR= orientationData;
  }

void BNOAbstraction::getEvent(sensors_event_t *evt)
  {
   *evt = event;
  }
