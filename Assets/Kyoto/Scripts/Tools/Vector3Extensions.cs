using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    public static class Vector3Extensions
    {
        // https://gist.github.com/jweimann/f51c7a333e07e3e7501e8ade499d11a8#file-vector3extensions-cs
    	public static Vector3 Flattened(this Vector3 vector)
    	{
    		return new Vector3(vector.x, 0f, vector.z);
    	}

    	public static float DistanceFlat(this Vector3 origin, Vector3 destination)
    	{
    		return Vector3.Distance(origin.Flattened(), destination.Flattened());
    	}

        // from https://www.alanzucconi.com/2015/07/22/how-to-snap-to-grid-in-unity3d/
        public static Vector3 Rounded(this Vector3 v, float snapValue)
        {
            return new Vector3
            (
                snapValue * Mathf.Round(v.x / snapValue),
                snapValue * Mathf.Round(v.y / snapValue),
                snapValue * Mathf.Round(v.z / snapValue)
            );
        }

        public static Vector3Int RoundedInt(this Vector3 v)
        {
            return new Vector3Int
            (
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y),
                Mathf.RoundToInt(v.z)
            );
        }

        // https://gist.github.com/omgwtfgames/f917ca28581761b8100f#file-vectorextensionmethods-cs
        public static Vector3 WithX(this Vector3 v, float x)
        {
            return new Vector3(x, v.y, v.z);
        }

        public static Vector3 WithY(this Vector3 v, float y)
        {
            return new Vector3(v.x, y, v.z);
        }

        public static Vector3 WithZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, z);
        }

        // https://gist.github.com/omgwtfgames/f917ca28581761b8100f#gistcomment-2828766
        public static Vector3 WithAddX(this Vector3 v, float x)
        {
            return new Vector3(v.x + x, v.y, v.z);
        }

        public static Vector3 WithAddY(this Vector3 v, float y)
        {
            return new Vector3(v.x, v.y + y, v.z);
        }

        public static Vector3 WithAddZ(this Vector3 v, float z)
        {
            return new Vector3(v.x, v.y, v.z + z);
        }

        public static Vector2 Vector2NoY(this Vector3 v)
        {
            return new Vector2(v.x, v.z);
        }

        public static Vector2Int Vector2IntNoY(this Vector3 v)
        {
            return new Vector2Int
            (
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.z)
            );
        }
    }
}
