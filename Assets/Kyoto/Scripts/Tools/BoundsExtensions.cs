using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    public static class Bounds3Extensions
    {
        public static Bounds RotateStepAround(this Bounds b, Vector2 pivot)
    	{
            Bounds newBounds = b;
            newBounds.extents = new Vector3(b.extents.z, b.extents.y, b.extents.x);

            Vector2 newCenter = b.center.Vector2NoY().RotateStepAround(pivot);

            newBounds.center = new Vector3(newCenter.x, b.center.y, newCenter.y);

    		return newBounds;
    	}

        public static Bounds Offset(this Bounds b, Vector2Int offset)
        {
            Bounds newBounds = b;
            newBounds.min += offset.Vector3NoY();
            newBounds.max += offset.Vector3NoY();

            return newBounds;
        }

        public static List<Vector2Int> ToVector2IntList(this Bounds b)
        {
            // We can't extend with IEnumerator, so this is a fix
            List<Vector2Int> arr = new List<Vector2Int>();

            Vector2Int boundsMin, boundsMax;
            boundsMin = b.min.Vector2IntNoY();
            boundsMax = b.max.Vector2IntNoY();
            Debug.Log("BoundsMinMax: " + boundsMin + ", " + boundsMax);
            for (int x = boundsMin.x; x < boundsMax.x; x++)
            {
                for (int y = boundsMin.y; y < boundsMax.y; y++)
                {
                    arr.Add(new Vector2Int(x, y));
                }
            }

            return arr;
        }
    }
}
