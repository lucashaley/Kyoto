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
        public GameObject pivot;
        public GameObject cube;
        public GameObject currentSelection;
        public ClickCatcher clickCatcher;
        public GridMover gridMover;

        public Vector2IntEvent movePositionerEvent;
        public Vector2IntEvent rotateToPositionerEvent;
        public UnityEvent rotatePositionerEvent;

        // this event is for when a placeable gets deselected
        // not sure we need it though
        public PlaceableEvent placeableEvent;

        public bool isMoving;

        private Renderer rend;
        public LayerMask mask;
        public Transform currentTileTransform;

        void Rotate()
        {
            // RERACTOR do we still need this?
            Debug.Log("ROTATE start");
            // Debug.Break();
            // Debug.Log(gridMover.footprint.x + gridMover.footprint.y);
            if ((gridMover.footprint.x + gridMover.footprint.y)%2 == 1)
            {
                // the result is odd
                Debug.Log("Oddness!");

                // Check around
                // rotatePositionerEvent.Invoke();
                rotateToPositionerEvent.Invoke(Vector2Int.one);

                // Debug.Log("Rotate footprint: " + transform.Position2dInt().Rotate90CW(gridMover.footprint));
                // Debug.Log(TileController.Instance.CheckTileOccupancy(transform.Position2dInt(), gridMover.footprint));
                Debug.Log("Current Footprint: " + currentSelection.GetComponent<Placeable>().GetCurrentFootprint());
                TileController.Instance.CheckTileOccupancyByPosition(transform.Position2dInt(), transform.Position2dInt() + gridMover.footprint);
            }
        }

        void Awake()
        {
            rend = gameObject.GetComponentInChildren<Renderer>();
            mask = LayerMask.GetMask("Tiles");
            pivot = transform.Find("Pivot").gameObject;
            cube = transform.Find("Pivot/PositionerCube_01").gameObject;

            // What is going on here?
            // Debug.Break();
            // currentTileTransform = transform.parent;
            currentTileTransform = transform;

            clickCatcher = ClickCatcher.Instance;
            clickCatcher.positioner = this;
            gridMover = GetComponent<GridMover>();
        }

        void OnEnable()
        {
            // Register to move itself
            movePositionerEvent.AddListener(gridMover.MoveToInt);
            // rotatePositionerEvent.AddListener(geo.GetComponent<GridMover>().RotateByInt);
            // rotatePositionerEvent.AddListener(gridMover.RotateByInt);
        }

        void OnDisable()
        {
            movePositionerEvent.RemoveListener(gridMover.MoveToInt);
            // rotatePositionerEvent.RemoveListener(geo.GetComponent<GridMover>().RotateByInt);
        }

        public void Activate(Placeable placeable)
        {
            if (currentSelection)
            {
                currentSelection.GetComponent<Placeable>().Deselect();
                // pivot.transform.ResetTransformation();
                gridMover.footprint = Vector2Int.one;
            }

            SetVolume(placeable.volume);
            gridMover.footprint = placeable.GetComponent<GridMover>().footprint;
            transform.position = placeable.transform.position;
            currentSelection = placeable.gameObject;
            currentTileTransform = TransformFromRaycast();
            GetComponent<BoxCollider>().enabled = true;
            rend.enabled = true;
        }

        public void SetVolume(Vector3Int vol)
        {
            cube.transform.localScale = vol;
            // cube.transform.localPosition = (Vector3)vol/2;
            // cube.transform.localPosition =

            GetComponent<BoxCollider>().size = vol;
            GetComponent<BoxCollider>().center = (Vector3)vol/2;
        }

        void Deactivate()
        {
            if (currentSelection) currentSelection.GetComponent<Placeable>().Deselect();
            currentSelection = null;
            GetComponent<BoxCollider>().enabled = false;
            rend.enabled = false;
        }

        void OnMouseDown()
        {
            Debug.Log("OnMouseDown");
            currentTileTransform = TransformFromRaycast();

            switch (CollisionNormal())
            {
                case Vector3 v when v == Vector3.up:
                    Rotate();
                    // rotatePositionerEvent.Invoke(90);
                    rotatePositionerEvent.Invoke();
                    break;
                case Vector3 v when v == Vector3.left || v == Vector3.back:
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

        public void Remove()
        {
            // REFACTOR: do we even need this layer any more?
            Deactivate();
        }

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
    }
}
