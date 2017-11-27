using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Random = System.Random;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;

public class roboHand_Manager : MonoBehaviour
{

	private float delayTime = 0.0f;
	
	public class Hand_CMD
	{
		private static SerialPort handPortL;
		private static SerialPort handPortR;
		public static Random rnd = new Random();
		static Stopwatch stopWatch = new Stopwatch();

		public static String inputCommandL = "255,255,255,255";
		public static String inputCommandR = "255,255,255,255";
		
		public static int loops = 1;
	
		public static void HandLoop()
		{
			stopWatch.Start();
			TimeSpan ts = stopWatch.Elapsed;
			//Init(); //Initialize COMPORTS Baud 115200 COM255 = L COM256 = R    

			//Thread.Sleep(500);
			WriteToHands(inputCommandL, inputCommandR);
			ts = stopWatch.Elapsed;
			PrintTime(ts);
			UnityEngine.Debug.Log("We have been through the Main() function " + loops + " times.");
			UnityEngine.Debug.Log("=================================================================");
			loops++;
			//return 0; //Should never execute as currently implemented.
		}// Main
	
		public static void Init() //can cause uncaught exception in system.dll if comports are not open
		{                          // another good reason to use device id's
								   /*============================================================================================\ 
								   | *Preconditions: Serial Ports are addressed to 255==LHand and 256==Rhand in A4 Fast CSV mode |
								   | *PostConditions: Leaves hands in 500,500,500,500 position with COM PORTS in BAUD 115200     |
								   \============================================================================================*/
			handPortL = new SerialPort("\\\\.\\COM255", 115200);
			handPortR = new SerialPort("\\\\.\\COM256", 115200);
	
			if (!handPortL.IsOpen)
			{
				try
				{
					handPortL.Open();
					handPortL.BaudRate = 115200;
					handPortL.WriteLine("H2");
					ClearBuffers("Left");
					var i = 1;
					UnityEngine.Debug.Log("End of handPortL init(), which executed a total of " + i + " times.");
					i++;
				}
				catch (Exception e)
				{
					UnityEngine.Debug.Log("Could not open comport for hands." + e);
				}
			}
	
			if (!handPortR.IsOpen)
			{
				try
				{
					handPortR.Open();
					handPortR.BaudRate = 115200;
					handPortR.WriteLine("H1");
					ClearBuffers("Right");
					var i = 1;
					UnityEngine.Debug.Log("End of handPortR init(), which executed a total of" + i + " times.");
					i++;
				}
				catch (Exception e)
				{
					UnityEngine.Debug.Log("Could not open comport for hands" + e);
				}
	
			}
			try
			{
				UnityEngine.Debug.Log("Init()'s WriteLine() Try Block Begin");
				handPortL.Write("500,500,500,500\n");
				handPortR.Write("500,500,500,500\n");
				ClearBuffers("Both");
				var i = 1;
				UnityEngine.Debug.Log("Wrote to L and R in Init() " + i + " Times");
				i++;
			}
			catch (Exception e)
			{
				UnityEngine.Debug.Log("Caught error in Init()'s WriteLine Try Block" + e);
	
			}
		}// Init
	
		private static int WriteToHands(String lCommand, String rCommand) //Precondition: Hands are initialized / COM Ports == Open
										  /*======================================================================================\ 
										  | * Precondition: Hands are init to baudrt 112500 / COM Ports Open LH=COM255 RH=COM256  |
										  | * Postcondition: Hands are written a random value from 100-800, buffers cleared after |
										  \======================================================================================*/
		{
			string temp = (rnd.Next(100, 700)).ToString() + "," + (rnd.Next(100, 700)).ToString() + ","
				+ (rnd.Next(100, 700)).ToString() + "," + (rnd.Next(100, 700)).ToString();
			UnityEngine.Debug.Log(temp);
			handPortL.Write(temp + "\n"); //changed to write()
			handPortR.Write(temp + "\n");
			ClearBuffers("Both");
			return 0;
		} // WriteToHands
	
		private static int ClearBuffers(String hand)
		{
			/*======================================================================================\ 
			| * Clears serial buffers for both hands in try catch blocks.                           |
			| * PostConditions: returns 1 for error in L, -1 for R, otherwise 0                     |
			\======================================================================================*/
	
			if(hand == "Left" || hand == "Both"){
					try
					{
						handPortL.DiscardOutBuffer();
						handPortL.DiscardInBuffer();
						
					}
					catch (Exception e)
					{
						UnityEngine.Debug.Log("ClearBuffers() L failed");
						return 1; //if one fails they all will fail, should write this seperate -- lazy.
						
					}
			
			}
	
			if(hand == "Right" || hand == "Both"){
					try
					{
						handPortR.DiscardOutBuffer();
						handPortR.DiscardInBuffer();
					}
					catch (Exception e)
					{
						UnityEngine.Debug.Log("Error in ClearBuffers() R");
						return -1;
					}
			
			}
			return 0;
		}// ClearBuffers
	
		public static string PrintTime(TimeSpan ts)
		{
			/*====================================================================================\ 
			| * Preconditions - Needs a non-null TimeSpan value                                   |
			| * Postconditions - prints a formatted string to stdout, and returns the same string.|
			\====================================================================================*/
			string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
				ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);
			UnityEngine.Debug.Log("RunTime " + elapsedTime);
			return elapsedTime;
		}// PrintTime
	} //End Class

	// Use this for initialization
	void Start ()
	{
		new Hand_CMD();
		Hand_CMD.Init();
	}
	
	// Update is called once per frame
	void Update ()
	{
		delayTime += Time.deltaTime;
		if (delayTime > .5f)
		{
			Hand_CMD.HandLoop();
			delayTime = 0.0f;
		}
		
	}
}
