using UnityEngine;
using System.Collections;

//It is common to create a class to contain all of your
//extension methods. This class must be static.
// https://learn.unity.com/tutorial/extension-methods#5c89416dedbc2a1410355318
namespace Kyoto
{
    public static class TransformExtensions
    {
        //Even though they are used like normal methods, extension
        //methods must be declared static. Notice that the first
        //parameter has the 'this' keyword followed by a Transform
        //variable. This variable denotes which class the extension
        //method becomes a part of.
        public static void ResetTransformation(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localRotation = Quaternion.identity;
            trans.localScale = new Vector3(1, 1, 1);
        }

        public static Vector2 Position2d(this Transform trans)
        {
            return trans.position.Vector2NoY();
        }
        
        public static Vector2Int Position2dInt(this Transform trans)
        {
            return trans.position.Vector2IntNoY();
        }

        public static Vector2Int LocalPosition2dInt(this Transform trans)
        {
            return trans.localPosition.Vector2IntNoY();
        }
    }
}
