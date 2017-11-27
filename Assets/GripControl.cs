using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GripControl : MonoBehaviour {

    public GameObject hand;
    public SteamVR_TrackedController controller;
    public MyNetworkManager networkManager;
    // Use this for initialization
    void Start () {



    }


    // hair trigger is axis 1.x
    // barley pressed 0.05
    // fully pressed 0.85-1
    bool gripping = false;
    bool gripToggleReady = false;
    public bool rightArm = true;

    // Update is called once per frame
    void Update () {
       // if (controller.controllerState.rAxis1.)
       float ht = controller.controllerState.rAxis1.x;
        if (ht > 0.05f) {


        }

        if (controller.triggerPressed && gripToggleReady)
        {
            gripToggleReady = false; // only trigger once until released again
            ToggleGrip();
        }
        else {
            gripToggleReady = true;
        }
       

        Debug.Log("axis 4:"+controller.controllerState.rAxis1.x+","+controller.controllerState.rAxis1.y);
     //   Debug.Log("axis 4:" + controller.controllerState.rAxis1.x);
    }

    void ToggleGrip() {
        gripping = !gripping;
        float finger1pos;
        float finger2pos;
        float finger3pos;
        if (gripping)
        {
            finger1pos = 7000f;
            finger2pos = 7000f;
            finger3pos = 7000f;
        }
        else {
            finger1pos = 0;
            finger2pos = 0;
            finger3pos = 0;
        }

        networkManager.SendMoveArmWithFingers(rightArm, 0, 0, 0, 0, 0, 0, finger1pos, finger2pos, finger3pos);



    }
}
