using System;
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kyoto
{
    [CreateAssetMenu(fileName = "New DateTimeVariable", menuName = "Kyoto/DateTimeVariable")]
    public class DateTimeVariable : ScriptableObject
    {
        public long DateTimeLong;

        public System.DateTime Value
        {
            get => System.DateTime.FromBinary(DateTimeLong);

            set => DateTimeLong = value.ToBinary();
        }

        public void Add(TimeSpan span)
        {
            DateTimeLong = System.DateTime.FromBinary(DateTimeLong).Add(span).ToBinary();
        }

        public float NormalizedTime ()
        {
            TimeSpan nowSpan = new TimeSpan(System.DateTime.FromBinary(DateTimeLong).Ticks);
            return (float)nowSpan.TotalSeconds/86400;
        }

        public float NormalizedDate ()
        {
            System.DateTime raw = System.DateTime.FromBinary(DateTimeLong);
            int numberOfDaysThisYear = System.DateTime.IsLeapYear(raw.Year)?366:365;
            return (float)raw.DayOfYear/numberOfDaysThisYear;
        }

        public int DayOfYear()
        {
            return System.DateTime.FromBinary(DateTimeLong).DayOfYear;
        }
    }
}
