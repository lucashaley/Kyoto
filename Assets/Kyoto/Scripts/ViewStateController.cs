using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Pixelplacement;

namespace Kyoto
{
    public class ViewStateController : StateMachine
    {
        public GameController gameController;
        public Transform cameraUserViewPivot;
        public Transform cameraStateViewPivot;
        public bool isRotating;

        void Awake ()
        {
            gameController = GameController.Instance;
            if (cameraUserViewPivot == null) {cameraUserViewPivot = GameObject.Find("CameraUserViewPivot").GetComponent<Transform>();}
            if (cameraStateViewPivot == null) {cameraStateViewPivot = GameObject.Find("CameraStateViewPivot").GetComponent<Transform>();}
        }

        // // Start is called before the first frame update
        void Start()
        {
            Assert.IsNotNull(gameController);
            Assert.IsNotNull(cameraUserViewPivot);
            Assert.IsNotNull(cameraStateViewPivot);
        }
        //
        // // Update is called once per frame
        // void Update()
        // {
        //
        // }

        public void SetStateView ()
        {
            if (!isRotating)
            {
                Tween.LocalRotation (cameraStateViewPivot, currentState.GetComponent<StateView>().viewData.Rotation, gameController.stateChangeTime, 0, Tween.EaseInOutStrong, Tween.LoopType.None, ()=>isRotating=true, ()=>isRotating=false);
            }
        }

        // do we still need this?
        public void SendStateMessage ()
        {
            GameObject.Find("World").BroadcastMessage(currentState.GetComponent<StateView>().message);
            Debug.Log(currentState.GetComponent<StateView>().message);
        }

        public void RotateViewLeft()
        {
            // currentState.GetComponent<StateViewing>().RotateViewLeft();
            if (!isRotating)
            {
                Tween.Rotate (cameraUserViewPivot, new Vector3 (0, 90, 0), Space.Self, gameController.stateChangeTime, 0f, Tween.EaseInOutStrong, Tween.LoopType.None, ()=>isRotating=true, ()=>isRotating=false);
            }

        }

        public void RotateViewRight()
        {
            // currentState.GetComponent<StateViewing>().RotateViewRight();
            if (!isRotating)
            {
                Tween.Rotate (cameraUserViewPivot, new Vector3 (0, -90, 0), Space.Self, gameController.stateChangeTime, 0f, Tween.EaseInOutStrong, Tween.LoopType.None, ()=>isRotating=true, ()=>isRotating=false);
            }
        }

        public string GetCurrentStateName()
        {
            return currentState.gameObject.name;
        }

        public void SetStateViewing()
        {
            ChangeState("ViewingState");
        }
        public void SetStateRaking()
        {
            ChangeState("RakingState");
        }
        public void SetStatePlacing()
        {
            ChangeState("PlacingState");
        }
    }
}
