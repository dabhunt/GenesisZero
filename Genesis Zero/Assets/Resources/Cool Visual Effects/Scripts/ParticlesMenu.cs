using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesMenu : MonoBehaviour
{
    public static int currentIndex;
    public GameObject[] particles;

    void Start()
    {
        SwitchItem(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            SwitchItem(currentIndex + 1);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            SwitchItem(currentIndex - 1);

        transform.parent.Rotate(Vector3.up * 10 * Time.deltaTime);
    }
    
    public void SwitchItem(int item)
    {
        if(item >= 0 && item < particles.Length)
        {
            particles[currentIndex].active = false;
            currentIndex = item;
            particles[currentIndex].active = true;
        }
    }
}
