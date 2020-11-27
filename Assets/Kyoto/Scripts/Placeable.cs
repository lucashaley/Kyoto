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

        public BoxCollider bounder;

        void Awake()
        {
            pivot = transform.Find("Pivot");
            positioner = Positioner.Instance;
            // positioner.placeableEvent.AddListener(this.Deselect);

            CreateBounder();
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
            geoCatch.SetActive(true);
        }

        /// <remarks>
        /// Should this be here or in the Positioner?
        /// Not working yet!
        /// </remarks>
        public void SetOccupancy(bool active)
        {
            // Debug.Log("Placeable: SetOccupancy", this);

            Vector2Int start, end = default;
            // (start, end) = GetCurrentFootprint();
            (start, end) = GetTileBounds();

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

        private void CreateBounder()
        {
            bounder = gameObject.AddComponent<BoxCollider>();
            // bounder.enabled = false;
            bounder.isTrigger = true;
            bounder.size = volume.Vector3WithY(0.2f);
            // bounder.center = (Vector3)volume * 0.5f;
            // bounder.center.y = 0.1f;
            bounder.center = bounder.size * 0.5f;
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
            Vector2Int end = Vector2Int.zero, start = Vector2Int.zero;
            // REFACTOR should we not subtract 1? We'd have to change
            // TileController.CheckTileOccupancyByPosition to < instead of <=
            Vector2Int adjustedFootprint = footprint - Vector2Int.one;
            // Debug.Log("position: " + transform.Position2dInt());
            // Debug.Log("adjustedFootprint: " + adjustedFootprint);

            switch (step)
            {
                case 0:
                    start = transform.Position2dInt();
                    // end = transform.Position2dInt() + adjustedFootprint;
                    end = start + adjustedFootprint;
                    break;

                case 1:
                    start = transform.Position2dInt() - (Vector2Int.up * footprint.x);
                    // end = transform.Position2dInt() - adjustedFootprint.Transpose();
                    end = start + adjustedFootprint.Transpose();
                    break;

                case 2:
                    start = transform.Position2dInt() - footprint;
                    // end = transform.Position2dInt() - adjustedFootprint;
                    end = start + adjustedFootprint;
                    break;

                case 3:
                    start = transform.Position2dInt() - (Vector2Int.right * footprint.y);
                    // end = transform.Position2dInt() + adjustedFootprint.Transpose();
                    end = start + adjustedFootprint;
                    break;

            }

            return (start, end);
        }

        public (Vector2Int min, Vector2Int max) GetTileBounds()
        {
            Vector2Int min = bounder.bounds.min.Vector2IntNoY();
            Vector2Int max = (bounder.bounds.max - Vector3.one).Vector2IntNoY();

            return (min, max);
        }
    }
}
