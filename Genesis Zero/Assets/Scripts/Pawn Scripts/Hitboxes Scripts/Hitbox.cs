﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Hitbox is a class that handles the damage detection for hurboxes.
 * Hitbox requires that the opposing target must be part of the side this hitbox damages.
 * For example, the player should be in the Allies layer and this hitbox should have HurtsAllies to hit the player.
 * The player must also contain a hurtbox with colliders in it's children
 */
public class Hitbox : MonoBehaviour
{
    private enum State { Active, Colliding, Deactive };
    State state;

    public float Damage;        // How much damage the hitbox can deal
    public bool Critical;
    public bool ignoreShieldHitbox;
    public bool HurtsAllies;    // Determines if this hitbox can harm the allies layer
    public bool HurtsEnemies;   // Determines if this hotbox can harm the enemies layer
    [Space]
    [Tooltip("If true, the hitbox ignores walls")]
    public bool Intangible;     // If True, then the hitbox can pass through walls

    [Tooltip("Number of times the hitbox can hit pawns. Cannot hit same target multiple times")]
    public int MaxHits = 1;     // Number of times the hitbox can hit something

    public float Knockbackforce;        // Force of knockback 
    public bool DirectionalKnockback;   // If true, the knockback direction is equal to the direction the hitbox is moving

    //[HideInInspector]
    public float StunTime = 0;
    [Tooltip("Percentage of Damage Reduction to ignore on the enemy it hits. Ex: .25 = 25% of damage reduction ignored")]
    public float IgnoredDamageReduction = 0;
    [HideInInspector]
    public float LifeTime = 99;
    public string SFXOnStart = "";
    public float SFX_vol = .65f;
    public delegate void OnKill();
    public OnKill killDelegate;

    [Tooltip("(X: Burntime, Y: Damage per second)")]
    public Vector2 Burn = new Vector2(0, 0);
    [Tooltip("X: Slowtime, Y: Multiplier (Ex: .7 = 30% speed reduction)")]
    public Vector2 Slow = new Vector2(0, 0);

    public Collider Collider;
    public Pawn Source;         // Source is a reference to the pawn that spawned this hitbox. Optional, used if things like critchance is calculated
    //public GameObject DamageNumberObject;
    public string hitEffectVFX = "VFX_BulletSparks";
    public float VFX_ScaleReduction = 15f;
    private List<GameObject> hittargets = new List<GameObject>();
    private Vector3 lastposition;
    private Vector3 spawnposition;
   
    private GameObject player;
    private void Awake()
    {
        if (GetComponent<Rigidbody>() == null)
        {
            gameObject.AddComponent<Rigidbody>();
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            spawnposition = transform.position;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (StateManager.instance.GameOver == true)
            return;
        if (Player.instance != null)
            player = Player.instance.gameObject;
        //colliders.Clear();
        state = State.Active;
        if (GetComponent<Collider>() == null)
        {
            if (GetComponent<Collider>())
            {
                Collider = GetComponent<Collider>();
            }
        }
        AudioManager.instance.PlayRandomSFXType(SFXOnStart, this.gameObject, .8f, 1.2f, SFX_vol);
        //AddCollliders(transform, colliders);
        lastposition = GetComponent<Collider>().transform.position;

        Collider[] collisions = null;
        if (GetComponent<SphereCollider>())
        {
            collisions = Physics.OverlapSphere(transform.position, GetComponent<SphereCollider>().radius);
        }
        else if (GetComponent<BoxCollider>())
        {
            collisions = Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().size, Quaternion.identity);
        }
        if (collisions != null)
        {
            foreach (Collider col in collisions)
            {
                CheckCollisions(col);
            }
        }
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

    }

    private void FixedUpdate()
    {
        if (state == State.Deactive)
        {
            try
            {
                Destroy(this.gameObject);
            }
            catch { }
        }
        else
        {
            if (LifeTime <= 0)
            {
                state = State.Deactive;
            }
            else
            {
                LifeTime -= Time.fixedDeltaTime;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Dect");
        CheckCollisions(other);
    }

    public bool CheckCollisions(Collider other)
    {
        if (state == State.Active)
        {
            state = State.Colliding;
        }

        bool hit = false;
        if (state == State.Colliding)
        {
            bool siblingcolliders = false;
            if (hittargets != null && other.transform.root.gameObject != null)
            {
                siblingcolliders = hittargets.Contains(other.transform.root.gameObject);
            }

            if (other != GetComponent<Collider>() && MaxHits > 0 && (other.GetComponentInParent<Hurtbox>() || other.GetComponent<Hurtbox>()) && CanDamage(other) && (other.GetComponentInParent<Pawn>() || other.GetComponent<Pawn>()) && !siblingcolliders && other.GetComponent<BodyPart>())
            {
                float finaldamage = Damage;
                Pawn p = other.GetComponentInParent<Pawn>();
                BodyPart bp = other.GetComponent<BodyPart>();
                bool special = (bp && bp.SpecialPart);
                if (special)
                {
                    Debug.Log("Special hit " + other.transform.root.name);
                    if (!ignoreShieldHitbox)
                        finaldamage *= bp.damagemultipler;
                }
                else
                {
                    //Debug.Log("Hit " + other.transform.root.name);
                }

                if (Source != null)
                {
                    if (Critical)
                    {
                        finaldamage *= Source.GetCritDamage().GetValue();
                    }
                }
                if (Slow.x > 0 && Slow.y > 0)
                {
                    p.Slow(Slow.x, Slow.y);
                }
                if (IgnoredDamageReduction > 0)
                {
                    //print("dmg reduc normal" + p.GetDamageReduction().GetValue());
                    print("dmg reduct befo:"+p.GetDamageReduction().GetValue()); //.3 instead
                    p.GetDamageReduction().AddRepeatingBonus(-IgnoredDamageReduction, -IgnoredDamageReduction, 5f, "IgnoredDamageReduction"+this.GetInstanceID());
                    p.UpdateStats();
                    print("dmg reduct afta:" + p.GetDamageReduction().GetValue()); //.3 instead
                }
                if (Knockbackforce > 0 && p.IsInvunerable() == false)
                {
                    if (DirectionalKnockback && Source != null)
                    {
                        p.KnockBack(transform.position - spawnposition, Knockbackforce);
                    }
                    else
                    {
                        //Debug.Log("Knockback! : " + Knockbackforce);
                        p.KnockBack(p.transform.position - transform.position, Knockbackforce);
                    }

                }
                if (StunTime > 0 && !p.IsStunImmune() && !p.IsStunned()) //only stun if the pawn is not stun immune not already stunned
                {
                    p.GetStunnedStatus().AddTime(StunTime);
                    print("stun sound");
                    AudioManager.instance.PlayRandomSFXType("Stunned", this.gameObject, .4f);
                }
				if(Source != p)
				{
					if (Burn.x > 0 && Burn.y > 0)
					{
						BurnCheck(p, other);
						p.Burn(Burn.x, Burn.y);
					}
				}

                float phealth = p.GetHealth().GetValue();
                float damagetaken = p.TakeDamage(finaldamage, Source);
                if (player != null)
                {
                    AudioManager.instance.PlayRandomSFXType("EnemyHit", this.gameObject, .8f, 1.2f, .9f);
                    player.GetComponent<Player>().GetComponent<UniqueEffects>().DamageGivenTrigger();
                }

                if (damagetaken >= phealth)
                    if (killDelegate != null) killDelegate();

                GameObject emit = VFXManager.instance.PlayEffect("DamageNumber", new Vector3(p.transform.position.x - 0f, transform.position.y + 1, p.transform.position.z - 4f));
				if (Vector3.Distance(emit.transform.position, p.transform.position) >= 3)
				{
					emit.transform.position = p.transform.position + new Vector3(0,1.5f,0);
				}
				emit.transform.Translate(new Vector2(Random.Range(-.5f,.5f), Random.Range(-.5f,.5f)));
				emit.GetComponent<DamageNumber>().SetNumber(damagetaken, Critical);
                if (Critical)
                {
                    emit.GetComponent<DamageNumber>().SetColor(new Color(1, 1, 0));
                }
                else if (special && bp.damagemultipler > 1)
                {
                    VFXManager.instance.PlayEffect("VFX_WeakPoint", this.transform.position);
                    if (AudioManager.instance != null && AudioManager.instance.IsPlaying("EnemyHit") < 1)
                        AudioManager.instance.PlayRandomSFXType("EnemyHit", this.gameObject, .4f);
                    if (player != null)
                        player.GetComponent<Player>().GetComponent<UniqueEffects>().WeakPointHit();
                    emit.GetComponent<DamageNumber>().SetColor(Color.red);
                }
                else if (special && bp.damagemultipler < 1)
                {
                    emit.GetComponent<DamageNumber>().SetColor(new Color(.25f, .25f, .25f));
                }

				bool invis = ((damagetaken == 0 && p.GetComponent<PlayerController>() && p.GetComponent<PlayerController>().IsRolling()) || p.IsDying());
				if (invis)
				{
					emit.GetComponent<DamageNumber>().SetColor(new Color(0, 0, 0, 0));
				}

                if (hitEffectVFX != "")
                {
                    GameObject vfx = VFXManager.instance.PlayEffect(hitEffectVFX, new Vector3(transform.position.x, transform.position.y, transform.position.z), 0f, Mathf.Clamp(damagetaken / VFX_ScaleReduction, .2f, 3.5f));
                }
               
                hittargets.Add(other.transform.root.gameObject);
                hit = true;
                --MaxHits;
            }
            else if (Intangible == false && other != GetComponent<Collider>() && !(other.GetComponentInParent<Hurtbox>() || other.GetComponent<Hurtbox>()) && !siblingcolliders && !other.isTrigger)
            {
                state = State.Deactive;
                return true;
            }
            else
            {
                //Debug.Log((other != GetComponent<Collider>()) + " " + (MaxHits > 0) + " " + (other.GetComponentInParent<Hurtbox>() || other.GetComponent<Hurtbox>()) + " " + CanDamage(other) + " " + (other.GetComponentInParent<Pawn>() || other.GetComponent<Pawn>()) + " " + !siblingcolliders +" "+ other.GetComponent<BodyPart>());
            }


        }

        if (MaxHits <= 0 && state != State.Deactive)
        {
            state = State.Deactive;
            return true;
        }
        else if (hit)
        {
            return true;
        }
        return false;
    }
     /* IMPORTANT FUNCTION, if hitbox is spawned from instantiate, this function should be called to initalize the hitbox with a source
     * If no source is set, offensive stats like critchance will not be applied to the hitbox. Returns true, if the hitbox crits
     */
	public bool InitializeHitbox(float damage, Pawn source)
	{
		this.Damage = damage;
		this.Source = source;
		spawnposition = source.transform.position;
		if (Random.Range(0, 100) < Source.GetCritChance().GetValue() * 100)
		{
			Critical = true;
			return true;
		}
		return false;
	}
	//this overload provides the option to initialize without inheriting crit,
	//which is useful for certain mods that are crit dependant and need to initialize a different bullet depending on if something crits
	public bool InitializeHitbox(float damage, Pawn source, bool inheritCrit)
	{
		this.Damage = damage;
		this.Source = source;
		spawnposition = source.transform.position;
		if (inheritCrit)
		{
			if ((Random.Range(0, 100) < Source.GetCritChance().GetValue() * 100))
			{
				Critical = true;
				return true;
			}
		}
		return false;
	}

	public bool CanDamage(Collider col)
	{
		//Debug.Log(col.gameObject.layer +" "+ LayerMask.NameToLayer("Allies"));
		if ((HurtsAllies && col.gameObject.layer == LayerMask.NameToLayer("Allies")) || (HurtsEnemies && col.gameObject.layer == LayerMask.NameToLayer("Enemies")))
		{
			//Debug.Log("CanHurt");
			return true;
		}
		return false;
	}

	public void SetStunTime(float time)
	{
		StunTime = time;
	}

	public void SetLifeTime(float time)
	{
		LifeTime = time;
	}

	public void BurnCheck(Pawn p, Collider other)
	{
		if (Player.instance.GetSkillStack("Pyromaniac") >= 1 && p.GetBurnImmunity() <= 0 && p.IsBurning())
		{
			float finaldamage = 0;
			if (Burn.x >= p.GetBurnStats().x && Burn.y >= p.GetBurnStats().y)
			{
				finaldamage = p.GetBurnStats().x * p.GetBurnStats().y * Player.instance.GetAbilityPowerAmount() / 16 * Player.instance.GetSkillStack("Pyromaniac");
			}
			else
			{
				finaldamage = Burn.x * Burn.y * Player.instance.GetAbilityPowerAmount() / 16 * Player.instance.GetSkillStack("Pyromaniac");
			}
			GameObject emit = Instantiate(Resources.Load<GameObject>("Hitboxes/PyroExplosion"), other.transform.position + new Vector3(0,1,0), Quaternion.identity);
			AudioManager.instance.PlaySound("SFX_FireExplosion");
			emit.GetComponent<Hitbox>().InitializeHitbox(finaldamage, p, false);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Collider col = GetComponent<Collider>();
		if (col.GetComponent<SphereCollider>() != null)
		{
			//Debug.Log("C");
			Gizmos.DrawWireSphere(col.transform.TransformPoint(col.GetComponent<SphereCollider>().center), col.GetComponent<SphereCollider>().radius);
		}

		if (col.GetComponent<BoxCollider>() != null)
		{
			//Debug.Log(col.GetComponent<BoxCollider>().center);
			Gizmos.DrawWireCube(col.transform.TransformPoint(col.GetComponent<BoxCollider>().center), col.GetComponent<BoxCollider>().size);
		}

	}

}
