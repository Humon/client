using UnityEngine;
using System.IO.Ports;
using Random = System.Random;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32;


public class HandHandler2
{
    private static int[] roboHandData = new int[10]; //new

    public static int loopCount = 0;
    
    public static Random rnd = new Random();

    public static bool doStart = true; //new

    public static bool doTest = true; //new

    static SerialPort handPortR;
    static SerialPort handPortL;
    // Use this for initialization
    
    public static void Start()
    {
        Debug.Log("Start can run");
        try
        {
            Debug.Log(" Attempt to open ports");
            handPortL = new SerialPort("\\\\.\\COM255", 115200);
            handPortR = new SerialPort("\\\\.\\COM256", 115200); //stable
            var i = 1;
            Debug.Log("!!! COM PORTS INITIALIZED " + i + " times !!!");
            i++;
        }
        catch (Exception e)
        {
            Debug.Log("Hand ports could not open " + e);
        }
        
        if (!handPortL.IsOpen)
        {
            try
            {
                handPortL.Open();
                handPortL.BaudRate = 115200;

                handPortL.WriteLine("H2"); //confirm which hand is which

                handPortL.DiscardOutBuffer();
                handPortL.DiscardInBuffer();
                var i = 1;
                Debug.Log("!!! COM PORTS Left Did Open " + i + " times !!!");
                i++;
            }
            catch (Exception e)
            {
                Debug.Log("Could not open comport for hands" + e);
            }

        }
        
        if (!handPortR.IsOpen)                                       
        {                                                            
            try                                                      
            {                                                        
                handPortR.Open();                                    
                handPortR.BaudRate = 115200;  //stable                       
                                                             
                handPortR.WriteLine("H1");                           
                                                             
                handPortR.DiscardOutBuffer();  
                handPortR.DiscardInBuffer();
                var i = 1;
                Debug.Log("!!! COM PORTS Right Did Open " + i + " times !!!");
                i++;
            }                                                        
            catch (Exception e)                          
            {                                                        
                Debug.Log("Could not open comport for hands" + e);
            }                                                        
                                                             
        }
        
        try
        {
            Debug.Log("Try Start Ran ");
            handPortL.WriteLine("500,500,500,500"); //change to write?
            handPortR.WriteLine("500,500,500,500");

            handPortL.DiscardOutBuffer();
            handPortL.DiscardInBuffer();
            handPortR.DiscardOutBuffer();
            handPortR.DiscardInBuffer();
            
            var i = 1;
            Debug.Log("Wrote to L and R " + i + " Times");
            i++;
        }
        catch (Exception e)
        {
            Debug.Log("Stuff sucks on start " + e);

        }
    }

    // Update is called once per frame

    public static void MainLoop()
    {
        //TESTING!!!


        while (true){
            Thread.Sleep(1000);
            Debug.Log("doTest ran  " + loopCount + " Times");
            loopCount++;
            //{
            //Start();
            //doStart = false;
            //}

            string temp = "";
        for (int i = 0; i < 4; i++)
        {
            roboHandData[i] = (int) ((rnd.Next(100, 700)) + 10);

            if (i < 3)
            {
                temp += roboHandData[i].ToString() + ",";
            }
            else
            {
                temp += roboHandData[i].ToString();
            }
        }
        Debug.Log(temp);

        try
        {
            handPortL.WriteLine(temp);
            handPortR.WriteLine(temp);

            handPortL.DiscardOutBuffer();
            handPortL.DiscardInBuffer();
            handPortR.DiscardOutBuffer();
            handPortR.DiscardInBuffer();
        }
        catch (Exception e)
        {
            Debug.Log("exception, Left Hand " + e);
        }

        //Debug.Log("Read Size " + handPortR.ReadBufferSize.ToString());
        //Debug.Log("Write Size " + handPortR.WriteBufferSize);
        //Debug.Log( handPortR.Dispose();
        //Debug.Log("Read Bytes " + handPortR.BytesToRead.ToString());
        //Debug.Log("Read Bytes " + handPortR.BytesToWrite);

    }


}

}



public class Hand_Stable_Test : MonoBehaviour
{
    private float timeCount = 0;
    private float totalTime = 0;
	// Use this for initialization
	void Start ()
	{
	    new HandHandler2(); //check instancing is correct
	    HandHandler2.Start();
	    HandHandler2.MainLoop();
	    int i = 1;
	    Debug.Log("Start has finished " + i + " times.");
	    i++;

	}


}
	

    
