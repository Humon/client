using UnityEngine;
using System;
using System.Diagnostics;
using System.Web;
using Random = System.Random;

public class SampleUserPolling_ReadWrite : MonoBehaviour
{
    public SerialController serialControllerL;
    public SerialController serialControllerR;
    static Stopwatch stopWatch = new Stopwatch();
    private TimeSpan ts;
    private long i;

    private static string Left = "F0P20\nF1P20\nF2P20\nF3P20";
    private static string Right = "F0P20\nF1P20\nF2P20\nF3P20";
    
    
    void Start()
    {
        serialControllerL = GameObject.Find("SerialControllerL").GetComponent<SerialController>();
        serialControllerR = GameObject.Find("SerialControllerR").GetComponent<SerialController>();
        stopWatch.Start();
    }
    void Update()
    {
        serialControllerL.SendSerialMessage(Left);
        serialControllerR.SendSerialMessage(Right);
        /*
        if (i % 2 == 0)
        {
            serialControllerL.SendSerialMessage("F0P70\nF1P70\nF2P70\nF3P70");
            serialControllerR.SendSerialMessage("F0P70\nF1P70\nF2P70\nF3P70");
        }
        else if (i % 2 == 1)
        {
            serialControllerL.SendSerialMessage("F0P20\nF1P30\nF2P40\nF3P50");
            serialControllerR.SendSerialMessage("F0P20\nF1P30\nF2P40\nF3P50");
        }
        */
        if (i % 50 == 0)
        {
            ts = stopWatch.Elapsed;
            PrintElapsedTime(ts);
        }
        i++;
        //UnityEngine.Debug.Log("We've been through the loop " + i + " times.");
    }
    public static string PrintElapsedTime(TimeSpan ts)
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
    
    public static void SetLeft(string cmd)
    {
        Left = cmd;

    }
    
    public static void SetRight(string cmd)
    {
        Left = cmd;

    }
}