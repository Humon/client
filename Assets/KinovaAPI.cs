using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class KinovaAPI : MonoBehaviour
{

  // TODO: Give external functions prefix to easily identify them as such (e.g., extern_InitRobot)
  //https://stackoverflow.com/questions/7276389/confused-over-dll-entry-points-entry-point-not-found-exception
  [DllImport ("ARM_base_32", EntryPoint = "InitRobot")]
  private static extern int _InitRobot ();

  [DllImport ("ARM_base_32", EntryPoint = "MoveArmHome")]
  private static extern int _MoveArmHome (bool rightArm);

  [DllImport ("ARM_base_32", EntryPoint = "MoveHand")]
  private static extern int _MoveHand (bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ);

  [DllImport ("ARM_base_32", EntryPoint = "MoveHandNoThetaY")]
  private static extern int _MoveHandNoThetaY (bool rightArm, float x, float y, float z, float thetaX, float thetaZ);

  [DllImport ("ARM_base_32", EntryPoint = "MoveFingers")]
  private static extern int _MoveFingers (bool rightArm, bool pinky, bool ring, bool middle, bool index, bool thumb);

  [DllImport ("ARM_base_32", EntryPoint = "CloseDevice")]
  private static extern int _CloseDevice (bool rightArm);

  [DllImport ("ARM_base_32", EntryPoint = "StopArm")]
  private static extern int _StopArm (bool rightArm);

  [DllImport ("ARM_base_32", EntryPoint = "MoveArmAngularVelocity")]
  private static extern int _MoveArmAngularVelocity (bool rightArm, float av1=0, float av2=0, float av3=0, float av4=0, float av5=0, float av6=0, float av7=0);

    [DllImport("ARM_base_32", EntryPoint = "MoveArmAngularVelocityLooped")]
    private static extern int _MoveArmAngularVelocityLooped(bool rightArm, int iterations, float av1 = 0, float av2 = 0, float av3 = 0, float av4 = 0, float av5 = 0, float av6 = 0, float av7 = 0);

    [DllImport("ARM_base_32", EntryPoint = "MoveArmAngularPosition")]
    private static extern int _MoveArmAngularPosition(bool rightArm, float ap1 = 0, float ap2=0, float ap3=0, float ap4=0, float ap5=0, float ap6=0, float ap7=0);

    [DllImport("ARM_base_32", EntryPoint = "CartesianPosition_MoveRelative")]
    private static extern int _CartesianPosition_MoveRelative(bool rightArm, float X, float Y, float Z, float ThetaX, float ThetaY, float ThetaZ);

    [DllImport("ARM_base_32", EntryPoint = "ClearAllTrajectories")]
    private static extern int _ClearAllTrajectories();



    private static bool initSuccessful = false;

  public static void InitRobot ()
  {
    Debug.Log ("trying to init robot...");
	if (initSuccessful) {
	  Debug.Log ("Already initialized");
	  return;
	}
	int errorCode = _InitRobot ();
	switch (errorCode) {
	case 0:
	  Debug.Log ("Kinova robotic arm loaded and device found");
	  initSuccessful = true;
	  break;
	case -1:
	  Debug.LogError ("Robot APIs troubles");
	  break;
	case -2:
	  Debug.LogError ("Robot - no device found");
	  break;
	case -3:
	  Debug.LogError ("Robot - more devices found - not sure which to use");
	  break;
	case -10:
	  Debug.LogError ("Robot APIs troubles: InitAPI");
	  break;
	case -11:
	  Debug.LogError ("Robot APIs troubles: CloseAPI");
	  break;
	case -12:
	  Debug.LogError ("Robot APIs troubles: SendBasicTrajectory");
	  break;
	case -13:
	  Debug.LogError ("Robot APIs troubles: GetDevices");
	  break;
	case -14:
	  Debug.LogError ("Robot APIs troubles: SetActiveDevice");
	  break;
	case -15:
	  Debug.LogError ("Robot APIs troubles: GetAngularCommand");
	  break;
	case -16:
	  Debug.LogError ("Robot APIs troubles: MoveHome");
	  break;
	case -17:
	  Debug.LogError ("Robot APIs troubles: InitFingers");
	  break;
	case -18:
	  Debug.LogError ("Robot APIs troubles: StartForceControl");
	  break;
    case -19:
	  Debug.LogError ("Robot APIs troubles: MoveArmAngularVelocity");
	  break;
	case -20:
	  Debug.LogError ("Robot APIs troubles: MoveArmAngularPosition");
	  break;
	case -123:
	  Debug.LogError ("Robot APIs troubles: Command Layer Handle");
	  break;
	default:
	  Debug.LogError ("Robot - unknown error from initialization");
	  break;
	}
  }

    public static int ClearAllTrajectories()
    {
        _ClearAllTrajectories();
        return 0;
    }

    public static int MoveArmAngularVelocity (bool rightArm, float av1, float av2, float av3, float av4, float av5, float av6, float av7)
  {
        if(initSuccessful) 
        {
           // Debug.LogError("Init was successful in MoveArmAngularVelocity");
            _MoveArmAngularVelocity (rightArm,av1,av2,av3,av4,av5,av6,av7);                    
        }
        return 0;
  }

    public static int MoveArmAngularVelocityLooped(bool rightArm, int iterations, float av1, float av2, float av3, float av4, float av5, float av6, float av7)
    {
        if (initSuccessful)
        {
            var imdifficult = _MoveArmAngularVelocityLooped(rightArm, iterations, av1, av2, av3, av4, av5, av6, av7);
            Debug.LogError("MovedArmAmgularVelocityLooped called with " + imdifficult);
        }
        return 0;
    }
    public static int MoveArmAngularPosition (bool rightArm, float ap1=0, float ap2=0, float ap3=0, float ap4=0, float ap5=0, float ap6=0, float ap7=0)
  {
        if(initSuccessful) 
        {
           _MoveArmAngularPosition (rightArm,ap1,ap2,ap3,ap4,ap5,ap6,ap7);                    
        }
        return 0;
  }

    public static int MoveArmCartesianPositionRelative(bool rightArm, float X, float Y, float Z, float ThetaX, float ThetaY, float ThetaZ)
    {
        if (initSuccessful)
        {
            _CartesianPosition_MoveRelative(rightArm, X, Y, Z, ThetaX, ThetaY, ThetaZ);
        }
        return 0;
    }

    public static void StopArm (bool rightArm)
  {
	if (initSuccessful) {
      _StopArm (rightArm);
	}
  }

  public static void MoveArmHome (bool rightArm)
  {
	if (initSuccessful) {
	  _MoveArmHome (rightArm);
	}
  }

  public static void MoveHand (bool rightArm, float x, float y, float z, float thetaX, float thetaY, float thetaZ)
  {
	if (initSuccessful) {

            Debug.LogError("Init was successful in MoveHand");
            _MoveHand (rightArm, x, y, z, thetaX, thetaY, thetaZ);
	}
  }

  public static void MoveHandNoThetaY (bool rightArm, float x, float y, float z, float thetaX, float thetaZ)
  {
	if (initSuccessful) {
	  _MoveHandNoThetaY (rightArm, x, y, z, thetaX, thetaZ);
	}
  }

  public static void MoveFingers (bool rightArm, bool pinky, bool ring, bool middle, bool index, bool thumb)
  {
	if (initSuccessful) {
	  _MoveFingers (rightArm, pinky, ring, middle, index, thumb);
	}
  }


  /**@brief OnApplicationQuit() is called when application closes.
   * 
   * section DESCRIPTION
   * 
   * OnApplicationQuit(): Is called on all game objects before the 
   * application is quit. In the editor it is called when the user 
   * stops playmode. This function is called on all game objects 
   * before the application is quit. In the editor it is called 
   * when the user stops playmode.
   */
  private void OnApplicationQuit ()
  {
	if (initSuccessful) {
	  Debug.Log("Closing Robot API...");
	  _CloseDevice (false);
	}
  }
}