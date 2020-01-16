using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Hitbox : MonoBehaviour
{
    float damage;
    
    bool HurtsAliies;
    bool HurtsEnemies;

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
        foreach (Collider col in colliders)
        {
            if (col == null)
            {
                colliders.Remove(col);
            }
        }
    }

    [ExecuteInEditMode]
    private void OnDrawGizmos()
    {
        if (colliders != null)
        {
            for (int i = 0; i < colliders.Count; i++)
            {
                Gizmos.color = Color.red;
                Collider col = colliders[i];
                // Debug.Log(i);

                if (col.GetComponent<SphereCollider>() != null)
                {
                    //Debug.Log("C");
                    Gizmos.DrawWireSphere(transform.position + col.transform.localPosition + col.GetComponent<SphereCollider>().center, col.GetComponent<SphereCollider>().radius);
                }

                if (col.GetComponent<BoxCollider>() != null)
                {
                    //Debug.Log(col.GetComponent<BoxCollider>().center);
                    Gizmos.DrawWireCube(transform.position + col.transform.localPosition + col.GetComponent<BoxCollider>().center, col.GetComponent<BoxCollider>().size);
                }

            }
        }
    }
}
