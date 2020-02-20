using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceScript : MonoBehaviour
{
    public int Amount = 0;
    private GameObject target;
    private bool added;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>() || other.GetComponent<Player>())
        {
            Player p = other.GetComponent<Player>();
            if (other.GetComponentInParent<Player>())
            {
                p = other.GetComponentInParent<Player>();
            }
            if (p != null && added == false)
            {
                p.AddEssence(Amount);
                added = true;
            }

            if (added == true)
            {
                Destroy(gameObject);
            }

        }
    }

}

