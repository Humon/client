using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;
using System.IO.Ports;
using Random = System.Random;
using System;


public class HandHandler
{
    private static int[] roboHandData = new int[10];
    
    public static Random rnd = new Random();

    public static bool doStart = true;

    public static bool doTest = true;

    static SerialPort handPortR;
    static SerialPort handPortL;
    // Use this for initialization
    public static void Start()
    {
        Debug.Log("Start can run");
        try
        {

            handPortL = new SerialPort("\\\\.\\COM255", 115200);
            handPortR = new SerialPort("\\\\.\\COM256", 115200);
            Debug.Log("Hand ports open ");
        }
        catch (System.Exception e)
        {
            Debug.Log("Hand ports could not open " + e);
        }
        
        if (!handPortL.IsOpen)
        {
            try
            {
                handPortL.Open();
                handPortL.BaudRate = 115200;

                handPortL.WriteLine("H2");

                handPortL.DiscardOutBuffer();
            }
            catch (System.Exception e)
            {
                Debug.Log("Could not open comport for hands" + e);
            }

        }
        
        if (!handPortR.IsOpen)                                       
        {                                                            
            try                                                      
            {                                                        
                handPortR.Open();                                    
                handPortR.BaudRate = 115200;                         
                                                             
                handPortR.WriteLine("H1");                           
                                                             
                handPortR.DiscardOutBuffer();                        
            }                                                        
            catch (System.Exception e)                          
            {                                                        
                Debug.Log("Could not open comport for hands" + e);
            }                                                        
                                                             
        }
        
        try
        {
            Debug.Log("Try Start Ran ");
            handPortL.WriteLine("500,500,500,500");
            handPortR.WriteLine("500,500,500,500");

            handPortL.DiscardOutBuffer();
            handPortR.DiscardOutBuffer();
        }
        catch (System.Exception e)
        {
            Debug.Log("Stuff sucks on start " + e);

        }
    }

    // Update is called once per frame

	public static int Main()
	{
		while (true)
		{


		}

		return 0;


	}


	/*
    
    TESTING!!!
    
    
    if (doStart)
    {
        Start();
        doStart = false;
    }

    while (doTest)
    {
        Thread.Sleep(100); 
        if (true)
        {
            string temp = "";
            for (int i = 0; i < 4; i++)
            {
                roboHandData[i] = (int) ((rnd.Next(400, 600)) + 10);

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
                handPortR.WriteLine(temp); 
            }
            catch (System.Exception e)
            {
                //Debug.Log("exception, Left Hand " + e);
            }

            handPortL.DiscardOutBuffer();
            handPortR.DiscardOutBuffer();

        }

    }
    return(0);
    
    END TESTING!!!

}
*/
    public static void MoveToPosition_R(String handCommand)
    {
        //handPortR.WriteLine(handCommand); 
	    handPortR.Write(handCommand);
        handPortR.DiscardOutBuffer();
    }
    
    public static void MoveToPosition_L(String handCommand)
    {
        //handPortL.WriteLine(handCommand);
	    handPortL.Write(handCommand);
        handPortL.DiscardOutBuffer();
    }

}



public class Hand_Initializer : MonoBehaviour
{
    private float delayTime = 0.0f;

    public static String LeftHand = "100,200,300,400,";
    public static String RightHand = "100,200,300,400,";

	// Use this for initialization
	void Start ()
	{
	    new HandHandler();
	    HandHandler.Start();
	    
	}
	
	// Update is called once per frame
	void Update ()
	{
	    string temp = "";

	    int[] roboHandData = new int[4];
	  
	    for (int i = 0; i < 4; i++)
	    {
	        roboHandData[i] = (int) ((UnityEngine.Random.value * 400.0f) + 100);

	        if (i != 3)
	        {
	            temp += roboHandData[i].ToString() + ",";
	        }
	        else
	        {
	            temp += roboHandData[i].ToString();
	        }
	    }
		LeftHand = temp;
		RightHand = temp;
		
	    Debug.Log(LeftHand);
	    Debug.Log(RightHand);
	    
	    
	    delayTime += Time.deltaTime;
	    if (delayTime >= 0.5f)
	    {
	        try
	        {
	            Debug.Log("Try Update ran ");
	            HandHandler.MoveToPosition_L(LeftHand + "\n");
	            HandHandler.MoveToPosition_R(RightHand + "\n");
	        }
	        catch(System.Exception e)
	        {
	            Debug.Log("Stuff sucks " + e);

	        }
	        delayTime = 0.0f;
	    }

	}
}
