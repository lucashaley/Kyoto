using System.Collections;
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
        public Vector2Int footprint
        {
            get
            {
                return volume.Vector2IntNoY();
            }
        }
        public Vector3Int volume = Vector3Int.one;
        public int rotationStep = 0;

        private Transform pivot;

        private GameObject tileCatch;
        private GameObject geoCatch;

        private Positioner positioner;

        void Awake()
        {
            pivot = transform.Find("Pivot");
            positioner = Positioner.Instance;
            // positioner.placeableEvent.AddListener(this.Deselect);

            CreateTileCatch();
            CreateGeoCatch();

            SetOccupancy(true);
        }

        /// <summary>
        /// Registers the doneMoving, so it can set occupancy
        /// </summary>
        void OnEnable()
        {
            // GetComponent<GridMover>().doneMoving.AddListener(SetOccupancy);
        }

        void OnDisable()
        {
            // GetComponent<GridMover>().doneMoving.RemoveListener(SetOccupancy);
        }

        /// <summary>
        /// This gets called from the Geometry collider MouseDown
        /// It makes this Placeable ready to be moved by the Positioner
        /// </summary>
        public void Select()
        {
            Debug.Log("Select: " + gameObject.name);
            // Audio cue

            // Turn geometry catch off
            geoCatch.SetActive(false);

            // Turn on Positioner, and assign it to this
            positioner.Activate(this);
            // Register the Place method for the Positioner
            // positioner.RegisterMoveEvent(Place);
            // positioner.rotatePositionerEvent.AddListener(RotateStep);
        }

        public void Deselect()
        {
            // positioner.DeregisterMoveEvent(Place);
            // positioner.rotatePositionerEvent.RemoveListener(GetComponent<GridMover>().RotateStep);
            // positioner.rotatePositionerEvent.RemoveListener(RotateStep);

            geoCatch.SetActive(true);
        }

        // public void Place(Vector2Int pos)
        // {
        //     ClearOccupancy();
        //
        //     GetComponent<GridMover>().MoveToInt(pos);
        // }
        //
        // public void RotateStep()
        // {
        //     // clear the Occupancy
        //     ClearOccupancy();
        //
        //     // Send the rotate Command
        //     GetComponent<GridMover>().RotateStep();
        //
        //     // set the Occupancy
        //     // This is done in a completed callback from the GridMover.
        // }
        //
        /// <remarks>
        /// Should this be here or in the Positioner?
        /// Not working yet!
        /// </remarks>
        public void SetOccupancy(bool active)
        {
            Debug.Log("Placeable: SetOccupancy", this);

            Vector2Int start, end = default;
            (start, end) = GetCurrentFootprint();

            Debug.Log("SetOccupancy: " + start + ", " + end);

            TileController.Instance.SetTileOccupancyByPosition(start, end, active ? this : null);
        }

        public void ClearOccupancy()
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
            tileCatch.transform.SetParent(pivot.GetChild(0), false);
            BoxCollider col = tileCatch.AddComponent<BoxCollider>();
            col.size = new Vector3(footprint.x, 0.3f, footprint.y);
            col.center = new Vector3(footprint.x*0.5f, -0.05f, footprint.y*0.5f);
        }

        public void SetTileCatch(bool active)
        {
            tileCatch.SetActive(active);
        }

        private void CreateGeoCatch()
        {
            geoCatch = new GameObject("GeoCatch", typeof(PlaceableGeometry));
            // REFACTOR: this assumes the model is the first child.
            geoCatch.transform.SetParent(pivot.GetChild(0), false);
            MeshCollider geo = geoCatch.AddComponent<MeshCollider>();
            geo.sharedMesh = gameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        public (Vector2Int start, Vector2Int end) GetCurrentFootprint()
        {
            return GetFootprintWithRotationStep(rotationStep);
        }

        public (Vector2Int start, Vector2Int end) GetFootprintWithRotationStep(int step)
        {
            Vector2Int end = Vector2Int.zero;
            // REFACTOR should we not subtract 1? We'd have to change
            // TileController.CheckTileOccupancyByPosition to < instead of <=
            Vector2Int adjustedFootprint = footprint - Vector2Int.one;

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
    }
}
