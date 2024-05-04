using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Animations;
using static BrutalCompanyMinus.Minus.MEvent;

namespace BrutalCompanyMinus.Minus
{
    public class LevelProperties
    {
        public int levelID;

        public Scale minScrapAmount, maxScrapAmount;

        public Scale minScrapValue, maxScrapValue;

        public LevelProperties(int levelID, Scale minScrapAmount, Scale maxScrapAmount, Scale minScrapValue, Scale maxScrapValue)
        {
            this.levelID = levelID;
            this.minScrapAmount = minScrapAmount;
            this.maxScrapAmount = maxScrapAmount;
            this.minScrapValue = minScrapValue;
            this.maxScrapValue = maxScrapValue;
        }

        public float GetScrapAmountMultiplier() => UnityEngine.Random.Range(minScrapAmount.Computef(EventType.Neutral), maxScrapAmount.Computef(EventType.Neutral));

        public float GetScrapValueMultiplier() => UnityEngine.Random.Range(minScrapValue.Computef(EventType.Neutral), maxScrapValue.Computef(EventType.Neutral));
    }
}
