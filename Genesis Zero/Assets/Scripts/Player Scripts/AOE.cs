using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : MonoBehaviour
{
	private float startScale; 
	private float endScale;
	private float lerpScale=0;
	private float lerpMultiplier;
	private SphereCollider collide;

    public void setScaleTarget(float startSize, float blastRadius, float lerpMulti){
    	startScale = startSize;
    	endScale = blastRadius;
    	lerpMultiplier = lerpMulti;
    	collide = GetComponent<SphereCollider>();
    	//print("blastRadius: " + endScale);
    }
    void FixedUpdate()
    {
    	//print("fixed update on AOE running");
        if (collide!= null){
            if (collide.radius >= endScale){
                print("destroying");
                Destroy(this.gameObject);
            } else{
                collide.radius = Mathf.Lerp(startScale, endScale,lerpScale);
                lerpScale += Time.deltaTime*lerpMultiplier;
            }
        }
    }
}
