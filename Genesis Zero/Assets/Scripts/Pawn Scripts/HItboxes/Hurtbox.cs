using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        Debug.Log("Colliding");
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
                    else if(bp.damagemultipler < 1)
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
                    Gizmos.DrawWireSphere(col.transform.position + col.GetComponent<SphereCollider>().center, col.GetComponent<SphereCollider>().radius);
                }
                if (col.GetComponent<BoxCollider>() != null)
                {
                    Gizmos.DrawWireCube(col.transform.position + col.GetComponent<BoxCollider>().center, col.GetComponent<BoxCollider>().size);
                }
            }
        }

    }
}
