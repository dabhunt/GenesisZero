using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssenceScript : MonoBehaviour
{
    public int Amount = 1;
    private GameObject target;
    private bool added;
    public float speedvar=1.1f;
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (target != null){
            Vector3 transTarget = target.transform.position;
            transTarget.y ++;
            float distance = Vector2.Distance(transform.position, transTarget);

            if (distance < 4){
                print("close enough to run");

                transform.LookAt(target.transform.position);
                speedvar = speedvar*1.1f;
                this.transform.position = Vector3.MoveTowards(this.transform.position, transTarget, speedvar * Time.deltaTime);
            }
            
        }
    }

    // Update is called once per frame
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

