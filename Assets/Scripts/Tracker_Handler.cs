
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
    public Vector3 rotationOffset = new Vector3(0, 0, 0); // is currently aligned to robot hand exactly.
    public float fingerClosedPos = 7000f; // totally closed.
    public float fingerOpenedPos = 2000f; // almost all the way open.



    
    public float moveFrequency = 0.005f; // What is the delay in seconds betweeen each command sent to robot? 0.05 seems to work very smoothly.

    public float roboMoveCount = 0.0f; // timer to keep track of the time since the last movement was sent.

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;    //Map VIVE trigger button to ID
    private bool triggerButtonDown = false;             //True when trigger button starts being pressed
    private bool triggerButtonUp = false;               //True when trigger button starts being released
    private bool triggerButtonPressed = false;          //True when trigger button is being held down

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;                  //Map VIVE side grip button to ID
    private bool gripButtonDown = false;                //True when side grip buttons starts being pressed
    private bool gripButtonUp = false;                  //True when side grip buttons button starts being released
    private bool gripButtonPressed = false;             //True when side grip buttons button is being held down


   // Toggle open/close of robot hand
    bool gripMoving = false;
    float gripMovingTimeout = 0f;
    bool gripToggled = false; //XXX Move up

    // How much do we need to pull trigger to have dead-trigger work?
    float deadGripThreshhold = 0.05f;



    public SteamVR_TrackedController controller;        // { get { return SteamVR_Controller.Input ((int)trackedHandObj.index); } }


    public MyNetworkManager myNetworkManager;


    void Awake()
    {
        controller.MenuButtonClicked += new ClickedEventHandler(ToggleGrip); // Add a listener to the VIVE menu button (above the DPAD) to toggle robot grip open/closed.
    }

    SteamVR_Controller.Device device;



    void Start ()
    {

        device = SteamVR_Controller.Input((int)controller.controllerIndex);

        //
    } 

    float currentTriggerDelta = 0f;
    float dpadTimeout = 0.3f;
    void Update()  //architecture dependant
    {
        currentTriggerDelta = controller.controllerState.rAxis1.x;
        UpdateHoldingDeadTriggerState();
        if (gripMoving && gripMovingTimeout > 0)
        {
            gripMovingTimeout -= Time.deltaTime;  //timeout during which the robot does not move for grip time
        }
        else if (gripMoving) {
            gripMoving = false;
        }

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


        bool flag = currentTriggerDelta > deadGripThreshhold; // Was the Vive trigger being pulled?
        
        // Debug.Log("rax y:" + controller.controllerState.rAxis1.y);
        // Debug.Log("<color=green>dead;" + deadGripThreshhold + "</color>, cur;" + currentTriggerDelta+", status:"+flag);
        if (flag)
        {

            FindObjectOfType<HUD>().canMoveRobot.text = "Enabled";
            FindObjectOfType<HUD>().canMoveRobot.color = Color.green;

        }
        else {
            FindObjectOfType<HUD>().canMoveRobot.text = "Disabled";
            FindObjectOfType<HUD>().canMoveRobot.color = Color.red;
            //  Debug.Log("<color=red>dead grip is OFF, robot OFF</color> ");

        }
 

        if (holdingDeadTriggerPreviousState == true && flag == false) {
            FreezeArmPosition();
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
        // Debug.Log("Callback with frozen pos :" + frozenPosition.x.ToString("0:00") + "," + frozenPosition.y.ToString("0:00") + "," + frozenPosition.z.ToString("0:00"));
        cartesianCommandToSend = frozenPosition;
        SendCurrentCommand();


    }

    CartesianPosition cartesianCommandToSend = new CartesianPosition();

    void ToggleGrip(object sender, ClickedEventArgs e)
    {
        // Begin opening or closing the grip.
        // ** IMPORTANT ** although Kinova arm may be receiving MOVE commands, FINGER move commands may not work until the fingers are manually moved from the Kinova wired controller.
        // So if the toggle grip doesn't work, make sure the fingers are moving using the Kinova wired controller first.


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
        gripMovingTimeout = 1f;
        float fingerStopTimeoutSpeed = 0.5f; // so that fingers close smoothly. TODO: separate Stop() function beteween arm motors and finger motors.
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

    void MoveArmToControllerPosition()
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


    // Should we move to Update?
    void FixedUpdate()
    {
        if (!myNetworkManager.isConnectedToServer())
        {
            // Not connected, return

            return;
        }

        roboMoveCount -= Time.deltaTime;
        if (roboMoveCount < 0 && mode == Mode.MovingRobotArm)
        {
            MoveArmToControllerPosition();
            roboMoveCount = moveFrequency;
        }

    }

    void StopArm ()
	{
		if (autoUnlockingEnabled) {
            if (myNetworkManager.isConnectedToServer()) {
			    myNetworkManager.SendStopArm (rightArm, true);
            }
		}
	}
 
	public Vector3 GetGlobalPosition ()
	{

		Vector3 newPosition = new Vector3 ((float)this.transform.position.x, (float)this.transform.position.y, (float)this.transform.position.z);

		return newPosition;
	}

    
	Vector3 GetLocalRotation () 
	{

		Vector3 rot = new Vector3 ((float)this.transform.rotation.eulerAngles.x, (float)this.transform.rotation.eulerAngles.y, (float)this.transform.rotation.eulerAngles.z);

		return rot;
	}


} 

