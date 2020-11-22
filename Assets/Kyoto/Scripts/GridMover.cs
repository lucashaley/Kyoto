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
        public bool usePivotOffset = true;

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
        public int rotationStep = 0;
        public Transform pivot;

        // What was this about? Creates nasty infinite loop
        // public Vector2IntEvent movePositionerEvent;
        public BoolEvent doneMoving;

        void Awake()
        {
            rend = GetComponentInChildren<Renderer>();
            // rotationPoint = transform.position + (footprint.Vector3NoY()/2);
            // rotationPoint = footprint.Vector3NoY() * 0.5f;
            // if ((footprint.x + footprint.y)%2 == 1)
            // {
            //     rotationPoint = new Vector3(0.5f, 0f, 0.5f);
            // } else {
            //     rotationPoint = new Vector3(footprint.x * 0.5f, 0f, footprint.y * 0.5f);
            // }
            // rotationPoint = new Vector3(0.5f, 0f, 0.5f);
            pivot = transform.Find("Pivot");

            SetPivot();

            // if (usePivotOffset)
            // {
            //     // pivot.Translate(new Vector3(0.5f, 0f, 0.5f));
            //
            //     Transform[] childrenTransforms = pivot.GetComponentsInChildren<Transform>();
            //     foreach (Transform t in childrenTransforms)
            //         // t.localPosition = new Vector3(-0.5f, 0f, -0.5f);
            //         t.localPosition = rotationPoint * -1f;
            //     // pivot.localPosition = new Vector3(0.5f, 0f, 0.5f);
            //     pivot.localPosition = rotationPoint;
            // }
        }

        public void SetPivot()
        {
            if ((footprint.x + footprint.y)%2 == 1)
            {
                rotationPoint = new Vector3(0.5f, 0f, 0.5f);
            } else {
                rotationPoint = new Vector3(footprint.x * 0.5f, 0f, footprint.y * 0.5f);
            }

            if (usePivotOffset)
            {
                Transform[] childrenTransforms = pivot.GetComponentsInChildren<Transform>();
                foreach (Transform t in childrenTransforms)
                    t.localPosition = rotationPoint * -1f;
                pivot.localPosition = rotationPoint;
            }
        }

        void StartTween()
        {
            isTweening = true;
            doneMoving.Invoke(false);
        }

        void EndTween()
        {
            isTweening = false;

            // REFACTOR this to one Transform extension?
            transform.position = transform.position.RoundedInt();
            transform.localScale = Vector3.one;

            pivot.localEulerAngles = Vector3Int.FloorToInt(pivot.localEulerAngles);

            Debug.Log("Invoking doneMoving");
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
        }

        void HandleRotateAround(float value)
        {
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

        public void RotateStep()
        {
            if (!isTweening)
            {
                Tween.LocalRotation(pivot,
                                    pivot.localEulerAngles + (Vector3.up * 90f),
                                    fadeValue.Value,
                                    0.0f,
                                    Tween.EaseInOutStrong,
                                    Tween.LoopType.None,
                                    StartTween,
                                    EndTween
                                    );
                if (fancy)
                {
                    Tween.LocalScale(transform,
                                   new Vector3(0.85f, 0.85f, 0.85f),
                                   fadeValue.Value,
                                   0,
                                   curve.curve,
                                   Tween.LoopType.None
                                   );
                }

                rotationStep = (rotationStep+1)%4;
            }
        }

        public void RotateByIntRelative(int degrees)
        {
            if (!isTweening)
            {
                Transform t = rotateBase ? rotateBase : transform;

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
