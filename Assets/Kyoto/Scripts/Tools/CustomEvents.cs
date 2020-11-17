using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Kyoto
{
    [System.Serializable]
    public class Vector2IntEvent : UnityEvent<Vector2Int>
    {
    }
    [System.Serializable]
    public class IntEvent : UnityEvent<int>
    {
    }
    [System.Serializable]
    public class PlaceableEvent : UnityEvent
    {
    }
    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }

    // We don't really need this stuff.
    public class CustomEvents : MonoBehaviour
    {
    }
}
