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
    public enum State { Headbutt, Firebreath, Pulse, Wild, MovingAway, MovingCloser, Centering, Cooling, Repositioning, Setting, PerformingAttack, Stunned }
    protected State bossstate = State.MovingAway; // Current behavior state
    private bool secondphase;
    [HideInInspector]
    public bool animating, boxanimating;
    private float chargetime = 1;   // Once it hits zero, boss performs a action based on conditions.
    private Transform looktarget;   // Where the boss looks to
    private Transform movetarget;   // Where the boss moves to or away

    private Vector3 templookposition;   // Where the boss will look at as long as the templooktime is > 0
    private float templooktime;         // How long the boss will look at the templookposition

    [HideInInspector]
    public Vector3 lookDir = Vector3.up;
    private Vector3 rotDir = Vector3.forward;
    private Vector3 lookposition;
    private Vector3 LastPosition;

    [Tooltip("Points the boss uses to determine how to do it's attacks")]
    public List<Transform> Waypoints;
    public Transform Center;

    private float lookAngle = 0;
    private float zdepth;

    public float TriggerRadius;
    public float TimeBeforeFight;
    [HideInInspector]
    public bool initiated, lookingatcamera;
    private int Heat, RepeatingAttack, Attack;
    private float actiontime = 1;
    private float chargeuptime = .5f;

    public GameObject Healthbar;
    private GameObject healthbar;
    private float HealthLoss;
    private float TotalHealth;
    private float LastHealth;   //The amount of health the boss had before taking damage;
    public GameObject Indicator;
    private List<GameObject> indicators;

    private Vector3 targetmovement;
    private Vector3 lasttargetposition;

    public GameObject FireballPrefab;
    public GameObject HeadbuttPrefab;
    public GameObject PulsePrefab;

    public GameObject HeadModel;

    public Animator animator;

    private Vector3 back, foward, up, down;
    protected void Awake()
    {
        zdepth = transform.position.z;
        lookposition = transform.position;
        LastPosition = transform.position;
        targetmovement = Vector3.zero;
    }

    new protected void Start()
    {
        base.Start();
        looktarget = Target;
        movetarget = Target;
        lasttargetposition = Target.position;
        TotalHealth = GetHealth().GetValue();
        LastHealth = TotalHealth;
        indicators = new List<GameObject>();
        back = Vector3.back; foward = Vector3.forward; up = Vector3.up; down = Vector3.down;
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
                healthbar = Instantiate(Healthbar, canvas.transform.position + (Healthbar.GetComponent<RectTransform>().position + new Vector3(0,412,0)), Quaternion.identity, canvas.transform);
                TimeBeforeFight = 0;
                Camera.main.GetComponent<BasicCameraZoom>().ChangeFieldOfView(30);
                StartCoroutine(CockBack(1.25f, Target.position - transform.position, 1));
            }
        }
    }

    new protected void FixedUpdate()
    {
        base.FixedUpdate();
        if (Target == null) { return; }
        if (initiated == false) { return; }

        // Determine the velocity of the target
        targetmovement = Target.position - lasttargetposition;
        lasttargetposition = Target.position;

        CheckActions(); // Checks and updates what actions the boss should do

        if (bossstate == State.Setting) // CHeck if the boss is doing special animation
        {
            boxanimating = true;
        }
        else
        {
            boxanimating = false;
        }

        if (boxanimating == false)
        {
            animator.gameObject.GetComponent<SnakeBossBase>().EnableIK();
            Vector3 looktargetposition = templooktime > 0 ? templookposition : looktarget.position;
            if (templooktime > 0)
            {
                templooktime -= Time.fixedDeltaTime;
            }

            if (state == AIState.Follow || state == AIState.Charge || state == AIState.Attack || state == AIState.Cooldown)
            {
                lookDir = Vector3.Slerp(lookDir, looktargetposition - transform.position, 5 * Time.fixedDeltaTime); // Rotate to face target
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
            lookposition = Vector3.Lerp(lookposition, looktargetposition, 3 * Time.fixedDeltaTime);

            if (bossstate == State.Centering || bossstate == State.Pulse)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(back, up), 2 * Time.fixedDeltaTime);
                HeadModel.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(back, up), 2 * Time.fixedDeltaTime);
                lookposition = transform.position + transform.forward;
                lookingatcamera = true;
            }
            else
            {
                Vector3 lookoffset = new Vector3(0, 0, lookDir.x > 0 ? -1f : -1f);
                transform.LookAt(lookposition + lookoffset);
                HeadModel.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(back, up), 2 * Time.fixedDeltaTime);
                //HeadModel.transform.LookAt(lookposition + lookoffset);
                lookingatcamera = false;
            }


            // Don't move until fight starts
            if (TimeBeforeFight > 0) { return; }


            if (animating == false)
            {
                // Move toward target, may move somewhere depending on state
                float speed = GetSpeed().GetValue() * 5;
                if (bossstate == State.MovingCloser && GetDistanceToTarget() - BehaviorProperties.AvoidRadius != 0)
                {
                    float diff = GetDistanceToTarget() - BehaviorProperties.AvoidRadius;
                    transform.position = Vector2.MoveTowards(transform.position, movetarget.position, (speed * diff / GetDistanceToTarget()) * Time.fixedDeltaTime);
                }
                else if (bossstate == State.MovingAway && GetDistanceToTarget() < 30)
                {
                    float diff = GetDistanceToTarget() - 20;
                    transform.position = Vector2.MoveTowards(transform.position, movetarget.position, (speed * diff / GetDistanceToTarget()) / 5 * Time.fixedDeltaTime);
                }
                else if (bossstate == State.Repositioning)
                {
                    GetComponent<SphereCollider>().isTrigger = true;
                    transform.position = Vector2.MoveTowards(transform.position, movetarget.position, speed * (Vector2.Distance(transform.position, movetarget.position) * Mathf.Clamp(chargetime / 5, .5f, 4)) * Time.fixedDeltaTime);
                    if (Vector2.Distance(movetarget.position, transform.position) < 2) { chargetime = Time.fixedDeltaTime / 2; }
                }
                else if (bossstate == State.Centering)
                {
                    GetComponent<SphereCollider>().isTrigger = true;
                    transform.position = Vector2.MoveTowards(transform.position, movetarget.position, speed * (Vector2.Distance(transform.position, movetarget.position) * Mathf.Clamp(chargetime / 5, .5f, 4)) * Time.fixedDeltaTime);
                }
                else if (bossstate == State.Cooling)
                {
                    // Expose weakpoints
                    transform.position = Vector2.MoveTowards(transform.position, movetarget.position, speed * (Vector2.Distance(transform.position, movetarget.position) * Mathf.Clamp(chargetime / 5, .5f, 4)) * Time.fixedDeltaTime);
                }

                Vector3 finalposition = Vector3.Lerp(LastPosition, new Vector3(transform.position.x, transform.position.y, zdepth), .3f);
                LastPosition = finalposition;
                transform.position = finalposition;
            }
            else
            {
                Vector3 finalposition = Vector3.Lerp(LastPosition, new Vector3(transform.position.x, transform.position.y, zdepth), 1);
                LastPosition = finalposition;
                transform.position = finalposition;
            }
        }
        else
        {
            animator.gameObject.GetComponent<SnakeBossBase>().DisableIK();
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

        //Update Indicators
        for (int i = 0; i < indicators.Count; ++i)
        {
            if (indicators[i] == null)
            {
                indicators.Remove(indicators[i]);
                --i;
            }
            else
            {
                indicators[i].GetComponent<BossIndicator>().SetOrigin(transform.position);

            }
        }

        if (GetComponent<SphereCollider>().isTrigger == true)
        {
            Collider[] hit = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius/2);
            foreach (Collider col in hit)
            {
                if (col.gameObject.layer == LayerMask.NameToLayer("BossBoundary"))
                {
                    Debug.Log("Stunned");
                    GetComponent<SphereCollider>().isTrigger = false;
                    SetBossstate(State.Stunned, .5f);
                }
            }
            /**
            BoxCollider2D Bossroom = GameObject.FindGameObjectWithTag("BossRoom").GetComponent<BoxCollider2D>();
            if (Bossroom)
            {
                if (!Bossroom.bounds.Contains((Vector2)transform.position + (Vector2)(Vector3.forward * GetComponent<SphereCollider>().radius)))
                {
                    GetComponent<SphereCollider>().isTrigger = false;
                    chargetime = Time.deltaTime;
                }
            }
            */
        }
    }

    public void CheckActions()
    {
        int action = 0;     // By Default;

        if (LastHealth != GetHealth().GetValue())
        {
            HealthLoss += LastHealth - GetHealth().GetValue();
            LastHealth = GetHealth().GetValue();
        }

        if (HealthLoss >= TotalHealth / 2 )
        {
            action = 1;
            SetBossstate(State.Setting, 4);
            HealthLoss = 0;
            animator.Play("BossWildTest");
        }
        else if (Heat >= 5)
        {
            action = 2;
            bossstate = State.Setting;
            chargetime = Time.fixedDeltaTime / 2;
            Heat = 0;
            Debug.Log("Cooling");
        }

        if (chargetime > 0)
        {
            chargetime -= Time.fixedDeltaTime;

            if (chargetime <= 0)
            {
                ChooseAction(action, false);
            }
        }

    }

    public void ChooseAction(int action, bool bypass)
    {
        // Do an action based on a set up state.
        if (bossstate == State.Repositioning || RepeatingAttack > 0)
        {
            movetarget = Target;    // Reset movetarget back to default target (player)

            if (RepeatingAttack <= 0)
            {
                Attack = (int)Random.Range(0, 3);
                if (GetDistanceToTarget() <= 6 && MovingAway() == false)    // Will most likely pulse if player is too close
                {
                    Attack = (int)Random.Range(0, 10) <= 6 ? 2 : Attack;
                }
            }

            GetComponent<SphereCollider>().isTrigger = false;

            if (Attack == 0)
            {
                SetRepeatingAttacks(3);
                actiontime = RepeatingAttack > 0 ? SetAttackTime(1.35f, 1) : SetAttackTime(2, 1);
                SetBossstate(State.Headbutt, actiontime);
                Vector3 target = PredictPath(1.25f);
                SpawnIndicator(transform.position, new Vector2(16, 6), target - transform.position, new Color(1, 0, 0, .1f), Vector2.zero, false, true, chargeuptime);
                LookAtVectorTemp(target + ((target - transform.position).normalized * 20), actiontime);
                StartCoroutine(MoveTo(transform.position + ((target - transform.position).normalized * 16), .25f, chargeuptime));
            }
            else if (Attack == 1)
            {
                SetRepeatingAttacks(3);
                actiontime = RepeatingAttack > 0 ? SetAttackTime(1.2f, 1) : SetAttackTime(1.5f, 1);
                SetBossstate(State.Firebreath, actiontime);
                Vector3 target = PredictPath(1f);
                SpawnIndicator(transform.position, new Vector2(24, 4), target - transform.position, new Color(1, 0, 0, .1f), Vector2.zero, false, true, actiontime - .1f);
                LookAtVectorTemp(target, actiontime - .1f);
                Invoke("SpitFireball", actiontime - .1f);
            }
            else
            {
                SetRepeatingAttacks(0);
                actiontime = RepeatingAttack > 0 ? SetAttackTime(1.2f, 1) : SetAttackTime(1.2f, 1);
                SetBossstate(State.Pulse, actiontime);
                SpawnIndicator(transform.position, new Vector2(18, 18), lookDir, new Color(1, 0, 0, .1f), Vector2.zero, true, false, actiontime);
            }

            if (RepeatingAttack < 0)
            {
                ++Heat;
            }

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
                SetBossstate(State.MovingAway, 3);
            }
            else
            {
                SetBossstate(State.MovingCloser, 3);
            }
            return;
        }


        // Do an action after a non-setup action.
        //float action = (int)Random.Range(0, 3);
        switch (action)
        {
            case 0:
                Debug.Log("Repos");
                SetBossstate(State.Repositioning, 2);
                movetarget = GetClosestWaypoint();
                break;
            case 1:
                SetBossstate(State.Centering, 1);
                movetarget = Center;
                break;
            case 2:
                SetBossstate(State.Cooling, 1);
                movetarget = transform;
                break;
            default:
                SetBossstate(State.Repositioning, 2);
                movetarget = GetClosestWaypoint();
                break;
        }
    }

    public float SetAttackTime(float actiontime, float chargeuptime)
    {
        this.actiontime = actiontime;
        this.chargetime = chargeuptime;
        return actiontime;
    }

    public void SetBossstate(State state, float time)
    {
        bossstate = state;
        chargetime = time;
    }

    /**
     * Sets repeatingattacks to up to the number specificed
     */
    public void SetRepeatingAttacks(int num)
    {
        if (RepeatingAttack <= 0)
        {
            RepeatingAttack = (int)Random.Range(0, num + 1);
        }
        else
        {
            --RepeatingAttack;
        }
    }

    public void LookAtVectorTemp(Vector3 position, float time)
    {
        templookposition = position;
        templooktime = time;
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

    /**
     * Returns the possible position of the player's position based on time.
     */
    public Vector3 PredictPath(float time)
    {
        float zspeed = targetmovement.y > 0 ? targetmovement.y - .1f : targetmovement.y + .1f;
        //Debug.Log("Z:" + zspeed);
        Vector3 position = Target.position + ((targetmovement * time * (1 / Time.fixedDeltaTime)) * Mathf.Clamp(GetDistanceToTarget() / 20, .1f, 1));
        position += new Vector3(0, 0, zspeed * 6 * Time.fixedDeltaTime);
        return position;
    }

    /**
     * Returns true if the player is moveing away from the boss. Also true if player is realy close
     */
    public bool MovingAway()
    {
        if (GetDistanceToTarget() < 3)
        {
            return true;
        }
        else if (Target.position.x - transform.position.x > 0 && targetmovement.x > 0)
        {
            return true;
        }
        else if (Target.position.x - transform.position.x < 0 && targetmovement.x < 0)
        {
            return true;
        }

        return false;
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

    IEnumerator MoveTo(Vector3 targetposition, float time, float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector3 startposition = transform.position;
        GetComponent<SphereCollider>().isTrigger = true;
        animating = true;
        float totaltime = time;
        float distance = Vector2.Distance(transform.position, targetPosition);
        float speed = distance / totaltime;
        for (float f = 0; f <= time; f += Time.fixedDeltaTime)
        {
            if (bossstate == State.Stunned)
            {
                break;
            }
            transform.position = Vector3.Lerp(startposition, targetposition, f / time);
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }
        animating = false;
    }

    IEnumerator SpawnPrefab(GameObject prefab, Vector3 position, Quaternion direction, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject hitbox = Instantiate(prefab, position, direction);
    }

    void RayCast(Vector2 target, float distance)
    {
        RaycastHit hit;
        BoxCollider2D Bossroom = GameObject.FindGameObjectWithTag("BossRoom").GetComponent<BoxCollider2D>();
        if (Bossroom.bounds.Contains(target))
        {

        }
        if (GetComponent<SphereCollider>())
        {
            SphereCollider col = GetComponent<SphereCollider>();
            bool hitdetect = Physics.SphereCast(transform.position, col.radius, transform.forward, out hit, distance, LayerMask.NameToLayer("BossBoundary"));
            if (hitdetect && hit.collider != col)
            {         
                //return true;
            }
        }
    }

    void SpitFireball()
    {
        Vector2 angle = (Vector2)transform.rotation.eulerAngles;
        Quaternion rot = Quaternion.Euler(angle.x, angle.y, 0);
        GameObject hitbox = Instantiate(FireballPrefab, transform.position, rot);
    }

    public void SpawnIndicator(Vector2 position, Vector2 size, Vector2 dir, Color color, Vector2 offset, bool centered, bool square, float time)
    {
        GameObject instance = (GameObject)Instantiate(Indicator, position, Indicator.transform.rotation);
        instance.GetComponent<BossIndicator>().SetIndicator(position, size, dir, color, centered, time);
        instance.GetComponent<BossIndicator>().SetOffset(offset);
        instance.GetComponent<BossIndicator>().SetCentered(centered);
        if (square)
        {
            instance.GetComponent<BossIndicator>().SetSquare();
        }
        else
        {
            instance.GetComponent<BossIndicator>().SetCircle();
        }
        Destroy(instance, time);
        indicators.Add(instance);
    }

    private Vector2 CastAtAngle(Vector2 position, Vector2 direction, float distance)
    {
        Vector2 result = position + direction.normalized * distance;
        return result;
    }

    private new void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        GizmosExtra.DrawWireCircle(transform.position, Vector3.forward, TriggerRadius);
        base.OnDrawGizmos();
    }
}
