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
        public BoolVariable canPlace;
        public Transform pivot;
        public Transform cube;
        public BoxCollider col;

        public Placeable currentPlaceable;
        public ClickCatcher clickCatcher;

        [Header("State Flags")]
        public bool isMoving;
        private bool isTweening;
        public bool fancy = true;

        public Vector3 rotationPoint = Vector3.one;
        public bool usePivotOffset = true;

        private Renderer rend;
        public LayerMask mask;
        public Transform currentTileTransform;
        public Tile currentPivotTile;
        public FloatVariable fadeValue;
        public AnimationCurveVariable curve;


        private bool CheckMove(Vector2Int destination)
        {
            // Debug.Log("CheckMove");

            Vector2Int delta = destination - currentPlaceable.transform.Position2dInt();
            // Vector2Int start, end;
            // (start, end) = currentPlaceable.GetTileBounds();
            //
            // // REFACTOR maybe an offset method for Bounds?
            //
            // start += delta;
            // end += delta;

            Bounds newBounds = currentPlaceable.bounder.bounds.Offset(delta);
            // Debug.Log("newBounds: " + newBounds);
            return TileController.Instance.CheckTileOccupancyByBounds(newBounds, currentPlaceable);

            // Debug.Log("Checking: " + start + ", " + end);

            // REFACTOR to use bounds?
            // return TileController.Instance.CheckTileOccupancyByPosition(start, end, currentPlaceable);
        }

        /// <summary>
        /// This is presumably where we check to see if the rotate will cause an
        /// invalid move.
        /// </summary>
        private bool CheckRotate()
        {
            Bounds rotatedBounds = currentPlaceable.GetBoundsRotated();
            Debug.Log("rotatedBounds: " + rotatedBounds.min + ", " + rotatedBounds.max);

            Debug.Log("Trying List version");
            return TileController.Instance.CheckTileOccupancyByBounds(rotatedBounds, currentPlaceable);
        }

        public void StartTween()
        {
        }
        public void EndTween()
        {
        }

        void Awake()
        {
            rend = gameObject.GetComponentInChildren<Renderer>();
            mask = LayerMask.GetMask("Tiles");
            pivot = transform.Find("Pivot");
            cube = transform.Find("Pivot/PositionerCube_01");
            // col = transform.Find("Pivot/Collider").GetComponent<BoxCollider>();
            col = GetComponent<BoxCollider>();

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

            if (currentPlaceable)
            {
                RemovePlaceable();
            }
            currentPlaceable = placeable;

            // pivot.localEulerAngles = Vector3.up * 90 * currentPlaceable.rotationStep;
            pivot.localRotation = currentPlaceable.pivot.localRotation;
            // Debug.Log("Postioner position: " + transform.position);

            // This might be the issue!
            transform.position = currentPlaceable.transform.position;
            // Debug.Log("Postioner position: " + transform.position);

            AddPlaceable(currentPlaceable);
            SetPivot();

            currentTileTransform = TransformFromRaycast();
            col.enabled = true;
            rend.enabled = true;

            currentPivotTile = TileController.Instance.GetTile(transform.Position2dInt());
        }

        public void Deactivate()
        {
            if (currentPlaceable)
            {
                RemovePlaceable();
            }
            col.enabled = false;
            rend.enabled = false;
        }

        public void SetPivot()
        {
            if ((currentPlaceable.footprint.x + currentPlaceable.footprint.y)%2 == 1)
            {
                Debug.Log("Odd footprint");
                rotationPoint = new Vector3(0.5f, 0f, 0.5f);
            } else {
                Debug.Log("Even footprint");
                rotationPoint = currentPlaceable.footprint.Vector3NoY() * 0.5f;
            }

            if (usePivotOffset)
            {
                // Debug.Log("currentPlaceable localPosition: " + currentPlaceable.transform.localPosition);
                // We only want the top level
                Transform[] childrenTransforms = pivot.GetComponentsInChildren<Transform>();
                foreach (Transform t in childrenTransforms)
                {
                    // only do the top most level of children
                    if (t.parent == pivot)
                        t.localPosition = rotationPoint * -1f;
                }

                switch (currentPlaceable.rotationStep)
                {
                    case 1:
                        rotationPoint = new Vector3(rotationPoint.z, 0f, -rotationPoint.x);
                        break;
                }

                pivot.localPosition = rotationPoint;
            }
        }

        private void AddPlaceable(Placeable placeable)
        {
            SetVolume(placeable.volume);
        }
        private void RemovePlaceable()
        {
            currentPlaceable.Deselect();
            currentPlaceable = null;
        }

        public void SetVolume(Vector3Int vol)
        {
            cube.localScale = vol;
            col.size = vol;
            col.center = (Vector3)vol/2;
        }

        /// INTERACTION ///

        void OnMouseDown()
        {
            currentTileTransform = TransformFromRaycast();

            switch (CollisionNormal())
            {
                // Player touched the top, so rotate
                case Vector3 v when v == Vector3.up:
                    if (!isMoving)
                    {
                        bool invalid = CheckRotate();
                        Debug.Log("Invalid Rotate: " + invalid);

                        // we have a legal move, so go ahead and rotate
                        if (!invalid)
                        {
                            // RotateStep();
                            GetComponent<SimpleMover>().RotateStep();
                            currentPlaceable.GetComponent<SimpleMover>().RotateStep();
                        }
                    }
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

        void OnMouseDrag()
        {
            // Check to see which tile we're in
            Transform newTile = TransformFromRaycast();

            // If we're still moving and in a new tile, move the positioner.
            if (isMoving && newTile != currentTileTransform)
            {
                bool invalid = CheckMove(newTile.Position2dInt());
                // Debug.Log("Invalid Move: " + invalid);

                if (!invalid)
                {
                    // MoveTo(newTile.Position2dInt());
                    GetComponent<SimpleMover>().MoveTo(newTile.Position2dInt());
                    currentPlaceable.GetComponent<SimpleMover>().MoveTo(newTile.Position2dInt());
                }

                currentTileTransform = newTile;
            }
        }

        void OnMouseUp()
        {
            isMoving = false;
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
                // Debug.Log("Hit: " + hit.transform.gameObject.name);
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

        public (Vector2Int, Vector2Int) RotateRectAroundPoint(Vector2Int start, Vector2Int end, Vector2 axis)
        {
            // From (1, 0) (1, 1)
            // and Axis (1.5, 1.5)
            // We want (0, 2) (1, 2)
            Vector2 aStart, aEnd;
            Vector2 newStart = Vector2.zero, newEnd = Vector2.zero;

            aStart = (Vector2)start - axis; // (-0.5, -1.5)
            aEnd = (Vector2)end - axis; // (-0.5, -0.5)

            // flip x and y
            Vector2 tempStart = new Vector2(-aStart.y, aStart.x); // (1.5, -0.5)
            Vector2 tempEnd = new Vector2(-aEnd.y, aEnd.x); // (0.5, -0.5)

            // add the axis back again, subtracting one for the tile size
            tempStart += axis - Vector2.one; // (3, 1) -> (2, 0)
            tempEnd += axis - Vector2.one; // (2, 1) -> (1, 0)

            Debug.Log("TempStart: " + tempStart);
            Debug.Log("TempEnd: " + tempEnd);

            // get min and max
            newStart = Vector2.Min(tempStart, tempEnd);
            newEnd = Vector2.Max(tempStart, tempEnd);

            return (newStart.Vector2Int(), newEnd.Vector2Int());
        }

        private void ResizeCollision(Vector3 size, Vector3 center)
        {
            Debug.Log("ResizeCollision: " + size + ", " + center);
            col.size = size;
            col.center = center;
        }

        private void ResizeCollisionByRotation()
        {
            // Debug.Log("ResizeCollisionByRotation");
            int rotationStep = currentPlaceable.rotationStep;
            Vector2 footprint= currentPlaceable.footprint;

            // switch (rotationStep)
            // {
            //     case 0:
            //         col.size =
            // }
        }

        private void ResizeCollisionByRendererBounds()
        {
            col.size = rend.bounds.size;
            col.center = rend.bounds.center - transform.position;
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
