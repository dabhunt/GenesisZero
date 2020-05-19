using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BossAI))]
public class BossEvents : MonoBehaviour
{
	public GameObject minionA1;
	public GameObject minionA2;
	public GameObject minionB1;
	public GameObject minionB2;

	public GameObject[] DestroyedOnWild;

	public Transform Spawnpoint1;
	public Transform Spawnpoint2;

	public void SpawnFirstGoons()
	{
		GameObject goon1 = (GameObject)Instantiate(minionA1, Spawnpoint1.position, Quaternion.identity);
		goon1.GetComponent<AIController>().AlertAndFollow(GetComponent<BossAI>().Target);
		GameObject goon2 = (GameObject)Instantiate(minionA2, Spawnpoint2.position, Quaternion.identity);
		goon2.GetComponent<AIController>().AlertAndFollow(GetComponent<BossAI>().Target);
	}

	public void SpawnSecondGoons()
	{
		GameObject goon1 = (GameObject)Instantiate(minionB1, Spawnpoint1.position, Quaternion.identity);
		goon1.GetComponent<AIController>().AlertAndFollow(GetComponent<BossAI>().Target);
		GameObject goon2 = (GameObject)Instantiate(minionB2, Spawnpoint2.position, Quaternion.identity);
		goon2.GetComponent<AIController>().AlertAndFollow(GetComponent<BossAI>().Target);
	}

	public void DestroyGameObjectsOnWild()
	{
		foreach (GameObject g in DestroyedOnWild)
		{
			Destroy(g);
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireSphere(Spawnpoint1.position, 1);
		Gizmos.DrawWireSphere(Spawnpoint2.position, 1);
	}
}
