using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    public class PlayState : State
    {
        public PlayStateObject playState;

        public void SetStateView()
        {
            Debug.Log("SetStateView: " + playState);
        }
    }
}
