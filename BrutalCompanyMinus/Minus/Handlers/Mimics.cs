using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Animations;
using UnityEngine;
using static BrutalCompanyMinus.Minus.MEvent;

namespace BrutalCompanyMinus.Minus.Handlers
{
    internal class Mimics
    {
        public static Scale[] spawnRateScales = new Scale[6];

        public static int[] originalSpawnRateValues = new int[6] { 100, 0, 0, 0, 0, 0 };

        public static void MoreMimics()
        {
            if (!Compatibility.mimicsPresent) return;
            for(int i = 0; i < 6; i++)
            {
                Compatibility.mimicNetworkSpawnChances[i].Value = Clamp(spawnRateScales[i].Compute(EventType.Bad));
            }
        }

        public static void NoMimics()
        {
            if (!Compatibility.mimicsPresent) return;
            Compatibility.mimicNetworkSpawnChances[0].Value = 100;
            for (int i = 1; i < 6; i++)
            {
                Compatibility.mimicNetworkSpawnChances[i].Value = 0;
            }
        }

        public static void Reset()
        {
            if (!Compatibility.mimicsPresent) return;
            for(int i = 0; i < 6; i++)
            {
                Compatibility.mimicNetworkSpawnChances[i].Value = originalSpawnRateValues[i];
            }
        }

        private static int Clamp(int value) => Mathf.Clamp(value, 0, 100);

    }
}
