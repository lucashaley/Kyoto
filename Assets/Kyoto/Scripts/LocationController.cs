using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityAtoms.BaseAtoms;

// https://suncalc.org/#/lat,lon,zoom/date/time/objectlevel/maptype

namespace Kyoto
{
    /// <summary>
    /// A singleton to store information about current location.
    /// </summary>
    public class LocationController : Singleton<LocationController>
    {
        public LocationData locationData;
        public SeasonSunlightController[] sunlightControllers;
        public SeasonAsset[] seasonAssets;
        public ColorVariable sun;

        // This is just passing on the values from the ScriptedObject data.
        // REFACTOR: Can we use a reference instead?
        public float Longitude
        {
            get => locationData.Longitude;
        }
        public float Latitude
        {
            get => locationData.Latitude;
        }
        public float OffsetUTC
        {
            get => locationData.OffsetUTC;
        }

        // Start is called before the first frame update
        void Start()
        {
            sunlightControllers = GetComponentsInChildren<SeasonSunlightController>();
            seasonAssets = GetComponentsInChildren<SeasonAsset>();
        }

        // Update is called once per frame
        void Update()
        {
            GetSunColor();
        }

        void GetSunColor()
        {
            Color averagedColor = Color.black;

            for (int i = 0; i < sunlightControllers.Length; i++)
            {
                averagedColor += sunlightControllers[i].weightedColor * seasonAssets[i].GetWeight();
            }

            sun.Value = averagedColor;
        }
    }
}
