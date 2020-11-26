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

        public Placeable currentNonMoverPlaceable;
        public ClickCatcher clickCatcher;

        // private Vector2IntEvent movePositionerEvent;
        // public UnityEvent rotatePositionerEvent;

        [Header("State Flags")]
        public bool isMoving;
        private bool isTweening;
        public bool fancy = true;

        private Vector3 rotationPoint = Vector3.one;
        public bool usePivotOffset = true;

        private Renderer rend;
        public LayerMask mask;
        public Transform currentTileTransform;
        public Tile currentPivotTile;
        public FloatVariable fadeValue;
        public AnimationCurveVariable curve;


        /// <summary>
        /// This is presumably where we check to see if the rotate will cause an
        /// invalid move.
        /// </summary>
        void Rotate()
        {
            Vector2Int start, end;
            (start, end) = currentNonMoverPlaceable.GetFootprintWithRotationStep(
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
            Debug.Log("MoveTo: " + destination);

            if (!isTweening)
            {
                // SetOccupancy(false);

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
            currentNonMoverPlaceable.SetOccupancy(false);
            isTweening = true;
        }
        private void EndTween()
        {
            isTweening = false;
            currentNonMoverPlaceable.SetOccupancy(true);
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
            currentNonMoverPlaceable = placeable;

            pivot.localEulerAngles = Vector3.up * 90 * currentNonMoverPlaceable.rotationStep;
            transform.position = currentNonMoverPlaceable.transform.position;

            AddPlaceable(currentNonMoverPlaceable);
            SetPivot();

            currentTileTransform = TransformFromRaycast();
            GetComponent<BoxCollider>().enabled = true;
            rend.enabled = true;

            currentPivotTile = TileController.Instance.GetTile(transform.Position2dInt());
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
            // Tile tile = TileFromRaycast();
            // Debug.Log("Tile: " + tile.gameObject.name, tile);

            // If we're still moving and in a new tile, move the positioner.
            if (isMoving && newTile != currentTileTransform)
            {
                MoveTo(newTile.Position2dInt());

                currentTileTransform = newTile;
            }
        }

        /// UTILITY METHODS ///

        private Transform TransformFromRaycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            LayerMask invertMask = ~(1 << mask);

            if (Physics.Raycast(ray, out hit, 100, invertMask))
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

        private Tile TileFromRaycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, mask.value))
            {
                Tile tile;
                Debug.Log("Hit: " + hit.transform.gameObject.name);
                if (hit.transform.gameObject.TryGetComponent<Tile>(out tile))
                {
                    return tile;
                } else {
                    return null;
                }
            }
            return null;
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
            currentNonMoverPlaceable.SetTileCatch(false);

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

        /// ACCESSORS ///

        /// <summary>
        /// These register methods for the Move event
        /// </summary>
        public void RegisterMoveEvent(UnityAction<Vector2Int> call)
        {
            // movePositionerEvent.AddListener(call);
        }
        public void DeregisterMoveEvent(UnityAction<Vector2Int> call)
        {
            // movePositionerEvent.RemoveListener(call);
        }

    }
}
