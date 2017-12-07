using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class RobotPoses {

    public static CartesianPosition readyToToast = new CartesianPosition(
        -0.6129154f,
        0.4196311f,
        -0.1113982f,
        2.732836f,
        -1.511767f,
        0.4928809f,
        2000,
        2000,
        2000
        );

    public static CartesianPosition readyToServerDrinks = new CartesianPosition(
         // Note that X and Z are flipped ?
         -0.4f,
        0.4196311f,
        .07f,
        3.04f,
        -.018f,
        3.011f,
        2000,
        2000,
        2000
        );
    // -0.5129154, 0.3196311, -0.1113982, 2.732836, -1.511767, -0.4928809 // these values place the arm front and center ready to toast.

    public static CartesianPosition readyToPushStartBlender = new CartesianPosition( // need pre pose so it doesn't make weird movements if arm is disoriented.
             // Note that X and Z are flipped ?
             -0.38f,
            0.34f,
            -0.06f,
            -2.09f,
            -0.20f,
            -2.70f,
            6970,
            6970,
            6970
            );
}


// this data structure should live in a DataStructures file.
public class CartesianPosition
{
    public float x = 0;
    public float y = 0;
    public float z = 0;
    public float thetaX = 0;
    public float thetaY = 0;
    public float thetaZ = 0;
    public float fp1 = 0;
    public float fp2 = 0;
    public float fp3 = 0;
    public CartesianPosition(
        float _x = -.5f,
        float _y = .3f,
        float _z = -0.1f,
        float _thetaX = 0f,
        float _thetaY = 0f,
        float _thetaZ = 0f,
        float _fp1 = 0f,
        float _fp2 = 0f,
        float _fp3 = 0f)
    {
        x = _x;
        y = _y;
        z = _z;
        thetaX = _thetaX;
        thetaY = _thetaY;
        thetaZ = _thetaZ;
        fp1 = _fp1;
        fp2 = _fp2;
        fp3 = _fp3;
    }
}


