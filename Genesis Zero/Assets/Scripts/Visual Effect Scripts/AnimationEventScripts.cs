using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventScripts : MonoBehaviour
{
  
    public void EnableGameObject(GameObject go)
    {
        go.SetActive(true);
    }
    public void DisableGameObject(GameObject go)
    {
        go.SetActive(false);
    }
}
