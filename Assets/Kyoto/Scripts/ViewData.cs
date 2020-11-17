using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    [CreateAssetMenu(fileName = "New ViewData", menuName = "Kyoto/View Data", order = 52)]
    public class ViewData : ScriptableObject
    {
        [SerializeField] private Vector3 rotation = default;

        public Quaternion Rotation
        {
            get => Quaternion.Euler(rotation);
        }
    }
}
