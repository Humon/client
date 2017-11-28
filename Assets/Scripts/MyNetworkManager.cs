using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

//Messages that the client can send to the dll running on the server.
public class MyMsgTypes
{
	public static short MSG_MOVE_ARM = 1000;
	public static short MSG_MOVE_ARM_NO_THETAY = 1001;
	public static short MSG_MOVE_ARM_HOME = 1002;
	public static short MSG_STOP_ARM = 1003;
	public static short MSG_MOVE_FINGERS = 1004;
    public static short MSG_MOVE_HAND = 1005;
    public static short MSG_MOVE_ARM_ANGULAR_VELOCITY = 1006;
    public static short MSG_MOVE_ARM_ANGULAR_POSITION = 1007;
    public static short MSG_MOVE_ARM_ANGULAR_VELOCITY_LOOPED = 1008;
    public static short MSG_MOVE_ARM_CARTESIAN_POSITION_WITH_FINGERS = 1009;
    public static short MSG_SET_ARM_POSITION = 1010;
    public static short MSG_SET_FINGER_POSITION = 1011;
    public static short MSG_MOVE_ARM_UPDATE = 1012; // MoveArm in ARM_base32.dll
    public static short MSG_FREEZE_POSITION = 1013;
    public static short MSG_GET_CARTESIAN_POSITIONS = 1014;
    public static short MSG_GET_CARTESIAN_COMMANDS = 1015;

    public static short MSG_GET_CACHED_CARTESIAN_COMMANDS = 1016;

}
public class GetCachedCartesianCommandsMessage : MessageBase
{

}

public class GetCartesianPositionsMessage : MessageBase
{

}

public class GetCartesianCommandsMessage : MessageBase
{

}


public class SetArmPositionMessage : MessageBase
{
    public bool rightArm;
    public float x;
    public float y;
    public float z;
    public float thetaX;
    public float thetaY;
    public float thetaZ;
}

public class SetFingerPositionMessage : MessageBase
{
    public bool rightArm;
    public float fp1;
    public float fp2;
    public float fp3;
}

public class MoveArmUpdateMessage : MessageBase
{

}

public class FreezePositionMessage : MessageBase
{

}

public class MoveArmWithFingersMessage : MessageBase
{
    public bool rightArm;
    public float x;
    public float y;
    public float z;
    public float thetaX;
    public float thetaY;
    public float thetaZ;
    public float fp1;
    public float fp2;
    public float fp3;

}

public class MoveArmAngularVelocityMessage : MessageBase
{
    public bool rightArm;
    public float av1;
    public float av2;
    public float av3;
    public float av4;
    public float av5;
    public float av6;
    public float av7;//Unsure if necessary
}

public class MoveArmAngularVelocityLoopedMessage : MessageBase //new
{
    public bool rightArm;
    public int iterations;
    public float av1;
    public float av2;
    public float av3;
    public float av4;
    public float av5;
    public float av6;
    public float av7;
}

public class MoveArmCartesianPosition_MoveRelativeMessage : MessageBase //new
{
    public bool rightArm;
    public float X;
    public float Y;
    public float Z;
    public float ThetaX;
    public float ThetaY;
    public float ThetaZ;
}

public class MoveArmAngularPositionMessage : MessageBase
{
    public bool rightArm;
    public float ap1;
    public float ap2;
    public float ap3;
    public float ap4;
    public float ap5;
    public float ap6;
    public float ap7;//Unsure if necessary
}

public class MoveArmMessage : MessageBase
{
	public bool rightArm;
	public float x;
	public float y;
	public float z;
	public float thetaX;
	public float thetaY;
	public float thetaZ;
}

public class MoveArmNoThetaYMessage : MessageBase
{
	public bool rightArm;
	public float x;
	public float y;
	public float z;
	public float thetaX;
	public float thetaZ;
}

public class MoveArmHomeMessage : MessageBase
{
	public bool rightArm;
}

public class StopArmMessage : MessageBase
{
	public bool rightArm;
	public bool suppressLog;
}

public class MoveFingersMessage : MessageBase
{
	public float []gloveData = new float[10];
}


public class MyNetworkManager : MonoBehaviour
{
  
  private bool handHasData = false;
  private bool handsInitialized = false;
	
  String commandLeft = "";
  String commandRight = "";
  
  SerialPort handPortR;
  SerialPort handPortL;

  //private float HandMoveDelay = 0.0f;

  public string address = "127.0.0.1";
  public int port = 11111;  
  public GameObject cameraRig;
  public VideoChatExample videoChat;

  private bool isAtStartup = true;
  private bool connectedToServer = false;
  private bool localRun = false;

  public static bool isServer = false;
    
  NetworkClient myClient;

    //public SampleUserPolling_ReadWrite handController;



    void Update ()
  {
	//HandMoveDelay += Time.deltaTime;
	if (isAtStartup) {
	  if (Input.GetKeyDown (KeyCode.C)) {
		SetupClient ();
	  }
	}

        if (Input.GetKeyDown(KeyCode.I)) {
            // Debug info about where arm thinks it is, should go next frame, and should go globally.
            GetCartesianCommands();
            GetCartesianPositions();
            //myClient.Send(MyMsgTypes.MSG_GET_CARTESIAN_COMMANDS, m);
            GetCachedCartesianCommands();
        }
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    //myClient.Send(MyMsgTypes.MSG_GET_CARTESIAN_COMMANDS, m);
        //}
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    //myClient.Send(MyMsgTypes.MSG_GET_CARTESIAN_COMMANDS, m);
        //}
    }

    public bool isConnectedToServer()
    {
        return connectedToServer;
    }

  void OnGUI ()
  {
	if (isAtStartup) {
	  GUI.Label (new Rect (2, 30, 200, 100), "Press C to start client (controller)");
	 }
  }

    
  // Create a client and connect to the server port
  public void SetupClient ()
  {
	myClient = new NetworkClient ();
	InitClient ();    
	myClient.Connect (address, port);
	Debug.Log ("Started client");
  }
    
  // Create a local client and connect to the local server
  public void SetupLocalClient ()
  {
	myClient = ClientScene.ConnectLocalServer ();
	InitClient ();
	//videoChat.remoteView.GetComponent<CameraController>().StartLocalStream();
	Debug.Log ("Started local client");
  }

  private void InitClient ()
  {
	myClient.RegisterHandler (MsgType.Connect, OnConnected);
	cameraRig.SetActive (true); // transitively enables VIVE controllers
	//if (!localRun) {
	//  videoChat.gameObject.SetActive (true);
	//}
	isAtStartup = false;
  }

  // Client function
  public void OnConnected (NetworkMessage netMsg)
  {
	    Debug.Log ("Connected to server on " + address + ":" + port);
        FindObjectOfType<HUD>().connected.text = "Connected";
        FindObjectOfType<HUD>().connected.color = Color.green;
        if (!localRun) {

	  //Invoke ("JoinVideoChat", 3.0f);
	}
	connectedToServer = true;
  }

  private void JoinVideoChat ()
  {
	videoChat.JoinVideoChat ();
  }


    public void GetCartesianPositions() {
        GetCartesianPositionsMessage m = new GetCartesianPositionsMessage();
        myClient.Send(MyMsgTypes.MSG_GET_CARTESIAN_POSITIONS, m);
    }

    public void GetCartesianCommands()
    {
        GetCartesianCommandsMessage m = new GetCartesianCommandsMessage();
        myClient.Send(MyMsgTypes.MSG_GET_CARTESIAN_COMMANDS, m);
    }

    public void GetCachedCartesianCommands()
    {
        GetCachedCartesianCommandsMessage m = new GetCachedCartesianCommandsMessage();
        myClient.Send(MyMsgTypes.MSG_GET_CACHED_CARTESIAN_COMMANDS, m);
    }


    // Sets a global var in ARM_32.CPP / DLL, does not send a move command yet
    public void SetArmPositions(bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ)
    {
        // Debug.Log("set arm position: " + x + "," + y + "," + z + ",");
        SetArmPositionMessage m = new SetArmPositionMessage();
        m.rightArm = rightArm;
        m.x = x;
        m.y = y;
        m.z = z;
        m.thetaX = thetaX;
        m.thetaY = thetaY;
        m.thetaZ = thetaZ;
        myClient.Send(MyMsgTypes.MSG_SET_ARM_POSITION, m);
    }

    public void SetFingerPosition(bool rightArm, float fp1, float fp2, float fp3)
    {
        // Debug.Log("send set finger position: " + fp1 +","+fp2+","+fp3);
        SetFingerPositionMessage m = new SetFingerPositionMessage();
        m.rightArm = rightArm;
        m.fp1 = fp1;
        m.fp2 = fp2;
        m.fp3 = fp3;
        myClient.Send(MyMsgTypes.MSG_SET_FINGER_POSITION, m);
    }

    public void MoveArmUpdate() {
        MoveArmUpdateMessage m = new MoveArmUpdateMessage();
        myClient.Send(MyMsgTypes.MSG_MOVE_ARM_UPDATE, m);
    }



    public void FreezePosition() {
       
        // NOTE: This allows the motor to sustain momentum, because freeze position takes the motor's *real* current position every frame
        // Better solution is to save frozen variables as global and not let them change until FROZEN is unlcoked.
        // Even better, call a Stop command on the robot and don't update ANY values until it's unfrozen (safer)
        FreezePositionMessage m = new FreezePositionMessage();
        //myClient.Send(MyMsgTypes.MSG_STOP_ARM, m);
        myClient.Send(MyMsgTypes.MSG_FREEZE_POSITION, m);
        // Debug.Log("<color=blue>FROZEN</color>");
        FindObjectOfType<HUD>().canMoveRobot.text = "Disabled";
        FindObjectOfType<HUD>().canMoveRobot.color = Color.red;
    }



    ////shawn test
    public void SendMoveArmWithFingers(bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ, float fp1, float fp2, float fp3)
    {
        if (!connectedToServer)
        {
            Debug.LogError("Not connected to server!");
            return;
        }
        Debug.Log("Sending move " + ArmSide(rightArm) + " arm...");
        MoveArmWithFingersMessage m = new MoveArmWithFingersMessage();
        m.rightArm = rightArm;
        m.x = x;
        m.y = y;
        m.z = z;
        m.thetaX = thetaX;
        m.thetaY = thetaY;
        m.thetaZ = thetaZ;
        m.fp1 = fp1;
        m.fp2 = fp2;
        m.fp3 = fp3;
        myClient.Send(MyMsgTypes.MSG_MOVE_ARM_CARTESIAN_POSITION_WITH_FINGERS, m);
    }

    //public void SendMoveArmAngularVelocity(bool rightArm, float av1, float av2, float av3, float av4, float av5, float av6, float av7)
    //{
    //    if (!connectedToServer)
    //    {
    //        Debug.LogError ("Not connected to server!");
    //        return;
    //    }
    //    Debug.Log("Sending move " + ArmSide(rightArm) + " arm...");
    //    MoveArmAngularVelocityMessage m = new MoveArmAngularVelocityMessage();
    //    m.rightArm = rightArm;
    //    m.av1 = av1;
    //    m.av2 = av2;
    //    m.av3 = av3;
    //    m.av4 = av4;
    //    m.av5 = av5;
    //    m.av6 = av6;
    //    m.av7 = av7;

    //    myClient.Send(MyMsgTypes.MSG_MOVE_ARM_ANGULAR_VELOCITY, m);
    //    Debug.LogError("Angular Velocity Sent!");

    //}

    //public void SendMoveArmAngularVelocityLooped(bool rightArm, int iterations, float av1, float av2, float av3, float av4, float av5, float av6, float av7)
    //{
    //    if (!connectedToServer)
    //    {
    //        Debug.LogError("Not connected to server!");
    //        return;
    //    }
    //    Debug.Log("Sending move " + ArmSide(rightArm) + " arm...");

    //    MoveArmAngularVelocityLoopedMessage m = new MoveArmAngularVelocityLoopedMessage();
    //    m.rightArm = rightArm;
    //    m.iterations = iterations;
    //    m.av1 = av1;
    //    m.av2 = av2;
    //    m.av3 = av3;
    //    m.av4 = av4;
    //    m.av5 = av5;
    //    m.av6 = av6;
    //    m.av7 = av7;

    //    myClient.Send(MyMsgTypes.MSG_MOVE_ARM_ANGULAR_VELOCITY_LOOPED, m);
    //    Debug.LogError("Angular Velocity Looped Sent!");

    //}
    
    //public void SendMoveArmAngularPosition(bool rightArm, float ap1, float ap2, float ap3, float ap4, float ap5, float ap6, float ap7)
    //{
    //    if (!connectedToServer)
    //    {
    //        Debug.LogWarning ("Not connected to server!");
    //        return;
    //    }

    //    Debug.Log("Sending move " + ArmSide(rightArm) + "with MoveArmAngularPosition sent!");

    //    MoveArmAngularPositionMessage m = new MoveArmAngularPositionMessage();
    //    m.rightArm = rightArm;
    //    m.ap1 = ap1;
    //    m.ap2 = ap2;
    //    m.ap3 = ap3;
    //    m.ap4 = ap4;
    //    m.ap5 = ap5;
    //    m.ap6 = ap6;
    //    m.ap7 = ap7;

    //    myClient.Send(MyMsgTypes.MSG_MOVE_ARM_ANGULAR_POSITION, m);

    //}

   
    
    //public class MoveArmAngularVelocityLoopedMessage : MessageBase //new
    //{
    //    public bool rightArm;
    //    public int iterations;
    //    public float av1;
    //    public float av2;
    //    public float av3;
    //    public float av4;
    //    public float av5;
    //    public float av6;
    //    public float av7;
    //}

    // end shawn test 10.1.17
 //   public void SendMoveArm (bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ)
 // {
   

	//Debug.Log ("Sending move " + ArmSide(rightArm) + " arm...");
 //   MoveArmMessage m = new MoveArmMessage();
 //   m.rightArm = rightArm;
 //   m.x = x;
 //   m.y = y;
 //   m.z = z;
 //   m.thetaX = thetaX;
 //   m.thetaY = thetaY;
 //   m.thetaZ = thetaZ;

 //   myClient.Send (MyMsgTypes.MSG_MOVE_ARM, m);
 // }

 // private void ReceiveMoveArm (NetworkMessage message)
 // {
	//MoveArmMessage m = message.ReadMessage<MoveArmMessage>();
	//Debug.Log ("Move " + ArmSide(m.rightArm) + " arm received!");
 //   KinovaAPI.MoveHand(m.rightArm, m.x, m.y, m.z, m.thetaX, m.thetaY, m.thetaZ);
 // }

 // public void SendMoveArmNoThetaY (bool rightArm, float x, float y, float z, float thetaX, float thetaZ)
 // {


	//Debug.Log ("Sending move " + ArmSide(rightArm) + " arm no theta y...");
	//MoveArmNoThetaYMessage m = new MoveArmNoThetaYMessage();
 //   m.rightArm = rightArm;
 //   m.x = x;
 //   m.y = y;
 //   m.z = z;
 //   m.thetaX = thetaX;
 //   m.thetaZ = thetaZ;

 //   myClient.Send (MyMsgTypes.MSG_MOVE_ARM_NO_THETAY, m);
 // }
    
 // public void SendMoveArmHome (bool rightArm)
 // {

	//Debug.Log ("Sending move " + ArmSide (rightArm) + " arm home...");
	//MoveArmHomeMessage m = new MoveArmHomeMessage();
 //   m.rightArm = rightArm;

 //   myClient.Send (MyMsgTypes.MSG_MOVE_ARM_HOME, m);
 // }
    
  public void SendStopArm (bool rightArm, bool suppressLog)
  {
	

        StopArmMessage m = new StopArmMessage();
        m.rightArm = rightArm;

        m.suppressLog = suppressLog;
       // Debug.Log("<color=red>Send stop arm at </color>"+Time.time);
        myClient.Send (MyMsgTypes.MSG_STOP_ARM, m);
  }
    
  private string ArmSide (bool rightArm)
  {
	return rightArm ? "right" : "left";
  }
  
  //Some code for open bionics hand
  
 // public void SendMoveToHands (int ringFinger)
 // {
	//if (!connectedToServer) {
	//	//Debug.LogWarning ("Not connected to server!");
	//	return;
	//}

	//Debug.Log ("ring finger data sent ");
	//ReceiveMoveToHandsMessage m = new ReceiveMoveToHandsMessage();
	//m.ring = ringFinger;

	//myClient.Send (MyMsgTypes.MSG_MOVE_FINGERS, m);
 // }
    
}
		