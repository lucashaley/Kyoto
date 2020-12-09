using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    public class SeasonAsset : WeightedCycle
    {
        protected override void SetWeight()
        {
            // weight = curve.Evaluate(timeController.GameDateNormalized());
            weight = curve.Evaluate(percentage.Value);
        }
    }
}
