using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Runtime.InteropServices;

using UnityEngine.Networking;


/**
 * Need to use left and right thumbFlexAnglesL etc. to get flex values betwween 0-1000, send appropriate proportion to robot hand
 * 
 * 
 * **/

namespace VML
{
    public class VMGLITE_Controller : MonoBehaviour
    {
        private bool debugSkeletonLeft = false;
        private bool debugSkeletonRight = true;

        public int COMPORT_LeftGlove = 1;
        public int COMPORT_RightGlove = 2;

        //root is common to both left and right hand
        //these will be used for future purposes
        public Transform root;
        public Transform spine;

        //right hand joints
        public Transform clavicleR;
        public Transform upperArmR;
        public Transform lowerArmR;
        public Transform handR;
        public Transform[] ThumbR = new Transform[3];
        public Transform[] IndexR = new Transform[3];
        public Transform[] MiddleR = new Transform[3];
        public Transform[] RingR = new Transform[3];
        public Transform[] LittleR = new Transform[3];

        //right hand fingers joint angles
        Vector3[] thumbFlexAnglesR = new Vector3[3];
        Vector3[] indexFlexAnglesR = new Vector3[3];
        Vector3[] middleFlexAnglesR = new Vector3[3];
        Vector3[] ringFlexAnglesR = new Vector3[3];
        Vector3[] littleFlexAnglesR = new Vector3[3];

        Vector3 handAnglesR = new Vector3(0, 0, 0);

        //left hand joint
        public Transform clavicleL;
        public Transform upperArmL;
        public Transform lowerArmL;
        public Transform handL;
        public Transform[] ThumbL = new Transform[3];
        public Transform[] IndexL = new Transform[3];
        public Transform[] MiddleL = new Transform[3];
        public Transform[] RingL = new Transform[3];
        public Transform[] LittleL = new Transform[3];

        //left hand finger joint angles
        Vector3[] thumbFlexAnglesL = new Vector3[3];
        Vector3[] indexFlexAnglesL = new Vector3[3];
        Vector3[] middleFlexAnglesL = new Vector3[3];
        Vector3[] ringFlexAnglesL = new Vector3[3];
        Vector3[] littleFlexAnglesL = new Vector3[3];

        Vector3 handAnglesL = new Vector3(0, 0, 0);


        //communicaton between glove and unity
        VMGLITE_Driver gloveL = new VMGLITE_Driver(), gloveR = new VMGLITE_Driver();

		private int testCount = 0;
        public MyNetworkManager myNetworkManager;
        public static float [] FingerData = new float[10];

        // Use this for initialization
        void Start()
        {
            for(int i = 0; i < 10; i++){
                FingerData [i] = (float)(i + 100);
            }
            //Debug.Log("Start\n");

            gloveL.Init(COMPORT_LeftGlove, Constants.LeftHanded, Constants.PKG_QUAT_FINGER);
            gloveL.StartCommunication();

            gloveR.Init(COMPORT_RightGlove, Constants.RightHanded, Constants.PKG_QUAT_FINGER);
            gloveR.StartCommunication();


        }

        void OnApplicationQuit()
        {
            //Debug.Log("Close app\n");
            gloveR.StopCommunication();
            gloveL.StopCommunication();

        }

        //module angle from sensor values (0 = valmin  1000 = valmax)
        float ModulateAngle(VMGLITEValues values, int sensorindex, float valmin, float valmax)
        {
            if ((sensorindex>=0)&&(sensorindex<Constants.NumSensors_LITE))
                return valmin + (valmax - valmin) * values.SensorValues[sensorindex] / 1000.0f;
            
            return 0.0f;
        }


        void UpdateHandAnglesLeft(VMGLITEValues values)
        {
            //thumb1_l
            thumbFlexAnglesL[0][0] = ModulateAngle(values, SensorIndexLeftHanded.Thumb_Lite, 50.0f, 90.0f); //roll over X is controller by abduction value 
            thumbFlexAnglesL[0][1] = ModulateAngle(values, SensorIndexLeftHanded.Thumb_Lite, -110.0f, -100.0f); //roll over X is controller by abduction value 
            thumbFlexAnglesL[0][2] = ModulateAngle(values, SensorIndexLeftHanded.Thumb_Lite, -50.0f, -100.0f); //roll over X is controller by abduction value 

            //thumb2_l
            thumbFlexAnglesL[1][0] = 0.0f;
            thumbFlexAnglesL[1][1] = 0.0f;
            thumbFlexAnglesL[1][2] = ModulateAngle(values, SensorIndexLeftHanded.Thumb_Lite, 10.0f, -30.0f); //roll over X is controller by abduction value 

            //thumb3_l
            thumbFlexAnglesL[2][0] = 0.0f;
            thumbFlexAnglesL[2][1] = 0.0f;
            thumbFlexAnglesL[2][2] = ModulateAngle(values, SensorIndexLeftHanded.Thumb_Lite, 15.0f, -50.0f); //roll over X is controller by abduction value 


            //index1_l
            indexFlexAnglesL[0][0] = 15.0f;
            indexFlexAnglesL[0][1] = 0.0f;// ModulateAngle(values, SensorIndexLeftHanded.AbdIndex, 0.0f, -15.0f); //roll over X is controller by abduction value 
            indexFlexAnglesL[0][2] = ModulateAngle(values, SensorIndexLeftHanded.Index_Lite, 0.0f, -50.0f); //roll over X is controller by abduction value 

            //index2_l
            indexFlexAnglesL[1][0] = 0.0f;
            indexFlexAnglesL[1][1] = 0.0f;
            indexFlexAnglesL[1][2] = ModulateAngle(values, SensorIndexLeftHanded.Index_Lite, 0.0f, -110.0f); //roll over X is controller by abduction value 

            //index3_l
            indexFlexAnglesL[2][0] = 0.0f;
            indexFlexAnglesL[2][1] = 0.0f;
            indexFlexAnglesL[2][2] = ModulateAngle(values, SensorIndexLeftHanded.Index_Lite, 0.0f, -55.0f); //roll over X is controller by abduction value 


            //middle1_l
            middleFlexAnglesL[0][0] = 4.50f;
            middleFlexAnglesL[0][1] = 0.0f;
            middleFlexAnglesL[0][2] = ModulateAngle(values, SensorIndexLeftHanded.Middle_Lite, 0.0f, -50.0f); //roll over X is controller by abduction value 

            //middle2_l
            middleFlexAnglesL[1][0] = 0.0f;
            middleFlexAnglesL[1][1] = 0.0f;
            middleFlexAnglesL[1][2] = ModulateAngle(values, SensorIndexLeftHanded.Middle_Lite, 0.0f, -110.0f); //roll over X is controller by abduction value 

            //middle3_l
            middleFlexAnglesL[2][0] = 0.0f;
            middleFlexAnglesL[2][1] = 0.0f;
            middleFlexAnglesL[2][2] = ModulateAngle(values, SensorIndexLeftHanded.Middle_Lite, 0.0f, -55.0f); //roll over X is controller by abduction value 


            //ring1_l
            ringFlexAnglesL[0][0] = 5.0f;
            ringFlexAnglesL[0][1] = 0.0f;// ModulateAngle(values, SensorIndexLeftHanded.AbdRing, 0.0f, 15.0f); //roll over X is controller by abduction value 
            ringFlexAnglesL[0][2] = ModulateAngle(values, SensorIndexLeftHanded.Ring_Lite, 0.0f, -50.0f); //roll over X is controller by abduction value 

            //ring2_l
            ringFlexAnglesL[1][0] = 0.0f;
            ringFlexAnglesL[1][1] = 0.0f;
            ringFlexAnglesL[1][2] = ModulateAngle(values, SensorIndexLeftHanded.Ring_Lite, 0.0f, -110.0f); //roll over X is controller by abduction value 

            //ring3_l
            ringFlexAnglesL[2][0] = 0.0f;
            ringFlexAnglesL[2][1] = 0.0f;
            ringFlexAnglesL[2][2] = ModulateAngle(values, SensorIndexLeftHanded.Ring_Lite, 0.0f, -55.0f); //roll over X is controller by abduction value 


            //little1_l
            littleFlexAnglesL[0][0] = 5.0f;
            littleFlexAnglesL[0][1] = 10.0f; //ModulateAngle(values, SensorIndexLeftHanded.AbdLittle, 5.0f, 25.0f); //roll over X is controller by abduction value 
            littleFlexAnglesL[0][2] = ModulateAngle(values, SensorIndexLeftHanded.Little_Lite, 0.0f, -50.0f); //roll over X is controller by abduction value 

            //little2_l
            littleFlexAnglesL[1][0] = 0.0f;
            littleFlexAnglesL[1][1] = 0.0f;
            littleFlexAnglesL[1][2] = ModulateAngle(values, SensorIndexLeftHanded.Little_Lite, 0.0f, -110.0f); //roll over X is controller by abduction value 

            //little3_l
            littleFlexAnglesL[2][0] = 0.0f;
            littleFlexAnglesL[2][1] = 0.0f;
            littleFlexAnglesL[2][2] = ModulateAngle(values, SensorIndexLeftHanded.Little_Lite, 0.0f, -55.0f); //roll over X is controller by abduction value 
        }


        void UpdateHandAnglesRight(VMGLITEValues values)
        {
            //thumb1_r
            thumbFlexAnglesR[0][0] = ModulateAngle(values, SensorIndexRightHanded.Thumb_Lite, 50.0f, 90.0f); //roll over X is controller by abduction value 
            thumbFlexAnglesR[0][1] = ModulateAngle(values, SensorIndexRightHanded.Thumb_Lite, -110.0f, -100.0f); //roll over X is controller by abduction value 
            thumbFlexAnglesR[0][2] = ModulateAngle(values, SensorIndexRightHanded.Thumb_Lite, -50.0f, -100.0f); //roll over X is controller by abduction value 

            //thumb2_r
            thumbFlexAnglesR[1][0] = 0.0f;
            thumbFlexAnglesR[1][1] = 0.0f;
            thumbFlexAnglesR[1][2] = ModulateAngle(values, SensorIndexRightHanded.Thumb_Lite, 10.0f, -30.0f); //roll over X is controller by abduction value 

            //thumb3_r
            thumbFlexAnglesR[2][0] = 0.0f;
            thumbFlexAnglesR[2][1] = 0.0f;
            thumbFlexAnglesR[2][2] = ModulateAngle(values, SensorIndexRightHanded.Thumb_Lite, 15.0f, -50.0f); //roll over X is controller by abduction value 


            //index1_r
            indexFlexAnglesR[0][0] = 15.0f;
            indexFlexAnglesR[0][1] = 0.0f;// ModulateAngle(values, SensorIndexLeftHanded.AbdIndex, 0.0f, -15.0f); //roll over X is controller by abduction value 
            indexFlexAnglesR[0][2] = ModulateAngle(values, SensorIndexRightHanded.Index_Lite, 0.0f, -50.0f); //roll over X is controller by abduction value 

            //index2_r
            indexFlexAnglesR[1][0] = 0.0f;
            indexFlexAnglesR[1][1] = 0.0f;
            indexFlexAnglesR[1][2] = ModulateAngle(values, SensorIndexRightHanded.Index_Lite, 0.0f, -110.0f); //roll over X is controller by abduction value 

            //index3_r
            indexFlexAnglesR[2][0] = 0.0f;
            indexFlexAnglesR[2][1] = 0.0f;
            indexFlexAnglesR[2][2] = ModulateAngle(values, SensorIndexRightHanded.Index_Lite, 0.0f, -55.0f); //roll over X is controller by abduction value 


            //middle1_r
            middleFlexAnglesR[0][0] = 4.50f;
            middleFlexAnglesR[0][1] = 0.0f;
            middleFlexAnglesR[0][2] = ModulateAngle(values, SensorIndexRightHanded.Middle_Lite, 0.0f, -50.0f); //roll over X is controller by abduction value 

            //middle2_r
            middleFlexAnglesR[1][0] = 0.0f;
            middleFlexAnglesR[1][1] = 0.0f;
            middleFlexAnglesR[1][2] = ModulateAngle(values, SensorIndexRightHanded.Middle_Lite, 0.0f, -110.0f); //roll over X is controller by abduction value 

            //middle3_r
            middleFlexAnglesR[2][0] = 0.0f;
            middleFlexAnglesR[2][1] = 0.0f;
            middleFlexAnglesR[2][2] = ModulateAngle(values, SensorIndexRightHanded.Middle_Lite, 0.0f, -55.0f); //roll over X is controller by abduction value 


            //ring1_r
            ringFlexAnglesR[0][0] = 5.0f;
            ringFlexAnglesR[0][1] = 0.0f;// ModulateAngle(values, SensorIndexLeftHanded.AbdRing, 0.0f, 15.0f); //roll over X is controller by abduction value 
            ringFlexAnglesR[0][2] = ModulateAngle(values, SensorIndexRightHanded.Ring_Lite, 0.0f, -50.0f); //roll over X is controller by abduction value 

            //ring2_r
            ringFlexAnglesR[1][0] = 0.0f;
            ringFlexAnglesR[1][1] = 0.0f;
            ringFlexAnglesR[1][2] = ModulateAngle(values, SensorIndexRightHanded.Ring_Lite, 0.0f, -110.0f); //roll over X is controller by abduction value 

            //ring3_r
            ringFlexAnglesR[2][0] = 0.0f;
            ringFlexAnglesR[2][1] = 0.0f;
            ringFlexAnglesR[2][2] = ModulateAngle(values, SensorIndexRightHanded.Ring_Lite, 0.0f, -55.0f); //roll over X is controller by abduction value 


            //little1_r
            littleFlexAnglesR[0][0] = 5.0f;
            littleFlexAnglesR[0][1] = 10.0f; //ModulateAngle(values, SensorIndexLeftHanded.AbdLittle, 5.0f, 25.0f); //roll over X is controller by abduction value 
            littleFlexAnglesR[0][2] = ModulateAngle(values, SensorIndexRightHanded.Little_Lite, 0.0f, -50.0f); //roll over X is controller by abduction value 

            //little2_r
            littleFlexAnglesR[1][0] = 0.0f;
            littleFlexAnglesR[1][1] = 0.0f;
            littleFlexAnglesR[1][2] = ModulateAngle(values, SensorIndexRightHanded.Little_Lite, 0.0f, -110.0f); //roll over X is controller by abduction value 

            //little3_r
            littleFlexAnglesR[2][0] = 0.0f;
            littleFlexAnglesR[2][1] = 0.0f;
            littleFlexAnglesR[2][2] = ModulateAngle(values, SensorIndexRightHanded.Little_Lite, 0.0f, -55.0f); //roll over X is controller by abduction value 

        }

        // Update is called once per frame
        void Update()
        {
			//Testing data for console
			//UnityEngine.Debug.Log ("Tracker_Handler.FingerData[9]: " + littleFlexAnglesL[0][2] + "Tracker_Handler.FingerData[8] " + ringFlexAnglesR[0][2]);// + "PointerFinger flex data are: indexFlexAnglesR[2] " + indexFlexAnglesR[2]);

            if (!MyNetworkManager.isServer)
            {
                
                FingerData[0] = littleFlexAnglesL[0][2] * 1.5f;
                FingerData[1] = ringFlexAnglesL[0][2] * 1.5f;
                FingerData[2] = middleFlexAnglesL[0][2] * 1.5f;
                FingerData[3] = indexFlexAnglesL[0][2] * 1.5f;
                FingerData[4] = thumbFlexAnglesL[0][2];

                FingerData[5] = thumbFlexAnglesR[0][2];
                FingerData[6] = indexFlexAnglesR[0][2] * 1.5f;
                FingerData[7] = middleFlexAnglesR[0][2] * 1.5f;
                FingerData[8] = ringFlexAnglesR[0][2] * 1.5f;
                FingerData[9] = littleFlexAnglesR[0][2] * 1.5f;
                
                //myNetworkManager.SendMoveFingers (FingerData);
            }


            //NEW TEST!!!
			/*
			for(int k = 5; k < 9; k++){
				int data = (int)((Random.value) * -49);
				Tracker_Handler.FingerData [k] = data;
			}

            */
			//END TEST ROUTINE!!!

            int i = 0;
            //update index
            if (gloveL.NewPackageAvailable())
            {

                VMGLITEValues v = gloveL.GetPackage();

                //update values, this depends on the hand bones definition, please chenge this part in your application
                UpdateHandAnglesLeft(v);

                //update finger rendering
                for (i = 0; i < 3; i++)
                {
                    ThumbL[i].localRotation = Quaternion.Euler(thumbFlexAnglesL[i]);
                    IndexL[i].localRotation = Quaternion.Euler(indexFlexAnglesL[i]);
                    MiddleL[i].localRotation = Quaternion.Euler(middleFlexAnglesL[i]);
                    RingL[i].localRotation = Quaternion.Euler(ringFlexAnglesL[i]);
                    LittleL[i].localRotation = Quaternion.Euler(littleFlexAnglesL[i]);

                }
                Vector3 Zaxis = new Vector3(0.0f, 0.0f, 1.0f);
                Vector3 Zrot = Quaternion.Euler(v.pitchH, -v.yawH + gloveL.GetYaw0(), v.rollH) * Zaxis;

                float yaw = Mathf.Rad2Deg * Mathf.Atan2(-Zrot[0], Zrot[2]);

                float yawD = 90.0f - yaw;
                float yawUpper = 1.5f * yawD / 3.0f;
                float yawLower = 1.5f * yawD / 3.0f;
                if (yawLower < 0.0f)
                {
                    yawLower = 0.0f;
                    yawUpper = yawD;
                }

                //eseguo roll 0.5 su hand 0.4 su lowerarm e 0.1 su upper arm
                upperArmL.localRotation = Quaternion.Euler(0.0f, 0.0f, yawUpper);
                lowerArmL.localRotation = Quaternion.Euler(0.0f, 0.0f, yawLower);


                //apply to upperArm another rotation around xaxis
                //get xaxis on global coordinate
                Vector3 xaxis = upperArmL.TransformVector(new Vector3(1.0f, 0.0f, 0.0f));
                upperArmL.RotateAround(upperArmL.position, xaxis, -v.pitchH);

                //roll, rotation along xaxis of lowerArm and xaxis of hand
                xaxis = lowerArmL.TransformVector(new Vector3(1.0f, 0.0f, 0.0f));
                upperArmL.RotateAround(upperArmL.position, xaxis, -0.25f * v.rollH);
                lowerArmL.RotateAround(lowerArmL.position, xaxis, -0.65f * v.rollH);
                handL.localRotation = Quaternion.Euler(-90.0f - 0.1f * v.rollH, 0.0f, 0.0f);


                if (debugSkeletonLeft)
                {
                    //Debug.ClearDeveloperConsole();
                    //Debug.Log("New package L\n");
                    DrawSkeletonLeft(v);
                }
            }


            //check if a new package is arrived from glove
            if (gloveR.NewPackageAvailable())
            {
                //Debug.Log("New package R\n");
                VMGLITEValues v = gloveR.GetPackage();

                //update values, this depends on the hand bones definition, please chenge this part in your application
                UpdateHandAnglesRight(v);

                //update fingers rendering position and rotation
                for (i = 0; i < 3; i++)
                {
                    ThumbR[i].localRotation = Quaternion.Euler(thumbFlexAnglesR[i]);
                    IndexR[i].localRotation = Quaternion.Euler(indexFlexAnglesR[i]);
                    MiddleR[i].localRotation = Quaternion.Euler(middleFlexAnglesR[i]);
                    RingR[i].localRotation = Quaternion.Euler(ringFlexAnglesR[i]);
                    LittleR[i].localRotation = Quaternion.Euler(littleFlexAnglesR[i]);
                }

                //compute wrist orientation vector taking into consideration reset yaw0
                Vector3 Zaxis = new Vector3(0.0f, 0.0f, 1.0f);
                Vector3 Zrot = Quaternion.Euler(v.pitchH, -v.yawH + gloveR.GetYaw0(), v.rollH) * Zaxis;

                float yaw = Mathf.Rad2Deg * Mathf.Atan2(-Zrot[0], Zrot[2]);

                float yawD = 90.0f + yaw;
                float yawUpper = 1.5f * yawD / 3.0f;
                float yawLower = 1.5f * yawD / 3.0f;

                //yawLower cannot be less than 0
                if (yawLower < 0.0f)
                {
                    yawLower = 0.0f;
                    yawUpper = yawD;
                }


                //fix yaw value, rotation along local Z axis
                upperArmR.localRotation = Quaternion.Euler(0.0f, 0.0f, yawUpper);
                lowerArmR.localRotation = Quaternion.Euler(0.0f, 0.0f, yawLower);


                //apply to upperArm another rotation around xaxis
                //get xaxis on global coordinate
                Vector3 xaxis = upperArmR.TransformVector(new Vector3(1.0f, 0.0f, 0.0f));
                upperArmR.RotateAround(upperArmR.position, xaxis, -v.pitchH);

                //roll, rotation along xaxis of lowerArm and xaxis of hand
                xaxis = lowerArmR.TransformVector(new Vector3(1.0f, 0.0f, 0.0f));
                upperArmR.RotateAround(upperArmR.position, xaxis, 0.25f * v.rollH);
                lowerArmR.RotateAround(lowerArmR.position, xaxis, 0.65f * v.rollH);
                handR.localRotation = Quaternion.Euler(-90.0f + 0.1f * v.rollH, 0.0f, 0.0f);

                if (debugSkeletonRight)
                {
                    //Debug.Log("New package R\n");
                    DrawSkeletonRight(v);
                }
            }
        }


        private void DrawSkeletonLeft(VMGLITEValues v)
        {
            //Debug.Log("QUAT_H:" + v.q0h + " " + v.q1h + " " + v.q2h + " " + v.q3h + "\n");
            //Debug.Log("RPY_H:" + v.rollH + " " + v.pitchH + " " + v.yawH + " YAW0:" + gloveL.GetYaw0() + "\n");
            //Debug.DrawRay(spine.position, new Vector3(0.1f, 0.0f, 0.0f), Color.magenta);
            //Debug.DrawRay(spine.position, new Vector3(0.0f, 0.1f, 0.0f), Color.green);
            //Debug.DrawRay(spine.position, new Vector3(0.0f, 0.0f, 0.1f), Color.blue);
            //Debug.DrawRay(spine.position, clavicleL.position - spine.position, Color.red);
            //Debug.DrawRay(clavicleL.position, upperArmL.position - clavicleL.position, Color.red);
            //Debug.DrawRay(upperArmL.position, lowerArmL.position - upperArmL.position, Color.red);
            //Debug.DrawRay(lowerArmL.position, handL.position - lowerArmL.position, Color.red);
            //Debug.DrawRay(handL.position, IndexL[0].position - handL.position, Color.red);
            //Debug.DrawRay(IndexL[0].position, IndexL[1].position - IndexL[0].position, Color.red);
            //Debug.DrawRay(IndexL[1].position, IndexL[2].position - IndexL[1].position, Color.red);

            Vector3 X = new Vector3(0.0f, 0.0f, 0.3f);
            Vector3 Xrot = Quaternion.Euler(v.pitchH, -v.yawH + gloveL.GetYaw0(), v.rollH) * X;

            //Debug.DrawRay(lowerArmL.position, Xrot, Color.magenta);

            Xrot = Quaternion.Euler(v.pitchH, -v.yawH + gloveL.GetYaw0(), v.rollH) * X;
            //Debug.DrawRay(handL.position, Xrot, Color.magenta);

            //get XRot in the lowerhand reference frame
            Vector3 XrotLowerArm = lowerArmL.InverseTransformVector(Xrot);

            float pitchHandRel = Mathf.Rad2Deg * Mathf.Atan2(XrotLowerArm[2], -XrotLowerArm[0]);

            Vector3 xaxis = handL.TransformDirection(new Vector3(0.1f, 0f, 0f));
            Vector3 yaxis = handL.TransformDirection(new Vector3(0.0f, 0.1f, 0.0f));
            Vector3 zaxis = handL.TransformDirection(new Vector3(0.0f, 0.0f, 0.1f));

            //Debug.DrawRay(handL.position, xaxis, Color.red);
            //Debug.DrawRay(handL.position, yaxis, Color.green);
            //Debug.DrawRay(handL.position, zaxis, Color.blue);

            xaxis = lowerArmL.TransformDirection(new Vector3(0.1f, 0f, 0f));
            yaxis = lowerArmL.TransformDirection(new Vector3(0.0f, 0.1f, 0.0f));
            zaxis = lowerArmL.TransformDirection(new Vector3(0.0f, 0.0f, 0.1f));

            //Debug.DrawRay(lowerArmL.position, xaxis, Color.red);
            //Debug.DrawRay(lowerArmL.position, yaxis, Color.green);
            //Debug.DrawRay(lowerArmL.position, zaxis, Color.blue);

            //Debug.Log("Pitch Hand Rel:" + pitchHandRel + "\n");
        }


        private void DrawSkeletonRight(VMGLITEValues v)
        {
            //Debug.Log("FING:" + v.SensorValues[0] + " " + v.SensorValues[1] + " " + v.SensorValues[2] + " " + v.SensorValues[3] + " " + v.SensorValues[4] + "\n");
            //Debug.Log("QUAT_H:" + v.q0h + " " + v.q1h + " " + v.q2h + " " + v.q3h + "\n");
            //Debug.Log("RPY_H:" + v.rollH + " " + v.pitchH + " " + v.yawH + " YAW0:" + gloveR.GetYaw0() + "\n");
            //Debug.DrawRay(spine.position, new Vector3(0.1f, 0.0f, 0.0f), Color.magenta);
            //Debug.DrawRay(spine.position, new Vector3(0.0f, 0.1f, 0.0f), Color.green);
            //Debug.DrawRay(spine.position, new Vector3(0.0f, 0.0f, 0.1f), Color.blue);
            //Debug.DrawRay(spine.position, clavicleR.position - spine.position, Color.red);
            //Debug.DrawRay(clavicleR.position, upperArmR.position - clavicleR.position, Color.red);
            //Debug.DrawRay(upperArmR.position, lowerArmR.position - upperArmR.position, Color.red);
            //Debug.DrawRay(lowerArmR.position, handR.position - lowerArmR.position, Color.red);

            //Debug.DrawRay(handR.position, ThumbR[0].position - handR.position, Color.red);
            //Debug.DrawRay(ThumbR[0].position, ThumbR[1].position - ThumbR[0].position, Color.red);
            //Debug.DrawRay(ThumbR[1].position, ThumbR[2].position - ThumbR[1].position, Color.red);

            //Debug.DrawRay(handR.position, IndexR[0].position - handR.position, Color.red);
            //Debug.DrawRay(IndexR[0].position, IndexR[1].position - IndexR[0].position, Color.red);
            //Debug.DrawRay(IndexR[1].position, IndexR[2].position - IndexR[1].position, Color.red);

            //Debug.DrawRay(handR.position, LittleR[0].position - handR.position, Color.red);
            //Debug.DrawRay(LittleR[0].position, LittleR[1].position - LittleR[0].position, Color.red);
            //Debug.DrawRay(LittleR[1].position, LittleR[2].position - LittleR[1].position, Color.red);

            //Debug.DrawRay(IndexR[0].position, LittleR[0].position - IndexR[0].position, Color.green);

            Vector3 medpos = (IndexR[0].position + LittleR[0].position);

            medpos[0] = medpos[0] / 2.0f;
            medpos[1] = medpos[1] / 2.0f;
            medpos[2] = medpos[2] / 2.0f;

            //Debug.DrawRay(handR.position, medpos - handR.position, Color.green);


            Vector3 X = new Vector3(0.0f, 0.0f, 0.3f);
            Vector3 Xrot = Quaternion.Euler(v.pitchH, -v.yawH + gloveR.GetYaw0(), v.rollH) * X;

            //Debug.DrawRay(lowerArmR.position, Xrot, Color.magenta);

            Xrot = Quaternion.Euler(v.pitchH, -v.yawH + gloveR.GetYaw0(), v.rollH) * X;
            //Debug.DrawRay(handR.position, Xrot, Color.magenta);

            //get XRot in the lowerhand reference frame
            Vector3 XrotLowerArm = lowerArmR.InverseTransformVector(Xrot);

            float pitchHandRel = Mathf.Rad2Deg * Mathf.Atan2(XrotLowerArm[2], XrotLowerArm[0]);

            //Debug.Log("Pitch Hand Rel:" + pitchHandRel + "\n");

            Xrot.Normalize();
            float yawRot = 180.0f * ((float)System.Math.Atan2(Xrot[0], Xrot[2])) / 3.14159f;


            Vector3 xaxis = lowerArmR.TransformDirection(new Vector3(0.1f, 0f, 0f));
            Vector3 yaxis = lowerArmR.TransformDirection(new Vector3(0.0f, 0.1f, 0.0f));
            Vector3 zaxis = lowerArmR.TransformDirection(new Vector3(0.0f, 0.0f, 0.1f));

            //Debug.DrawRay(lowerArmR.position, xaxis, Color.red);
            //Debug.DrawRay(lowerArmR.position, yaxis, Color.green);
            //Debug.DrawRay(lowerArmR.position, zaxis, Color.blue);

            xaxis = upperArmR.TransformDirection(new Vector3(0.1f, 0f, 0f));
            yaxis = upperArmR.TransformDirection(new Vector3(0.0f, 0.1f, 0.0f));
            zaxis = upperArmR.TransformDirection(new Vector3(0.0f, 0.0f, 0.1f));

            //Debug.DrawRay(upperArmR.position, xaxis, Color.red);
            //Debug.DrawRay(upperArmR.position, yaxis, Color.green);
            //Debug.DrawRay(upperArmR.position, zaxis, Color.blue);

            xaxis = handR.TransformDirection(new Vector3(0.1f, 0f, 0f));
            yaxis = handR.TransformDirection(new Vector3(0.0f, 0.1f, 0.0f));
            zaxis = handR.TransformDirection(new Vector3(0.0f, 0.0f, 0.1f));

            //Debug.DrawRay(handR.position, xaxis, Color.red);
            //Debug.DrawRay(handR.position, yaxis, Color.green);
            //Debug.DrawRay(handR.position, zaxis, Color.blue);


            //Debug.Log("QUATH:" + v.q0h.ToString("F3") + " " + v.q1h.ToString("F3") + " " + v.q2h.ToString("F3") + " " + v.q3h.ToString("F3") + "\n");

        }
    }
}

