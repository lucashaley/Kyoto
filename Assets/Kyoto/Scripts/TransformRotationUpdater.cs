using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    public class TransformRotationUpdater : MonoBehaviour
    {
        public enum TransformType { Translate, Rotate }
        public enum TransformDirection { Forward, Back, Left, Right, Up, Down };

        public TransformType type;
        public TransformDirection axis;
        public FloatVariable variable;

        public void UpdateTransform()
        {
            Vector3 dir = Vector3.zero;
            switch (axis)
            {
                case TransformDirection.Forward:
                    dir = Vector3.forward;
                    break;
                case TransformDirection.Back:
                    dir = Vector3.back;
                    break;
                case TransformDirection.Left:
                    dir = Vector3.left;
                    break;
                case TransformDirection.Right:
                    dir = Vector3.right;
                    break;
                case TransformDirection.Up:
                    Debug.Log("Direction: Up");
                    dir = Vector3.up;
                    break;
                case TransformDirection.Down:
                    dir = Vector3.down;
                    break;
            }

            switch (type)
            {
                case TransformType.Translate:
                    // This can be relative or absolute?
                    // Also, what about a Vector3 input? How would that work for Rotation?
                    // transform.localRotation = Quaternion.Euler(dir * variable.Value);
                    break;
                case TransformType.Rotate:
                    // Debug.Log("TransformRotationUpdater: Rotate", this);
                    Debug.Log("TransformRotationUpdater.Rotate: " + (dir * variable.Value), this);
                    transform.localRotation = Quaternion.Euler(dir * variable.Value);
                    break;
            }
        }
    }
}
