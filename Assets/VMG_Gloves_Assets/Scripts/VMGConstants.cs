using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class Constants
{
    //VMG30, VMG30+ and VMG10 constants
    public const byte PKG_NONE = 0;
    public const byte PKG_QUAT_FINGER = 1;
    public const byte PKG_RAW_FINGER = 3;


    public const int VMG30_PKG_SIZE = 90;
    public const int NumSensors = 23;
    public const int NumFingerSensors = 10;
    public const int NumPressureSensors = 5;
    public const int NumAbductionSensors = 4;
    public const int RightHanded = 0;
    public const int LeftHanded = 1;
    public const int BaudRate = 230400;

  

    //vmglite contants
    public const int NumSensors_LITE = 5;
    public const int NumFingers_LITE = 5;
    public const byte PKG_QUAT_FINGER_LITE = 1;     //send quaternion and finger data
    public const byte PKG_QUAT_LITE = 2;            //send only quaternion
    public const byte PKG_IMU_FINGER_LITE = 3;      //send IMU data and finger data
    public const byte PKG_IMU_LITE = 3;             //send IMU data

}


static class SensorIndexRightHanded
{
    //VMG30 and VMG30+ sensor indexes
    public const int ThumbPh1 = 1;      //!< thumb 1st phalange
    public const int ThumbPh2 = 0;      //!< thumb 2nd phalange
    public const int IndexPh1 = 3;      //!< index first phalange
    public const int IndexPh2 = 2;      //!< index second phalange
    public const int MiddlePh1 = 5;      //!< index first phalange
    public const int MiddlePh2 = 4;      //!< index second phalange
    public const int RingPh1 = 7;      //!< index first phalange
    public const int RingPh2 = 6;      //!< index second phalange
    public const int LittlePh1 = 9;      //!< index first phalange
    public const int LittlePh2 = 8;      //!< index second phalange

    public const int PalmArch = 10;
    public const int ThumbCrossOver = 12;

    public const int AbdThumb = 19;
    public const int AbdIndex = 20;
    public const int AbdRing = 21;
    public const int AbdLittle = 22;

    //these work for either VMG30 and VMG10
    public const int PressThumb = 13;
    public const int PressIndex = 15;
    public const int PressMiddle = 16;
    public const int PressRing = 17;
    public const int PressLittle = 18;


    //VMG10 sensor indexes
    public const int Thumb_VMG10 = 0;
    public const int Index_VMG10 = 2;
    public const int Middle_VMG10 = 4;
    public const int Ring_VMG10 = 6;
    public const int Little_VMG10 = 8;

    //VMGLITE sensor indexes
    public const int Thumb_Lite = 4;
    public const int Index_Lite = 3;
    public const int Middle_Lite = 2;
    public const int Ring_Lite = 1;
    public const int Little_Lite = 0;
}

static class SensorIndexLeftHanded
{
    //VMG30 and VMG30+ sensor indexes
    public const int ThumbPh1 = 8;      //!< thumb 1st phalange
    public const int ThumbPh2 = 9;      //!< thumb 2nd phalange
    public const int IndexPh1 = 6;      //!< index first phalange
    public const int IndexPh2 = 7;      //!< index second phalange
    public const int MiddlePh1 = 4;      //!< index first phalange
    public const int MiddlePh2 = 5;      //!< index second phalange
    public const int RingPh1 = 2;      //!< index first phalange
    public const int RingPh2 = 3;      //!< index second phalange
    public const int LittlePh1 = 0;      //!< index first phalange
    public const int LittlePh2 = 1;      //!< index second phalange

    public const int PalmArch = 10;
    public const int ThumbCrossOver = 12;

   
    public const int AbdThumb = 22;
    public const int AbdIndex = 21;
    public const int AbdRing = 20;
    public const int AbdLittle = 19;

    //these work for wither VMG30 and VMG10
    public const int PressThumb = 18;
    public const int PressIndex = 17;
    public const int PressMiddle = 16;
    public const int PressRing = 15;
    public const int PressLittle = 14;

    //VMG10 sensor indexes
    public const int Thumb_VMG10 = 8;
    public const int Index_VMG10 = 6;
    public const int Middle_VMG10 = 4;
    public const int Ring_VMG10 = 2;
    public const int Little_VMG10 = 0;

    //VMGLITE sensor indexes
    public const int Thumb_Lite = 0;
    public const int Index_Lite = 1;
    public const int Middle_Lite = 2;
    public const int Ring_Lite = 3;
    public const int Little_Lite = 4;

}


//constants for IMU and Flex sensors filtering
static class FilterConst
{
    public const int Filter_None = 0;       
    public const int Filter_Low = 2;        
    public const int Filter_Middle = 4;     
    public const int Filter_High = 6;       
    public const int Filter_Very_High = 8;  
   
}