using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pixelplacement;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    public class GridMover : MonoBehaviour
    {
        // Footprint of the Object
        public Vector2Int footprint = Vector2Int.one;

        public Transform moveBase, rotateBase;

        public bool fancy = true;
        public bool fromCenter = true;

        // States
        // REFACTOR not sure we need isMoving any more
        public bool isMoving;
        public bool isRotating;
        public bool isTweening;

        public FloatVariable fadeValue;
        public AnimationCurveVariable curve;

        public Renderer rend;

        public Vector3 rotationPoint;
        private float rotationStart;
        public Transform pivot;

        // What was this about? Creates nasty infinite loop
        // public Vector2IntEvent movePositionerEvent;
        public BoolEvent doneMoving;

        void Awake()
        {
            rend = GetComponentInChildren<Renderer>();
            // rotationPoint = transform.position + (footprint.Vector3NoY()/2);
            rotationPoint = footprint.Vector3NoY() * 0.5f;
            pivot = transform.Find("Pivot");
        }

        void StartTween()
        {
            isTweening = true;
            doneMoving.Invoke(false);
        }

        void EndTween()
        {
            isTweening = false;
            doneMoving.Invoke(true);
        }

        public void MoveToInt(Vector2Int destination)
        {
            if (!isTweening)
            {
                Tween.Position(transform,
                              destination.Vector3NoY(),
                              fadeValue.Value,
                              0.0f,
                              Tween.EaseInOutStrong,
                              Tween.LoopType.None,
                              // () => isTweening = true,
                              // () => isTweening = false
                              StartTween,
                              EndTween
                              );
                // REFACTOR this to a separate fader component?
                Tween.ShaderVector (rend.material,
                                    "_Color",
                                    new Vector4 (1, 1, 1, 0),
                                    fadeValue.Value,
                                    0,
                                    curve.curve,
                                    Tween.LoopType.None);
                Tween.LocalScale(transform.GetChild(0),
                                 new Vector3(0.85f, 0.85f, 0.85f),
                                 fadeValue.Value,
                                 0,
                                 curve.curve,
                                 Tween.LoopType.None
                                 );
            }
            // movePositionerEvent.Invoke(destination);
        }

        void HandleRotateAround(float value)
        {
            Debug.Log("RotateAround: " + value);
            // transform.RotateAround(transform.TransformPoint(rotationPoint),
            //                        Vector3.up,
            //                        value);
            pivot.RotateAround(pivot.TransformPoint(rotationPoint),
                                  Vector3.up,
                                  value);

            // REFACTOR: have a callback that sets the position etc to whole values
        }

        /// <remarks>
        /// We're going to assume 90 CW rotation.
        /// </remarks>
        public void RotateTo(Vector2Int destination)
        {

        }

        public void RotateByInt(int degrees)
        {
            if (!isTweening)
            {
                Transform t = rotateBase ? rotateBase : transform;

                // Debug.Log("Check if odd footprint: " + ((footprint.x + footprint.y)%2 == 1));
                // if ((footprint.x + footprint.y)%2 == 1)
                // {
                //     Debug.Log("Current position sum: " + (transform.localPosition.x + transform.localPosition.z));
                //     float currentPoint = transform.position.x + transform.position.z;
                //     Vector3 newPosition = Vector3.zero;
                //     if (currentPoint - (int)currentPoint == 0.5)
                //     {
                //         Debug.Log("MUNG");
                //         newPosition = transform.position.Rounded(1);
                //     } else {
                //         newPosition = footprint.Vector3NoY()/2;
                //     }
                //     Tween.LocalPosition(transform,
                //                   newPosition,
                //                   fadeValue.Value,
                //                   0.0f,
                //                   Tween.EaseInOutStrong,
                //                   Tween.LoopType.None,
                //                   () => isTweening = true,
                //                   () => isTweening = false
                //                   );
                // }
                // Tween.Rotate(t,
                //               new Vector3 (0, degrees, 0),
                //               Space.Self,
                //               fadeValue.Value,
                //               0.0f,
                //               Tween.EaseInOutStrong,
                //               Tween.LoopType.None,
                //               () => isTweening = true,
                //               () => isTweening = false
                //               );

                Debug.Log("Start " + gameObject.name + ": " + transform.rotation.y);
                Debug.Log("End: " + (transform.rotation.y + degrees));
                Debug.Log("Point: " + (transform.localPosition + (footprint.Vector3NoY()/2)));
                // rotationPoint = transform.position + (footprint.Vector3NoY()/2);
                rotationStart = pivot.rotation.y;
                Tween.ValueRelative(rotationStart,
                            rotationStart + degrees,
                            HandleRotateAround,
                            fadeValue.Value,
                            0.0f,
                            Tween.EaseInOutStrong,
                            Tween.LoopType.None,
                            StartTween,
                            EndTween
                            );
                if (fancy)
                {
                    Tween.LocalScale(t,
                                   new Vector3(0.85f, 0.85f, 0.85f),
                                   fadeValue.Value,
                                   0,
                                   curve.curve,
                                   Tween.LoopType.None
                                   );
                }
            }
        }
    }
}
