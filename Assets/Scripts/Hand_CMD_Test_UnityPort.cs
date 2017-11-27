using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System.IO.Ports;
using Random = System.Random;
using System;
using System.Threading;

public class Sean
{
	static private SerialPort handPortL;
	static private SerialPort handPortR;
	
	public static Random rnd = new Random();
	// Use this for initialization
	public static void Init () {
		handPortL = new SerialPort("\\\\.\\COM255", 115200);
		handPortR = new SerialPort("\\\\.\\COM256", 115200);
		
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
				Console.WriteLine("!!! COM PORTS Left Did Open " + i + " times !!!");
				i++;
			}
			catch (Exception e)
			{
				Console.WriteLine("Could not open comport for hands" + e);
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
				Console.WriteLine("!!! COM PORTS Right Did Open " + i + " times !!!");
				i++;
			}                                                        
			catch (Exception e)                          
			{                                                        
				Console.WriteLine("Could not open comport for hands" + e);
			}                                                        
                                                             
		}
		
		try
		{
			Console.WriteLine("Try Start Ran ");
			handPortL.WriteLine("500,500,500,500"); //change to write?
			handPortR.WriteLine("500,500,500,500");

			handPortL.DiscardOutBuffer();
			handPortL.DiscardInBuffer();
			handPortR.DiscardOutBuffer();
			handPortR.DiscardInBuffer();
            
			var i = 1;
			Console.WriteLine("Wrote to L and R " + i + " Times");
			i++;
		}
		catch (Exception e)
		{
			Console.WriteLine("Stuff sucks on start " + e);

		}
	}

	 public static void updatePosition()
	{
		string temp = (rnd.Next(100, 700)).ToString() + "," + (rnd.Next(100, 700)).ToString() 
		              + "," + (rnd.Next(100, 700)).ToString() + "," + (rnd.Next(100, 700)).ToString();
		Console.WriteLine(temp);

		handPortL.WriteLine(temp); //change to write?
		handPortR.WriteLine(temp);

		handPortL.DiscardOutBuffer();
		handPortL.DiscardInBuffer();
		handPortR.DiscardOutBuffer();
		handPortR.DiscardInBuffer();
	}

	// Update is called once per frame
	public static int Main ()
	{
		Init();
		while (true)
		{
			//var i = 1;
			//Console.WriteLine("In Loop for the " + i + "th time.");
			Thread.Sleep(500);
			updatePosition();
		}

		return 0;
	}
}


public class Hand_CMD_Test_UnityPort : MonoBehaviour
{
	void Start()
	{
		Debug.Log("In Start");
		new Sean();
		Debug.Log("In Start");
		Sean.Main();
		
	}
	void update()
	{

	}

}



