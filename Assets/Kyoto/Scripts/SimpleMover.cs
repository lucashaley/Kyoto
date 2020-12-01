
using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Pixelplacement;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    public class SimpleMover : MonoBehaviour
    {
        public Transform rotateBase;
        public Transform translateBase;
        public UnityEvent beforeTween;
        public UnityEvent afterTween;

        public bool isTweening;
        public bool fancy = true;
        public FloatVariable fadeValue;
        public AnimationCurveVariable curve;

        private void StartTween()
        {
            isTweening = true;
            beforeTween.Invoke();
        }

        private void EndTween()
        {
            isTweening = false;
            translateBase.localPosition = translateBase.localPosition.RoundedInt();
            translateBase.localScale = translateBase.localScale.RoundedInt();
            afterTween.Invoke();
        }

        public void RotateStep()
        {
            if (!isTweening)
            {
                Tween.LocalRotation(rotateBase,
                                    rotateBase.localEulerAngles + (Vector3.up * 90f),
                                    fadeValue.Value,
                                    0.0f,
                                    Tween.EaseInOutStrong,
                                    Tween.LoopType.None,
                                    StartTween,
                                    EndTween
                                    );
                if (fancy)
                {
                    Tween.LocalScale(translateBase,
                                   new Vector3(0.85f, 0.85f, 0.85f),
                                   fadeValue.Value,
                                   0,
                                   curve.curve,
                                   Tween.LoopType.None
                                   );
                }
            }
        }

        public void MoveTo(Vector2Int destination)
        {
            if (!isTweening)
            {
                // SetOccupancy(false);

                Tween.Position(translateBase,
                              destination.Vector3NoY(),
                              fadeValue.Value,
                              0.0f,
                              Tween.EaseInOutStrong,
                              Tween.LoopType.None,
                              StartTween,
                              EndTween
                              );
                Tween.LocalScale(translateBase,
                                 new Vector3(0.85f, 0.85f, 0.85f),
                                 fadeValue.Value,
                                 0,
                                 curve.curve,
                                 Tween.LoopType.None
                                 );
            }
        }
    }
}
