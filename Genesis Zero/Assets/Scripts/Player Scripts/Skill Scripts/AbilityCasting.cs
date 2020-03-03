using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player), typeof(PlayerController))]
public class AbilityCasting : MonoBehaviour
{
    Player player;
    PlayerController PC;
    SkillManager skillmanager;

    [Header("Time Dilation Ability")]
    public float timeScale = .5f;
    public float effectDuration = 3.2f;

    private float AbitityCasttime1;
    private float AbilityCooldown1;
    private float TotalAbitityCasttime1;
    private float TotalAbilityCooldown1;

    private float AbitityCasttime2;
    private float AbilityCooldown2;
    private float TotalAbitityCasttime2;
    private float TotalAbilityCooldown2;
    public GameObject abilityCooldownPanel;
    private AbilityCD ui;
    private Vector2 aimDir;
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        skillmanager = player.GetSkillManager();
        PC = GetComponent<PlayerController>();
        ui = abilityCooldownPanel.GetComponent<AbilityCD>();
        aimDir = new Vector2(0, 0);
    }
    // Update is called once per frame
    void Update()
    {

        Vector3 pos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Camera.main.transform.position.z - transform.position.z));
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(pos);
        aimDir = GetComponent<PlayerController>().screenXhair.transform.position - transform.position;
        UpdateAbilities();

    }
    public void CastAbility1()
    {
        if (CanCastAbility1())
        {
            CastAbility(skillmanager.GetAbility1().name, 1);
        }
    }

    public void CastAbility2()
    {
        if (CanCastAbility2())
        {
            CastAbility(skillmanager.GetAbility2().name, 2);
        }
    }

    private void CastAbility(string name, int num)
    {
        switch (name)
        {
            case "Pulse Burst":
                InitializeAbility(3, 0, num);
                CastPulseBurst();
                break;
            case "Burst Charge":
                InitializeAbility(3, 0, num);
                CastBurstCharge();
                break;
            case "Overdrive":
                InitializeAbility(20, 0, num);
                CastOverdrive(num);
                break;
            case "Time Dilation":
                InitializeAbility(10, 0, num);
                CastSlowDown();
                break;
            case "Culling Blast":
                InitializeAbility(1, 0, num);
                CastSpartanLaser();
                break;
            case "Wound Sealant":
                InitializeAbility(100, 0, num);
                CastWoundSealant();
                break;
        }
        ui.Cast();
    }

    private bool CanCastAbility1()
    {
        return (AbitityCasttime1 <= 0 && AbilityCooldown1 <= 0 && skillmanager.GetAbility1() != null);
    }

    private bool CanCastAbility2()
    {
        return (AbitityCasttime2 <= 0 && AbilityCooldown2 <= 0 && skillmanager.GetAbility2() != null);
    }

    private void InitializeAbility(float cooldown, float casttime, int num)
    {
        if (num == 1)
        {
            InitializeAbility1(cooldown, casttime);
        }
        else
        {
            InitializeAbility2(cooldown, casttime);
        }
    }

    private void InitializeAbility1(float cooldown, float casttime)
    {
        AbilityCooldown1 = cooldown;
        TotalAbilityCooldown1 = cooldown;
        AbitityCasttime1 = casttime;
        TotalAbitityCasttime1 = casttime;
    }

    private void InitializeAbility2(float cooldown, float casttime)
    {
        AbilityCooldown2 = cooldown;
        TotalAbilityCooldown2 = cooldown;
        AbitityCasttime2 = casttime;
        TotalAbitityCasttime2 = casttime;
    }

    private void UpdateAbilities()
    {
        Mathf.Clamp(AbilityCooldown1 -= Time.deltaTime, 0, TotalAbilityCooldown1);
        Mathf.Clamp(AbilityCooldown2 -= Time.deltaTime, 0, TotalAbilityCooldown2);
        Mathf.Clamp(AbitityCasttime1 -= Time.deltaTime, 0, TotalAbitityCasttime1);
        Mathf.Clamp(AbitityCasttime2 -= Time.deltaTime, 0, TotalAbitityCasttime2);
    }
    public float GetAbilityCooldownRatio1()
    {
        return AbilityCooldown1 / TotalAbilityCooldown1;
    }

    public float GetAbilityCooldownRatio2()
    {
        return AbilityCooldown2 / TotalAbilityCooldown2;
    }

    public void ResetAbilityCooldown1()
    {
        AbilityCooldown1 = 0;
    }

    public void ResetAbilityCooldown2()
    {
        AbilityCooldown2 = 0;
    }

    private void CastPulseBurst()
    {
        player.GetComponent<PlayerController>().SetVertVel(0);
        player.KnockBackForced(-aimDir + Vector2.up, 25);
        GameObject hitbox = SpawnGameObject("PulseBurstHitbox", CastAtAngle(transform.position, aimDir, 1), Quaternion.identity);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(GetComponent<Player>().GetDamage().GetValue() / 4, GetComponent<Player>());
        hitbox.GetComponent<Hitbox>().SetStunTime(1);
        hitbox.GetComponent<Hitbox>().SetLifeTime(.1f);
    }

    private void CastBurstCharge()
    {
        player.GetComponent<PlayerController>().SetVertVel(0);
        player.KnockBackForced(aimDir + Vector2.up, 25);
        GameObject hitbox = SpawnGameObject("BurstChargeHitbox", transform.position, Quaternion.identity);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(GetComponent<Player>().GetDamage().GetValue() / 4, GetComponent<Player>());
        hitbox.transform.parent = transform;
        hitbox.GetComponent<Hitbox>().SetLifeTime(.5f);
    }
    private void CastOverdrive(int num)
    {
        if (num == 1) // Reset Ability2
        {
            ResetAbilityCooldown2();
        }
        else // Reset Ability1
        {
            ResetAbilityCooldown1();
        }
        player.GetSpeed().AddBonus(player.GetSpeed().GetBaseValue() / 2, 2); // 50% MS for 2 seconds
    }

    private void CastSlowDown()
    {
        StateManager.instance.ChangeTimeScale(timeScale, effectDuration);
    }

    private void CastSpartanLaser()
    {
        GameObject hitbox = SpawnGameObject("SpartanLaser", CastAtAngle(transform.position, aimDir, .5f), GetComponent<Gun>().firePoint.rotation);
        hitbox.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue() + 5, player);
    }

    private void CastWoundSealant()
    {
        SkillObject skill = player.GetSkillManager().GetSkillFromString("Wound Sealant");
        player.GetSkillManager().RemoveSkill(skill);
        player.Heal(55);
        ResetAbilityCooldown1();
    }

    private GameObject SpawnGameObject(string name, Vector2 position, Quaternion quat)
    {
        GameObject effect = Instantiate(Resources.Load<GameObject>("Hitboxes/" + name), position, quat);
        return effect;
    }
    private Vector2 CastAtAngle(Vector2 position, Vector2 direction, float distance)
    {
        Vector2 result = position + direction.normalized * distance;
        return result;
    }
}