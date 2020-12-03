using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityAtoms.BaseAtoms;

public class CameraManager : MonoBehaviour
{
    public bool isTweening;
    public FloatConstant rotateDuration;
    public Transform userViewPivot;
    public Transform stateViewPivot;

    // REFACTOR this into an input system
    void Update()
    {
        // use keyboard
        float i = Input.GetAxis("RotateView");
        if (i != 0f)
        {
            if (i > 0)
            {
                // RotateViewCW();
                RotateUserViewCW();
            } else {
                // RotateViewCCW();
                RotateUserViewCCW();
            }
        }
    }

    public void RotateUserView(Vector3 axis)
    {
        if (!isTweening)
        {
            Tween.Rotate (userViewPivot, axis*90f, Space.Self, rotateDuration.Value, 0f, Tween.EaseInOutStrong, Tween.LoopType.None, ()=>isTweening=true, ()=>isTweening=false);
        }
    }

    public void RotateUserViewCCW()
    {
        RotateUserView(Vector3.up);
    }

    public void RotateUserViewCW()
    {
        RotateUserView(Vector3.down);
    }

    public void SetPlayStateCameraAngle(Vector3 newAngle)
    {
        Debug.Log("SetPlayStateCameraAngle: " + newAngle);
        Tween.LocalRotation (stateViewPivot, newAngle, rotateDuration.Value, 0, Tween.EaseInOutStrong, Tween.LoopType.None, ()=>isTweening=true, ()=>isTweening=false);
    }
}
