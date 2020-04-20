using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DitzelGames.FastIK;

public class SnakeBossBase : MonoBehaviour
{
    public Vector3 maxoffset;
    public GameObject Boss;
    private Vector3 origin;
    // Start is called before the first frame update
    void Start()
    {
        origin = gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Boss.GetComponent<BossAI>().animating)
        {
            transform.position = Vector3.Lerp(transform.position, origin + new Vector3((maxoffset.x * ((Boss.transform.position.x - origin.x) / 10)), 0, 0), Time.deltaTime);
        }
    }

    public void DisableIK()
    {
        FastIKFabric[] iks = gameObject.GetComponentsInChildren<FastIKFabric>();
        foreach (FastIKFabric ik in iks)
        {
            ik.enabled = false;
        }           
    }

    public void EnableIK()
    {
        FastIKFabric[] iks = gameObject.GetComponentsInChildren<FastIKFabric>();
        foreach (FastIKFabric ik in iks)
        {
            ik.enabled = true;
        }
    }
}
