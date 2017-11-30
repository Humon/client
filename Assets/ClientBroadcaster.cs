using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class ClientBroadcaster : MonoBehaviour
{


    // Stream or broadcast from one person to many others?
    public bool oneToManyBroadcast;
    public int numberReceivers = 2;

    private HostData[] hostData;
    private NetworkView clientView;

    // Use this for initialization
    void Start()
    {


        MasterServer.ClearHostList();
        MasterServer.RequestHostList("ClientBroadcaster");
        hostData = MasterServer.PollHostList();



        clientView = gameObject.AddComponent<NetworkView>();
        clientView.stateSynchronization = NetworkStateSynchronization.Off;
        clientView.group = 6;

       
    }

    IEnumerator DelayedConnection()
    {
        yield return new WaitForSeconds(2.0f);

        string connectionResult = "";

        if (hostData.Length > 0)
        {
            connectionResult = "" + Network.Connect(hostData[0]);
            Debug.Log("connected:" + connectionResult);
        }

    }

    [RPC]
    CartesianPosition GetCurrentArmCartesianPosition(string cartPosAsStr, NetworkMessageInfo info)
    {
        string[] ps = cartPosAsStr.Split(',');
        CartesianPosition cartPos = new CartesianPosition(
            int.Parse(ps[0]),
            int.Parse(ps[1]),
            int.Parse(ps[2]),
            int.Parse(ps[3]),
            int.Parse(ps[4]),
            int.Parse(ps[5]),
            int.Parse(ps[6]),
            int.Parse(ps[7]),
            int.Parse(ps[8])
            );
        Debug.Log("cartpos:" + cartPos.x);
        return cartPos;

    }


    public void StartServer()
    {
        Network.InitializeServer(1, 2301, true);

        MasterServer.RegisterHost("ClientBroadcaster", "Test");
        Debug.Log("client broadcaster server started");


    }
}

