using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    /// <summary>
    /// This class just keeps track of if a tile has a placeable.
    /// </summary>
    public class TileOccupancy : MonoBehaviour
    {
        public Placeable occupier;

        public void SetOccupier(Placeable inPlaceable)
        {
            // Debug.Log("SetOccupier: " + inPlaceable, this);
            if (inPlaceable)
            {
                occupier = inPlaceable;
            } else {
                occupier = null;
            }
        }

        public bool IsOccupied(Placeable placeable = null)
        {
            // Debug.Log("IsOccupied: " + transform.Position2dInt() + ", " + occupier + ", " + (occupier != null), this);
            return (occupier != null || occupier != placeable);
        }
    }
}
