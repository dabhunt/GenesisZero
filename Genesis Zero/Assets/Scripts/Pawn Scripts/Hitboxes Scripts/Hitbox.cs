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

    public float Damage;        // How much damage the hitbox can deal

    public bool HurtsAllies;    // Determines if this hitbox can harm the allies layer
    public bool HurtsEnemies;   // Determines if this hotbox can harm the enemies layer
    [Space]
    public bool Intangible;     // If True, then the hitbox can pass through walls
    public int MaxHits = 1;     // Number of times the hitbox can hit something

    public Collider Collider;
    public Pawn Source;         // Source is a reference to the pawn that spawned this hitbox. Optional, used if things like critchance is calculated
    private List<GameObject> hittargets;

    private Vector3 lastposition;

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
        if (GetComponent<Collider>() == null)
        {
            if (GetComponent<Collider>())
            {
                Collider = GetComponent<Collider>();
            }
        }
        //AddCollliders(transform, colliders);
        lastposition = GetComponent<Collider>().transform.position;
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
            if (other != GetComponent<Collider>())
            {
                //Debug.Log((other != collider) + " " + maxhits + " " + (other.GetComponentInParent<Hurtbox>() || other.GetComponent<Hurtbox>()) + " " + CanDamage(other) + " " + other.GetComponentInParent<Pawn>());
            }

            bool siblingcolliders = hittargets.Contains(other.transform.root.gameObject);
            if (other != GetComponent<Collider>() && MaxHits > 0 && (other.GetComponentInParent<Hurtbox>() || other.GetComponent<Hurtbox>()) && CanDamage(other) && other.GetComponentInParent<Pawn>() && !siblingcolliders)
            {
                float finaldamage = Damage;
                Pawn p = other.GetComponentInParent<Pawn>();

                BodyPart bp = other.GetComponent<BodyPart>();
                if (bp && bp.SpecialPart)
                {
                    Debug.Log("Special hit " + other.transform.root.name);
                    finaldamage *= bp.damagemultipler;
                }
                else
                {
                    Debug.Log("Hit " + other.transform.root.name);
                }

                if (Source != null)
                {
                    if (Random.Range(0, 100) > Source.GetCritChance().GetValue() * 100)
                    {
                        finaldamage *= Source.GetCritDamage().GetValue();
                    }
                }
                p.TakeDamage(Damage);

                hittargets.Add(other.transform.root.gameObject);

                --MaxHits;
            }
            else if (Intangible == false && other != GetComponent<Collider>() && !siblingcolliders)
            {
                state = State.Deactive;
            }

            if (MaxHits <= 0)
            {
                state = State.Deactive;
            }

        }

    }

    /**
     * IMPORTANT FUNCTION, if hitbox is spawned from instantiate, this function should be called to initalize the hitbox with a source
     * If no source is set, offensive stats like critchance will not be applied to the hitbox.
     */
    public void InitializeHitbox(float damage, Pawn source)
    {
        this.Damage = damage;
        this.Source = source;
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
        Collider col = GetComponent<Collider>();
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
