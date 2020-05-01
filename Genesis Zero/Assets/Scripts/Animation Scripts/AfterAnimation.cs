using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfterAnimation : MonoBehaviour
{
    // Start is called before the first frame update
    public void After()
    {
        GetComponentInParent<SafeBox>().AfterAnimation();
    }
}
