using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    [System.Serializable]
    public class DateTime : ISerializationCallbackReceiver
    {
        [HideInInspector] public System.DateTime raw;
        [HideInInspector] [SerializeField] private string rawString;
        [HideInInspector] private const float daySeconds = 86400;

        // Serialization
        public void OnBeforeSerialize()
        {
            rawString = raw.ToString();
        }

        public void OnAfterDeserialize() {
            System.DateTime.TryParse(rawString, out raw);
        }

        // Conversions
        public static implicit operator DateTime(System.DateTime dt)
        {
            return new DateTime() {raw = dt};
        }

        public static implicit operator System.DateTime(DateTime dt)
        {
            return (dt.raw);
        }

        // Display
        public string DisplayDateShort()
        {
            return raw.ToShortDateString();
        }

        // Simple Accessors
        public int Year ()
        {
            return raw.Year;
        }

        public int Day ()
        {
            return raw.Day;
        }

        public int Hour ()
        {
            return raw.Hour;
        }

        public int Minute ()
        {
            return raw.Minute;
        }

        public int Second ()
        {
            return raw.Second;
        }

        public int Millisecond ()
        {
            return raw.Millisecond;
        }

        public float DayOfYear ()
        {
            return (float)raw.DayOfYear;
        }

        // Complex Accessors
        public float NormalizedTime ()
        {
            TimeSpan nowSpan = new TimeSpan(raw.Hour, raw.Minute, raw.Second);
            // float nowSeconds = (float)nowSpan.TotalSeconds;
            // return nowSeconds/daySeconds;
            return (float)nowSpan.TotalSeconds/daySeconds;
        }

        public float NormalizedDate ()
        {
            int numberOfDaysThisYear = System.DateTime.IsLeapYear(raw.Year)?366:365;
            return (float)raw.DayOfYear/numberOfDaysThisYear;
        }

        // Simple Assignments
        public void AddSeconds(float seconds)
        {
            raw = raw.AddSeconds(seconds);
        }

        public void AddTimeSpan(System.TimeSpan timeSpan)
        {
            raw = raw.Add(timeSpan);
        }
    }
}
