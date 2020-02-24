using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Base Gun Settings")]
    public Transform firePoint;
    public GameObject basicProjectile;
    [Header("Bullet Modifiers")]
    [Header("1. Explosive Shot")]
	public GameObject explosiveProjectile;
	public float explosiveCoolDelay = 1.5f;
	public float SubsequentBlastRadiusBonus = .7f;
	[Header("2. Knockback")]
	public float knockBackPerStack = 5f;
	//subsequent meaning each additional duplicate of this mod gives you .7f bigger blast radius
    [Header("Crosshair Spread")]
    public float spreadSpeed = 5f;
    public float minXGap = 14f;
    public float maxXGap = 22f;

	//[Header("Explosive Shot")]
    private float spreadAngle;
    private OverHeat overheat;
    private Player player;
    private ExplosiveShot expShot;

    //Crosshair Variables
    private RectTransform[] crossArr;
    private RectTransform screenXhair;
    private float spread;
    private void Start() 
    {
        player = GetComponent<Player>();
        overheat = GetComponent<OverHeat>();
        screenXhair = GetComponent<PlayerController>().screenXhair;
        crossArr = new RectTransform[] {(RectTransform) screenXhair.Find("top"), 
                            (RectTransform) screenXhair.Find("bottom"), 
                            (RectTransform) screenXhair.Find("left"), 
                            (RectTransform) screenXhair.Find("right")};
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
        	int amount = player.GetSkillManager().GetSkillStack("Explosive Shot");
	    	//changeblastradius, the more of the skill you have the bigger it is.
	    	expShot.ModifyBlastRadius(amount*SubsequentBlastRadiusBonus);
        }
        //apply generic modifications
        instance = ModifyProjectile(instance);
        instance.transform.Rotate(Vector3.forward,Random.Range(-spreadAngle, spreadAngle),Space.World);
        instance.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue(), player);
    }

    private void FixedUpdate() 
    {
        UpdateCrosshairBloom();
    }
    public GameObject GetProjectile()
    {
    	//theoretically projectile effects should be additive if possible, though this may be unrealistic
    	// eventually this should be refactored so it does a skill check BEFORE getting projectiles, so that multishot and more creative changes can be made
    	if (player.HasSkill("Explosive Shot"))
        {
    		//only fire an explosive shot if the player has 0 heat
    		//to balance this the cooldelay is increased to 1.5x the base cool delay
    		//this will be true on the first shot and only on the first shot
    		if (overheat.GetHeat() <= overheat.GetHeatAddedPerShot())
    		{
    			return explosiveProjectile;
    		}
    		overheat.ModifyCoolDelay(explosiveCoolDelay);
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
    	//adds knockbackforce to the bullet equal to the amount of stacks the player has
    	hit.Knockbackforce += knockBackPerStack * player.GetSkillManager().GetSkillStack("Knockback");
    	return bullet;
    }
    private void UpdateCrosshairBloom()
    {
        Vector3[] vecs = {Vector3.up, Vector3.down, Vector3.left, Vector3.right};
        float diff = ((maxXGap - minXGap) * overheat.GetHeat()) / 100;
        if (overheat.GetHeat() == 0)
            return;
        if (GetComponent<PlayerController>().GetFireInput() > 0)
        {
            var tmpMax = minXGap + diff;
            spread = Mathf.Lerp(spread, tmpMax, spreadSpeed * Time.fixedDeltaTime);
        }
        else
        {
            var tmpMin = minXGap + diff;
            spread = Mathf.Lerp(spread, tmpMin, spreadSpeed * Time.fixedDeltaTime);
        }
        spread = Mathf.Clamp(spread, minXGap, maxXGap);
        for (int i = 0; i < crossArr.Length; i++)
            crossArr[i].transform.localPosition = vecs[i] * spread;
    }
}