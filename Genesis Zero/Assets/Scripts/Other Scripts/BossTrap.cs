using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrap : MonoBehaviour
{
	public GameObject Indicator;
	public Vector2 size = new Vector2(5, 10);
	public Vector3 direction = new Vector3(.01f, -1, 0);
	public float interval = 3;
	public float delay = 1.1f;
	public float offset = 0;

	public GameObject Projectile;
	float currtime = 0;
	// Start is called before the first frame update
	void Start()
	{
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (currtime < interval && offset <= 0)
		{
			currtime += Time.fixedDeltaTime;
			if (currtime >= interval)
			{
				SpawnIndicator(transform.position, size, direction, new Color(1, 0, 0, .1f), Vector2.zero, false, true, delay);
				Invoke("FireProjectile", delay);
				currtime = 0;
			}
		}
		else
		{
			offset -= Time.fixedDeltaTime;
		}
	}

	public void FireProjectile()
	{
		Vector2 angle = (Vector2)transform.rotation.eulerAngles;
		Quaternion rot = Quaternion.Euler(angle.x, angle.y, 0);
		GameObject hitbox = Instantiate(Projectile, transform.position, rot);
		hitbox.GetComponent<Hitbox>().SetLifeTime((size.x - hitbox.gameObject.GetComponent<Collider>().bounds.size.magnitude/2) / hitbox.GetComponent<Projectile>().speed);
		//hitbox.gameObject.GetComponent<Projectile>().
		//AudioManager.instance.PlaySound("SFX_FireExplosion", false, 0);
	}


	public void SpawnIndicator(Vector2 position, Vector2 size, Vector2 dir, Color color, Vector2 offset, bool centered, bool square, float time)
	{
		Vector2 angle = dir;
		Quaternion rot = Quaternion.Euler(angle.x, angle.y, 0);
		GameObject instance = (GameObject)Instantiate(Indicator, position, rot);
		instance.GetComponent<BossIndicator>().SetIndicator(position, size, dir, color, centered, time);
		instance.GetComponent<BossIndicator>().SetOffset((Vector2)transform.position - instance.GetComponent<BossIndicator>().GetOrigin());
		instance.GetComponent<BossIndicator>().SetCentered(centered);
		//instance.GetComponent<BossIndicator>().HideFirstFrame = true;
		if (square)
		{
			instance.GetComponent<BossIndicator>().SetSquare();
		}
		else
		{
			instance.GetComponent<BossIndicator>().SetCircle();
		}
		Destroy(instance, time);
		//indicators.Add(instance);
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;
		Vector2 center = new Vector2(direction.normalized.x * size.x, direction.normalized.y * size.x);
		Vector2 normal = Vector2.Perpendicular(center).normalized * size.y / 2;
		Gizmos.DrawLine(transform.position, (Vector2)transform.position + (Vector2)center);
		Vector2 center2 = new Vector2(direction.normalized.y * size.y, direction.normalized.x * size.y);

		Gizmos.DrawLine((Vector2)transform.position - normal, (Vector2)transform.position + normal);
		Gizmos.DrawLine((Vector2)transform.position - normal + center, (Vector2)transform.position + normal + center);
		//End lines
		//Gizmos.DrawLine((Vector2)transform.position - center2 / 2, (Vector2)transform.position + center2 / 2);
		//Gizmos.DrawLine((Vector2)transform.position - center2 / 2 + center, (Vector2)transform.position + center2 / 2 + center);

		//Side lines
		//Gizmos.DrawLine((Vector2)transform.position - center2 / 2 , (Vector2)transform.position - center2 / 2 + center);
		//Gizmos.DrawLine((Vector2)transform.position + center2 / 2 , (Vector2)transform.position + center2 / 2 + center);

		Gizmos.DrawLine((Vector2)transform.position - normal, (Vector2)transform.position - normal + center);
		Gizmos.DrawLine((Vector2)transform.position + normal, (Vector2)transform.position + normal + center);
	}
}
