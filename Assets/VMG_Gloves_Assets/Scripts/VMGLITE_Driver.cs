using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;




public class VMGLITEValues
{
    public float q0h, q1h, q2h, q3h;    //!< hand quaternion representing hand orientation
    
    public float rollH, pitchH, yawH;   //!< hand orientation;
    
    public int[] SensorValues = new int[Constants.NumSensors_LITE];      //!< all values from dataglove sensors

    public int timestamp;   //!< values representing package generation tick in ms

    public void ResetValues()
    {
        //reset allsensor values
        q0h = 0.0f; q1h = 0.0f; q2h = 0.0f; q3h = 0.0f;
        
        rollH = 0.0f; pitchH = 0.0f; yawH = 0.0f;
        
        int i = 0;
        for (i = 0; i < Constants.NumSensors_LITE; i++) SensorValues[i] = 0;
        timestamp = 0;
    }
}

public class VMGLITE_Driver
{
    private int ComPort = 1;                                //!< comport used for dataglove communication
    private int GloveType = Constants.RightHanded;          //!< Rightor Left Handed
    private int GloveStreamMode = Constants.PKG_QUAT_FINGER;  //!< streaming package mode

    public VMGLITEValues sensorValues = new VMGLITEValues();

    private bool _newPackageAvailable;

    private bool ComThreadRunning = false;
    private Thread comThread;

    private object _lock = new object();

    private float YAW0; //set initial yaw angle in order to correctly align dataglove to the environment
    private Quaternion q0;

    //imu values mean filtering
    private int IMUFilter;
    List<Quaternion> lQuatH = new List<Quaternion>();

    /*** driver initialization 
     /pars comport dataglove communication port
     /pars type dataglove type (RightHanded or LeftHanded)
     /pars stream streaming type
     ***/
    public void Init(int comport, int type, int stream)
    {
        ComPort = comport;
        GloveType = type;
        GloveStreamMode = stream;
        sensorValues.ResetValues();
        _newPackageAvailable = false;
        IMUFilter = FilterConst.Filter_High;
    }

    /*** Start dataglove communication */
    public void StartCommunication()
    {
        //open comport

        //start stream reading thread
        ComThreadRunning = true;
        comThread = new Thread(GloveCommunication) { Name = "GloveCommunication" };
        comThread.Start();
    }

    public void SetYaw0(float yaw0)
    {
        YAW0 = yaw0;
    }

    public float GetYaw0()
    {
        return YAW0;
    }

    public Quaternion GetQ0()
    {
        return q0;
    }

    public void StopCommunication()
    {
        ComThreadRunning = false;
    }

    /*** return true if a new package is available from the streaming */
    public bool NewPackageAvailable()
    {
        bool retval;
        lock (_lock)
        {
            retval = _newPackageAvailable;
            _newPackageAvailable = false;
        }
        return retval;
    }

    public VMGLITEValues GetPackage()
    {
        VMGLITEValues ret;
        lock (_lock)
        {
            ret = sensorValues;
        }
        return ret;
    }

    public void SetIMUFilter(int value)
    {

    }

    private void GloveCommunication()
    {
        bool FirstPackage = true;
        byte[] SendBuffer = new byte[256];
        byte[] RecvBuffer = new byte[1024];
        int NumBytesRecv = 0;
        int NumPkgRecv = 0;
        SerialPort vmgcom;
        bool vmgcomOk = false;

        string str; str = "COM" + ComPort;

        if (ComPort > 10) str = "\\\\.\\COM" + ComPort;

        Debug.Log("Open " + str + "\n");

        vmgcom = new SerialPort(str, Constants.BaudRate);
        vmgcom.Open();
        if (vmgcom.IsOpen)
        {
            Debug.Log("Serial port correctly opened\n");
            vmgcomOk = true;
        }
        else
        {
            Debug.Log("Serial port error\n");
            vmgcomOk = false;
        }

        //if comport opened succesfully then send start streaming command
        if (vmgcomOk)
        {
            vmgcom.ReadTimeout = 1;
            //send start streaming
            SendBuffer[0] = (byte)'$';
            SendBuffer[1] = (byte)0x0a;
            SendBuffer[2] = (byte)0x03;
            SendBuffer[3] = (byte)Constants.PKG_QUAT_FINGER_LITE;
            SendBuffer[4] = (byte)(SendBuffer[0] + SendBuffer[1] + SendBuffer[2] + SendBuffer[3]);
            SendBuffer[5] = (byte)'#';
            vmgcom.Write(SendBuffer, 0, 6);
        }

        //Debug.Log("Thread Started\n");

        while (ComThreadRunning)
        {
            //Debug.Log("Read Bytes\n");
            try
            {
                //read bytes from the dataglove stream
                int bytesRead = vmgcom.Read(RecvBuffer, NumBytesRecv, 10);
                if (bytesRead > 0)
                {
                    NumBytesRecv += bytesRead;
                    //check if a valid package is present in the buffer
                    if (NumBytesRecv > Constants.VMG30_PKG_SIZE)
                    {
                        //check header
                        int i = 0;
                        byte bcc = 0;
                        bool HeaderFound = false;
                        while ((!HeaderFound) && (i < (NumBytesRecv - 2)))
                        {
                            if ((RecvBuffer[i] == '$') && (RecvBuffer[i + 1] == 0x0a)) HeaderFound = true;
                            else i++;
                        }
                        //if header found then parse package
                        if (HeaderFound)
                        {
                            //i == pos header
                            //bcc
                            int pospackage = i + 2;
                            bcc = ((byte)'$') + 0x0a;
                            //package len
                            byte pkglen = RecvBuffer[pospackage];
                            if ((pkglen + pospackage) < NumBytesRecv)
                            {
                                //package found
                                //see dataglove datasheet for package definitions

                                //check bcc
                                byte bccrecv = RecvBuffer[pospackage + pkglen - 1];
                                for (i = 0; i < pkglen - 1; i++)
                                {
                                    bcc += RecvBuffer[pospackage + i];
                                }

                                //if bcc is correct and package termination found then check for sensors values
                                if ((bcc == bccrecv) && (RecvBuffer[pospackage + pkglen] == '#'))
                                {
                                    //parse package
                                    int datastart = pospackage + 1;

                                    //check initial information, package type, glove id and package timestamp
                                    int pkgtype = RecvBuffer[datastart]; datastart++;
                                    int id = RecvBuffer[datastart] * 256 + RecvBuffer[datastart + 1]; datastart += 2;
                                    int timestamp = (RecvBuffer[datastart] << 24) + (RecvBuffer[datastart + 1] << 16) + (RecvBuffer[datastart + 2] << 8) + (RecvBuffer[datastart + 3]); datastart += 4;

                                    //Debug.Log("ID:" + id + " Time:" + timestamp + "\n");


                                    if (pkgtype == Constants.PKG_QUAT_FINGER_LITE)
                                    {
                                  
                                        int q0h = (RecvBuffer[datastart] << 24) + (RecvBuffer[datastart + 1] << 16) + (RecvBuffer[datastart + 2] << 8) + (RecvBuffer[datastart + 3]); datastart += 4;
                                        int q1h = (RecvBuffer[datastart] << 24) + (RecvBuffer[datastart + 1] << 16) + (RecvBuffer[datastart + 2] << 8) + (RecvBuffer[datastart + 3]); datastart += 4;
                                        int q2h = (RecvBuffer[datastart] << 24) + (RecvBuffer[datastart + 1] << 16) + (RecvBuffer[datastart + 2] << 8) + (RecvBuffer[datastart + 3]); datastart += 4;
                                        int q3h = (RecvBuffer[datastart] << 24) + (RecvBuffer[datastart + 1] << 16) + (RecvBuffer[datastart + 2] << 8) + (RecvBuffer[datastart + 3]); datastart += 4;

                                        //get fingers values
                                        int[] sensors = new int[Constants.NumSensors];
                                        for (i = 0; i < Constants.NumSensors; i++)
                                        {
                                            sensors[i] = (RecvBuffer[datastart] << 8) + RecvBuffer[datastart + 1]; datastart += 2;
                                        }

                                        //convert quaternions to float
                                        float q00H = (float)(q0h / 65536.0);
                                        float q11H = (float)(q1h / 65536.0);
                                        float q22H = (float)(q2h / 65536.0);
                                        float q33H = (float)(q3h / 65536.0);

                                       
                                        lQuatH.Add(new Quaternion(q00H, q11H, q22H, q33H));

                                        if (IMUFilter > 0)
                                        {
                                            
                                            if (lQuatH.Count > IMUFilter)
                                            {
                                                lQuatH.RemoveAt(0);
                                            }

                                            //hand quanternion
                                            float q0hsum = 0.0f, q1hsum = 0.0f, q2hsum = 0.0f, q3hsum = 0.0f;
                                            int numval = lQuatH.Count;
                                            for (i = 0; i < numval; i++)
                                            {
                                                Quaternion q = lQuatH[i];
                                                q0hsum += q.x;
                                                q1hsum += q.y;
                                                q2hsum += q.z;
                                                q3hsum += q.w;
                                            }

                                            q00H = q0hsum / numval;
                                            q11H = q1hsum / numval;
                                            q22H = q2hsum / numval;
                                            q33H = q3hsum / numval;


                                        }

                                        Quaternion qh = new Quaternion(q22H, q11H, q33H, q00H);

                                        Vector3 euler = qh.eulerAngles;

                                        float rollH = euler[1];
                                        float pitchH = euler[0];
                                        float yawH = euler[2];
                                        //compute hand roll pitch and yaw
                                        //float rollH = -Mathf.Rad2Deg * Mathf.Atan2(2.0f * (q00H * q11H + q22H * q33H), 1.0f - 2.0f * (q11H * q11H + q22H * q22H));
                                        //float pitchH = -Mathf.Rad2Deg * Mathf.Asin(2.0f * (q00H * q22H - q33H * q11H));
                                        //float yawH = Mathf.Rad2Deg * Mathf.Atan2(2.0f * (q00H * q33H + q11H * q22H), 1.0f - 2.0f * (q22H * q22H + q33H * q33H));



                                        if (pitchH >= 180.0f) pitchH = -360.0f + pitchH;
                                        if (pitchH <= -180.0f) pitchH = 360.0f + pitchH;
                                       
                                        if (yawH >= 180.0f) yawH = -360.0f + yawH;
                                        if (yawH <= -180.0f) yawH = 360.0f + yawH;

                                        if (rollH >= 180.0f) rollH = -360.0f + rollH;
                                        if (rollH <= -180.0f) rollH = 360.0f + rollH;


                                        if (FirstPackage)
                                        {
                                            FirstPackage = false;
                                            q0 = Quaternion.Euler(rollH, pitchH, -yawH);
                                            SetYaw0(yawH);
                                        }



                                        //update sensor values (protected)
                                        lock (_lock)
                                        {
                                            sensorValues.timestamp = timestamp;


                                            sensorValues.pitchH = pitchH;
                                            sensorValues.rollH = rollH;
                                            sensorValues.yawH = yawH;// -YAW0;

                                            

                                            sensorValues.q0h = q00H;
                                            sensorValues.q1h = q11H;
                                            sensorValues.q2h = q22H;
                                            sensorValues.q3h = q33H;

                                          


                                            for (i = 0; i < Constants.NumSensors_LITE; i++)
                                            {
                                                sensorValues.SensorValues[i] = sensors[i];
                                            }

                                            _newPackageAvailable = true;
                                        }
                                    }
                                    NumPkgRecv++;
                                }
                                // Debug.Log("PKGRECV: " + NumPkgRecv + "\n");

                                //shift streaming buffer
                                int finpos = pospackage + pkglen;
                                int bytesrem = NumBytesRecv - finpos - 1;
                                for (i = 0; i < bytesrem; i++)
                                {
                                    RecvBuffer[i] = RecvBuffer[finpos + 1 + i];
                                }
                                NumBytesRecv = bytesrem;


                            }
                        }
                        else
                        {
                            Debug.Log("Header not found\n");
                            NumBytesRecv = 0;
                        }
                    }
                }
            }
            catch
            {
                //serial port generates an exeption, do nothing
            }
        }

        //trhead completed, send cend streaming and close the port
        if (vmgcomOk)
        {
            vmgcom.ReadTimeout = 1;
            //send start streaming
            SendBuffer[0] = (byte)'$';
            SendBuffer[1] = (byte)0x0a;
            SendBuffer[2] = (byte)0x03;
            SendBuffer[3] = (byte)Constants.PKG_NONE;
            SendBuffer[4] = (byte)(SendBuffer[0] + SendBuffer[1] + SendBuffer[2] + SendBuffer[3]);
            SendBuffer[5] = (byte)'#';
            vmgcom.Write(SendBuffer, 0, 6);
        }
        //Debug.Log("Thread end\n");
        vmgcom.Close();
    }
}