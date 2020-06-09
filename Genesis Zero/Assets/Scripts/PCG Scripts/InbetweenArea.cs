using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class InbetweenArea : MonoBehaviour
{
    public Color[] AcceptableColors;
    //public bool RandomizeBetweenColorValues = false;
    public GameObject[] billboards;
    [TooltipAttribute("Billboard location will be randomized between the spawnvector + this offset")]
    public Vector3 offset;
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(this.transform.position, offset);
    }
}
