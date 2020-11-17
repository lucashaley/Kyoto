using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pixelplacement;

namespace Kyoto
{
    public class StateView : State
    {
        public ViewData viewData;
        public string message;

        public UnityEvent doEnable;
        public UnityEvent doDisable;

        void OnEnable()
        {
            doEnable.Invoke();
        }

        void OnDisable()
        {
            doDisable.Invoke();
        }
    }
}
