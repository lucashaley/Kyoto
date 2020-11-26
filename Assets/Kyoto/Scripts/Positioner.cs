using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityAtoms.BaseAtoms;
using Pixelplacement;

namespace Kyoto
{
    public class Positioner : Singleton<Positioner>
    {
        public Transform pivot;
        public Transform cube;
        // public Placeable currentPlaceable;
        public Placeable currentNonMoverPlaceable;
        public ClickCatcher clickCatcher;
        // public GridMover gridMover;

        private Vector2IntEvent movePositionerEvent;
        public UnityEvent rotatePositionerEvent;

        public bool isMoving;
        private bool isTweening;
        public bool fancy = true;
        // public int rotationStep = 0;
        private Vector3 rotationPoint = Vector3.one;
        public bool usePivotOffset = true;

        private Renderer rend;
        public LayerMask mask;
        public Transform currentTileTransform;
        public FloatVariable fadeValue;
        public AnimationCurveVariable curve;


        /// <summary>
        /// This is presumably where we check to see if the rotate will cause an
        /// invalid move.
        /// </summary>
        void Rotate()
        {
            Vector2Int start, end;
            (start, end) = GetFootprintWithRotationStep(
                    (currentNonMoverPlaceable.rotationStep+1)%4);
            Debug.Log("Start: " + start);
            Debug.Log("End: " + end);
            bool occupied = TileController.Instance.CheckTileOccupancyByPosition(start, end, currentNonMoverPlaceable);
            Debug.Log("Occupied: " + occupied);

            // we have a legal move, so go ahead and rotate
            if (!occupied)
            {
                RotateStep();
                // rotatePositionerEvent.Invoke();
            }
        }

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

                currentNonMoverPlaceable.rotationStep = (currentNonMoverPlaceable.rotationStep+1)%4;
            }
        }


        void MoveTo(Vector2Int destination)
        {
            if (!isTweening)
            {
                Tween.Position(transform,
                              destination.Vector3NoY(),
                              fadeValue.Value,
                              0.0f,
                              Tween.EaseInOutStrong,
                              Tween.LoopType.None,
                              StartTween,
                              EndTween
                              );
                Tween.LocalScale(pivot,
                                 new Vector3(0.85f, 0.85f, 0.85f),
                                 fadeValue.Value,
                                 0,
                                 curve.curve,
                                 Tween.LoopType.None
                                 );
            }
        }

        private void StartTween()
        {
            isTweening = true;
        }
        private void EndTween()
        {
            isTweening = false;
        }

        void Awake()
        {
            rend = gameObject.GetComponentInChildren<Renderer>();
            mask = LayerMask.GetMask("Tiles");
            pivot = transform.Find("Pivot");
            cube = transform.Find("Pivot/PositionerCube_01");

            currentTileTransform = transform;

            clickCatcher = ClickCatcher.Instance;
            clickCatcher.positioner = this;
            // gridMover = GetComponent<GridMover>();
            // gridMover.AddMover(transform);

            movePositionerEvent = new Vector2IntEvent();
        }

        void OnEnable()
        {
        }

        void OnDisable()
        {
        }

        public void Activate(Placeable placeable)
        {
            Debug.Log("Activate: " + gameObject.name);

            if (currentNonMoverPlaceable)
            {
                RemovePlaceable();
            }

            Debug.Log("Move Positioner to Placeable");
            transform.position = placeable.transform.position;

            AddPlaceable(placeable);
            SetPivot();

            currentTileTransform = TransformFromRaycast();
            GetComponent<BoxCollider>().enabled = true;
            rend.enabled = true;
        }

        public void Deactivate()
        {
            if (currentNonMoverPlaceable)
            {
                RemovePlaceable();
            }
            GetComponent<BoxCollider>().enabled = false;
            rend.enabled = false;
        }

        public void SetPivot()
        {
            Debug.Log("SetPivot");
            if ((currentNonMoverPlaceable.footprint.x + currentNonMoverPlaceable.footprint.y)%2 == 1)
            {
                rotationPoint = new Vector3(0.5f, 0f, 0.5f);
            } else {
                rotationPoint = new Vector3(currentNonMoverPlaceable.footprint.x * 0.5f, 0f, currentNonMoverPlaceable.footprint.y * 0.5f);
            }

            if (usePivotOffset)
            {
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

        /// INTERACTION ///

        void OnMouseDown()
        {
            currentTileTransform = TransformFromRaycast();

            switch (CollisionNormal())
            {
                // Player touched the top, so rotate
                case Vector3 v when v == Vector3.up:
                    Rotate();
                    break;
                // Touching the bottom does nothing
                case Vector3 v when v == Vector3.down:
                    break;
                // Touching any other side moves
                default:
                    isMoving = true;
                    break;
            }
        }

        void OnMouseUp()
        {
            isMoving = false;
        }

        void OnMouseDrag()
        {
            // Check to see which tile we're in
            Transform newTile = TransformFromRaycast();

            // If we're still moving and in a new tile, move the positioner.
            if (isMoving && newTile != currentTileTransform)
            {
                // this is the old method with a GridMover.
                // movePositionerEvent.Invoke(newTile.Position2dInt());

                // this is the new method using just the positioner.
                MoveTo(currentTileTransform.Position2dInt());

                currentTileTransform = newTile;
            }
        }

        /// UTILITY METHODS ///

        private Transform TransformFromRaycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                Tile tile;
                if (hit.transform.gameObject.TryGetComponent<Tile>(out tile))
                {
                    return hit.transform;
                } else {
                    return currentTileTransform;
                }
            } else {
                return currentTileTransform;
            }
        }

        private Vector3 CollisionNormal()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                return hit.normal;
            } else {
                return Vector3.zero;
            }
        }

        private void AddPlaceable(Placeable placeable)
        {
            Debug.Log("AddPlaceable: " + placeable.gameObject.name, placeable);
            currentNonMoverPlaceable = placeable;
            currentNonMoverPlaceable.transform.SetParent(pivot);
            currentNonMoverPlaceable.transform.localPosition = Vector3.zero;

            SetVolume(placeable.volume);
        }
        private void RemovePlaceable()
        {
            currentNonMoverPlaceable.transform.SetParent(GameObject.Find("Placeables").transform);
            currentNonMoverPlaceable.Deselect();
            currentNonMoverPlaceable = null;
        }

        public void SetVolume(Vector3Int vol)
        {
            cube.localScale = vol;
            GetComponent<BoxCollider>().size = vol;
            GetComponent<BoxCollider>().center = (Vector3)vol/2;
        }

        public void SetOccupancy(bool isOccupied)
        {
            Debug.Log("Positioner.SetOccupancy", this);
            Vector2Int start, end = default;

            (start, end) = GetCurrentFootprint();

            Debug.Log("Positioner.SetOccupancy: " + start + ", " + end);
            TileController.Instance
                          .SetTileOccupancyByPosition
                            (
                                start,
                                end,
                                isOccupied ? currentNonMoverPlaceable : null
                            );
        }

        public (Vector2Int start, Vector2Int end) GetFootprintWithRotationStep(int step)
        {
            Vector2Int end = Vector2Int.zero;
            // REFACTOR should we not subtract 1? We'd have to change
            // TileController.CheckTileOccupancyByPosition to < instead of <=
            Vector2Int adjustedFootprint = currentNonMoverPlaceable.footprint - Vector2Int.one;
            // Debug.Log("adjustedFootprint: " + adjustedFootprint);
            switch (step)
            {
                case 0:
                    end = transform.Position2dInt() + adjustedFootprint;
                    break;

                case 1:
                    end = transform.Position2dInt() - adjustedFootprint.Transpose();
                    break;

                case 2:
                    end = transform.Position2dInt() - adjustedFootprint;
                    break;

                case 3:
                    end = transform.Position2dInt() + adjustedFootprint.Transpose();
                    break;

            }

            return (transform.Position2dInt(), end);
        }

        public (Vector2Int start, Vector2Int end) GetCurrentFootprint()
        {
            return GetFootprintWithRotationStep(currentNonMoverPlaceable.rotationStep);
        }

        /// ACCESSORS ///

        /// <summary>
        /// These register methods for the Move event
        /// </summary>
        public void RegisterMoveEvent(UnityAction<Vector2Int> call)
        {
            movePositionerEvent.AddListener(call);
        }
        public void DeregisterMoveEvent(UnityAction<Vector2Int> call)
        {
            movePositionerEvent.RemoveListener(call);
        }

    }
}
