using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    public static class Vector2Extensions
    {
        public static Vector2Int Vector2Int(this Vector2 v)
        {
            return new Vector2Int
            (
                Mathf.RoundToInt(v.x),
                Mathf.RoundToInt(v.y)
            );
        }

        public static Vector3 Vector3NoY(this Vector2Int v)
        {
            return new Vector3
            (
                (float)v.x,
                0f,
                (float)v.y
            );
        }

        public static Vector3Int Vector3Int(this Vector2 v)
        {
            return new Vector3Int
            (
                Mathf.RoundToInt(v.x),
                0,
                Mathf.RoundToInt(v.y)
            );
        }

        public static Vector3Int Vector3Int(this Vector2Int v)
        {
            return new Vector3Int
            (
                v.x,
                0,
                v.y
            );
        }

        public static Vector2Int Transpose(this Vector2Int v)
        {
            return new Vector2Int(v.y, v.x);
        }
    }
}
