using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingularityPull : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider[] collisions = null;
        if (GetComponent<SphereCollider>())
        {
            collisions = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
        }
        foreach (Collider col in collisions)
        {
            if (col.GetComponent<Pawn>() && col.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                Pawn p = col.GetComponent<Pawn>();
                float distance = Vector2.Distance(p.transform.position, transform.position);
                float radius = 3;
                float force = 10 * (distance / radius);
                p.KnockBack(transform.position - p.transform.position, force);
                p.GetStunnedStatus().AddTime(1);
                Debug.Log("Pulled");
            }
        }
    }
}
