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

        public static Vector3 Vector3NoY(this Vector2 v)
        {
            return new Vector3
            (
                v.x,
                0f,
                v.y
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

        public static Vector2Int Swizzle(this Vector2Int v)
        {
            return new Vector2Int(v.y, v.x);
        }

        public static Vector2 Swizzle(this Vector2 v)
        {
            return new Vector2(v.y, v.x);
        }

        public static Vector2 RotateStepAround(this Vector2 v, Vector2 pivot)
        {
            Vector2 delta = v - pivot;
            return new Vector2(pivot.x + delta.y, pivot.y + -delta.x);
        }
    }
}
