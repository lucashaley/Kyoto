using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    public class PlaceableGeometry : MonoBehaviour
    {
        // When clicked, turn on placer
        void OnMouseUp()
        {
            // REFACTOR to use Events
            GetComponentInParent<Placeable>().Select();
        }
    }
}
