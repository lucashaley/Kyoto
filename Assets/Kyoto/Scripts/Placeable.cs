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
        public Vector2Int footprint = Vector2Int.one;
        public Vector3Int volume = Vector3Int.one;

        private GameObject model;

        private GameObject tileCatch;
        private GameObject geoCatch;

        private Positioner positioner;

        void Awake()
        {
            // Just added the "Geometry" subonbject, messed things up
            model = transform.Find("Pivot").gameObject;

            CreateTileCatch();
            CreateGeoCatch();

            positioner = Positioner.Instance;
            positioner.placeableEvent.AddListener(this.Deselect);
        }

        /// <summary>
        /// Registers the doneMoving, so it can set occupancy
        /// </summary>
        void OnEnable()
        {
            GetComponent<GridMover>().doneMoving.AddListener(SetOccupancy);
            GetComponent<GridMover>().doneMoving.AddListener(ClampTransform);
        }

        void OnDisable()
        {
            GetComponent<GridMover>().doneMoving.RemoveListener(SetOccupancy);
            GetComponent<GridMover>().doneMoving.RemoveListener(ClampTransform);
        }

        public void Select()
        {
            geoCatch.SetActive(false);
            positioner.Activate(this);

            positioner.movePositionerEvent.AddListener(Place);
            positioner.rotatePositionerEvent.AddListener(GetComponent<GridMover>().RotateStep);
        }

        public void Deselect()
        {
            positioner.movePositionerEvent.RemoveListener(Place);
            positioner.rotatePositionerEvent.RemoveListener(GetComponent<GridMover>().RotateStep);

            geoCatch.SetActive(true);
        }

        public void Place(Vector2Int pos)
        {
            // REFACTOR: do we still need this level? Can be direct?
            GetComponent<GridMover>().MoveToInt(pos);
        }

        /// <remarks>
        /// If doneMoving is true, it sends this Placeable.
        /// </remarks>
        void SetOccupancy(bool doneMoving)
        {
            foreach (Vector2Int v in this)
            {
                // Debug.Log("Occupancy: " + v);
                TileController.Instance.SetTileOccupancy(v, doneMoving ? this : null);
            }
        }

        void ClampTransform(bool doneMoving)
        {
            //Moved this to the GridMover
            // transform.position = transform.position.RoundedInt();
            // transform.localScale = Vector3.one;
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
            tileCatch.transform.SetParent(transform, false);
            BoxCollider col = tileCatch.AddComponent<BoxCollider>();
            col.size = new Vector3(footprint.x, 0.3f, footprint.y);
            col.center = new Vector3(footprint.x*0.5f, -0.05f, footprint.y*0.5f);
        }

        private void CreateGeoCatch()
        {
            geoCatch = new GameObject("GeoCatch", typeof(PlaceableGeometry));
            geoCatch.transform.SetParent(transform, false);
            MeshCollider geo = geoCatch.AddComponent<MeshCollider>();
            geo.sharedMesh = gameObject.GetComponentInChildren<MeshFilter>().sharedMesh;
        }

        public (Vector2Int start, Vector2Int end) GetCurrentFootprint()
        {
            Debug.Log("Footprint rotation: " + GetComponent<GridMover>().pivot.rotation.eulerAngles);
            // Keep in mind iterating through this will need to stop before the end.
            return (transform.Position2dInt(), transform.Position2dInt() + footprint);
        }
    }
}
