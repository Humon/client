using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;

public class MyMsgTypes
{
    public static short MSG_STOP_ARM = 1003;
    public static short MSG_MOVE_ARM_CARTESIAN_POSITION_WITH_FINGERS = 1009;
    public static short MSG_FREEZE_ARM_POSITION = 1013;
    public static short MSG_CHAT = 1019;
    public static short MSG_REQUEST_CARTESIAN_POSITION = 1018;
}


public class GetCartesianCommandsMessage : MessageBase
{

}

public class MoveArmPositionWithFingersMessage : MessageBase
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

public class RequestCartesianPositionMessage : MessageBase
{
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

public class ChatMessage : MessageBase
{
    public string text;
}


public class FreezeArmPositionMessage : MessageBase
{
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

public class StopArmMessage : MessageBase
{
    public bool rightArm;
    public bool suppressLog;
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
    

    private bool isAtStartup = true;
    private bool connectedToServer = false;


    NetworkClient myClient;

    //public SampleUserPolling_ReadWrite handController;


    void RequestCurrentCartesianPosition() {

    }

    void SendChatMessage(string t)
    {
        ChatMessage m = new ChatMessage();
        m.text = t;
        myClient.Send(MyMsgTypes.MSG_CHAT, m);

        //NetworkManager.singleton.client.Send((short)MyMsgTypes.MSG_CHAT,m);

    }


    public void RequestArmPositionsFromServer() {
        RequestCartesianPositionMessage m = new RequestCartesianPositionMessage();
        myClient.Send(MyMsgTypes.MSG_REQUEST_CARTESIAN_POSITION, m);
    }

   public void RequestArmPositionsFromServerAndFreeze() {
        Debug.Log("requesting freeze pos from server.");
        FreezeArmPositionMessage m = new FreezeArmPositionMessage();
        myClient.Send(MyMsgTypes.MSG_FREEZE_ARM_POSITION,m);

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SendChatMessage("from client");
        }
        //HandMoveDelay += Time.deltaTime;
        if (isAtStartup)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                SetupClient();
            }
        }

    }

    public bool isConnectedToServer()
    {
        return connectedToServer;
    }

    void OnGUI()
    {
        return;
        if (isAtStartup)
        {
            GUI.Label(new Rect(2, 30, 200, 100), "Press C to start client (controller)");
        }
    }

    void GetCartPosFromServer(NetworkMessage netMsg)
    {
        // server should send us goodies.
    }

    // Create a client and connect to the server port
    public void SetupClient()
    {
        myClient = new NetworkClient();


        myClient.Connect(address, port);
        Debug.Log("Started client");
        FindObjectOfType<HUD>().serverStatus.text = "Trying to connect .. is server up and started?";
        //myClient = ClientScene.ConnectLocalServer();
        myClient.RegisterHandler(MsgType.Connect, OnConnected);

        

        isAtStartup = false;
        NetworkServer.Listen(port);
    }

    // Client function
    public void OnConnected(NetworkMessage netMsg)
    {
        Debug.Log("Connected to server on " + address + ":" + port);
        FindObjectOfType<HUD>().serverStatus.text = "Connected to server!";
        FindObjectOfType<HUD>().serverStatus.color = Color.green;
        FindObjectOfType<HUD>().joinServerButton.SetActive(false);

        connectedToServer = true;
        
        myClient.RegisterHandler(MyMsgTypes.MSG_CHAT, OnClientChatMessage); // Basic hello world of server <--> client comm. Can be deleted
        myClient.RegisterHandler(MyMsgTypes.MSG_REQUEST_CARTESIAN_POSITION, OnReceivedCartesianPosition);
        myClient.RegisterHandler(MyMsgTypes.MSG_FREEZE_ARM_POSITION, OnReceivedFreezeArm);

    }

    

    private void OnReceivedCartesianPosition(NetworkMessage m)
    {
        // Called after the DeadTrigger is released 
        Debug.Log("Receive cart pos callback!");

        // After we receive the current arm cartesian position from the server, 
        // Send a final signal to the server to move the arm to its current position,
        // resulting in a freeze at that position until further commands are sent.

        // So the arm won't move again until DeadTrigger is pulled.

        var msg = m.ReadMessage<RequestCartesianPositionMessage>();

        //cartesianPositionUpdated = true;
        CartesianPosition currentPosition = new CartesianPosition(
            msg.x,
            msg.y,
            msg.z,
            msg.thetaX,
            msg.thetaY,
            msg.thetaZ,
            msg.fp1,
            msg.fp2,
            msg.fp3
            );



        FindObjectOfType<HUD>().currentPosition.text = currentPosition.x.ToString("0.00") + "," +
            currentPosition.y.ToString("0.00") + ", " +
            currentPosition.z.ToString("0.00") + ", " +
            currentPosition.thetaX.ToString("0.00") + ", " +
            currentPosition.thetaY.ToString("0.00") + ", " +
            currentPosition.thetaZ.ToString("0.00") + ", " +
            currentPosition.fp1.ToString("0") + ", " +
            currentPosition.fp2.ToString("0") + ", " +
            currentPosition.fp3.ToString("0");

    }



    //bool cartesianPositionUpdated = false;
    //CartesianPosition lastKnownCartesianPositionOfArm = new CartesianPosition();
    private void OnReceivedFreezeArm(NetworkMessage m) {
        // Called after the DeadTrigger is released 
        Debug.Log("Receive FREEZE pos callback!");
    
        // After we receive the current arm cartesian position from the server, 
        // Send a final signal to the server to move the arm to its current position,
        // resulting in a freeze at that position until further commands are sent.

        // So the arm won't move again until DeadTrigger is pulled.

        var msg = m.ReadMessage<FreezeArmPositionMessage>();
        
        //cartesianPositionUpdated = true;
        CartesianPosition frozenPosition = new CartesianPosition(
            msg.x,
            msg.y,
            msg.z,
            msg.thetaX,
            msg.thetaY,
            msg.thetaZ,
            msg.fp1,
            msg.fp2,
            msg.fp3
            );
        FindObjectOfType<Tracker_Handler>().FreezeArmPositionCallback(frozenPosition);
        

    }

    private void OnClientChatMessage(NetworkMessage netMsg)
    {
        var msg = netMsg.ReadMessage<ChatMessage>();
        Debug.Log("New chat message on client: " + msg.text);

        //chat.Add(msg.value);
    }

    
    
    
    // Not currently used, but can be a more accurate/instantaneous "Freeze" than GetCartesianPosition
    public void GetCartesianCommands()
    {
        GetCartesianCommandsMessage m = new GetCartesianCommandsMessage();
        // myClient.Send(MyMsgTypes.MSG_GET_CARTESIAN_COMMANDS, m);
    }
    
    

    public void SendMoveArmWithFingers(bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ, float fp1, float fp2, float fp3)
    {
        if (!connectedToServer)
        {
            Debug.LogError("Not connected to server!");
            return;
        }
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
    

    public void SendStopArm(bool rightArm, bool suppressLog)
    {


        StopArmMessage m = new StopArmMessage();
        m.rightArm = rightArm;

        m.suppressLog = suppressLog;
        // Debug.Log("<color=red>Send stop arm at </color>"+Time.time);
        myClient.Send(MyMsgTypes.MSG_STOP_ARM, m);
    }

    private string ArmSide(bool rightArm)
    {
        return rightArm ? "right" : "left";
    }
    



}
