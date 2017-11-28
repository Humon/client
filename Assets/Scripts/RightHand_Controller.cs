using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO.Ports;

public class RightHand_Controller : MonoBehaviour
{
    private int[] roboHandData = new int[10];
    private float commandDelay = 0.0f;

    private bool doReset = false;
    public static bool doExecute = false;

    //SerialPort handPortR;
    SerialPort handPortR;
    // Use this for initialization
    void Start()
    {
        handPortR = new SerialPort("\\\\.\\COM256", 115200);
        //handPortR = new SerialPort("\\\\.\\COM256", 115200);

        //Debug.Log("COM256 status: " + handPortR.IsOpen);
        //Debug.Log("COM256 baud: " + handPortR.BaudRate);
        Debug.Log("COM256 status: " + handPortR.IsOpen);
        Debug.Log("COM256 baud: " + handPortR.BaudRate);

        /*
        if (!handPortR.IsOpen)
        {
            try
            {
                handPortR.Open();
                Debug.Log("COM256 status: " + handPortR.IsOpen);
                handPortR.BaudRate = 115200;
                Debug.Log("COM256 baud: " + handPortR.BaudRate);
                handPortR.WriteLine("A4");
                //handPortR.WriteLine("A2");
                handPortR.WriteLine("H1");
                handPortR.WriteLine("G0O");
            }
            catch (System.IO.IOException e)
            {
                Debug.Log("Could not open comport for hands" + e);
            }

        }
        */


        if (!handPortR.IsOpen)
        {
            try
            {
                handPortR.Open();
                Debug.Log("COM256 status: " + handPortR.IsOpen);
                handPortR.BaudRate = 115200;
                Debug.Log("COM256 baud: " + handPortR.BaudRate);
                handPortR.WriteLine("A4");
                //handPortL.WriteLine("A2");
                handPortR.WriteLine("H1");
                handPortR.WriteLine("G0O");

                handPortR.DiscardInBuffer();
                handPortR.DiscardOutBuffer();
            }
            catch (System.IO.IOException e)
            {
                Debug.Log("Could not open comport for hands" + e);
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        commandDelay += Time.deltaTime;
        if (commandDelay > 1.0f && doExecute)
        {

            for (int i = 0; i < 4; i++)
            {
                roboHandData[i] = (int)(UnityEngine.Random.value * 18.0f * -1.0f);

                if (i < 5 && handPortR.IsOpen)
                {

                    string temp = "F" + ((int)i).ToString() + "P" + ((int)(roboHandData[i] * -2)).ToString();
                    try
                    {
                        handPortR.WriteLine(temp);
                        Debug.Log(temp);

                        handPortR.DiscardInBuffer();
                        handPortR.DiscardOutBuffer();
                    }
                    catch (System.Exception)
                    {
                        Debug.Log("timeout is the issue, right hand");
                    }

                    /*
                    temp = "F" + ((int)(i)).ToString() + "P" + ((int)(roboHandData[i + 5] * -2)).ToString();
                    handPortR.WriteLine(temp);
                    Debug.Log(temp);
                    
                    handPortR.DiscardInBuffer();
                    handPortR.DiscardOutBuffer();
                    //new WaitForSeconds(0.25f);
                    */
                }

            }

            doExecute = false;

        }
        else if (commandDelay > 2.0f)
        {
            commandDelay = 0.0f;
        }
    }
}
