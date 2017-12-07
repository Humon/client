using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DpadComm : MonoBehaviour {

    public  enum DpadPosition {
        Up,
        Down,
        Left,
        Right
    }


	// Use this for initialization
	void Start () {
		
	}

    static float dpadTimeout = .3f;
    void Update () {
        dpadTimer -= Time.deltaTime;

    }

    static float deadZoneRadius = 0.3f; // deadzone.
    static float dpadTimer = 0f;
    static bool InsideDeadZone(SteamVR_Controller.Device device) {
        Vector2 touchpad = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
        //Debug.Log("deadzone?:" + touchpad.x + "," + touchpad.y);

        return (Mathf.Abs(touchpad.x) > deadZoneRadius && Mathf.Abs(touchpad.y) > deadZoneRadius);
    }


    public static bool PadPressed(DpadPosition pos, SteamVR_Controller.Device device) {
        //Debug.Log("detecting press:"+)
        if (dpadTimer > 0) return false; // pad was just pressed and we wait for cooldown
        if (InsideDeadZone(device)) return false; // pad was pressed but near center, so ignore

        Vector2 touchpad = device.GetAxis(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad);
        //Debug.Log("touch:" + touchpad.x + "," + touchpad.y);
        bool flag = false;
        switch (pos) {
            case DpadPosition.Up:
                flag = touchpad.y > deadZoneRadius;
                Debug.Log("Flag up:" + flag);
                if (flag) dpadTimer = dpadTimeout;
                return flag;
                break;
            case DpadPosition.Left:
                flag = touchpad.x < -deadZoneRadius;
                if (flag) dpadTimer = dpadTimeout;

                Debug.Log("Flag left:" + flag);

                return flag;
                break;
            case DpadPosition.Right:
                flag = touchpad.x > deadZoneRadius;
                if (flag) dpadTimer = dpadTimeout;

                return flag;
                break;
            case DpadPosition.Down:

                flag = touchpad.y < -deadZoneRadius;
                if (flag) dpadTimer = dpadTimeout;

                return flag;
                break;
            default:break;
        }
        
        return false;
    }
}
