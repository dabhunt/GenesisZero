using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Base Gun Settings")]
    public Transform firePoint;
    public GameObject basicProjectile;
    [Header("Bullet Modifiers")]
    [Header("1. PyroTechnics")]
    public GameObject explosiveProjectile;
    //extra seconds the player has to wait before
    public float explosiveCoolDelay = .5f;
    public float blastRadiusBonusPerStack = .7f;
    // each additional duplicate of this mod gives you .7f bigger blast radius
    [Header("2. Knockback")]
    public float knockBackPerStack = 1.5f;
    [Header("3. Peripheral Bullet")]
    public float minSpread = 10f;
    public float spreadMultiplier = .3f;
    [Header("4. Peircing Bullets")]
    public int piercesPerStack = 1;
    [Header("5. Ignition Bullets")]
    public float burnDamagePerStack = 2f;
    public float burnTime = 3f;
    [Header("6. Hardware Exploit")]
    //in seconds
    public float stunDuration = .4f;
    public float exploitCoolDelay = .6f;
    public Color stunBulletColor;
    //only reduces it's own, not others by .1
    public float reductionPerStack = .1f;
    [Header("Atom Splitter (Multishot)")]
    public float AS_heatMulti = .6f;


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
        GameObject instance = (GameObject) Instantiate(GetProjectile(), spawnpoint, firePoint.transform.rotation);
        expShot = instance.GetComponent<ExplosiveShot>();
        if (expShot != null)
        {
        	int amount = player.GetSkillStack("PyroTechnics");
	    	//changeblastradius, the more of the skill you have the bigger it is.
	    	expShot.ModifyBlastRadius(amount* blastRadiusBonusPerStack);
        }
        //apply generic modifications
        instance = ModifyProjectile(instance);
        bool inheritCrit = true;
        //if it's a crit before being initialized, that means a mod is calculating crit manually
        if (instance.GetComponent<Hitbox>().Critical)
            inheritCrit = false;
        instance.transform.Rotate(Vector3.forward,Random.Range(-spreadAngle, spreadAngle),Space.World);
       instance.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue(), player, inheritCrit);
        //add 1 to stacks, because Compound X applies like a secondary stack of Atom splitter
        int stacks = player.GetSkillStack("Compound X") + 1;
        bool right = controller.IsAimingRight();
        //if you have just atom splitter, it will spawn 1 bullet above and below your gun based on minSpread value
        if (ac.IsAbilityActive("Atom Splitter"))
        {
            float heatFromExtraBullets = 0; 
            for (int s = 0; s < stacks; s++)
            {
                for (int i = 0; i < 2; i++)
                {
                    //if it's divisible by 2, then reverse the value of the min offset to go below the gun instead of above
                    heatFromExtraBullets++;
                    //multiply by -1 to alternate placing extra bullets on top vs underneath cursor
                    if ((i + 1) % 2 == 0)
                        spreadAngle *= -1;
                    float angle = spreadAngle + minSpread*(spreadAngle*spreadMultiplier)*(s+1);
                    if (!right) { angle *= -1;}
                    //extra bullets have an individual chance to crit, and thus apply the pyrotechnics AOE
                    GameObject extraBullet = (GameObject)Instantiate(GetProjectile(), spawnpoint, instance.transform.rotation);
                    Hitbox hit = extraBullet.GetComponent<Hitbox>();
                    inheritCrit = true;
                    //if it's a crit before being initialized, that means a mod is calculating crit manually and that initializeHit should not do it
                    if (hit.Critical)
                        inheritCrit = false;
                    extraBullet.transform.Rotate(Vector3.forward, angle, Space.World);
                    //extraBullet.GetComponent<Hitbox>().MaxHits += 2;
                    hit.InitializeHitbox(player.GetDamage().GetValue(), player, inheritCrit);
                }
            }
            overheat.ModifyHeatPerShot(heatFromExtraBullets*AS_heatMulti);
        }
    }
    private void FixedUpdate() 
    {
        UpdateCrosshairBloom();
    }
    public GameObject GetProjectile()
    {
        if (player.HasSkill("PyroTechnics"))
        {
            //manually calculate crit for this mod, to find out if it should be instantiated
            if (Random.Range(0, 100) < player.GetCritChance().GetValue() * 100)
            {//crit!, meaning this bullet will also become an AOE explosive
                Hitbox hit = explosiveProjectile.GetComponent<Hitbox>();
                hit.Damage = player.GetDamage().GetValue();
                hit.Critical = true;
                return explosiveProjectile;
            }
    	}
    
        /*
    	else if()
        {
    		//more skill checks that change bullet
    	}
        */
    	//if script makes it to here, you have no skills to change your projectile
    	return basicProjectile;
    }
    //any effects that should apply to all bullets after they have been instantiated go here, such as knockback increasers for all bullets
    // effects put here will also apply to special bullets
    public GameObject ModifyProjectile(GameObject bullet)
    {	
    	Hitbox hit = bullet.GetComponent<Hitbox>();
    	//adds knockbackforce, burndamage, & piercing equal to the bullet equal to the amount of stacks the player has
    	hit.Knockbackforce += knockBackPerStack * player.GetSkillStack("Knockback");
        hit.MaxHits += piercesPerStack * player.GetSkillStack("Piercing Bullets");
        float bDmg = burnDamagePerStack * player.GetSkillStack("Ignition Bullets");
        hit.Burn = new Vector2(burnTime, bDmg);
        float totalCoolDelay = 0;
        int exploitStacks = player.GetSkillStack("Hardware Exploit");
        if (player.HasSkill("Hardware Exploit"))
        {
            //get the reduction amount, multiply by the amount of stacks
            float reduction = player.GetSkillStack("Hardware Exploit") * reductionPerStack;
            float delay = exploitCoolDelay - reduction;
            totalCoolDelay += exploitCoolDelay;
            //if heat is at 0, apply the stun
            if (overheat.GetHeat() <= overheat.GetHeatAddedPerShot())
            {
                bullet =VFXManager.instance.ChangeColor(bullet, stunBulletColor);
                hit.StunTime = stunDuration;
            }
           
        }
        //modifycooldelay needs a multiplier, so 1 + whatever delays there are
        overheat.ModifyCoolDelay(1+totalCoolDelay);
        return bullet;
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