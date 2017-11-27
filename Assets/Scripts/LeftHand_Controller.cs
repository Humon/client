using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.IO.Ports;

public class LeftHand_Controller : MonoBehaviour
{
    private int[] roboHandData = new int[10];
    private float commandDelay = 0.0f;

    private bool sendExecute = false;

    //SerialPort handPortR;
    SerialPort handPortL;
    // Use this for initialization
    void Start()
    {
        handPortL = new SerialPort("\\\\.\\COM255", 115200);
        //handPortR = new SerialPort("\\\\.\\COM256", 115200);

        //Debug.Log("COM256 status: " + handPortR.IsOpen);
        //Debug.Log("COM256 baud: " + handPortR.BaudRate);
        Debug.Log("COM255 status: " + handPortL.IsOpen);
        Debug.Log("COM255 baud: " + handPortL.BaudRate);

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


        if (!handPortL.IsOpen)
        {
            try
            {
                handPortL.Open();
                Debug.Log("COM255 status: " + handPortL.IsOpen);
                handPortL.BaudRate = 115200;
                Debug.Log("COM255 baud: " + handPortL.BaudRate);
                //handPortL.WriteLine("A4");
                //handPortL.WriteLine("A2");
                handPortL.WriteLine("H2");
                //handPortL.WriteLine("G0O");

                //handPortL.DiscardInBuffer();
                handPortL.DiscardOutBuffer();
            }
            catch (System.IO.IOException e)
            {
                Debug.Log("Could not open comport for hands" + e);
            }

        }
        
        //handPortL.WriteLine("500,500,500,500");
    }

    // Update is called once per frame
    void Update()
    {

        commandDelay += Time.deltaTime;

        if (commandDelay > 2.0f)
        {
            string temp = "";
            for (int i = 0; i < 4; i++)
            {
                roboHandData[i] = (int) ((UnityEngine.Random.value * 900.0f) + 10);

                if (i < 3)
                {
                    temp += roboHandData[i].ToString() + ",";
                }
                else
                {
                    temp += roboHandData[i].ToString();
                }
            }
            try
            {
                handPortL.WriteLine(temp);
            }
            catch (System.Exception e)
            {
                Debug.Log("exception, Left Hand " + e);
            }
            
            //handPortL.DiscardInBuffer();
            handPortL.DiscardOutBuffer();
            
            Debug.Log(temp);
            commandDelay = 0.0f;
        }
        
        
/*
       
            if (i < 5 && handPortL.IsOpen)
            {

                string temp = "F" + ((int)i).ToString() + "P" + ((int)(roboHandData[i] * -2)).ToString();
                try
                {
                    handPortL.WriteLine(temp);
                }
                catch (System.Exception)
                {
                    Debug.Log("exception, Left Hand");
                }

                Debug.Log(temp);

                handPortL.DiscardInBuffer();
                handPortL.DiscardOutBuffer();

           
                temp = "F" + ((int)(i)).ToString() + "P" + ((int)(roboHandData[i + 5] * -2)).ToString();
                handPortR.WriteLine(temp);
                Debug.Log(temp);
                
                handPortR.DiscardInBuffer();
                handPortR.DiscardOutBuffer();
                //new WaitForSeconds(0.25f);
   
            }
            handPortL.WriteLine(temp);
            Debug.Log(temp);
            
            sendExecute = false;
            commandDelay = 0.0f;

        }
        */
    }
}
