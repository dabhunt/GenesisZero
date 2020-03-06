using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject NewColor(GameObject vfx, Color color)
    {
        RecursiveChildColor(vfx.transform, color);
        return vfx;
    }
    public void RecursiveChildColor(Transform t, Color color)
    {
        foreach (Transform child in t)
        {
            //if the child has a particle system or a trail renderer, set the startColor values to the desired color
            if (child.gameObject.GetComponent<ParticleSystem>() != null)
            {
                var main = child.gameObject.GetComponent<ParticleSystem>().main;
                main.startColor = color;
            }
            if (child.gameObject.GetComponent<TrailRenderer>() != null)
            {
                child.gameObject.GetComponent<TrailRenderer>().startColor = color;
            }
            if (child.childCount > 0)
            {
                RecursiveChildColor(child, color);
            }
        }
    }
}

