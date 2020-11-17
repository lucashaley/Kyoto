using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using Pixelplacement;

namespace Kyoto
{
    [RequireComponent(typeof(Fadeable))]
    public class PlaceableSelector : MonoBehaviour
    {
        public bool isMoving;
        public bool isTweening;
        public Fadeable fader;
        public FloatVariable fadeValue;
        public AnimationCurveVariable curve;

        private Renderer rend;
        public LayerMask mask;
        public Transform currentTileTransform;

        void Start()
        {
            fader = gameObject.GetComponent<Fadeable>();
            rend = gameObject.GetComponentInChildren<Renderer>();
            mask = LayerMask.GetMask("Tiles");
        }

        void OnMouseDown()
        {
            Debug.Log("Teleport MouseDown");
            isMoving = true;
            currentTileTransform = TransformFromRaycast();
        }

        private Transform TransformFromRaycast()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, mask))
            {
                Debug.DrawLine(ray.origin, hit.point);
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
    }
}
