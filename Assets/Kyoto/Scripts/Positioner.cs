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
        public GridMover gridMover;

        private Vector2IntEvent movePositionerEvent;
        // public Vector2IntEvent rotateToPositionerEvent;
        public UnityEvent rotatePositionerEvent;

        // this event is for when a placeable gets deselected
        // not sure we need it though
        // public PlaceableEvent placeableEvent;

        public bool isMoving;

        private Renderer rend;
        public LayerMask mask;
        public Transform currentTileTransform;

        /// <summary>
        /// This is presumably where we check to see if the rotate will cause an
        /// invalid move.
        /// </summary>
        void Rotate()
        {
            Vector2Int start, end;
            (start, end) = currentNonMoverPlaceable.GetFootprintWithRotationStep(
                    (currentNonMoverPlaceable.GetComponent<GridMover>().rotationStep+1)%4);
            bool occupied = TileController.Instance.CheckTileOccupancyByPosition(start, end, currentNonMoverPlaceable);
            Debug.Log("Occupied: " + occupied);

            // we have a legal move, so go ahead and rotate
            if (!occupied) rotatePositionerEvent.Invoke();
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
            gridMover = GetComponent<GridMover>();
            gridMover.AddMover(transform);

            movePositionerEvent = new Vector2IntEvent();
        }

        void OnEnable()
        {
            // Register to move itself
            // movePositionerEvent.AddListener(gridMover.MoveToInt);
            movePositionerEvent.AddListener(gridMover.MoveMovers);
            rotatePositionerEvent.AddListener(gridMover.RotateStep);
        }

        void OnDisable()
        {
            // movePositionerEvent.RemoveListener(gridMover.MoveToInt);
            movePositionerEvent.RemoveListener(gridMover.MoveMovers);
            rotatePositionerEvent.RemoveListener(gridMover.RotateStep);
        }

        public void Activate(Placeable placeable)
        {
            if (currentNonMoverPlaceable)
            {
                RemovePlaceable();
            }

            transform.position = placeable.transform.position;
            AddPlaceable(placeable);

            // currentNonMoverPlaceable = placeable;
            // currentNonMoverPlaceable.transform.SetParent(transform);
            // currentNonMoverPlaceable.transform.parent = transform;

            // if (currentPlaceable)
            // {
            //     gridMover.RemoveMover(currentPlaceable.transform);
            //     currentPlaceable.Deselect();
            //     gridMover.footprint = Vector2Int.one;
            // }

            // SetVolume(placeable.volume);

            // gridMover.footprint = placeable.footprint;
            // gridMover.SetPivot();
            // pivot.localEulerAngles = placeable.transform.Find("Pivot").transform.localEulerAngles;

            // currentPlaceable = placeable;
            currentTileTransform = TransformFromRaycast();
            GetComponent<BoxCollider>().enabled = true;
            rend.enabled = true;
        }

        void Deactivate()
        {
            if (currentNonMoverPlaceable)
            {
                RemovePlaceable();
            }
            // if (currentPlaceable) currentPlaceable.GetComponent<Placeable>().Deselect();
            // currentPlaceable = null;
            GetComponent<BoxCollider>().enabled = false;
            rend.enabled = false;
        }

        public void Remove()
        {
            // REFACTOR: do we even need this layer any more?
            Deactivate();
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
            Transform newTile = TransformFromRaycast();
            if (isMoving && newTile != currentTileTransform)
            {
                movePositionerEvent.Invoke(newTile.Position2dInt());
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
            currentNonMoverPlaceable = placeable;
            currentNonMoverPlaceable.transform.SetParent(transform);

            gridMover.AddMover(placeable.transform);

            SetVolume(placeable.volume);

            gridMover.footprint = placeable.footprint;
            gridMover.SetPivot();
            pivot.localEulerAngles = placeable.transform.Find("Pivot").transform.localEulerAngles;

        }
        private void RemovePlaceable()
        {
            currentNonMoverPlaceable.transform.SetParent(GameObject.Find("Placeables").transform);
            gridMover.RemoveMover(currentNonMoverPlaceable.transform);
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
            movePositionerEvent.AddListener(call);
        }
        public void DeregisterMoveEvent(UnityAction<Vector2Int> call)
        {
            movePositionerEvent.RemoveListener(call);
        }

    }
}
