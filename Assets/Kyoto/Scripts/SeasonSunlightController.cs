using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    public class SeasonSunlightController : MonoBehaviour
    {
        private SeasonSunlight[] sunlights;
        public Color weightedColor;

        // Start is called before the first frame update
        void Start()
        {
            sunlights = GetComponentsInChildren<SeasonSunlight>();
        }

        // Update is called once per frame
        void Update()
        {
            Color averagedColor = Color.black;

            for (int i = 0; i < sunlights.Length; i++)
            {
                // averagedColor += sunlights[i].sunColor * sunlights[i].GetWeight();
                averagedColor = Color.Lerp(averagedColor, sunlights[i].sunColor, sunlights[i].GetWeight());
                // Debug.Log(sunlights[i].GetWeight());
                // Debug.Log("Color:" + averagedColor);
            }

            weightedColor = averagedColor;
        }
    }
}
