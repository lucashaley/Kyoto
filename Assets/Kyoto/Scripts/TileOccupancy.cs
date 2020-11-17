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
            if (inPlaceable)
            {
                occupier = inPlaceable;
                GetComponentInChildren<Renderer>().material.color = Color.red;
            } else {
                occupier = null;
                GetComponentInChildren<Renderer>().material.color = Color.white;
            }
        }
    }
}
