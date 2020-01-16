using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistic
{
    float maxamount;           // Max amount of base stat with no changes

    float currentamount;        // Value of the base stat
    float currentmaxamount;     // Current max value of the stat (includes bonuses)

    List<Bonus> bonusamounts;   // List of bonuses to the stat. The key is the added/subtracted value, and the value is the time it stays.
    float bonuses;
    float maxbonuses;
    List<Bonus> exhaustedbonuses;

    List<Bonus> multipliers;   // List of multiplier bonuses to the maxvalue of the stat.
    float multiplier;
    List<Bonus> exhaustedmultipliers;

    public Statistic(float amount)
    {
        bonusamounts = new List<Bonus>();
        multipliers = new List<Bonus>();
        exhaustedbonuses = new List<Bonus>();
        exhaustedmultipliers = new List<Bonus>();
        this.maxamount = amount;
        currentamount = amount;
        currentmaxamount = amount;
        bonuses = 0;
    }

    /**
     * Updates the bonus and checks if they are still working, removes bonuses that have timedout
     * Do not call this function. Should be only used in the pawn class
     */
    public void UpdateStatistics()
    {
        CheckBonuses();
        CheckMultipliers();
    }

    /**
     * Returns the value of the statistic before bonuses/multipliers
     */
    public float GetBaseValue()
    {
        return maxamount;
    }

    /**
     * Returns the value of the statistic with bonuses/multipliers
     */
    public float GetValue()
    {
        return currentamount + bonuses;
    }

    /**
     * Sets the value of the statistic
     */
    public void SetValue(float value)
    {
        Mathf.Clamp(value, 0, GetMaxValue());
        AddValue(value - this.currentamount);

    }

    /**
     * Returns the max value of the statistic with bonuses/multipliers
     */
    public float GetMaxValue()
    {
        return (maxamount * multiplier) + maxbonuses;
    }

    /**
     * Sets the max value of the statistic permanently
     */
    public void SetMaxValue(float value)
    {
        float diff = value - maxamount;
        maxamount = value;
        AddValue(diff);
        if (maxamount < currentamount)
        {
            currentamount = maxamount;
        }
    }

    /**
     * Adds to the value of the Statistic, can also be negative
     */
    public void AddValue(float amount)
    {
        if (bonusamounts.Count > 0)
        {
            foreach (Bonus bonus in bonusamounts)
            {
                Vector2 result = FillDifference(bonus.GetValue(), bonus.GetMaxValue(), amount);
                bonus.SetValue(result.x);
                amount = result.y;
            }

        }
        Vector2 r = FillDifference(currentamount, GetBaseValue(), amount);
        currentamount = r.x;
    }

    /**
     * If factor is 1 then nothing is changed, 1.2 is a 20% increase
     */
    public void MultiplyValue(float factor)
    {
        float value = GetValue() * factor;
        float diff = GetValue() - value;
        AddValue(diff);
    }

    /**
     * If factor is 1 then nothing is changed, 1.2 is a 20% increase
     */
    public void MultiplyMaxValue(float factor)
    {
        SetMaxValue(maxamount * factor);
    }

    //---------------------------------------- BONUSES ---------------------------------------------------------//

    private Vector2 FillDifference(float value, float max, float fill)
    {
        Vector2 result = new Vector2();
        if (fill > 0)
        {
            float diff = max - value;
            if (fill > diff)
            {
                result.x = max;
                result.y = fill - diff;
            }
            else
            {
                result.x = value + fill;
                result.y = 0;
            }
        }
        else if (fill < 0)
        {
            if (Mathf.Abs(fill) > value)
            {
                result.x = 0;
                result.y = fill + value;
            }
            else
            {
                result.x = value + fill;
                result.y = 0;
            }
        }


        return result;
    }

    /**
     *  If value is 0, then no useable value is added to the stat
     */
    public void AddBonus(float value, float maxvalue, float time)
    {
        Bonus b = new Bonus(value, maxvalue, time);
        bonusamounts.Add(b);
    }
    /**
    *  If value is added to the stat and increases the max value as well
    */
    public void AddBonus(float value, float time)
    {
        Bonus b = new Bonus(value, value, time);
        bonusamounts.Add(b);
    }

    public void AddMultiplier(float value, float time)
    {
        Bonus b = new Bonus(value, value, time);
        multipliers.Add(b);
    }

    private void CheckBonuses()
    {
        float b = 0;
        float mb = 0;
        if (bonusamounts.Count > 0)
        {
            exhaustedbonuses.Clear();
            foreach (Bonus bonus in bonusamounts)
            {
                if (bonus.CheckBonus())
                {
                    b += bonus.GetValue();
                    mb += bonus.GetMaxValue();
                }
                else
                {
                    exhaustedbonuses.Add(bonus);
                }
            }

            foreach(Bonus bonus in exhaustedbonuses)
            {
                RecalculateBonuses(bonusamounts, bonus);
            }


            if (b != bonuses)
            {
                bonuses = b;
            }
            if (mb != maxbonuses)
            {
                maxbonuses = mb;
            }

            return;
        }
        if (b != bonuses)
        {
            bonuses = b;
        }
        if (mb != maxbonuses)
        {
            maxbonuses = mb;
        }
    }

    private void CheckMultipliers()
    {
        float multi = 1;
        if (multipliers.Count > 0)
        {
            exhaustedmultipliers.Clear();
            foreach (Bonus bonus in multipliers)
            {
                if (bonus.CheckBonus())
                {
                    multi *= bonus.GetValue();
                }
                else
                {
                    exhaustedmultipliers.Add(bonus);
                }
            }
            foreach (Bonus bonus in exhaustedmultipliers)
            {
                RecalculateBonuses(exhaustedmultipliers, bonus);
            }

            if (multi != multiplier)
            {
                multiplier = multi;
                return;
            }
        }
        if (multi != multiplier)
        {
            multiplier = multi;
        }
    }

    /**
     * Places any unused bonuses to the other bonues and then removes the bonues from the list
     */
    private void RecalculateBonuses(List<Bonus> bonuses, Bonus bonus)
    {
        float value = bonus.GetValue();

        foreach (Bonus b in bonuses)
        {
            if (b != bonus)
            {
                float bvalue = b.GetValue();
                float bmaxvalue = b.GetMaxValue();

                float diff = bmaxvalue - bvalue;
                if (diff > 0)
                {
                    if (value >= diff)
                    {
                        b.SetValue(bmaxvalue);
                        value -= diff;
                    }
                    else
                    {
                        b.SetValue(bvalue + value);
                        value = 0;
                        bonuses.Remove(bonus);
                        return;
                    }
                }

                if (value <= 0)
                {
                    bonuses.Remove(bonus);
                    return;
                }
            }         
        }


        //If there is no more bonus, check if the stats' base value is low
        float svalue = currentamount;
        float smaxvalue = GetBaseValue();
        float sdiff = smaxvalue - svalue;
        if (value > 0 && sdiff > 0)
        {
            if (value >= sdiff)
            {
                SetValue(smaxvalue);
            }
            else
            {
                SetValue(svalue + value);
            }
        }
        if (bonus != null)
        {
            bonuses.Remove(bonus);
        }
    }
    /**
     * Object type that contains data on a timed bonus. If the time is reduced to zero, it should be removed
     */
    public class Bonus
    {
        private float value;
        private float maxvalue;
        private float time;

        public Bonus(float value, float maxvalue, float time)
        {
            this.value = value;
            Mathf.Clamp(value, 0, maxvalue);
            this.maxvalue = maxvalue;
            this.time = time;
        }

        public bool CheckBonus()
        {
            SetTime(GetTime() - Time.deltaTime);
            return GetTime() > 0;
        }

        public float GetValue()
        {
            return value;
        }

        public void SetValue(float value)
        {
            Mathf.Clamp(value, 0, maxvalue);
            this.value = value;
        }

        public float GetMaxValue()
        {
            return maxvalue;
        }

        public void SetMaxValue(float value)
        {
            this.maxvalue = value;
        }

        public float GetTime()
        {
            return time;
        }

        public void SetTime(float time)
        {
            Mathf.Clamp(time, 0, Mathf.Infinity);
            this.time = time;
        }
    }

}
