using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    public class Transform2dInt : MonoBehaviour
    {
        private Vector2Int _position;
        public  Vector2Int Position
        {
            get => GetComponent<Transform>().position.Vector2IntNoY();
        }
        public Vector2Int Rotation;
        public Vector2Int Scale;

        void OnValidate()
        {

        }
    }
}
