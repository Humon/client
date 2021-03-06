﻿
/** 
 * @file Tracker_Handler.cs
 * @author Various - Last MM
 * @date 11/25/2017
 * @link https://github.com/SpaceVR-O1/Human
 * @version 0.2
 *
 * @brief Track a HTC VIVE puck, get its position and velocity, and interact with GameObjects.
 *
 * @section DESCRIPTION
 * 
 * TO-DO???
 * This script gets the device number (1 to 4) for the controller at program load.
 * Device number 0 = VIVE HMD
 */

/**
* System.Timers contains threaded timers to help with UI timing.
* 
* System.Collections contains interfaces and classes that define 
* various collections of objects, such as lists, queues, bit arrays, 
* hash tables and dictionaries.
*
* System.Runtime.InteropServices contains members to support COM interop 
* and use of external DLL's, SDK's, and API's.
* 
* UnityEngine contains Unity's C# library to control the game engine.
* 
*/
using System;
using System.Timers;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;

public class Tracker_Handler : MonoBehaviour
{
    public static bool LOCAL_DEBUG_STATEMENTS_ON = false;

    //If false, then we are sending the command to the left arm
    public bool rightArm = true;
    public bool autoUnlockingEnabled = true; // When enabled StopArm is called. This happens every update

    //Full Range Demo Mode and Offset
    public Vector3 offset = new Vector3(0.5f,-1.4f,0.37f);
    public Vector3 rotationOffset = new Vector3(20, 0, 0);
    public float fingerClosedPos = 7000f;
    public float fingerOpenedPos = 2000f;


    //private float OffsetX = -0.50f;// -0.5f; -1.04 // robot left?
    //private float OffsetY = 1.30f; // 1.2f; 0.8 // robot forwards?
    //private float OffsetZ = -0.0f; // -0.5f; -0.53
    //private float thetaOffsetX = 0.0f;
    //private float thetaOffsetY = 0.0f;
    //private float thetaOffsetZ = 0.0f;

    public Vector3 RobotPosition = new Vector3(-0.4f, 0.4f, -0.5f);
    public Vector3 newRobotPosition = new Vector3(0f, 0f, 0f);
    public Vector3 RobotRotation = new Vector3(3.1f, 0.12f, -0.03f);
    public Vector3 initialTrackerPosition = new Vector3(0f, 0f, 0f);
    public Vector3 initialTrackerRotation = new Vector3(0f, 0f, 0f);

    public bool tracking = false;
    public bool firstrun = true;

    public float moveFrequency = 0.005f; // XXX change to 10ms to match controller loop seconds //IMPORTANT: Make sure this carries over to the 64-bit Unity 2017 version of the code.
    // Turns out we need only one of "moveFrequency" or "unlockFrequncey", and the Stop() and Move() commands bein synced makes the robot move predictably.
    //public float unlockFrequency = 0.5f; // every x seconds the StopArm is called. This clears the tragectory buffer 
    //                                     //Testing different values of unlock Frequency now
    //                                     //Test 1: unlockFrequency = 0.05, Result: arms seemed to move slower than 0.5
    //                                     //Test 2: unlockFrequency = 1, Result: arms seemed to move even slower
    //                                     //Test 3: unlockFrequency = 0.1, Results: arms reacted somewhat similarly but 
    //public float unlockFrequencyCount = 0.0f;
    public float roboMoveCount = 0.0f; // seconds temp that counts if time has elapsed greater than the move frequency. If it is send the command

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;    //Map VIVE trigger button to ID
    private bool triggerButtonDown = false;             //True when trigger button starts being pressed
    private bool triggerButtonUp = false;               //True when trigger button starts being released
    private bool triggerButtonPressed = false;          //True when trigger button is being held down

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;                  //Map VIVE side grip button to ID
    private bool gripButtonDown = false;                //True when side grip buttons starts being pressed
    private bool gripButtonUp = false;                  //True when side grip buttons button starts being released
    private bool gripButtonPressed = false;             //True when side grip buttons button is being held down

    // Charlie's toggle grip
   
    bool gripMoving = false;
    float gripMovingTimeout = 0f;
    bool gripToggled = false; //XXX Move up
    // hair trigger is axis 1.x
    // barley pressed 0.05
    // fully pressed 0.85-1

    float deadGripThreshhold = 0.05f;



    // private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    // private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;

    public SteamVR_TrackedController controller;        // { get { return SteamVR_Controller.Input ((int)trackedHandObj.index); } }
  //  private SteamVR_TrackedObject trackedHandObj;

    //private GameObject pickup;                          //Used by Unity3D collider and rigid body components to allow user interaction

    public MyNetworkManager myNetworkManager;

    /**@brief Used for initialization of this class
    * 
    * @section DESCRIPTION
    *
    * Start(): Is called before the first frame update only if 
    * the script instance is enabled. For objects added to the 
    * scene, the Start function will be called on all scripts before
    * Update, etc are called for any of them. 
    */

    //Moved Initcontroller code directly to Awake to make code more readable
    void Awake()
    {
        //Gets the SteamVR_TrackedObject for the controller or tracker this script is attached to
        //trackedHandObj = GetComponent<SteamVR_TrackedObject>();

        //IMPORTANT: Make sure the moveFrequency is set to 0.005 seconds in the UI

        // Call MoveArmToControllerPosition and UnlockArm every 5 ms 
        //InvokeRepeating("MoveArmToControllerPosition", 0.0f, moveFrequency);


        //SetStopTimeoutAndMoveFrequency(unlockFrequency);
        controller.MenuButtonClicked += new ClickedEventHandler(ToggleGrip);
    }

    SteamVR_Controller.Device device;

//void SetStopTimeoutAndMoveFrequency(float si) {
//    unlockFrequency = si;
//    moveFrequency = si;
//   //CancelInvoke();
//    //InvokeRepeating("UnlockArm", beginAfterSeconds, repeatInterval);
//    FindObjectOfType<HUD>().stopInterval.text = unlockFrequency.ToString();
//}

void Start ()
    {

        device = SteamVR_Controller.Input((int)controller.controllerIndex);

        //
    } //END START() FUNCTION

    float currentTriggerDelta = 0f;
    float dpadTimeout = 0.3f;
    void Update()  //architecture dependant
    {
        currentTriggerDelta = controller.controllerState.rAxis1.x;
        UpdateHoldingDeadTriggerState();
        FindObjectOfType<HUD>().gripMovingTimeout.text = gripMovingTimeout.ToString();
        if (gripMoving && gripMovingTimeout > 0)
        {
            gripMovingTimeout -= Time.deltaTime;  //timeout during which the robot does not move for grip time
        }
        else if (gripMoving) {
            gripMoving = false;
            FindObjectOfType<HUD>().gripMoving.text = "False";
            //SetStopTimeoutAndMoveFrequency(unlockFrequency);
        }

        //if ()
        if (controller.padPressed)
        {
            if (DpadComm.PadPressed(DpadComm.DpadPosition.Up, device))
            {
                SetArmPose(RobotPoses.readyToServerDrinks);
            }

            else if (DpadComm.PadPressed(DpadComm.DpadPosition.Down, device))
            {
                ToggleMode();
            }
            else if (DpadComm.PadPressed(DpadComm.DpadPosition.Left, device))
            {
                myNetworkManager.RequestArmPositionsFromServer();
            }
            else if (DpadComm.PadPressed(DpadComm.DpadPosition.Right, device)) {
                SetArmPose(RobotPoses.readyToPushStartBlender);

            }


            


        }


        if (mode == Mode.DrawingWhiteZone){
            if (currentTriggerDelta > .05f)
            {
                if (drawWhiteZoneState == DrawingWhiteZoneState.Ready) {
                    whiteZone.lastSize = 0;
                    drawWhiteZoneState = DrawingWhiteZoneState.Drawing;
                    whiteZone.beginningCorner = GetActualTrackerPosition();

                }
                whiteZone.endCorner = GetActualTrackerPosition();
                float newSize = (whiteZone.beginningCorner - whiteZone.endCorner).magnitude;
                float distToTravelForHapticFeedback = .05f;
                if (newSize - whiteZone.lastSize > distToTravelForHapticFeedback)
                { 
                    whiteZone.lastSize = newSize;
                    device.TriggerHapticPulse(1200);
                }



            } else
            {

                if (drawWhiteZoneState == DrawingWhiteZoneState.Drawing) {
                    drawWhiteZoneState = DrawingWhiteZoneState.Ready;
                    Debug.Log("white zone:" + whiteZone.beginningCorner + "," + whiteZone.endCorner);
                }
                // we're in draw white zone mode, ready to begin drawing (hold trigger)
                drawWhiteZoneHapticPulseTimer -= Time.deltaTime;
                if (drawWhiteZoneHapticPulseTimer < 0f) {
                    drawWhiteZoneHapticPulseTimer = readyDrawWhiteZoneIntercal;
                    StartCoroutine(TriggerDoubleHapticPulse());
                }

            }
        }
        
    }



    class WhiteZone {
        public Vector3 beginningCorner = new Vector3(1.0f,-0.9f,-1.0f); // a deafult starting zone that is quite large and to the right of the robot;
        public Vector3 endCorner = new Vector3(-0.8f, 0.5f, 0.7f);
        public float lastSize; // used for haptic feedback while drawing as size increases.
        public bool Contains(Vector3 p) {
            bool insideX = (p.x > beginningCorner.x && p.x < endCorner.x) || (p.x < beginningCorner.x && p.x > endCorner.x);
            bool insideY = (p.y > beginningCorner.y && p.y < endCorner.y) || (p.y < beginningCorner.y && p.y > endCorner.y);
            bool insideZ = (p.z > beginningCorner.z && p.z < endCorner.z) || (p.z < beginningCorner.z && p.z > endCorner.z);
            //Debug.Log("XYZ:" + insideX + "," + insideY + "," + insideZ);
            return insideX && insideY && insideZ;
        }
    }
    WhiteZone whiteZone = new WhiteZone();

    float drawWhiteZoneHapticPulseTimer = 0f;
    float readyDrawWhiteZoneIntercal = 0.5f;
    float drawingWhiteZoneInterval = 0.2f;

    IEnumerator TriggerDoubleHapticPulse(float delay = 0.3f) {
        for (int i = 0; i < 5; i++) {
            device.TriggerHapticPulse(1200);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield return new WaitForSeconds(delay);
        for (int i = 0; i < 5; i++)
        {
            device.TriggerHapticPulse(1200);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    enum DrawingWhiteZoneState {
        Ready,
        Drawing
    }
    DrawingWhiteZoneState drawWhiteZoneState = DrawingWhiteZoneState.Ready;

    enum Mode {
        MovingRobotArm,
        DrawingWhiteZone
    }
    Mode mode = Mode.MovingRobotArm;

    void ToggleMode() {

        if (mode == Mode.MovingRobotArm)
        {
            mode = Mode.DrawingWhiteZone;
        } else {
            mode = Mode.MovingRobotArm;
        
        }
        Debug.Log("Toggled mode to:" + mode.ToString());
    }

    // Charlie's dead switch - must pull hair trigger a little to allow movement.
    bool holdingDeadTriggerPreviousState = false;
    bool UpdateHoldingDeadTriggerState()
    {
        //Debug.Log("check hold dead");
        
       

        bool flag = currentTriggerDelta > deadGripThreshhold;
        
        // Debug.Log("rax y:" + controller.controllerState.rAxis1.y);
        // Debug.Log("<color=green>dead;" + deadGripThreshhold + "</color>, cur;" + currentTriggerDelta+", status:"+flag);
        if (flag)
        {
            //if (firstrun) {
            //    initialTrackerPosition = GetGlobalPosition();
            //    initialTrackerRotation = GetLocalRotation();
            //    firstrun = false;
            //    }

            FindObjectOfType<HUD>().canMoveRobot.text = "Enabled";
            FindObjectOfType<HUD>().canMoveRobot.color = Color.green;

        }
        else {
            FindObjectOfType<HUD>().canMoveRobot.text = "Disabled";
            FindObjectOfType<HUD>().canMoveRobot.color = Color.red;
            //  Debug.Log("<color=red>dead grip is OFF, robot OFF</color> ");

        }
        if (controller.controllerState.rAxis1.x < 0.05) {
            firstrun = true;
        }

        if (holdingDeadTriggerPreviousState == true && flag == false) {
            FreezeArmPosition();
            FindObjectOfType<HUD>().lastFrozenTime.text = Time.time.ToString();
        }

        holdingDeadTriggerPreviousState = flag;
        return flag;
    }

   

    

    void SetArmPose(CartesianPosition pose)
    {

        // Make sure to call this so that the FIRST command that is sent the robot is already a pose (not 0,0,0,0,0)
        cartesianCommandToSend = new CartesianPosition(
            pose.x,
            pose.y,
            pose.z,
            pose.thetaX,
            pose.thetaY,
            pose.thetaZ,
            pose.fp1,
            pose.fp2,
            pose.fp3
            );
        SendCurrentCommand();
        
    }

    bool armFrozen = false;
    void FreezeArmPosition() {
        Debug.Log("Freeze initiated");
        // Request from the server the currenmt position and wait for the callback.
        myNetworkManager.RequestArmPositionsFromServerAndFreeze();
    }

    public void FreezeArmPositionCallback(CartesianPosition frozenPosition) {
        armFrozen = true;
        Debug.Log("Callback with frozen pos :" + frozenPosition.x.ToString("0:00") + "," + frozenPosition.y.ToString("0:00") + "," + frozenPosition.z.ToString("0:00"));
        cartesianCommandToSend = frozenPosition;
        SendCurrentCommand();


    }

    


    CartesianPosition cartesianCommandToSend = new CartesianPosition();

    void ToggleGrip(object sender, ClickedEventArgs e)
    {
        // Begin opening or closing the grip.
        // During this time the hand should be frozen? or not?
        gripToggled = !gripToggled;
        if (gripToggled)
        {
            FindObjectOfType<HUD>().gripStatus.text = "Closed";
            FindObjectOfType<HUD>().gripStatus.color = new Color(0, 0, .6f);
            //Debug.Log("<color=green>grip is ON</color> ");
            cartesianCommandToSend.fp1 = fingerClosedPos;
            cartesianCommandToSend.fp2 = fingerClosedPos;
                //finger3pos = 7000f;
        }
        else
        {
            cartesianCommandToSend.fp1 = fingerOpenedPos;
            cartesianCommandToSend.fp2 = fingerOpenedPos;
            FindObjectOfType<HUD>().gripStatus.text = "Open";
            FindObjectOfType<HUD>().gripStatus.color = new Color(.3f,.3f, 1);
            //Debug.Log("<color=red>grip is OFF</color> ");
            //  finger3pos = 4000f;
        }

        // while gripmoving is true, no further updates to position will be sent until timeout expires.
       
        gripMoving = true;
        FindObjectOfType<HUD>().gripMoving.text = "True";
        gripMovingTimeout = 1f;
        float fingerStopTimeoutSpeed = 0.5f; // so that fingers close smoothly. TODO: separate Stop() function beteween arm motors and finger motors.
        /*SetStopTimeoutAndMoveFrequency*///(fingerStopTimeoutSpeed);
        //myNetworkManager.MoveArmUpdate();
    }


    Vector3 GetCurrentOffset() {
        // Ask robot where it is
        return offset;
        //return Vector3.zero;
    }

    Vector3 GetRotationOffset() {
        return rotationOffset;
    }

    Vector3 GetActualTrackerPosition() {

        return GetGlobalPosition() + GetCurrentOffset();
    }

    void KP_MoveArmToControllerPosition()
    {
        Vector3 trackerPosition = GetActualTrackerPosition();
        Vector3 roe = GetRotationOffset();

        if (!gripMoving && holdingDeadTriggerPreviousState)
        {
            if (whiteZone.Contains(trackerPosition))
            {
                if (armFrozen) armFrozen = false;
                float pi = Mathf.PI;
                // Only update the target move position if we're not opening/closeing the hand && we're holding dead trigger.
                cartesianCommandToSend.x = trackerPosition.z;
                cartesianCommandToSend.y = -trackerPosition.y;
                cartesianCommandToSend.z = -trackerPosition.x;
                cartesianCommandToSend.thetaX = ((transform.rotation.eulerAngles.x * (pi / 180.0f) + pi)) + roe.x * Mathf.Deg2Rad;
                cartesianCommandToSend.thetaY = (-(transform.rotation.eulerAngles.y * (pi / 180.0f) - pi / 2)) + roe.y * Mathf.Deg2Rad;
                cartesianCommandToSend.thetaZ = (-(transform.rotation.eulerAngles.z * (pi / 180.0f) - pi)) + roe.z * Mathf.Deg2Rad;
                SendCurrentCommand();
            }
            else {
                if (!armFrozen) {
                    FreezeArmPosition();
                }
                device.TriggerHapticPulse(1200);
            }
        }
        else if (gripMoving) {

            // The fingers have received their new destinations in cartesianCommandToSend, 
            // and we need to send the signal every frame until the fingers arrive (currently assumed after some fixed timeout.)
            SendCurrentCommand();
        }


       


    }

    void SendCurrentCommand() {
        StopArm(); // clear all trajectories
        // send the values we stored in cartesianCommandtoSend
        myNetworkManager.SendMoveArmWithFingers(
           true,
            cartesianCommandToSend.x,
            cartesianCommandToSend.y,
            cartesianCommandToSend.z,
            cartesianCommandToSend.thetaX,
            cartesianCommandToSend.thetaY,
            cartesianCommandToSend.thetaZ,
            cartesianCommandToSend.fp1,
            cartesianCommandToSend.fp2,
            cartesianCommandToSend.fp3
       );
    }

    //void SendMoveArmWithOffsets(bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ)
    //{
    //    myNetworkManager.SendMoveArm(true, x+OffsetX, y+OffsetY, z+OffsetZ, thetaOffsetX+thetaX, thetaOffsetY+thetaY, thetaOffsetZ+thetaZ);
    //}

	void FixedUpdate ()
	{
        if (!myNetworkManager.isConnectedToServer())
        {
            // Not connected, return
           
            return;
        }

  //      Vector3 controllerPosition = GetGlobalPosition ();
		//Vector3 controllerRotation = GetLocalRotation ();
		roboMoveCount -= Time.deltaTime;
        //unlockFrequencyCount -= Time.deltaTime;
        if (roboMoveCount < 0 && mode == Mode.MovingRobotArm)
        {
            
            KP_MoveArmToControllerPosition();
            roboMoveCount = moveFrequency;
		}

        //if (unlockFrequencyCount < 0) {
        //    UnlockArm();
        //    unlockFrequencyCount = unlockFrequency;
        //}

  //      //following code is not consistent
		//if (Main.DEBUG_STATEMENTS_ON && LOCAL_DEBUG_STATEMENTS_ON) {
		//	Debug.Log ("Controller #" + (int)trackedHandObj.index + " POSITION is:");
		//	Debug.Log ("Global X = " + controllerPosition.x + " Local X =  " + this.transform.localPosition.x);
		//	Debug.Log ("Global Y = " + controllerPosition.y + " Local Y =  " + this.transform.localPosition.y);
		//	Debug.Log ("Global Z = " + controllerPosition.z + " Local Z =  " + this.transform.localPosition.z);

		//	Debug.Log ("Controller #" + (int)trackedHandObj.index + " ROTATION is:");
		//	Debug.Log ("Local thetaX =  " + this.transform.localPosition.x);
		//	Debug.Log ("Local thetaY =  " + this.transform.localPosition.y);
		//	Debug.Log ("Local thetaZ =  " + this.transform.localPosition.z);
		//}

		//if (controller == null) {
		//	if (Main.DEBUG_STATEMENTS_ON)
		//		Debug.Log ("Hand controller not found. Please turn on at least one HTC VIVE controller.");
		//	return; //Stops null reference expections
		//}
        
	}//END FixedUpdate() FUNCTION

    void StopArm ()
	{
		if (autoUnlockingEnabled) {
            if (myNetworkManager.isConnectedToServer()) {
			    myNetworkManager.SendStopArm (rightArm, true);
            }
		}
	}
	/**
   * meters for x, y, z
   * radians for thetaX, thetaY, thetaZ
   **/
 //       void MoveArm (float x, float y, float z, float thetaX, float thetaY, float thetaZ)
	//{
	//	try {
            
	//		myNetworkManager.SendMoveArm (rightArm, x, y, z, thetaX, thetaY, thetaZ);

	//	} catch (EntryPointNotFoundException e) {
	//		Debug.Log (e.Data);
	//		Debug.Log (e.GetType ());
	//		Debug.Log (e.GetBaseException ());
	//	}
	//}
	/**@brief OnTriggerEnter() is called on collider trigger events.
   * 
   * section DESCRIPTION
   * 
   * OnTriggerEnter(): TO-DO???
   */
	//private void OnTriggerEnter (Collider collider)
	//{
	//	if (Main.DEBUG_STATEMENTS_ON)
	//		Debug.Log ("Colllider trigger ENTER");
	//	//pickup = collider.gameObject;
	//}
	///**@brief OnTriggerExit() is called on collider trigger events.
 //  * 
 //  * section DESCRIPTION
 //  * 
 //  * OnTriggerEnter(): TO-DO???
 //  */
	//private void OnTriggerExit (Collider collider)
	//{
	//	if (Main.DEBUG_STATEMENTS_ON)
	//		Debug.Log ("Colllider trigger EXIT");
	//	pickup = null;
	//}
	/**@brief GetGlobalPosition() returns X, Y, Z coordinate of hand controller  
 * 
 * section DESCRIPTION
 * 
 * GetPosition(): returns X, Y, Z float coordinates to
 * ??? decimal points of hand controller in the global reference frame.
 */
	public Vector3 GetGlobalPosition ()
	{

		Vector3 newPosition = new Vector3 ((float)this.transform.position.x, (float)this.transform.position.y, (float)this.transform.position.z);

		return newPosition;
	}

	/**@brief GetLocalPosition() returns X, Y, Z coordinate of hand controller  
  * 
  * section DESCRIPTION
  * 
  * GetPosition(): returns X, Y, Z float coordinates to 
  * ??? decimal points of hand controller in the Hand Mounted Display 
  * LOCAL reference frame.
  */
	public Vector3 GetLocalPosition ()
	{

		Vector3 newPosition = new Vector3 ((float)this.transform.localPosition.x, (float)this.transform.localPosition.y, (float)this.transform.localPosition.z);

		return newPosition;
	}

	/**@brief GetLocalRotation() returns thetaX, thetaY, thetaZ angles of hand controller  
  * 
  * section DESCRIPTION
  * 
  * GetPosition(): returns thetaX, thetaY, thetaZ float angles in
  * degrees to ??? decimal points of hand controller in the 
  * Hand Mounted Display LOCAL reference frame.
  */
	Vector3 GetLocalRotation () 
	{

		Vector3 rot = new Vector3 ((float)this.transform.rotation.eulerAngles.x, (float)this.transform.rotation.eulerAngles.y, (float)this.transform.rotation.eulerAngles.z);

		return rot;
	}

    /**@brief GetGlobalVelocity() returns X, Y, Y velocity vector of hand controller  
  * 
  * section DESCRIPTION
  * 
  * GetGlobalVelocity(): returns X, Y, Y velocity vector of hand 
  * controller in Unity3D units per second to ??? decimal points 
  * by calculating change in position of hand controller between 
  * two game engine frames renders / calls to Update().
  */
    Vector3 GetGlobalVelocity (Vector3 previousPosition)
	{

		float frameRate = (1 / Time.deltaTime);

		float newXvelocity = (previousPosition.x - this.transform.position.x) / frameRate;  
		float newYvelocity = (previousPosition.y - this.transform.position.y) / frameRate;
		float newZvelocity = (previousPosition.z - this.transform.position.z) / frameRate;

		Vector3 newVelocity = new Vector3 (newXvelocity, newYvelocity, newZvelocity);

		return newVelocity;
	}
	/**@brief GetGlobalAcceleration() returns X, Y, Y acceleration vector of hand controller  
  * 
  * section DESCRIPTION
  * 
  * GetGlobalAcceleration(): TO-DO???
  */
	Vector3 GetGlobalAcceleration (Vector3 previousVelocity)
	{

		Vector3 acceleration = new Vector3 (0.00f, -9.81f, 0.00f); //WRONG!!!

		return acceleration;
	}
} //END HANDCONTROLLER CLASS

