﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * Statistic is a data type that contains and manipulates a value and is designed for statistic related numbers
 * It's main key feature is allowing for temporary bonuses. Both additive and multiplicative. (Requires management to work)
 * This class is designed to work with the Pawn class primarily to keep track and manage the basic stats of pawns.
 */
public class Statistic
{
    float maxamount;           // Max amount of base stat with no changes

    float currentamount;        // Value of the base stat
    float currentmaxamount;     // Current max value of the stat (includes bonuses)

    List<Bonus> bonusamounts;   // List of bonuses to the stat. The key is the added/subtracted value, and the value is the time it stays.
    float bonuses;
    float maxbonuses;
    List<Bonus> exhaustedbonuses;

    public Statistic(float amount)
    {
        bonusamounts = new List<Bonus>();
        exhaustedbonuses = new List<Bonus>();
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
        return maxamount + maxbonuses;
    }

    /**
     * Sets the max value of the statistic permanently
     */
    public void SetMaxValue(float value)
    {
        float diff = value - maxamount;
        maxamount = value;
        AddValue(diff);
    }

    /**
     * Add to the max value of the statistic permanently
     */
    public void AddMaxValue(float value)
    {
        SetMaxValue(GetBaseValue() + value);
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
        //Debug.Log(currentamount+" -> "+r.x);
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


    /**
     * Gets the ratio of the value and maxvalue
     */
     public float GetRatio()
    {
        return GetValue() / GetMaxValue();
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
        else
        {
            result.x = value;
            result.y = 0;
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

    /**
    *  If value is added to the stat and increases the max value as well. With a identifier
    */
    public void AddRepeatingBonus(float value, float maxvalue, float time, string id)
    {
        if (value == 0)
            return;
        Bonus b = new Bonus(value, maxvalue, time, id);
        if (ContainsBonus(id))
        {
            SetRepeatingBonus(b);
        }
        else
        {
            bonusamounts.Add(b);
        }
    }

    /**
     * Returns true if bonus amounts contains a bonus with the same id.
     */
    public bool ContainsBonus(string id)
    {
        foreach (Bonus b in bonusamounts)
        {
            if (b.GetIdentifier() == id) return true;
        }
        return false;
    }

    /**
     * Returns true if bonus amounts contains a bonus with the same id.
     */
    public void SetRepeatingBonus(Bonus bonus)
    {
        foreach (Bonus b in bonusamounts)
        {
            if (b.GetIdentifier() == bonus.GetIdentifier())
            {
                b.SetMaxValue(bonus.GetMaxValue());
                b.SetValue(bonus.GetValue());
                b.SetTime(bonus.GetTime());
            }
        }
    }

    /**
     * End the repeating bonus if it exists
     */
    public void EndRepeatingBonus(string id)
    {
        foreach (Bonus b in bonusamounts)
        {
            if (b.GetIdentifier() == id)
            {
                b.SetTime(0);
            }
        }
    }
    public Bonus GetBonus(string id)
    {
        foreach (Bonus b in bonusamounts)
        {
            if (b.GetIdentifier() == id) return b;
        }
        return null;
    }
    public void CheckBonuses()
    {
        float b = 0;
        float mb = 0;
        if (bonusamounts.Count > 0)
        {
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
                b += RecalculateBonuses(bonusamounts, bonus);
            }


            if (b != bonuses)
            {
                bonuses = b;
            }
            if (mb != maxbonuses)
            {
                maxbonuses = mb;
            }

            exhaustedbonuses.Clear();
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

    /**
     * Places any unused bonuses to the other bonues and then removes the bonues from the list
     */
    private float RecalculateBonuses(List<Bonus> bonuses, Bonus bonus)
    {
        float value = bonus.GetValue();
        float filledamount = 0;

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
                        filledamount += diff;
                    }
                    else
                    {
                        b.SetValue(bvalue + value);
                        value = 0;
                        bonuses.Remove(bonus);
                        return filledamount;
                    }
                }

                if (value <= 0)
                {
                    bonuses.Remove(bonus);
                    return filledamount;
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
                currentamount = GetBaseValue();
                filledamount += sdiff;
            }
            else
            {
                currentamount += value;
                filledamount += value;
            }
        }

        if (bonus != null)
        {
            bonuses.Remove(bonus);
        }

        return filledamount;
    }
    /**
     * Object type that contains data on a timed bonus. If the time is reduced to zero, it should be removed
     */
    public class Bonus
    {
        private float value;
        private float maxvalue;
        private float time;
        private string identifier = "";

        public Bonus(float value, float maxvalue, float time)
        {
            this.value = value;
            Mathf.Clamp(value, 0, maxvalue);
            this.maxvalue = maxvalue;
            this.time = time;
        }

        public Bonus(float value, float maxvalue, float time, string id)
        {
            this.value = value;
            Mathf.Clamp(value, 0, maxvalue);
            this.maxvalue = maxvalue;
            this.time = time;
            identifier = id;
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

        public string GetIdentifier()
        {
            return identifier;
        }

        public void SetIdentifier(string id)
        {
            identifier = id;
        }
    }

}
