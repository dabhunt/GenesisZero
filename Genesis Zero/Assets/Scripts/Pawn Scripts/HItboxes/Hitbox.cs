using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Hitbox is a class that handles the damage detection for hurboxes.
 * Hitbox requires that the opposing target must be part of the side this hitbox damages.
 * For example, the player should be in the Allies layer and this hitbox should have HurtsAllies to hit the player.
 * The player must also contain a hurtbox with colliders in it's children
 */
public class Hitbox : MonoBehaviour
{
    private enum State { Active, Colliding, Deactive };
    State state;

    public float damage;

    public bool HurtsAllies;
    public bool HurtsEnemies;

    public int maxhits = 1;

    public Collider collider;
    private List<GameObject> hittargets;

    private void Awake()
    {

    }

    // Start is called before the first frame update
    void Start()
    {
        //colliders.Clear();
        state = State.Active;
        if (collider == null)
        {
            if (GetComponent<Collider>())
            {
                collider = GetComponent<Collider>();
            }
        }
        //AddCollliders(transform, colliders);
        hittargets = new List<GameObject>();
    }

    public void AddCollliders(Transform currentparent, List<Collider> colliders)
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        foreach (Transform t in children)
        {
            Collider[] cols = t.GetComponents<Collider>();
            for (int i = 0; i < cols.Length; i++) // Add Colliders to the hitbox
            {
                colliders.Add(cols[i]);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (state == State.Deactive)
        {
            Destroy(this.gameObject);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (state == State.Active)
        {
            state = State.Colliding;
        }

        if (state == State.Colliding)
        {
            if (other != collider)
            {
                Debug.Log((other != collider) + " " + maxhits + " " + (other.GetComponentInParent<Hurtbox>() || other.GetComponent<Hurtbox>()) + " " + CanDamage(other) + " " + other.GetComponentInParent<Pawn>());
            }

            if (other != collider && maxhits > 0 && (other.GetComponentInParent<Hurtbox>() || other.GetComponent<Hurtbox>()) && CanDamage(other) && other.GetComponentInParent<Pawn>() && !hittargets.Contains(other.transform.root.gameObject))
            {
                float finaldamage = damage;
                Pawn p = other.GetComponentInParent<Pawn>();

                BodyPart bp = other.GetComponent<BodyPart>();
                if (bp && bp.SpecialPart)
                {
                    Debug.Log("SPecial hit");
                    finaldamage *= bp.damagemultipler;
                }

                Debug.Log("Hit");
                p.TakeDamage(damage);

                hittargets.Add(other.transform.root.gameObject);

                --maxhits;
                if (maxhits <= 0)
                {
                    state = State.Deactive;
                }
            }
            
        }
    }

    public void CheckCollisions(Collider collider)
    {
        Collider[] cols = null;
        if (collider.GetComponent<SphereCollider>() != null)
        {
            cols = Physics.OverlapSphere(collider.transform.position, collider.GetComponent<SphereCollider>().radius);
        }
        if (collider.GetComponent<BoxCollider>() != null)
        {
            cols = Physics.OverlapBox(collider.transform.position, collider.GetComponent<BoxCollider>().size);
        }
        if (cols != null)
        {
            if (cols.Length > 1)
            {
                Debug.Log("Collide" + cols.Length);
            }

            foreach (Collider col in cols)
            {
                if (col != collider)
                {
                    Debug.Log((col != collider) + " " + maxhits + " " + (col.GetComponentInParent<Hurtbox>() || col.GetComponent<Hurtbox>()) + " " + CanDamage(col) + " " + col.GetComponentInParent<Pawn>());
                }
                
                if (col != collider && maxhits > 0 && (col.GetComponentInParent<Hurtbox>() || col.GetComponent<Hurtbox>()) && CanDamage(col) && col.GetComponentInParent<Pawn>())
                {
                    if(state == State.Active)
                    {
                        state = State.Colliding;
                    }

                    Pawn p = col.GetComponentInParent<Pawn>();
                    Debug.Log("Hit");
                    p.TakeDamage(damage);
                    --maxhits;
                }
                else if (maxhits <= 0)
                {
                    state = State.Deactive;
                    break;
                }
            }
        }

    }


    public bool CanDamage(Collider col)
    {
        //Debug.Log(col.gameObject.layer +" "+ LayerMask.NameToLayer("Allies"));
        if ((HurtsAllies && col.gameObject.layer == LayerMask.NameToLayer("Allies")) || (HurtsEnemies && col.gameObject.layer == LayerMask.NameToLayer("Enemies")))
        {
            //Debug.Log("CanHurt");
            return true;
        }
        return false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Collider col = collider;
        if (col.GetComponent<SphereCollider>() != null)
        {
            //Debug.Log("C");
            Gizmos.DrawWireSphere(col.transform.position - col.GetComponent<SphereCollider>().center, col.GetComponent<SphereCollider>().radius);
        }

        if (col.GetComponent<BoxCollider>() != null)
        {
            //Debug.Log(col.GetComponent<BoxCollider>().center);
            Gizmos.DrawWireCube(col.transform.position + col.GetComponent<BoxCollider>().center, col.GetComponent<BoxCollider>().size);
        }

    }

}
