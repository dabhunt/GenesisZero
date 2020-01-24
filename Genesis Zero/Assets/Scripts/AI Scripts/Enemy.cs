using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Pawn
{
    private float radius = 1f;
    Transform target;
    Animator ani;
    public Transform raypointtarget;
    public float targetdistance = 2f;
    public float checkerdistance = .1f;
    
    public void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    public void Update()
    {
        base.Update();
    }

    public Transform GetTarget()
    {
        return target;
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(raypointtarget.position, -Vector2.up, checkerdistance);
        if (hit.collider != null)
        {

            return true;
        }
        else { return false; }
    }

    public bool inRange(float distance)
    {
        // Debug.Log(gameObject.name + ": distance from player is:" + Vector3.Distance(this.transform.position, target.position));
        if (Vector3.Distance(transform.position, GetTarget().position) < distance)
            return true;
        return false;
    }

    public float distancefromTarget()
    {
        return Vector3.Distance(transform.position, GetTarget().position);
    }

    //getter and setter for radius
    public float Radius { get => radius; set => radius = value; }
    public Animator Ani { get => ani; set => ani = value; }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, targetdistance);
    }
}
