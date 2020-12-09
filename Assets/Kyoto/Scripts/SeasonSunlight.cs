using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    public class SeasonSunlight : WeightedCycle
    {
        public Color sunColor;

        protected override void SetWeight()
        {
            // weight = curve.Evaluate(timeController.GameTimeNormalized());
            weight = curve.Evaluate(percentage.Value);
        }

    }
}
