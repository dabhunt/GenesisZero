using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Base Gun Settings")]
    public Transform firePoint;
    public GameObject basicProjectile;
    public Color CritBulletColor = new Color(1, 1, .46f);
    [Header("Bullet Modifiers")]
    [Header("1. AOE on crit (pyrotechnics)")]
    public GameObject explosiveProjectile;
    //extra seconds the player has to wait before
    public float explosiveCoolDelay = .5f;
    //this bonus is a multiplier, meaning 20% more than base for each additional stack
    public float blastRadiusBonusPerStack = 1.2f;
    public float blastRadius = 3f;
    [Header("2. Knockback on crit")]
    public float knockBackPerStack = 1.5f;
    [Header("3. Peripheral Bullet")]
    public float minSpread = 10f;
    public float spreadMultiplier = .3f;
    [Header("4. Peircing Bullets")]
    public int piercesPerStack = 1;
    [Header("5. Ignition Bullets")]
    public float burnDamagePerStack = 4f;
    public float burnTime = 3f;
    [Header("6. Hardware Exploit")]
    //in seconds
    public float stunDuration = .4f;
    public float stunIncreasePerStack = .2f;
    public Color stunBulletColor;
    //only reduces it's own, not others by .1
    public float reductionPerStack = .1f;
    [Header("Atom Splitter (Multishot)")]
    public float AS_heatMulti = .7f;


    [Header("Crosshair Spread")]
    public float spreadSpeed = 5f;
    public float minXGap = 14f;
    public float maxXGap = 22f;

	//[Header("Explosive Shot")]
    private float spreadAngle;
    private OverHeat overheat;
    private Player player;
    private ExplosiveShot expShot;
    private PlayerController controller;
    AbilityCasting ac;

    //Crosshair Variables
    private RectTransform[] crossArr;
    private RectTransform screenXhair;
    private float spread;
    private GameObject vfx_MuzzleFlash;
    private void Start() 
    {
        player = GetComponent<Player>();
        overheat = GetComponent<OverHeat>();
        controller = player.GetComponent<PlayerController>();
        ac = player.GetComponent<AbilityCasting>();
        screenXhair = controller.screenXhair;
        crossArr = new RectTransform[] {(RectTransform) screenXhair.Find("top"), 
                            (RectTransform) screenXhair.Find("bottom"), 
                            (RectTransform) screenXhair.Find("left"), 
                            (RectTransform) screenXhair.Find("right")};
        UpdateCrosshairBloom();
    }

    public void Shoot()
    {
        
        spreadAngle = overheat.ShootBloom();
        //print("spreadAngle: "+spreadAngle);
        Vector3 spawnpoint = new Vector3(firePoint.transform.position.x, firePoint.transform.position.y, 0);
        //instantiate the players next bullet, passing in the crit variable to decide what type of bullet
        bool crit = Crit();
        vfx_MuzzleFlash = VFXManager.instance.PlayEffectReturn("VFX_MuzzleFlash", spawnpoint, 0, "");
        vfx_MuzzleFlash.transform.SetParent(firePoint);
        vfx_MuzzleFlash.transform.localPosition = Vector3.zero;
        GameObject instance = (GameObject) Instantiate(GetProjectile(crit), spawnpoint, Quaternion.identity);
        //sets the object to look towards where it should be firing
        instance.transform.LookAt(controller.worldXhair.transform);
        instance = ApplyModifiers(instance,crit);
        expShot = instance.GetComponent<ExplosiveShot>();
        if (expShot != null)
        {
            //changeblastradius, the more of the skill you have the bigger it is.
            // the function returns 1 if you have only 1 of that skill
            float multi = player.GetSkillManager().GetSkillStackAsMultiplier(("PyroTechnics"), blastRadiusBonusPerStack);
	    	expShot.ModifyBlastRadius(multi);
        }
        //means that instantiate hitbox will not calculate crit on it's own
        bool inheritCrit = false;
        instance.transform.Rotate(Vector3.forward,Random.Range(-spreadAngle, spreadAngle),Space.World);
        instance.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue(), player, inheritCrit);
        //add 1 to stacks, because Compound X applies like a secondary stack of Atom splitter
        int stacks = player.GetSkillStack("Compound X") + 1;
        bool right = controller.IsAimingRight();
        //if you have just atom splitter, it will spawn 1 bullet above and below your gun based on minSpread value
        if (ac.IsAbilityActive("Atom Splitter"))
        {
            float ExtraBullets = 0; 
            for (int s = 0; s < stacks; s++)
            {
                for (int i = 0; i < 2; i++)
                {
                    //if it's divisible by 2, then reverse the value of the min offset to go below the gun instead of above
                    ExtraBullets++;
                    //multiply by -1 to alternate placing extra bullets on top vs underneath cursor
                    if ((i + 1) % 2 == 0)
                        spreadAngle *= -1;
                    float angle = spreadAngle + minSpread*(spreadAngle*spreadMultiplier)*(s+1);
                    if (!right) { angle *= -1;}
                    //extra bullets have an individual chance to crit, and thus can apply the pyrotechnics AOE seperate
                    bool extraCrit = Crit();
                    GameObject extraBullet = (GameObject)Instantiate(GetProjectile(extraCrit), spawnpoint, instance.transform.rotation);
                    extraBullet = ApplyModifiers(extraBullet,extraCrit);
                    Hitbox hit = extraBullet.GetComponent<Hitbox>();
                    extraBullet.transform.Rotate(Vector3.forward, angle, Space.World);
                    //extraBullet.GetComponent<Hitbox>().MaxHits += 2;
                    hit.InitializeHitbox(player.GetDamage().GetValue()*.65f, player, inheritCrit);
                }
            }
            //adds extra heat for each of extra bullets fired
            float heatPerExtraBullets = ExtraBullets * AS_heatMulti * overheat.GetHeatAddedPerShot().GetValue();
            overheat.Increment(heatPerExtraBullets);
            overheat.GetDelayBeforeCooling().AddRepeatingBonus(.25f, .25f, .5f, "ExtraBulletCoolDelay");
        }

    }
    private void FixedUpdate() 
    {
        UpdateCrosshairBloom();
    }
    //calling this determines if the shot will be a crit ahead of time
    private bool Crit()
    {
        if (Random.Range(0, 100) < player.GetCritChance().GetValue() * 100)
        {
            return true;
        }
        return false;
    }
    //All generic bullet effects should go inside this function, including Crit proc / different projectiles being returned 
    public GameObject GetProjectile(bool crit)
    {
        GameObject projectile = basicProjectile;
        if (crit)
        {
            if (player.GetSkillStack("PyroTechnics") > 0)
                return explosiveProjectile;
        }
        return projectile;
    }
    public GameObject ApplyModifiers(GameObject projectile,bool crit)
    {
        Hitbox hit = projectile.GetComponent<Hitbox>();
        if (crit)
        {//Any effects that need to apply due to crit should go here
            //apply a burn to crits if you have ignition bullets
            hit.Critical = true;
            hit.Damage = player.GetDamage().GetValue();
            projectile = VFXManager.instance.ChangeColor(projectile, CritBulletColor);
            vfx_MuzzleFlash = VFXManager.instance.ChangeColor(vfx_MuzzleFlash, CritBulletColor);
            float burnDmg = burnDamagePerStack * player.GetSkillStack("Ignition Bullets");
            if (burnDmg > 0)
                hit.Burn = new Vector2(3, burnDmg);
            //apply stun on crit if you have it
            int exploitStacks = player.GetSkillStack("Hardware Exploit");
            if (exploitStacks > 0)
            {
                projectile = VFXManager.instance.ChangeColor(projectile, stunBulletColor);
                vfx_MuzzleFlash = VFXManager.instance.ChangeColor(vfx_MuzzleFlash, stunBulletColor);
                hit.StunTime = stunDuration + (1 - exploitStacks) * stunIncreasePerStack;
            }
            //apply Knockback on crit if you have it
            hit.Knockbackforce += knockBackPerStack * player.GetSkillStack("Knockback");
        }
        return projectile;
    }
    private void UpdateCrosshairBloom()
    {
        Vector3[] vecs = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        //float diff = ((maxXGap - minXGap) * overheat.GetHeat()) / 100;
        float target = maxXGap * ((overheat.GetHeat()+ minXGap)/ 100 );
        if (overheat.GetHeat() == 0)
            return;
        spread = Mathf.Lerp(spread, target, spreadSpeed * Time.fixedDeltaTime);
        spread = Mathf.Clamp(spread, minXGap, maxXGap);
        for (int i = 0; i < crossArr.Length; i++) { 
            crossArr[i].transform.localPosition = vecs[i] * spread;
        }
    }
}

//the cursors should lerp towards the maximum size not based on input, but based on heat value