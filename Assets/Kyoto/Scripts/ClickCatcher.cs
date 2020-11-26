using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;

namespace Kyoto
{
    public class ClickCatcher : Singleton<ClickCatcher>
    {
        public Positioner positioner;

        // Start is called before the first frame update
        void Awake()
        {
            positioner = Positioner.Instance;
        }

        void OnMouseUpAsButton()
        {
            positioner?.Deactivate();
        }
    }
}
