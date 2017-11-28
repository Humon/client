using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotVideoChat : MonoBehaviour {
   
    // Gets the list of devices and prints them to the console.
    public void SeekCameras()
    {
        Debug.Log("seek cameras");
        WebCamDevice[] devices = WebCamTexture.devices;
        for (int i = 0; i < devices.Length; i++) { 
            Debug.Log(devices[i].name);
        }
    }
}
