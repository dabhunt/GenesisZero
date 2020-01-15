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

    List<Bonus> multipliers;   // List of multiplier bonuses to the maxvalue of the stat.
    float multiplier;

    public Statistic(float amount)
    {
        this.maxamount = amount;
        currentamount = amount;
        currentmaxamount = amount;
        bonuses = 0;
    }

    /**
     * Updates the bonus and checks if they are still working, removes bonuses that have timedout
     */
    public void Update()
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

    public float GetValue()
    {
        return currentamount + bonuses;
    }

    public void SetValue(float value)
    {
        Mathf.Clamp(value, 0, GetMaxValue());
        AddValue(value - this.currentamount);

    }

    public float GetMaxValue()
    {
        return (maxamount * multiplier) + bonuses;
    }

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

    public Vector2 FillDifference(float value, float max, float fill)
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
        else if(fill < 0)
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

    public void CheckBonuses()
    {
        if (bonusamounts.Count > 0)
        {
            float b = 0;
            foreach (Bonus bonus in bonusamounts)
            {
                if (bonus.CheckBonus())
                {
                    b += bonus.GetValue();
                }
                else
                {
                    RecalculateBonuses(bonusamounts, bonus);
                }
            }
            if (b != bonuses)
            {
                bonuses = b;
            }
        }
    }

    public void CheckMultipliers()
    {
        if (multipliers.Count > 0)
        {
            float multi = 1;
            foreach (Bonus bonus in multipliers)
            {
                if (bonus.CheckBonus())
                {
                    multi *= bonus.GetValue();
                }
                else
                {
                    RecalculateBonuses(multipliers, bonus);
                }
            }
            if (multi != multiplier)
            {
                multiplier = multi;
            }
        }
    }

    /**
     * Places any unused bonuses to the other bonues and then removes the bonues from the list
     */
    public void RecalculateBonuses(List<Bonus> bonuses, Bonus bonus)
    {
        float value = bonus.GetValue();
        bonuses.Remove(bonus);

        foreach (Bonus b in bonuses)
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
                    return;
                }
            }

            if (value <= 0)
            {
                return;
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
