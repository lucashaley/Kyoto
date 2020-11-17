﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
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
        public FloatVariable rakeThreshold;
        public BoxCollider tileCollider;
        // public Vector3 boundsCenter;
        public Vector3 boundsMin, boundsMax;
        public TileEdge enterEdge, exitEdge;

        public Texture TopBottom, LeftRight;
        public Texture BottomLeft, BottomRight, TopLeft, TopRight;

        // private Transform2dInt transform2d;
        // public Vector2Int Position2d { get => transform2d.Position; }

        void Awake()
        {
            // refactor
            // viewState = GameObject.Find("GameController").GetComponent<ViewStateController>();
            tileCollider = GetComponent<BoxCollider>();
            // boundsCenter = tileCollider.bounds.center;
            boundsMin  = tileCollider.bounds.min;
            boundsMax = tileCollider.bounds.max;
            enterEdge = exitEdge = TileEdge.None;
            TileController.Instance.AddTile(this, transform.Position2dInt());
        }

        void OnMouseDown()
        {
            // REFACTOR to use UnityAtoms
            // inputController.isRaking = true;
        }
        void OnMouseUp()
        {
            // REFACTOR to use UnityAtoms
            // inputController.isRaking = false;
        }

        void OnCollisionEnter(Collision col)
        {
            Debug.Log("Collision: " + gameObject.name + ", " + col.transform.gameObject.name);
        }

        void OnTriggerEnter(Collider col)
        {
            if (Input.GetMouseButton(0))
            {
                enterEdge = GetEdge(Input.mousePosition);
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (Input.GetMouseButton(0))
            {
                exitEdge = GetEdge(Input.mousePosition);
            }

            // gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", switcher);

            // check if we're in the Raking state
            // REFACTOR to use UnityAtoms
            // if (gameController.viewStateController.currentState.gameObject.name == "RakingState")
            if (1 == 1)
            {
                // REFACTOR
                // stop repeating all the set textures
                Texture newTexture;
                // Debug.Log(enterEdge & exitEdge);
                if ((enterEdge == TileEdge.Top || enterEdge == TileEdge.Bottom) && (exitEdge == TileEdge.Top || exitEdge == TileEdge.Bottom))
                {
                    newTexture = TopBottom;
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", TopBottom);
                }
                if ((enterEdge == TileEdge.Left || enterEdge == TileEdge.Right) && (exitEdge == TileEdge.Left || exitEdge == TileEdge.Right))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", LeftRight);
                }
                if ((enterEdge == TileEdge.Left || exitEdge == TileEdge.Left)  && (enterEdge == TileEdge.Bottom || exitEdge == TileEdge.Bottom))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", BottomLeft);
                }
                if ((enterEdge == TileEdge.Right || exitEdge == TileEdge.Right)  && (enterEdge == TileEdge.Bottom || exitEdge == TileEdge.Bottom))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", BottomRight);
                }
                if ((enterEdge == TileEdge.Left || exitEdge == TileEdge.Left)  && (enterEdge == TileEdge.Top || exitEdge == TileEdge.Top))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", TopLeft);
                }
                if ((enterEdge == TileEdge.Right || exitEdge == TileEdge.Right)  && (enterEdge == TileEdge.Top || exitEdge == TileEdge.Top))
                {
                    gameObject.GetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", TopRight);
                }
                //REFACTOR
                // and just do it here once
                // GameObject.SetComponentInChildren<Renderer>().material.SetTexture("_BumpMap", newTexture);
            }
        }

        protected TileEdge GetEdge(Vector3 mousePosition)
        {
            int layerMask = 1 << 26;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
            {
                Debug.LogWarning("<color=lime>Tile</color>.<color=cyan>GetEdge</color>", hit.collider.gameObject);
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
