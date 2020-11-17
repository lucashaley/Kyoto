using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pixelplacement;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    /// <summary>
    /// A singleton to control the timescale of the game.
    /// </summary>
    public class TimeController : Singleton<TimeController>
    {
        [Header("Date and Time")]
        [SerializeField] public float timeScale = 1;
        public bool useTimeScale = false;
        public bool overrideDate;
        public String overriddenDate;

        [Header("Location Start")]
        [SerializeField] public DateTime locationStartDateTime;
        [Header("Real DateTime")]
        [SerializeField] public DateTime realDateTime;
        [Header("Game DateTime")]
        [SerializeField] public DateTime gameDateTime;

        [Header("Normalized")]
        public FloatVariable gameDateNormalizedVariable;
        public FloatVariable gameTimeNormalizedVariable;

        void Start()
        {
            Time.timeScale = useTimeScale ? timeScale : 1.0f;
            gameDateTime = System.DateTime.Now;
            realDateTime = System.DateTime.Now;
            locationStartDateTime = System.DateTime.Now;
        }

        // Update is called once per frame
        void Update()
        {
            if (useTimeScale)
            {
                gameDateTime.AddSeconds(Time.deltaTime);
            } else {
                TimeSpan difference = new TimeSpan((long)(TimeSpan.TicksPerSecond * Time.deltaTime * timeScale));
                gameDateTime.AddTimeSpan(difference);
            }
            realDateTime.AddSeconds(Time.unscaledDeltaTime);
            gameDateNormalizedVariable.Value = gameDateTime.NormalizedDate();
            gameTimeNormalizedVariable.Value = gameDateTime.NormalizedTime();
        }

        public System.DateTime getGameDateTime ()
        {
            return (System.DateTime)gameDateTime;
        }

        public float GameTimeNormalized()
        {
            return gameTimeNormalizedVariable.Value;
        }

        public float GameDateNormalized()
        {
            return gameDateNormalizedVariable.Value;
        }
    }
}
