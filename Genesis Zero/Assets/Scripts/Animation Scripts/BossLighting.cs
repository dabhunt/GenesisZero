using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLighting : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject[] LightingAnimations;
    void Start()
    {
        
    }
    public void AnimateAll(bool on, float speed)
    {
        foreach (GameObject obj in LightingAnimations)
        {
            Animation ani = obj.GetComponent<Animation>();
            if (ani != null) //if default clip matches passed string
            {
                if (on)
                {
                    ani[ani.clip.name].speed = speed;
                    ani[ani.clip.name].wrapMode = WrapMode.PingPong;
                    ani.Play();
                }
                else 
                {
                    ani.Stop();
                }
            }
        }
    }
    public Animation AnimateSingle(string name, bool on, float speed)
    {
        foreach (GameObject obj in LightingAnimations)
        {
            Animation ani = obj.GetComponent<Animation>();
            if (ani != null && ani.clip.name == name) //if default clip matches passed string
            {
                if (on)
                {
                    ani[ani.clip.name].speed = speed;
                    ani[ani.clip.name].wrapMode = WrapMode.PingPong;
                    ani.Play();
                    return ani;
                }
                else
                {
                    ani.Stop();
                    return ani;
                }
            }
        }
        return null;
    }
}
