using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [Header("Base Gun Settings")]
    public Transform firePoint;
    public GameObject basicProjectile;
    [Header("Explosive Shot")]
	public GameObject explosiveProjectile;
	public float explosiveCoolDelay = 1.5f;
	//subsequent meaning each additional duplicate of this mod gives you .7f bigger blast radius
	public float SubsequentBlastRadiusBonus = .7f;
	//[Header("Explosive Shot")]
    private float spreadAngle;
    private OverHeat overheat;
    private Player player;
    private ExplosiveShot expShot;
    private void Start() 
    {
        player = GetComponent<Player>();
        overheat = GetComponent<OverHeat>();
       
    }
    public void Shoot()
    {   
        spreadAngle = overheat.CalculateBloom();
        //print("spreadAngle: "+spreadAngle);
        Vector3 spawnpoint = new Vector3(firePoint.transform.position.x, firePoint.transform.position.y, 0);
        GameObject instance = (GameObject) Instantiate(GetProjectile(), spawnpoint, firePoint.transform.rotation);
        expShot = instance.GetComponent<ExplosiveShot>();
        if (expShot != null){
        	ModifyProjectile();
        }
        instance.transform.Rotate(Vector3.forward,Random.Range(-spreadAngle, spreadAngle), Space.World);
        instance.GetComponent<Hitbox>().InitializeHitbox(player.GetDamage().GetValue(), player);
    }
    public GameObject GetProjectile()
    {
    	//theoretically projectile effects should be additive if possible, though this may be unrealistic
    	// eventually this should be refactored so it does a skill check BEFORE getting projectiles, so that multishot and more creative changes can be made
    	if(player.HasSkill("Explosive Shot")){
    		//only fire an explosive shot if the player has 0 heat
    		//to balance this the cooldelay is increased to 1.5x the base cool delay
    		//this will be true on the first shot and only on the first shot
    		if (overheat.GetHeat() <= overheat.GetHeatAddedPerShot())
    		{
    			return explosiveProjectile;
    		}
    		overheat.ModifyCoolDelay(explosiveCoolDelay);
    	}
    	// else if(){
    		//more skill checks that change bullet
    	// }
    	// if script makes it to here, you have no skills to change your projectile
    	return basicProjectile;
    }
    public void ModifyProjectile()
    {
    	int amount = player.GetSkillManager().GetSkillStack("Explosive Shot");
    	//changeblastradius, the more of the skill you have the bigger it is.
    	expShot.ModifyBlastRadius(amount*SubsequentBlastRadiusBonus);
    }
}