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
        if (Boss != null && !Boss.GetComponent<BossAI>().boxanimating)
        {
			Recalculate();
			//GetComponent<Animator>().applyRootMotion = true;
		}
		else
		{
			transform.position = origin;
			//GetComponent<Animator>().applyRootMotion = false;
		}
    }

	void Recalculate()
	{
		GameObject BossRoom = GameObject.FindGameObjectWithTag("BossRoom");
		transform.position = Vector3.Lerp(transform.position, origin + new Vector3((maxoffset.x * ((Boss.transform.position.x - origin.x) / 10)), (maxoffset.y * ((Boss.transform.position.y - BossRoom.transform.position.y + BossRoom.GetComponent<BoxCollider2D>().offset.y)) / 10), 0), Time.deltaTime / 2);
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
			if (ik.enabled == false)
			{
				float laststrength = ik.SnapBackStrength;
				ik.SnapBackStrength = 1;
				ik.ResolveIK();
				ik.SnapBackStrength = laststrength;
				ik.enabled = true;
			}
        }
		Recalculate();
	}
}
