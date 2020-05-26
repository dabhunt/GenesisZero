using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Experimental.VFX;
using UnityEngine.VFX.Utility;

public class EssenceScript : MonoBehaviour
{
    private VisualEffect vEffect;
    static readonly ExposedProperty boolAttribute = "_isNearby";

    public int Amount = 1;
    private GameObject target;
    private bool added;
    public float speedvar=1.1f;
    public float attractionDistance=5f;
    Animator eAnimator; //Essence Animator
    // Start is called before the first frame update
    void Start()
    {
        vEffect = GetComponent<VisualEffect>();
        eAnimator = GetComponent<Animator>();
        eAnimator.SetInteger("EssenceAmt", Amount);
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        if (target != null){
            Vector3 transTarget = target.transform.position;
            transTarget.y ++;
            float distance = Vector2.Distance(transform.position, transTarget);
            vEffect.SetBool(boolAttribute, false);
            //GetComponent<Animator>().
            if (distance < attractionDistance){
                if (GetComponent<Floating>() != null)
                {
                    Destroy(GetComponent<Floating>());
                    //GetComponent<Animator>().SetBool("PlayerTouch", true);
                }
                //transform.LookAt(target.transform.position);
                speedvar = speedvar*1.1f;
                this.transform.position = Vector3.MoveTowards(this.transform.position, transTarget, speedvar * Time.deltaTime);
                //vEffect.SetBool(boolAttribute, true);

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

