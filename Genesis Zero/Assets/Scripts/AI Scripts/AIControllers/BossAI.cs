using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/**
 * Kenny Doan
 * BossAI is the class representing the boss of the game
 */
public class BossAI : AIController
{
    public enum State { Headbutt, Firebreath, Pulse, Wild, MovingAway, MovingCloser, Centering, Cooling, Repositioning }
    protected State bossstate = State.MovingAway; // Current behavior state
    private bool secondphase;
    private bool animating;
    private float chargetime = 1;   // Once it hits zero, boss performs a action based on conditions.
    private Transform looktarget;   // Where the boss looks to
    private Transform movetarget;   // Where the boss moves to or away

    private Vector3 lookDir = Vector3.up;
    private Vector3 rotDir = Vector3.forward;
    private Vector3 lookposition;

    [Tooltip("Points the boss uses to determine how to do it's attacks")]
    public List<Transform> Waypoints;
    public Transform Center;

    private float lookAngle = 0;
    private float zdepth;

    public float TriggerRadius;
    public float TimeBeforeFight;
    private bool initiated;

    public GameObject Healthbar;
    private GameObject healthbar;
    protected void Awake()
    {
        zdepth = transform.position.z;
        lookposition = transform.position;
    }

    new protected void Start()
    {
        base.Start();
        looktarget = Target;
        movetarget = Target;
    }

    new protected void Update()
    {
        base.Update();
        if (GetDistanceToTarget() < TriggerRadius) { initiated = true; }
        if (initiated && TimeBeforeFight > 0)
        {
            TimeBeforeFight -= Time.deltaTime;
            if (TimeBeforeFight < 0)
            {
                GameObject canvas = GameObject.FindGameObjectWithTag("CanvasUI");
                healthbar = Instantiate(Healthbar, canvas.transform.position + (Healthbar.GetComponent<RectTransform>().position + new Vector3(0, -135, 0)) / canvas.GetComponent<Canvas>().referencePixelsPerUnit, Quaternion.identity, canvas.transform);
                TimeBeforeFight = 0;
                StartCoroutine(Spandout(.5f, Camera.main.fieldOfView, 30));
                StartCoroutine(CockBack(1.25f, Target.position - transform.position, 1));
            }
        }
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if (Target == null) { return; }
        if (initiated == false) { return; }

        CheckActions(); // Checks and updates what actions the boss should do

        if (state == AIState.Follow || state == AIState.Charge || state == AIState.Attack || state == AIState.Cooldown)
        {
            lookDir = Vector3.Slerp(lookDir, looktarget.position - transform.position, 5 * Time.fixedDeltaTime); // Rotate to face target
        }
        else if (state == AIState.Patrol)
        {
            //For now, patrolling just moves the drone in a circle
            lookDir = Quaternion.AngleAxis(5 * Time.fixedDeltaTime, Vector3.forward) * lookDir;
        }
        else if (state == AIState.Idle) // Stunned
        {

        }

        // Set where the boss looks at, default player
        lookposition = Vector3.Lerp(lookposition, looktarget.position, 3 * Time.fixedDeltaTime);

        if (bossstate == State.Centering || bossstate == State.Pulse)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back, Vector3.up), 2 * Time.fixedDeltaTime);
            lookposition = transform.position + transform.forward;
        }
        else
        {
            Vector3 lookoffset = new Vector3(0, 0, lookDir.x > 0 ? -1f : -1f);
            transform.LookAt(lookposition + lookoffset);
        }


        // Don't move until fight starts
        if (TimeBeforeFight > 0) { return; }


        // Move toward target, may move somewhere depending on state
        float speed = GetSpeed().GetValue() * 5;
        if (animating == false)
        {
            if (bossstate == State.MovingCloser && GetDistanceToTarget() - BehaviorProperties.AvoidRadius != 0)
            {
                float diff = GetDistanceToTarget() - BehaviorProperties.AvoidRadius;
                transform.position = Vector2.MoveTowards(transform.position, movetarget.position, (speed * diff / GetDistanceToTarget()) * Time.fixedDeltaTime);
            }
            else if (bossstate == State.MovingAway && GetDistanceToTarget() < 30)
            {
                float diff = GetDistanceToTarget() - 20;
                transform.position = Vector2.MoveTowards(transform.position, movetarget.position, (speed * diff / GetDistanceToTarget()) / 10 * Time.fixedDeltaTime);
            }
            else if (bossstate == State.Repositioning)
            {
                GetComponent<SphereCollider>().isTrigger = true;
                transform.position = Vector2.MoveTowards(transform.position, movetarget.position, speed * (Vector2.Distance(transform.position, movetarget.position) / 20) * Time.fixedDeltaTime);
            }
            else if (bossstate == State.Centering)
            {
                GetComponent<SphereCollider>().isTrigger = true;
                transform.position = Vector2.MoveTowards(transform.position, movetarget.position, speed * (Vector2.Distance(transform.position, movetarget.position) / 15) * Time.fixedDeltaTime);
            }
            else if (bossstate == State.Cooling)
            {
                // Expose weakpoints
                transform.position = Vector2.MoveTowards(transform.position, movetarget.position, speed * (Vector2.Distance(transform.position, movetarget.position) / 10) * Time.fixedDeltaTime);
            }

            transform.position = new Vector3(transform.position.x, transform.position.y, zdepth);
        }


        //Display Health
        if (healthbar && initiated)
        {
            healthbar.GetComponentInChildren<Slider>().value = GetHealth().GetRatio();
        }
        else if (healthbar)
        {
            healthbar.SetActive(true);
        }
    }

    public void CheckActions()
    {
        if (chargetime > 0)
        {
            chargetime -= Time.fixedDeltaTime;

            if (chargetime <= 0)
            {
                // Do an action based on a set up state.
                if (bossstate == State.Repositioning)
                {
                    movetarget = Target;    // Reset movetarget back to default target (player)

                    float attack = (int)Random.Range(0, 2);
                    if (attack == 0)
                    {
                        SetBossstate(State.Headbutt, 3);
                    }
                    else
                    {
                        SetBossstate(State.Firebreath, 3);
                    }

                    GetComponent<SphereCollider>().isTrigger = false;
                    return;
                }
                else if (bossstate == State.Centering)
                {
                    movetarget = Target;    // Reset movetarget back to default target (player)

                    GetComponent<SphereCollider>().isTrigger = false;
                    SetBossstate(State.Pulse, 1);
                    return;
                }
                else if (bossstate == State.Cooling)
                {
                    movetarget = Target;    // Reset movetarget back to default target (player)

                    float move = (int)Random.Range(0, 2);
                    if (move == 0)
                    {
                        SetBossstate(State.MovingAway, 2);
                    }
                    else
                    {
                        SetBossstate(State.MovingCloser, 2);
                    }
                    return;
                }

                // Do an action after a non-setup action.
                float action = (int)Random.Range(0, 3);
                switch (action)
                {
                    case 0:
                        SetBossstate(State.Repositioning, 2);
                        movetarget = GetClosestWaypoint();
                        break;
                    case 1:
                        SetBossstate(State.Centering, 2);
                        movetarget = Center;
                        break;
                    case 2:
                        SetBossstate(State.Cooling, 3);
                        //CockBack(3, transform.position + new Vector3(0, -3, 0), 2);
                        movetarget = transform;
                        break;
                    default:
                        SetBossstate(bossstate = State.Repositioning, 3);
                        break;
                }
            }
        }
        else
        {
            chargetime = 1; // In case something happens
        }
    }

    public void SetBossstate(State state, float time)
    {
        bossstate = state;
        chargetime = time;
    }

    public Transform GetNearestVisibleWaypoint()
    {
        if (!EmptyWaypoints()) return transform;

        Transform min = Waypoints[0];
        float mindist = Vector2.Distance(min.position, Target.position);

        RaycastHit hit;
        SphereCollider col = GetComponent<SphereCollider>();

        foreach (Transform t in Waypoints)
        {
            bool hitdetect = Physics.SphereCast(t.position, col.radius / 2, Target.position - t.position, out hit, 30);

            float currdist = Vector2.Distance(t.position, Target.position);
            if (currdist < mindist && hitdetect && hit.collider.GetComponentInParent<Player>())
            {
                min = t;
                mindist = currdist;
            }
        }
        return min;
    }

    // Returns waypoint furthest away from target
    public Transform GetFurthestWaypoint()
    {
        if (!EmptyWaypoints()) return transform;

        Transform max = Waypoints[0];
        float maxdist = Vector2.Distance(max.position, Target.position);
        foreach (Transform t in Waypoints)
        {
            float currdist = Vector2.Distance(t.position, Target.position);
            if (currdist > maxdist)
            {
                max = t;
                maxdist = currdist;
            }
        }
        return max;
    }

    // Returns waypoint closest to the target
    public Transform GetClosestWaypoint()
    {
        if (!EmptyWaypoints()) return transform;

        Transform min = Waypoints[0];
        float mindist = Vector2.Distance(min.position, Target.position);
        foreach (Transform t in Waypoints)
        {
            float currdist = Vector2.Distance(t.position, Target.position);
            if (currdist < mindist)
            {
                min = t;
                mindist = currdist;
            }
        }
        return min;
    }

    public bool EmptyWaypoints()
    {
        if (Waypoints != null)
        {
            Debug.LogWarning("Waypoints not initialized for boss");
            return true;
        }
        return false;
    }

    IEnumerator Spandout(float time, float start, float target)
    {
        for (float f = 0; f <= time; f += Time.fixedDeltaTime)
        {
            Camera.main.fieldOfView = start + ((target - start) * f / time);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    IEnumerator CockBack(float time, Vector3 dir, float distance)
    {
        animating = true;
        Vector3 diff = Vector3.zero;
        for (float f = 0; f <= time; f += Time.fixedDeltaTime)
        {
            //Debug.Log(Mathf.Cos(Mathf.PI * (f / time)));
            Vector3 vector = (Mathf.Cos(Mathf.PI * (f / time)) * -dir.normalized * distance / (10 * time));
            Vector3 translation = vector - diff;
            //diff = vector;
            transform.position += translation;
            if (time - f < Time.fixedDeltaTime * 2 && animating == true)
            {
                animating = false;
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
    }

    private new void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        GizmosExtra.DrawWireCircle(transform.position, Vector3.forward, TriggerRadius);
        base.OnDrawGizmos();
    }
}
