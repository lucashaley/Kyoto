﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    /// <summary>
    /// This class is the base class for objects you can place in the scene.
    /// It allows for selecting, moving, and rotating.
    /// </summary>
    /// <remarks>
    /// The selection happens in the PlaceableSelector class.
    /// </remarks>
    public class Placeable : MonoBehaviour, IEnumerable<Vector2Int>
    {
        public Vector2Int footprint = Vector2Int.one;
        public Vector3Int volume = Vector3Int.one;

        private GameObject pivot;

        private GameObject tileCatch;
        private GameObject geoCatch;

        private Positioner positioner;

        void Awake()
        {
            // Just added the "Geometry" subonbject, messed things up
            pivot = transform.Find("Pivot").gameObject;

            CreateTileCatch();
            CreateGeoCatch();

            positioner = Positioner.Instance;
            positioner.placeableEvent.AddListener(this.Deselect);

            SetOccupancy(true);
        }

        /// <summary>
        /// Registers the doneMoving, so it can set occupancy
        /// </summary>
        void OnEnable()
        {
            GetComponent<GridMover>().doneMoving.AddListener(SetOccupancy);
            // GetComponent<GridMover>().doneMoving.AddListener(ClampTransform);
        }

        void OnDisable()
        {
            GetComponent<GridMover>().doneMoving.RemoveListener(SetOccupancy);
            // GetComponent<GridMover>().doneMoving.RemoveListener(ClampTransform);
        }

        public void Select()
        {
            geoCatch.SetActive(false);
            positioner.Activate(this);

            positioner.movePositionerEvent.AddListener(Place);
            // Addng a interstitial layer to make sure the occupancy is clear
            // positioner.rotatePositionerEvent.AddListener(GetComponent<GridMover>().RotateStep);
            positioner.rotatePositionerEvent.AddListener(RotateStep);
        }

        public void Deselect()
        {
            positioner.movePositionerEvent.RemoveListener(Place);
            // positioner.rotatePositionerEvent.RemoveListener(GetComponent<GridMover>().RotateStep);
            positioner.rotatePositionerEvent.RemoveListener(RotateStep);

            geoCatch.SetActive(true);
        }

        public void Place(Vector2Int pos)
        {
            ClearOccupancy();

            GetComponent<GridMover>().MoveToInt(pos);
        }

        public void RotateStep()
        {
            // clear the Occupancy
            ClearOccupancy();

            // Send the rotate Command
            GetComponent<GridMover>().RotateStep();

            // set the Occupancy
            // This is done in a completed callback from the GridMover.
        }

        /// <remarks>
        /// If doneMoving is true, it sends this Placeable.
        /// </remarks>
        void SetOccupancy(bool doneMoving)
        {
            Debug.Log("DoneMoving: SetOccupancy", this);
            // foreach (Vector2Int v in this)
            // {
            //     // Debug.Log("Occupancy: " + v);
            //     TileController.Instance.SetTileOccupancy(v, doneMoving ? this : null);
            // }
            Vector2Int start, end = default;
            (start, end) = GetCurrentFootprint();

            Debug.Log("SetOccupancy: " + start + ", " + end);

            TileController.Instance.SetTileOccupancyByPosition(start, end, doneMoving ? this : null);
        }

        void ClearOccupancy()
        {
            SetOccupancy(false);
        }

        public IEnumerator<Vector2Int> GetEnumerator()
        {
            for (int i = 0; i < footprint.x; i++)
            {
                for (int j = 0; j < footprint.y; j++)
                {
                    yield return new Vector2Int(transform.Position2dInt().x + i, transform.Position2dInt().y + j);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void CreateTileCatch()
        {
            // Create the collider dynamically on runtime. So it doesn't clutter up the inspector.
            tileCatch = new GameObject("TileCatch");
            tileCatch.layer = 8;
            // REFACTOR: this assumes the model is the first child.
            tileCatch.transform.SetParent(pivot.transform.GetChild(0), false);
            BoxCollider col = tileCatch.AddComponent<BoxCollider>();
            col.size = new Vector3(footprint.x, 0.3f, footprint.y);
            col.center = new Vector3(footprint.x*0.5f, -0.05f, footprint.y*0.5f);
        }

        private void CreateGeoCatch()
        {
            geoCatch = new GameObject("GeoCatch", typeof(PlaceableGeometry));
            // REFACTOR: this assumes the model is the first child.
            geoCatch.transform.SetParent(pivot.transform.GetChild(0), false);
            MeshCollider geo = geoCatch.AddComponent<MeshCollider>();
            geo.sharedMesh = gameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        public (Vector2Int start, Vector2Int end) GetCurrentFootprint()
        {
            return GetFootprintWithRotationStep(GetComponent<GridMover>().rotationStep);
        }

        public (Vector2Int start, Vector2Int end) GetFootprintWithRotationStep(int step)
        {
            Vector2Int end = Vector2Int.zero;
            // REFACTOR should we not subtract 1? We'd have to change
            // TileController.CheckTileOccupancyByPosition to < instead of <=
            Vector2Int adjustedFootprint = footprint - Vector2Int.one;
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
            // Debug.Log("Footprint rotation: " +
            //         GetComponent<GridMover>().pivot.rotation.eulerAngles);
            // Keep in mind iterating through this will need to stop before the end.
            return (transform.Position2dInt(), end);
        }
    }
}
