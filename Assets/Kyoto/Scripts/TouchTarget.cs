using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    public class TouchTarget : MonoBehaviour
    {
        private Rigidbody rbody;
        private int tileLayer;

        // Start is called before the first frame update
        void Awake()
        {
            rbody = GetComponent<Rigidbody>();
            tileLayer = LayerMask.GetMask("Tiles");
        }

        // REFACTOR use the TileController to just get the tiles directly
        // without using a raycast?
        void FixedUpdate()
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayer))
            {
                // check if we hit a tile
                if (hit.collider.gameObject.TryGetComponent(out Tile tile))
                {
                    // refactor to limit y value
                    rbody.MovePosition(hit.point);
                }
            }
        }

        void OnDrawGizmos()
        {
            Color g = Color.yellow;
            g.a = 0.25f;
            Gizmos.color = g;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }

    }
}
