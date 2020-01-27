using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * Status is a object class that uses time/value to check if the status is true or not.
 * If time is 0 then the status is false, otherwise it is true, unless the threshold is changed
 */
public class Status
{
    float time;         // time variable, always decrements
    float threshhold;   // used to denote when the status us true or not.
    float factor;
    bool isActive;

    public Status(float time)
    {
        factor = 1;
        this.time = time;
        if (time > 0)
        {
            isActive = true;
        }
    }

    public Status(float time, float threshhold)
    {
        factor = 1;
        this.time = time;
        if (time > threshhold)
        {
            isActive = true;
        }
    }

    public Status(float time, float threshhold, float factor)
    {
        this.factor = factor;
        this.time = time;
        if (time > threshhold)
        {
            isActive = true;
        }
    }

    /**
     * Updates the time of the status, changes status state to false if time is zero
     */
    public void UpdateStatus()
    {
        if (time > threshhold)
        {
            time -= Time.deltaTime;
            isActive = true;
        }
        else if (time <= threshhold)
        {
            time -= Time.deltaTime;
            isActive = false;
        }
        else
        {
            time = 0;
        }
    }

    /**
     * Returns whether or not the status is true based on it's conditions
     */
    public bool IsActive()
    {
        return isActive;
    }
    // Returns whether or not the status is false based on it's conditions
    public bool IsNotActive() { return !IsActive(); }
    // Returns whether or not the status is true based on it's conditions
    public bool IsTrue() { return IsActive(); }
    // Returns whether or not the status is false based on it's conditions
    public bool IsFalse() { return !IsActive(); }

    public void SetTime(float time)
    {
        Mathf.Clamp(time, 0, Mathf.Infinity);
        this.time = time;
    }

    public void SetValue(float value)
    {
        Mathf.Clamp(time, 0, Mathf.Infinity);
        this.time = value;
    }

    public void AddTime(float time)
    {
        this.time += time;
    }

    public void AddValue(float value)
    {
        this.time += value;
    }

    public float GetTime()
    {
        return this.time;
    }

    public float GetValue()
    {
        return this.time;
    }

    public float GetThreshhold()
    {
        return threshhold;
    }

    public void SetThreshhold(float value)
    {
        threshhold = value;
    }

    public float GetFactor()
    {
        return factor;
    }

    public void SetFactor(float factor)
    {
        this.factor = factor;
    }

    /**
     * Multiplies the current time by the factor
     */
    public void MultiplyTime(float factor)
    {
        time *= this.factor;
    }

    /**
     * Sets time to 0
     */
    public void RemoveTime()
    {
        time = 0;
    }
}
