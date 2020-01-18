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

    public Vector3 lastposition;

    private void Awake()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        }
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
        lastposition = collider.transform.position;
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

    private void FixedUpdate()
    {
        /**
        RaycastHit hit;
        Vector3 pos = collider.transform.position;
        Vector3 direction = pos - lastposition;
        float distance = Mathf.Abs(Vector3.Distance(pos, lastposition)) * 10;
        //Debug.Log(direction +" "+ Mathf.Abs(Vector3.Distance(pos, lastposition)) * 10);
        Debug.DrawRay(pos, direction,Color.blue, distance);
        if (Physics.Raycast(pos, direction, out hit, distance, LayerMask.NameToLayer("Allies")))
        {
            //Debug.Log("Rayed: " + hit.collider.gameObject.name);
        }
        lastposition = collider.transform.position;
    */
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Dect");
        CheckCollisions(other);
    }

    public void CheckCollisions(Collider other)
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
                    Debug.Log("Special hit");
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
