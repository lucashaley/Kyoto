using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    public class PlaceableGeometry : MonoBehaviour
    {
        public BoolVariable canPlace;
        // public bool canClick;

        // public void SetClickable(bool inCanClick)
        // {
        //     canClick = inCanClick;
        // }

        // When clicked, turn on placer
        void OnMouseUp()
        {
            // REFACTOR to use Events
            if (canPlace.Value)
                GetComponentInParent<Placeable>().Select();
        }
    }
}
