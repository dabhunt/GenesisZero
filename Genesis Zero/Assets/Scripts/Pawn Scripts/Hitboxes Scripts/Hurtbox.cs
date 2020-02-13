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


    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Colliding");
        if (GetComponent<Pawn>() && GetComponent<Pawn>().GetKnockBackForce() > 0 && collision.collider.isTrigger == false)
        {
            Vector2 vel = GetComponent<Rigidbody>().velocity;
            Debug.Log(gameObject.name + ": " + vel.magnitude);

            float damage = (int)(vel.magnitude * 5) / 2;
            GetComponent<Pawn>().TakeDamage(damage, null);
            GameObject dn = VFXManager.instance.PlayEffect("DamageNumber", transform.position);
            dn.GetComponent<DamageNumber>().SetNumber(damage);

            Vector3 newVec = GetComponent<Rigidbody>().velocity * 1 / 2;
            GetComponent<Rigidbody>().velocity = - newVec;

            if (collision.gameObject.GetComponent<Pawn>())
            {
                collision.gameObject.GetComponent<Pawn>().TakeDamage(vel.magnitude * 5, null);
                GameObject dn2 = VFXManager.instance.PlayEffect("DamageNumber", collision.transform.position);
                dn2.GetComponent<DamageNumber>().SetNumber(damage);
                collision.gameObject.GetComponent<Rigidbody>().velocity += newVec/2;
            }

            GetComponent<Pawn>().SetKnockBackForce(0);
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
