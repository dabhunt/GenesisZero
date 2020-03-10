using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingularityPull : MonoBehaviour
{
    private List<GameObject> PulledTargets;
    // Start is called before the first frame update
    void Start()
    {
        PulledTargets = new List<GameObject>();
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
                p.KnockBack((transform.position - p.transform.position) + Vector3.up/2, force);
                p.GetStunnedStatus().AddTime(1);
                PulledTargets.Add(col.gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (GameObject g in PulledTargets)
        {
            g.transform.position = Vector2.Lerp(g.transform.position, transform.position, Time.fixedDeltaTime);
        }
    }
}
