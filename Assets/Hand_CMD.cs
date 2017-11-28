using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using Random = System.Random;
using System.Threading;
using System.IO.Ports;
using System.Diagnostics;
using System.Runtime.CompilerServices;

public class Hand_CMD : MonoBehaviour
{

	private float delayTime = 0.0f;
	
	public class Hand_CMD_Test
	{
		private static System.IO.Ports.SerialPort handPortL;
		private static System.IO.Ports.SerialPort handPortR;
		public static Random rnd = new Random();
		static Stopwatch stopWatch = new Stopwatch();

		public static String inputCommandL = "";
		public static String inputCommandR = "";
		
		private static Thread writeThread;
	
		public static void HandLoop()
		{
			stopWatch.Start();
			TimeSpan ts = stopWatch.Elapsed;
			int i = 1;
			//Init(); //Initialize COMPORTS Baud 115200 COM255 = L COM256 = R    

			//Thread.Sleep(500);
			WriteToHands(inputCommandL, inputCommandR);
			ts = stopWatch.Elapsed;
			PrintTime(ts);
			UnityEngine.Debug.Log("We have been through the Main() function " + i + " times.");
			UnityEngine.Debug.Log("=================================================================");
			i++;
			//return 0; //Should never execute as currently implemented.
		}// Main
	
		public static void Init() //can cause uncaught exception in system.dll if comports are not open
		{                          // another good reason to use device id's
								   /*============================================================================================\ 
								   | *Preconditions: Serial Ports are addressed to 255==LHand and 256==Rhand in A4 Fast CSV mode |
								   | *PostConditions: Leaves hands in 500,500,500,500 position with COM PORTS in BAUD 115200     |
								   \============================================================================================*/
			
			
			string s2 = "";
			string s1 = "";
			writeThread = new Thread(()=>WriteToHands(s1,s2));
			writeThread.Start();
			

			handPortL = new SerialPort("\\\\.\\COM255", 115200);
			handPortR = new SerialPort("\\\\.\\COM256", 115200);
			
	
			if (!handPortL.IsOpen)
			{
				try
				{
					handPortL.Open();
					handPortL.DtrEnable = true;
					handPortL.BaudRate = 115200;
					handPortL.WriteLine("H2");
					ClearBuffers("LeftHand");
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
					handPortR.DtrEnable = true;
					handPortR.BaudRate = 115200;
					handPortR.WriteLine("H1");
					ClearBuffers("RightHand");
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
				ClearBuffers("BothHands");
				var i = 1;
				UnityEngine.Debug.Log("Wrote to L and R in Init() " + i + " Times");
				i++;
			}
			catch (Exception e)
			{
				UnityEngine.Debug.Log("Caught error in Init()'s WriteLine Try Block" + e);
	
			}
		}// Init
	
		public static int WriteToHands(String lCommand, String rCommand) //Precondition: Hands are initialized / COM Ports == Open
										  /*======================================================================================\ 
										  | * Precondition: Hands are init to baudrt 112500 / COM Ports Open LH=COM255 RH=COM256  |
										  | * Postcondition: Hands are written a random value from 100-800, buffers cleared after |
										  \======================================================================================*/
		{
			while (true)
			{
				string temp = (rnd.Next(100, 700)).ToString() + "," + (rnd.Next(100, 700)).ToString() + ","
				              + (rnd.Next(100, 700)).ToString() + "," + (rnd.Next(100, 700)).ToString();
				//UnityEngine.Debug.Log(temp);
				handPortL.Write(temp + "\n"); //changed to write()
				handPortR.Write(temp + "\n");
				ClearBuffers("Both");

				Thread.Sleep(500);
			}
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
						
						//handPortL.Close();
						
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
						
						//handPortL.Close();
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
		new Hand_CMD_Test();
		Hand_CMD_Test.Init();
		Hand_CMD_Test.WriteToHands("", "");
	}
	
	// Update is called once per frame
	void Update ()
	{
		delayTime += Time.deltaTime;
		if (delayTime > .5f)
		{
			//Hand_CMD_Test.HandLoop();
			delayTime = 0.0f;
		}
		
	}
}
