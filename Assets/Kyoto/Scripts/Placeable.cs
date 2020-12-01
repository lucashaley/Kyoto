using System.Collections;
using System.Collections.Generic;
// using System.Linq;
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

        public Transform pivot;

        private GameObject tileCatch;
        private GameObject geoCatch;

        private Positioner positioner;

        public BoxCollider bounder;

        // public List<Tile> occipiedTiles; // this is dynamic, which we don't really need
        public Tile[] occupiedTiles;

        void Awake()
        {
            pivot = transform.Find("Pivot");
            positioner = Positioner.Instance;

            occupiedTiles = new Tile[footprint.x * footprint.y];

            StartCoroutine(Initialization());
        }

        private IEnumerator Initialization()
        {
            Debug.Log("Initialization");
            bounder = CreateBounder();
            CreateTileCatch();
            CreateGeoCatch();

            // We need to wait here, because for stupid reasons
            // Unity was trying to SetOccupancy before the bounder
            // has completed being made.
            yield return new WaitUntil(() => bounder != null);

            SetOccupancy(true);
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
        }

        public void Deselect(bool fromPositioner = false)
        {
            if (fromPositioner)
            {
                Quaternion rot = transform.rotation;
                transform.SetParent(GameObject.Find("Placeables").transform);

                transform.rotation = Quaternion.identity;
                pivot.rotation = rot;
            }
            geoCatch.SetActive(true);
        }

        public void BeforeTween()
        {
            SetOccupancy(false);
        }
        public void AfterTween()
        {
            rotationStep = (rotationStep+1)%4;
            SetOccupancy(true);
        }

        /// <remarks>
        /// </remarks>
        public void SetOccupancy(bool active = true)
        {
            int i = 0;
            foreach (Vector2Int v in this)
            {
                occupiedTiles[i] = TileController.Instance.SetTileAsOccupied(v, active ? this : null);
                i++;
            }
        }

        public void ClearOccupancy()
        {
            SetOccupancy(false);
        }

        // REFACTOR switch this name with SetOccupancy
        public void SetOccupancyTo()
        {
            SetOccupancy(true);
        }

        public IEnumerator<Vector2Int> GetEnumerator()
        {
            // REFACTOR to use Bounds.Iterator
            Vector2Int boundsMin, boundsMax;
            boundsMin = bounder.bounds.min.Vector2IntNoY();
            boundsMax = bounder.bounds.max.Vector2IntNoY();
            for (int x = boundsMin.x; x < boundsMax.x; x++)
            {
                for (int y = boundsMin.y; y < boundsMax.y; y++)
                {
                    yield return new Vector2Int(x, y);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private BoxCollider CreateBounder()
        {
            Debug.Log("CreateBounder");
            GameObject bounderObject = new GameObject("Bounder");
            BoxCollider newBounder = bounderObject.AddComponent<BoxCollider>();
            newBounder.isTrigger = true;
            newBounder.size = volume.Vector3WithY(0.2f);
            newBounder.center = (newBounder.size * 0.5f);
            bounderObject.transform.SetParent(pivot, false);
            bounderObject.transform.position -= pivot.localPosition;

            return newBounder;
        }

        public Bounds GetBoundsRotated()
        {
            return bounder.bounds.RotateStepAround(pivot.Position2d());
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

        public (Vector2Int min, Vector2Int max) GetTileBounds()
        {
            Vector2Int min = bounder.bounds.min.Vector2IntNoY();
            Vector2Int max = (bounder.bounds.max - Vector3.one).Vector2IntNoY();

            return (min, max);
        }
    }
}
