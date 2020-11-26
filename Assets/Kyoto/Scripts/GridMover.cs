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
        public Placeable placeable;

        // It would be cool to have this just be a list of things to move
        // So the same GridMover can move the Placeable and the Positioner
        // at the same time.
        public List<Transform> movers;

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
            placeable = GetComponent<Placeable>();

            pivot = transform.Find("Pivot");

            // SetPivot();
        }

        public void SetPivot()
        {
            Debug.Log("SetPivot");
            if ((footprint.x + footprint.y)%2 == 1)
            {
                rotationPoint = new Vector3(0.5f, 0f, 0.5f);
            } else {
                rotationPoint = new Vector3(footprint.x * 0.5f, 0f, footprint.y * 0.5f);
            }

            if (usePivotOffset)
            {
                // THIS IS THE MOTHERFUCKING PROBLEM
                // We only want the top level
                Transform[] childrenTransforms = pivot.GetComponentsInChildren<Transform>();
                foreach (Transform t in childrenTransforms)
                {
                    // only do the top most level of children
                    if (t.parent == pivot)
                        t.localPosition = rotationPoint * -1f;
                }
                pivot.localPosition = rotationPoint;
            }
        }

        void StartTween()
        {
            isTweening = true;
        }

        void EndTween()
        {
            isTweening = false;

            // REFACTOR this to one Transform extension?
            // Here's where we clamp the position and rotation to make up for
            // weird tween artifacts
            transform.position = transform.position.RoundedInt();
            transform.localScale = Vector3.one;

            pivot.localEulerAngles = Vector3Int.FloorToInt(pivot.localEulerAngles);

            Debug.Log("Invoking doneMoving");
            doneMoving.Invoke(true);
        }

        void EndTween(Transform t)
        {
            t.position = t.position.RoundedInt();
            transform.localScale = Vector3.one;
            t.localEulerAngles = Vector3Int.FloorToInt(t.localEulerAngles);
        }


        // public void MoveToInt(Vector2Int destination)
        // {
        //     if (!isTweening)
        //     {
        //         Tween.Position(transform,
        //                       destination.Vector3NoY(),
        //                       fadeValue.Value,
        //                       0.0f,
        //                       Tween.EaseInOutStrong,
        //                       Tween.LoopType.None,
        //                       // () => isTweening = true,
        //                       // () => isTweening = false
        //                       StartTween,
        //                       EndTween
        //                       );
        //         // REFACTOR this to a separate fader component?
        //         Tween.ShaderVector (rend.material,
        //                             "_Color",
        //                             new Vector4 (1, 1, 1, 0),
        //                             fadeValue.Value,
        //                             0,
        //                             curve.curve,
        //                             Tween.LoopType.None);
        //         Tween.LocalScale(transform.GetChild(0),
        //                          new Vector3(0.85f, 0.85f, 0.85f),
        //                          fadeValue.Value,
        //                          0,
        //                          curve.curve,
        //                          Tween.LoopType.None
        //                          );
        //     }
        // }

        // REFACTOR this name sucks
        public void MoveMovers(Vector2Int destination)
        {
            if (!isTweening)
            {
                foreach (Transform m in movers)
                {
                    Debug.Log("Moving " + m.gameObject.name);
                    Move(m, destination);
                }
            }
        }
        // REFACTOR this name sucks
        private void Move(Transform t, Vector2Int destination)
        {
            Tween.Position(t,
                          destination.Vector3NoY(),
                          fadeValue.Value,
                          0.0f,
                          Tween.EaseInOutStrong,
                          Tween.LoopType.None,
                          // () => isTweening = true,
                          // () => isTweening = false
                          StartTween,
                          () => EndTween(t)
                          );
            // REFACTOR this to a separate fader component?
            // Tween.ShaderVector (rend.material,
            //                     "_Color",
            //                     new Vector4 (1, 1, 1, 0),
            //                     fadeValue.Value,
            //                     0,
            //                     curve.curve,
            //                     Tween.LoopType.None);
            Tween.LocalScale(t.GetChild(0),
                             new Vector3(0.85f, 0.85f, 0.85f),
                             fadeValue.Value,
                             0,
                             curve.curve,
                             Tween.LoopType.None
                             );
        }

        // void HandleRotateAround(float value)
        // {
        //     pivot.RotateAround(pivot.TransformPoint(rotationPoint),
        //                           Vector3.up,
        //                           value);
        //
        //     // REFACTOR: have a callback that sets the position etc to whole values
        // }

        /// <remarks>
        /// We're going to assume 90 CW rotation.
        /// </remarks>
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

        // public void RotateByIntRelative(int degrees)
        // {
        //     if (!isTweening)
        //     {
        //         Transform t = rotateBase ? rotateBase : transform;
        //
        //         rotationStart = pivot.rotation.y;
        //         Tween.ValueRelative(rotationStart,
        //                     rotationStart + degrees,
        //                     HandleRotateAround,
        //                     fadeValue.Value,
        //                     0.0f,
        //                     Tween.EaseInOutStrong,
        //                     Tween.LoopType.None,
        //                     StartTween,
        //                     EndTween
        //                     );
        //         if (fancy)
        //         {
        //             Tween.LocalScale(t,
        //                            new Vector3(0.85f, 0.85f, 0.85f),
        //                            fadeValue.Value,
        //                            0,
        //                            curve.curve,
        //                            Tween.LoopType.None
        //                            );
        //         }
        //     }
        // }

        public void AddMover(Transform t)
        {
            movers.Add(t);
        }
        public void RemoveMover(Transform t)
        {
            movers.Remove(t);
        }
    }
}
