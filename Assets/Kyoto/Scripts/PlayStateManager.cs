using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    public class PlayStateManager : StateMachine
    {
        public Vector3Variable playStateCameraAngle;
        public BoolVariable playStateCanPlace;
        public BoolVariable playStateCanRake;

        void Start()
        {
            Physics.queriesHitTriggers = false;
        }

        public void HandleStateEntered (GameObject state)
    	{
    		Debug.Log ("Entered the " + state.name + " state!");
            playStateCameraAngle.Value = state.GetComponent<PlayState>().playState.cameraView;
            playStateCanPlace.Value = state.GetComponent<PlayState>().playState.canPlace;
            playStateCanRake.Value = state.GetComponent<PlayState>().playState.canRake;
    	}
    }
}
