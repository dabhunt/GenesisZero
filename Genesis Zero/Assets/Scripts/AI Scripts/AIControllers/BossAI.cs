using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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
	private float zdepth, deathtime;

	public float TriggerRadius;
	public float TimeBeforeFight;
	[HideInInspector]
	public bool initiated, introdialogue, lookingatcamera, animationswitch, Wild, firsttrigger, secondtrigger, died;
	private int Heat, RepeatingAttack, Attack, deathtrigger;
	private float actiontime = 1;
	private float chargeuptime = .5f;

	//Headbutt variables
	private float headbutttime, headbuttdelay, headbutttotaltime, headbuttdistance;
	private Vector3 headbuttstartposition, headbutttarget, headbuttangle;

	public GameObject Healthbar;
	private GameObject healthbar;
	private float HealthLoss, TotalHealth, LastHealth, PassedTime, LastZDepth;   //The amount of health the boss had before taking damage;
	public GameObject Indicator;
	private List<GameObject> indicators;

	private Vector3 targetmovement;
	private Vector3 lasttargetposition;

	public GameObject FireballPrefab;
	public GameObject HeadbuttPrefab;
	public GameObject PulsePrefab;
	public GameObject FlameGround;
	public GameObject WildVFX;

	public GameObject ForwardLookObject;
	public MiddleMan Middleman;
	public GameObject HeadModel;
	public Animator animator;

	private GameObject camera;
	private Color color;

	private Vector3 back, foward, up, down;
	protected override void Awake()
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
		camera = Camera.main.gameObject;
		LastZDepth = transform.position.z;
	}

	new protected void Update()
	{
		base.Update();

		if (deathtime > 1 && Input.GetKeyDown(KeyCode.Alpha0)) // Button to load to credits
		{
			LoadCredits();
		}

		if (GetDistanceToTarget() < TriggerRadius && initiated == false)
		{
			GameInputManager.instance.DisablePlayerControls();
			GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().movementInput = Vector2.zero;
			initiated = true;
		}
		if (initiated)
		{
			PassedTime += Time.deltaTime;
			if (introdialogue == false && PassedTime >= .85f)
			{
				DialogueManager.instance.TriggerDialogue("PreBoss4", false);
				AudioManager.instance.PlaySound("SFX_BossRoar(1)");
				introdialogue = true;
			}

			if (TimeBeforeFight > 0)
			{
				TimeBeforeFight -= Time.deltaTime;
				if (DialogueManager.instance.IsDialoguePlaying() == false && PassedTime > 1.25f)
				{
					TimeBeforeFight = 0;
				}

				if (TimeBeforeFight <= 0)
				{
					GameInputManager.instance.EnablePlayerControls();
					AudioManager.instance.PlaySound("SFX_BossRoar(0)");
					camera.transform.DOShakePosition(duration: 1.25f, strength: 1, vibrato: 5, randomness: 60, snapping: false, fadeOut: true);
					Camera.main.GetComponent<BasicCameraZoom>().ChangeFieldOfView(30, 1);
					GameObject canvas = GameObject.FindGameObjectWithTag("CanvasUI");
					healthbar = canvas.transform.Find("BossHealthbar").gameObject;
					healthbar.SetActive(true);
					TimeBeforeFight = 0;
					SetBossstate(State.Setting, 1.25f);
					animator.Play("Intro");
					//StartCoroutine(CockBack(1.25f, Target.position - transform.position, 1));
				}
				else
				{
					SetInvunerable(Time.deltaTime * 2);
				}
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
		LastZDepth = Mathf.Lerp(LastZDepth, zdepth, Time.fixedDeltaTime);

		if (TimeBeforeFight <= 0 && IsDying() == false)
		{
			CheckActions(); // Checks and updates what actions the boss should do
		}

		if (bossstate == State.Setting) // CHeck if the boss is doing special animation
		{
			if (animationswitch == false)
			{
				animationswitch = true;
			}
			boxanimating = true;
		}
		else if (IsDying() == false)
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

			//if (bossstate == State.Centering || bossstate == State.Pulse) { looktargetposition = LookForwardTarget.transform.position; }

			// Set where the boss looks at, default player
			float strength = bossstate == State.Firebreath ? 10 : 3;
			lookposition = Vector3.Lerp(lookposition, looktargetposition, strength * Time.fixedDeltaTime);

			if (bossstate == State.Pulse || bossstate == State.Centering)
			{
				/**
				Quaternion look = Quaternion.LookRotation((ForwardLookObject.transform.position) - transform.position);
				transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation((ForwardLookObject.transform.position) - transform.position), Time.fixedDeltaTime);
				HeadModel.transform.rotation = transform.rotation;
				lookposition = transform.position + transform.forward;
				**/
				lookingatcamera = true;
			}
			else
			{
				lookingatcamera = false;
			}

			Vector3 lookoffset = new Vector3(0, 0, lookDir.x > 0 ? -1f : -1f);
			//transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(lookposition + lookoffset - transform.position), Time.fixedDeltaTime * 180);
			transform.LookAt(lookposition + lookoffset);
			HeadModel.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(back, up), Time.fixedDeltaTime);
			//HeadModel.transform.LookAt(lookposition + lookoffset);


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
					if (Vector2.Distance(movetarget.position, transform.position) < 2 && chargetime <= 1) { chargetime = Time.fixedDeltaTime / 2; }
				}
				else if (bossstate == State.Headbutt && headbuttdelay <= 0)
				{
					GetComponent<SphereCollider>().isTrigger = true;
					Vector3 startposition = headbuttstartposition;
					float totaltime = headbutttotaltime;
					float distance = headbuttdistance;
					float hbspeed = distance / totaltime;
					transform.position = Vector3.Lerp(startposition, headbutttarget, (totaltime - headbutttime) / totaltime);
					//transform.position += (headbutttarget - startposition).normalized * hbspeed * Time.fixedDeltaTime;
					LastPosition = transform.position;
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
				else if (bossstate == State.Stunned)
				{
					GetComponent<SphereCollider>().isTrigger = false;
				}

				if (bossstate != State.Headbutt)
				{
					Vector3 finalposition = Vector3.Lerp(LastPosition, new Vector3(transform.position.x, transform.position.y, LastZDepth), .3f);
					LastPosition = finalposition;
					transform.position = finalposition;
				}

			}
			else
			{
				Vector3 finalposition = Vector3.Lerp(LastPosition, new Vector3(transform.position.x, transform.position.y, LastZDepth), 1);
				LastPosition = finalposition;
				transform.position = finalposition;
			}
		}
		else
		{
			//this.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") && 
			if (animationswitch == true && bossstate != State.Setting)
			{
				transform.position = HeadModel.transform.position;
				LastPosition = transform.position;
				transform.rotation = HeadModel.transform.rotation;
				Middleman.UpdateJoint();
				SetBossstate(State.Repositioning, 2);
				movetarget = GetClosestWaypointToBoss();
				animationswitch = false;
			}
			//transform.rotation = HeadModel.transform.rotation;
			animator.gameObject.GetComponent<SnakeBossBase>().DisableIK();
		}

		//Display Health
		if (IsDying())
		{
			if (died == false)
			{
				DisableUIExceptDialogue();
				SetBossstate(State.Setting, 20f);
				animator.SetTrigger("Dead");
				Camera.main.GetComponent<BasicCameraZoom>().ChangeFieldOfViewTemporary(45, 1, .25f);
				KillNearbyEnemies();
				died = true;
				deathtrigger = 1;
			}

			deathtime += Time.deltaTime;
			boxanimating = true;
			healthbar.SetActive(false);
		}
		else if (healthbar && initiated)
		{
			healthbar.GetComponentInChildren<Slider>().value = GetHealth().GetRatio();
		}

		if (died)
		{
			DeathChecks();
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

		if (!(GameObject.FindGameObjectWithTag("BossRoom").GetComponent<BoxCollider2D>().bounds.Contains((Vector2)transform.position) || GameObject.FindGameObjectWithTag("BossRoom").GetComponent<BoxCollider2D>().bounds.Contains((Vector2)transform.Find("Hitbox 2").position)))
		{
			Bounds bound = GameObject.FindGameObjectWithTag("BossRoom").GetComponent<BoxCollider2D>().bounds;
			transform.position = Vector2.Lerp(transform.position, bound.center, Time.fixedDeltaTime);
			if (bound.Contains(transform.position) && bound.Contains(transform.Find("Hitbox 2").position))
			{
				GetComponent<SphereCollider>().isTrigger = false;
			}
			else
			{
				//Debug.Log("Stunned");
				GetComponent<SphereCollider>().isTrigger = true;
				if (bossstate == State.Headbutt)
				{
					camera.transform.DOShakePosition(duration: .75f, strength: 1, vibrato: 5, randomness: 60, snapping: false, fadeOut: true);
				}
				chargetime = .5f;
				SetBossstate(State.Stunned, .5f);
			}

		}
		else if (GetComponent<SphereCollider>().isTrigger == true)
		{
			Collider[] hit = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius / 2);
			foreach (Collider col in hit)
			{
				if (col.gameObject.layer == LayerMask.NameToLayer("BossBoundary"))
				{
					Debug.Log("Stunned");
					GetComponent<SphereCollider>().isTrigger = false;
					chargetime = .5f;
					if (bossstate == State.Headbutt)
					{
						camera.transform.DOShakePosition(duration: .75f, strength: 1, vibrato: 5, randomness: 60, snapping: false, fadeOut: true);
					}
					SetBossstate(State.Stunned, .5f);
				}
			}
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

		if (HealthLoss >= TotalHealth / 4 && GetComponent<BossEvents>())
		{
			if (firsttrigger == false) // Top quarter health
			{
				GetComponent<BossEvents>().SpawnFirstGoons();
				firsttrigger = true;
			}
			else if (Wild && secondtrigger == false) // Bottom quarter health
			{
				GetComponent<BossEvents>().SpawnSecondGoons();
				secondtrigger = true;
			}
		}
		if (HealthLoss >= TotalHealth / 2 && GetHealth().GetValue() > 0)
		{
			action = 1;
			SetBossstate(State.Setting, 2.5f);
			HealthLoss = 0;
			animator.SetTrigger("WildTrigger"); //Set wild trigger
			Invoke("BurnGround", 1f);
			FlameGround.SetActive(true);        //Set the flame ground to true/active
			animator.SetBool("Wild", true);
			camera.transform.DOShakePosition(duration: 2.4f, strength: 1, vibrato: 5, randomness: 60, snapping: false, fadeOut: true);
			Camera.main.GetComponent<BasicCameraZoom>().ChangeFieldOfViewTemporary(45, 1.1f, .25f);
			AudioManager.instance.PlaySound("SFX_BossRoar(0)"); // Play sound Effect
			GetComponent<BossEvents>().DestroyGameObjectsOnWild(); //Destroy top platforms
			if (WildVFX)
			{
				Invoke("WildVFXSpawn", .5f);
			}
			boxanimating = true;
			Wild = true;
		}
		else if (Heat >= 5)
		{
			action = 2;
			bossstate = State.Setting;
			chargetime = Time.fixedDeltaTime / 2;
			Heat = 0;
			animator.SetTrigger("Overheating");
		}

		if (chargetime > 0)
		{
			chargetime -= Time.fixedDeltaTime;

			if (chargetime <= 0)
			{
				ChooseAction(action, false);
			}
		}

		if (headbuttdelay > 0)
		{
			headbuttdelay -= Time.fixedDeltaTime;
			if (headbuttdelay <= 0)
			{
				headbuttstartposition = transform.position;
				headbutttarget = headbuttstartposition + headbuttangle;
				//Debug.Log("Distance: " + Vector2.Distance(transform.position, headbutttarget));
			}
		}
		else
		{
			if (headbutttime > 0)
			{
				headbutttime -= Time.fixedDeltaTime;
			}
			else
			{
				headbutttime = 0;
			}
			headbuttdelay = 0;
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
			//Attack = 0;
			GetComponent<SphereCollider>().isTrigger = false;

			if (Attack == 0)
			{
				animator.SetTrigger("Headbutting");
				actiontime = RepeatingAttack > 0 ? SetAttackTime(1.35f, 1) : SetAttackTime(1.4f, 1.1f);
				SetRepeatingAttacks(2);
				SetBossstate(State.Headbutt, actiontime);
				Vector3 target = PredictPath(1.25f);
				SpawnIndicator(transform.position, new Vector2(16, 6), target - transform.position, new Color(1, 0, 0, .1f), Vector2.zero, false, true, chargeuptime);
				LookAtVectorTemp(target + ((target - transform.position).normalized * 20), actiontime);
				//StartCoroutine(MoveTo(transform.position + ((target - transform.position).normalized * 16), .25f, chargeuptime));
				headbuttangle = ((target - transform.position).normalized * 16);
				HeadButt(transform.position + ((target - transform.position).normalized * 16), .25f, chargeuptime);
				Invoke("SpawnHeadbutt", chargeuptime);
				AudioManager.instance.PlaySound("SFX_ChargeHeadSlam");
			}
			else if (Attack == 1)
			{
				animator.SetTrigger("FireSpitting");
				actiontime = RepeatingAttack > 0 ? SetAttackTime(.8f, .7f) : SetAttackTime(1f, .9f);
				SetRepeatingAttacks(2);
				SetBossstate(State.Firebreath, actiontime);
				Vector3 target = PredictPath(.2f);
				SpawnIndicator(transform.position, new Vector2(24, 4), target - transform.position, new Color(1, 0, 0, .1f), Vector2.zero, false, true, actiontime - .1f);
				LookAtVectorTemp(target, actiontime - .1f);
				Invoke("SpitFireball", actiontime - .1f);
				AudioManager.instance.PlaySound("SFX_ChargeFireBall");
			}
			else
			{
				SetRepeatingAttacks(0);
				animator.SetTrigger("Pulsing");
				actiontime = RepeatingAttack > 0 ? SetAttackTime(1.4f, 1.2f) : SetAttackTime(1.4f, 1.2f);
				SetBossstate(State.Pulse, actiontime);
				SpawnIndicator(transform.position, new Vector2(18, 18), lookDir, new Color(1, 0, 0, .1f), Vector2.zero, true, false, chargeuptime);
				Invoke("Pulse", chargeuptime);
				LookAtVectorTemp(ForwardLookObject.transform.position, actiontime);
				AudioManager.instance.PlaySound("SFX_ChargePulse");
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
				//Debug.Log("Repos");
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
		this.chargeuptime = chargeuptime;
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
			float minnum = Wild ? 2 : 0;
			minnum = num == 0 ? 0 : minnum;
			float max = Wild ? num + 2 : num;
			RepeatingAttack = (int)Random.Range(0, max);
		}
		else
		{
			--RepeatingAttack;
		}
	}

	public void DeathChecks()
	{
		FlameGround.SetActive(false);
		if (deathtime >= 3 && deathtrigger == 1)
		{
			Image img = GameObject.FindGameObjectWithTag("CanvasUI").transform.Find("BlackUnderUI").GetComponent<Image>();
			img.color = new Color(img.color.r, img.color.g, img.color.b, 0);
			DOTween.To(() => color.a, x => color.a = x, 1, 3);
			img.enabled = true;
			//DialogueManager.instance.TriggerDialogue("PreBossTut1", false);

			deathtrigger = 2;
		}
		else if (deathtime >= 6 && deathtrigger == 2)
		{
			Image img = GameObject.FindGameObjectWithTag("CanvasUI").transform.Find("BlackUnderUI").GetComponent<Image>();
			img.color = new Color(img.color.r, img.color.g, img.color.b, 1);
			DialogueManager.instance.TriggerDialogue("BossEndDialogue1", false);
			//GameInputManager.instance.DisablePlayerControls(); // Disable controls
			deathtrigger = 3;
		}
		else if (deathtime >= 13 && deathtrigger == 3)
		{
			DialogueManager.instance.TriggerDialogue("BossEndDialogue2", false);
			deathtrigger = 4;
		}
		else if (deathtime >= 21 && deathtrigger == 4)
		{
			DialogueManager.instance.TriggerDialogue("BossEndDialogue3", false);
			deathtrigger = 5;
		}
		else if (deathtime >= 27 && deathtrigger == 5)
		{
			DialogueManager.instance.TriggerDialogue("BossEndDialogue4", false);
			deathtrigger = 6;
		}
		else if (deathtime >= 33 && deathtrigger == 6)
		{
			DialogueManager.instance.TriggerDialogue("BossEndDialogue5", false);
			deathtrigger = 7;
		}
		else if (deathtime >= 33 && deathtrigger == 7)
		{
			DialogueManager.instance.TriggerDialogue("BossEndDialogue6", false);
			deathtrigger = 8;
		}
		else if (deathtime >= 39 && deathtrigger == 8)
		{
			DialogueManager.instance.TriggerDialogue("BossEndDialogue7", false);
			deathtrigger = 9;
		}
		else if (deathtime >= 45 && deathtrigger == 9)
		{
			DialogueManager.instance.TriggerDialogue("BossEndDialogue8", false);
			deathtrigger = 10;
		}
		else if (deathtime >= 51 && deathtrigger == 10)
		{
			DialogueManager.instance.TriggerDialogue("BossEndDialogue9", false);
			deathtrigger = 11;
		}
		else if (deathtime >= 57 && deathtrigger == 11)
		{
			DialogueManager.instance.TriggerDialogue("BossEndDialogue10", false);
			deathtrigger = 12;
		}
		else if (deathtime >= 61 && deathtrigger == 12)
		{
			LoadCredits();
			deathtrigger = 13;
		}

		if (deathtrigger > 1)
		{
			Image img = GameObject.FindGameObjectWithTag("CanvasUI").transform.Find("BlackUnderUI").GetComponent<Image>();
			img.color = color;
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

	// Returns waypoint closest to the target
	public Transform GetClosestWaypointToBoss()
	{
		if (!EmptyWaypoints()) return transform;

		Transform min = Waypoints[0];
		float mindist = Vector2.Distance(min.position, transform.position);
		foreach (Transform t in Waypoints)
		{
			float currdist = Vector2.Distance(t.position, transform.position);
			if (currdist < mindist)
			{
				min = t;
				mindist = currdist;
			}
		}
		return min;
	}

	public void KillNearbyEnemies()
	{
		try
		{
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
			foreach (GameObject e in enemies)
			{
				if (e.gameObject.activeSelf == true && e != this)
				{
					e.GetComponentInParent<Pawn>().GetHealth().SetValue(0);
				}
			}
			Invoke("DestroyPickups", .5f);
		}
		catch { }
	}

	void DestroyPickups()
	{
		StateManager.instance.DestroyPopUpsWithTag("Pickups");
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
		Vector3 preditedpath = ((targetmovement * time * (1 / Time.fixedDeltaTime)) * Mathf.Clamp(GetDistanceToTarget() / 20, .1f, 1));
		if (targetmovement.y == 0)
		{
			preditedpath.y = 0;
		}
		//Debug.Log("Z:" + zspeed);
		Vector3 position = Target.position + preditedpath;
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
		animating = true;
		float totaltime = time;
		float distance = Vector2.Distance(transform.position, targetPosition);
		float speed = distance / totaltime;
		for (float f = 0; f <= time; f += Time.fixedDeltaTime)
		{
			GetComponent<SphereCollider>().isTrigger = true;
			if (bossstate == State.Stunned)
			{
				transform.position += (startposition - targetposition).normalized * Time.fixedDeltaTime;
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
		AudioManager.instance.PlaySound("SFX_Fireball", false, 0);
	}

	void SpawnHeadbutt()
	{
		Vector2 angle = (Vector2)transform.rotation.eulerAngles;
		Quaternion rot = Quaternion.Euler(angle.x, angle.y, 0);
		GameObject hitbox = Instantiate(HeadbuttPrefab, transform.position, rot);
		hitbox.transform.position += Vector3.forward * 0; //Fix this later.
		hitbox.transform.parent = transform;
		hitbox.GetComponent<Hitbox>().LifeTime = .25f;
		GetComponent<SphereCollider>().isTrigger = true;
	}

	void HeadButt(Vector3 targetposition, float time, float delay)
	{
		headbuttdelay = delay;
		headbutttime = time;
		headbutttotaltime = time;
		headbutttarget = targetposition;
		headbuttstartposition = transform.position;
		headbuttdistance = Vector2.Distance(headbuttstartposition, headbutttarget);
	}

	void Pulse()
	{
		Vector2 angle = (Vector2)transform.rotation.eulerAngles;
		Quaternion rot = Quaternion.Euler(angle.x, angle.y, 0);
		GameObject hitbox = Instantiate(PulsePrefab, transform.position, rot);
		hitbox.transform.parent = transform;
		hitbox.GetComponent<Hitbox>().LifeTime = .1f;
		camera.transform.DOShakePosition(duration: .75f, strength: 1, vibrato: 5, randomness: 60, snapping: false, fadeOut: true);
		AudioManager.instance.PlaySound("SFX_FireExplosion", false, 0);
	}

	void BurnGround()
	{
		FlameGround.SetActive(true);
	}

	void WildVFXSpawn()
	{
		GameObject emit = Instantiate(WildVFX, HeadModel.transform.position, Quaternion.identity);
		//emit.transform.parent = HeadModel.transform;
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

	public void LoadCredits()
	{
		StateManager.instance.LoadCredits();
	}

	public void DisableUIExceptDialogue()
	{
		Transform t = GameObject.FindGameObjectWithTag("CanvasUI").transform;
		foreach (Transform child in t)
		{
			if (child.name != "BlackUnderUI")
			{
				child.gameObject.SetActive(false);
			}

		}
	}

	private Vector2 CastAtAngle(Vector2 position, Vector2 direction, float distance)
	{
		Vector2 result = position + direction.normalized * distance;
		return result;
	}

	protected override void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.black;
		GizmosExtra.DrawWireCircle(transform.position, Vector3.forward, TriggerRadius);
		base.OnDrawGizmosSelected();
	}
}
