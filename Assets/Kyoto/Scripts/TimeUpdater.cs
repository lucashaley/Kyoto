using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms;
using UnityAtoms.BaseAtoms;

namespace Kyoto
{
    public class TimeUpdater : MonoBehaviour
    {
        public FloatVariable timeScale;
        public DateTimeVariable realDateTimeVariable, gameDateTimeVariable;
        // public FloatVariable gameDateVariable, gameTimeVariable;
        public FloatVariable gameDateNormalizedVariable;
        public FloatVariable gameTimeNormalizedVariable;
        public IntVariable dayOfYear;

        public LocationController locationController;
        public FloatVariable azimuthVariable, altitudeVariable;

        void Start()
        {
            // Set the engine timeScale
            // Time.timeScale = timeScale.Value;
            realDateTimeVariable.Value = gameDateTimeVariable.Value = System.DateTime.Now;

            locationController = LocationController.Instance;
        }

        // Update is called once per frame
        void Update()
        {
            TimeSpan difference = new TimeSpan((long)(TimeSpan.TicksPerSecond
                                                    * Time.deltaTime
                                                    * timeScale.Value));
            gameDateTimeVariable.Add(difference);

            gameTimeNormalizedVariable.Value = gameDateTimeVariable.NormalizedTime();
            gameDateNormalizedVariable.Value = gameDateTimeVariable.NormalizedDate();
            dayOfYear.Value = gameDateTimeVariable.DayOfYear();
            UpdateOrbit();
        }

        void ChangeDayOfYear()
        {
            dayOfYear.Value = 0;
        }

        void UpdateOrbit()
        {
            float azimuth, altitude;
            // Hessburg version
            //ensure inputs stay within required limits
            float latitude=Mathf.Clamp(locationController.Latitude, -90.0f, 90.0f);
            float longitude=Mathf.Clamp(locationController.Longitude, -180.0f, 180.0f);
            float offsetUTC=Mathf.Clamp(locationController.OffsetUTC, -12.0f, 14.0f);
            int dayOfYear=Mathf.Clamp(gameDateTimeVariable.DayOfYear(), 1, 366);
            float timeInHours=Mathf.Clamp((float)(gameDateTimeVariable.Value.Hour + ((float)gameDateTimeVariable.Value.Minute/60)),
                                                  0.0f,
                                                  24.00f);

            // calculate azimut & altitute
            float cosLatitude = Mathf.Cos(latitude*Mathf.Deg2Rad);
            float sinLatitude = Mathf.Sin(latitude*Mathf.Deg2Rad);
            float D = DeclinationOfTheSun(dayOfYear);
            float sinDeclination=Mathf.Sin(D);
            float cosDeclination=Mathf.Cos(D);
            float W = 360.0f/365.24f;
            float A = W * (dayOfYear + 10.0f);
            float B = A + (360.0f/Mathf.PI) * 0.0167f * Mathf.Sin(W * (dayOfYear-2) * Mathf.Deg2Rad);
            float C = (A - (Mathf.Atan(Mathf.Tan(B*Mathf.Deg2Rad)/Mathf.Cos(23.44f*Mathf.Deg2Rad))*Mathf.Rad2Deg)) / 180.0f;
            float EquationOfTime = 720.0f * (C - Mathf.Round(C)) / 60.0f;
            float hourAngle = (timeInHours + longitude / (360.0f / 24.0f) - offsetUTC - 12.0f + EquationOfTime) * (1.00273790935f-1.0f/365.25f)*(360.0f / 24.0f)*Mathf.Deg2Rad;

            // float azimuth = Mathf.Atan2(-cosDeclination * Mathf.Sin(hourAngle), sinDeclination * cosLatitude - cosDeclination * Mathf.Cos(hourAngle) * sinLatitude);
            azimuth = Mathf.Atan2(-cosDeclination * Mathf.Sin(hourAngle), sinDeclination * cosLatitude - cosDeclination * Mathf.Cos(hourAngle) * sinLatitude);
            if (azimuth<0) azimuth = azimuth + 6.28318530717959f;
            // float altitude = Mathf.Asin(sinDeclination * sinLatitude + cosDeclination * Mathf.Cos(hourAngle) * cosLatitude );
            altitude = Mathf.Asin(sinDeclination * sinLatitude + cosDeclination * Mathf.Cos(hourAngle) * cosLatitude );

            azimuth = azimuth / Mathf.Deg2Rad;
            altitude = altitude / Mathf.Deg2Rad;
            if(altitude<0.0f) altitude=360.0f+altitude;

            altitudeVariable.Value = altitude;
            azimuthVariable.Value = azimuth;
        }

        private float DeclinationOfTheSun(int dayOfYear)
        {
            float WD = 360.0f/365.24f;
            float AD = WD * (dayOfYear + 10.0f);
            float BD = AD + (360.0f/Mathf.PI) * 0.0167f * Mathf.Sin(WD * (dayOfYear-2) * Mathf.Deg2Rad);
            return -Mathf.Asin(Mathf.Sin(23.44f * Mathf.Deg2Rad) * Mathf.Cos(BD * Mathf.Deg2Rad) );
        }
    }
}
