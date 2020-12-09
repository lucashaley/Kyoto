using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    [SelectionBase]
    public class Tile : MonoBehaviour
    {
        public enum TileEdge {
            None    = 0,
            Top     = 1,
            Left    = 2,
            Bottom  = 4,
            Right   = 8
        }

        // REFACTOR to use UnityAtoms?
        // public ViewStateController viewState;
        // REFACTOR to use UnityAtoms
        // float rakeThreshold = 0.5f;
        public BoolVariable canRake;
        public BoolVariable isRaking;
        public FloatVariable rakeThreshold;
        public BoxCollider tileCollider;
        // public Vector3 boundsCenter;
        public Vector3 boundsMin, boundsMax;
        public TileEdge enterEdge, exitEdge;

        public Vector2 corner = Vector2.zero;
        public bool isStraight;
        public bool isPerpendicular;

        public Texture TopBottom, LeftRight;
        public Texture BottomLeft, BottomRight, TopLeft, TopRight;

        // REFACTOR separate out the raking from the basic tile info?

        void Awake()
        {
            // Debug.Log("Tile: Awake");
            // refactor
            // viewState = GameObject.Find("GameController").GetComponent<ViewStateController>();
            tileCollider = GetComponent<BoxCollider>();
            // boundsCenter = tileCollider.bounds.center;
            boundsMin  = tileCollider.bounds.min;
            boundsMax = tileCollider.bounds.max;
            enterEdge = exitEdge = TileEdge.None;
            TileController.Instance.AddTile(this, transform.Position2dInt());

            // Change the name of the Tile to reflect it's position
            gameObject.name = "Tile " + transform.Position2dInt();
        }

        public void OnCanRakeChangeEvent(bool canRake)
        {
            Debug.Log("OnIsRakingChangeEvent");
        }

        // Something has gone wrong here.
        void OnMouseDown()
        {
            Debug.Log("canRake: " + canRake.Value);
            if (canRake.Value)
            {
                isRaking.Value = true;
            }
        }
        void OnMouseUp()
        {
            isRaking.Value = false;
        }

        void OnCollisionEnter(Collision col)
        {
            Debug.Log("Collision: " + gameObject.name + ", " + col.transform.gameObject.name);
        }

        void OnTriggerEnter(Collider col)
        {
            Debug.Log("TriggerEnter: " + gameObject.name + ", " + col.transform.gameObject.name);
            if (Input.GetMouseButton(0))
            {
                enterEdge = GetEdge(Input.mousePosition);
            }
        }

        void OnTriggerExit(Collider col)
        {
            Debug.Log("TriggerExit: " + gameObject.name + ", " + col.transform.gameObject.name);
            if (Input.GetMouseButton(0))
            {
                exitEdge = GetEdge(Input.mousePosition);
            }

            // gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", switcher);

            // check if we're in the Raking state
            // REFACTOR to use UnityAtoms
            // if (gameController.viewStateController.currentState.gameObject.name == "RakingState")
            if (canRake.Value)
            {
                // REFACTOR
                // stop repeating all the set textures
                Texture newTexture;
                Debug.Log(enterEdge & exitEdge);
                if ((enterEdge == TileEdge.Top || enterEdge == TileEdge.Bottom) && (exitEdge == TileEdge.Top || exitEdge == TileEdge.Bottom))
                {
                    newTexture = TopBottom;
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", TopBottom);
                    isStraight = true;
                    isPerpendicular = true;
                }
                if ((enterEdge == TileEdge.Left || enterEdge == TileEdge.Right) && (exitEdge == TileEdge.Left || exitEdge == TileEdge.Right))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", LeftRight);
                    isStraight = true;
                    isPerpendicular = false;
                }
                if ((enterEdge == TileEdge.Left || exitEdge == TileEdge.Left)  && (enterEdge == TileEdge.Bottom || exitEdge == TileEdge.Bottom))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", BottomLeft);
                    isStraight = false;
                    corner = Vector2.zero;
                }
                if ((enterEdge == TileEdge.Right || exitEdge == TileEdge.Right)  && (enterEdge == TileEdge.Bottom || exitEdge == TileEdge.Bottom))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", BottomRight);
                    isStraight = false;
                    corner = Vector2.right;
                }
                if ((enterEdge == TileEdge.Left || exitEdge == TileEdge.Left)  && (enterEdge == TileEdge.Top || exitEdge == TileEdge.Top))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", TopLeft);
                    isStraight = false;
                    corner = Vector2.up;
                }
                if ((enterEdge == TileEdge.Right || exitEdge == TileEdge.Right)  && (enterEdge == TileEdge.Top || exitEdge == TileEdge.Top))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", TopRight);
                    isStraight = false;
                    corner = Vector2.one;
                }
                //REFACTOR
                // and just do it here once
                // GameObject.SetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", newTexture);

                UpdateRake();
            }
        }
        
        /// <summary>
        /// Called when the script is loaded or a value is changed in the
        /// inspector (Called in the editor only).
        /// </summary>
        void OnValidate()
        {
            UpdateRake();
        }

        private void UpdateRake()
        {
            Renderer rend = GetComponentInChildren<Renderer>();
            // Debug.Log(Shader.PropertyToID("Corner"));
            MaterialPropertyBlock prop = new MaterialPropertyBlock();
            rend.GetPropertyBlock(prop);
            // Debug.Log(prop.isEmpty);
            prop.SetVector("Corner", corner);
            prop.SetFloat("IsStraight", System.Convert.ToSingle(isStraight));
            prop.SetFloat("IsPerpendicular", System.Convert.ToSingle(isPerpendicular));
            // Debug.Log(prop);
            rend.SetPropertyBlock(prop);
        }

        protected TileEdge GetEdge(Vector3 mousePosition)
        {
            int layerMask = LayerMask.GetMask("Tiles");
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                // Debug.LogWarning("<color=lime>Tile</color>.<color=cyan>GetEdge</color>", hit.collider.gameObject);
                // world coords
                Vector3 closestPoint = tileCollider.ClosestPointOnBounds(hit.point);
                // local coords
                // Vector3 closestPoint = transform.InverseTransformPoint(tileCollider.ClosestPointOnBounds(hit.point));
                // Debug.Log("Closest point: " + closestPoint);
                // if (Mathf.Approximately(closestPoint.x, boundsMin.x)) {Debug.Log("Left");}
                if ((Mathf.Abs(closestPoint.x - boundsMin.x)) < rakeThreshold.Value)
                {
                    // Debug.Log("Left");
                    return TileEdge.Right;
                }
                if ((Mathf.Abs(closestPoint.z - boundsMin.z)) < rakeThreshold.Value)
                {
                    // Debug.Log("Top");
                    return TileEdge.Top;
                }
                if ((Mathf.Abs(closestPoint.x - boundsMax.x)) < rakeThreshold.Value)
                {
                    // Debug.Log("Right");
                    return TileEdge.Left;
                }
                if ((Mathf.Abs(closestPoint.z - boundsMax.z)) < rakeThreshold.Value)
                {
                    // Debug.Log("Bottom");
                    return TileEdge.Bottom;
                }
            }
            return TileEdge.None;
        }
    }
}
