using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    [CreateAssetMenu]
    public class PlayStateObject : ScriptableObject
    {
        public Vector3 cameraView;
        public bool canPlace;
        public bool canRake;
    }
}
