using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    [CreateAssetMenu(fileName = "New LocationData", menuName = "Kyoto/Location Data")]
    public class LocationData : ScriptableObject
    {
        [SerializeField] private string locationName = default;
        [SerializeField] private float longitude = default, latitude = default;
        [SerializeField] private float offsetUTC = default;
        // [SerializeField] private List<SeasonAsset> seasonAssets;

        public float Longitude
        {
            get => longitude;
        }

        public float Latitude
        {
            get => latitude;
        }

        public float OffsetUTC
        {
            get => offsetUTC;
        }
    }
}
