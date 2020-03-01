using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Kenny Doan
 * Hurtbox is a script that manages the BodyParts in the child objects below. 
 * It is primarily used by the Hitbox class in order to have multi hurtbox detection
 */
[ExecuteInEditMode]
public class Hurtbox : MonoBehaviour
{

    public List<Collider> colliders;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        colliders.Clear();
        AddCollliders(transform, colliders);
    }
    public void AddCollliders(Transform currentparent, List<Collider> colliders)
    {

        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform t in children)
        {
            if (t.GetComponent<BodyPart>() != null)
            {
                Collider[] cols = t.GetComponents<Collider>();
                for (int i = 0; i < cols.Length; i++) // Add Colliders to the hitbox
                {
                    colliders.Add(cols[i]);
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        try
        {
            foreach (Collider col in colliders)
            {
                if (col == null)
                {
                    colliders.Remove(col);
                }

            }

        }
        catch { }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("SpecialT");
    }


    private void OnCollisionStay(Collision collision)
    {
        Vector2 vel = GetComponent<Rigidbody>().velocity;
        Vector3 avg = Vector3.zero;
        int num = collision.contactCount > 0 ? 0 : 1;
        for (int i = 0; i < collision.contactCount; i++)
        {
            avg += collision.GetContact(i).point;
            ++num;
        }
        avg = new Vector3(avg.x / num, avg.y / num, avg.z / num);
        //collision.GetContact(0).point;
        Vector2 hitdir = avg - transform.position;
        float angle = Vector2.Angle(hitdir, vel);

        //if(GetComponent<Player>()) Debug.Log(angle);
        //Debug.Log("Colliding");
        if (GetComponent<Pawn>() && GetComponent<Pawn>().GetKnockBackForce() > 3 && ((vel.magnitude > 4 && collision.collider.isTrigger == false) || GetComponent<Pawn>().IsForcedKnockBack()))
        {
            if (GetComponent<Pawn>().IsForcedKnockBack() && Mathf.Abs(angle) < 180)
            {
                GetComponent<Pawn>().KnockBackForced(-GetComponent<Pawn>().GetKnockBackVector(), GetComponent<Pawn>().GetKnockBackForce() / 2);
                return;
            }
            else if (Mathf.Abs(angle) < 75)  // if the object hits the other object directly, then they take the damage
            {

                float damage = (int)(vel.magnitude * 4) / 2;
                GetComponent<Pawn>().TakeDamage(damage, null);
                GameObject dn = VFXManager.instance.PlayEffect("DamageNumber", new Vector3(transform.position.x, transform.position.y + 1, transform.position.z - .5f));
                dn.GetComponent<DamageNumber>().SetNumber(damage);
                dn.GetComponent<DamageNumber>().SetColor(new Color(.5f, .5f, 1f));

                Vector3 newVec = GetComponent<Rigidbody>().velocity * 1 / 2;
                GetComponent<Rigidbody>().velocity = -newVec;

                if (collision.gameObject.GetComponent<Pawn>())
                {
                    collision.gameObject.GetComponent<Pawn>().TakeDamage(vel.magnitude * 5, null);
                    GameObject dn2 = VFXManager.instance.PlayEffect("DamageNumber", new Vector3(collision.transform.position.x, collision.transform.position.y + 1, collision.transform.position.z - .5f));
                    dn2.GetComponent<DamageNumber>().SetNumber(damage);
                    dn2.GetComponent<DamageNumber>().SetColor(new Color(.5f, .5f, 1f));
                    collision.gameObject.GetComponent<Rigidbody>().velocity += newVec / 2;
                }

                GetComponent<Pawn>().SetKnockBackForce(0);
            }

            /**
            Vector2 kdir = GetComponent<Pawn>().GetKnockBackVector();
            float kangle = Mathf.Atan2(kdir.y, kdir.x);
            Vector2 cdir = collision.transform.position - transform.position;
            float cangle = Mathf.Atan2(cdir.y, cdir.x);
            GetComponent<Pawn>().KnockBack(GetComponent<Pawn>().GetKnockBackVector() * -1, 0);
                */
        }
    }

    [ExecuteInEditMode]
    private void OnDrawGizmosSelected()
    {
        if (colliders != null)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                Collider col = colliders[i];
                BodyPart bp = col.GetComponent<BodyPart>();
                if (bp != null && bp.SpecialPart == true)
                {
                    if (bp.damagemultipler > 1)
                    {
                        Gizmos.color = Color.yellow;
                    }
                    else if (bp.damagemultipler < 1)
                    {
                        Gizmos.color = Color.grey;
                    }
                    else
                    {
                        Gizmos.color = Color.red;
                    }
                }
                else
                {
                    Gizmos.color = Color.red;
                }

                if (col.GetComponent<SphereCollider>() != null)
                {
                    //Debug.Log(col.transform.localEulerAngles);
                    Gizmos.DrawWireSphere(col.transform.position + col.GetComponent<SphereCollider>().center, col.GetComponent<SphereCollider>().radius * transform.localScale.x);
                }
                if (col.GetComponent<BoxCollider>() != null)
                {
                    Gizmos.DrawWireCube(col.transform.position + col.GetComponent<BoxCollider>().center, col.GetComponent<BoxCollider>().size * transform.localScale.x);
                }
            }
        }

    }
}
