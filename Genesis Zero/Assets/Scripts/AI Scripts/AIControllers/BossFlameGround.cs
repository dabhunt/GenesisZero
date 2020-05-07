using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFlameGround : MonoBehaviour
{
	public GameObject FlameGroundHitbox;
	public float interval;
	private float currtime;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		currtime += Time.deltaTime;
		if (currtime >= interval)
		{
			GameObject hitbox = (GameObject)Instantiate(FlameGroundHitbox, transform.position, Quaternion.identity);
			hitbox.GetComponent<Hitbox>().LifeTime = interval;
			hitbox.GetComponent<BoxCollider>().size = GetComponent<BoxCollider>().size;
			currtime = 0;
		}
    }
}
