using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOE : MonoBehaviour
{
	private float startScale; 
	private float endScale;
	private float lerpScale=0;
	private float lerpMultiplier;
	private SphereCollider collider;

    public void setScaleTarget(float startSize, float blastRadius, float lerpMulti){
    	startScale = startSize;
    	endScale = blastRadius;
    	lerpMultiplier = lerpMulti;
    	collider = GetComponent<SphereCollider>();
    	//print("blastRadius: " + endScale);
    }
    void FixedUpdate(){
    		//print("fixed update on AOE running");
            if (collider!= null){
                if (collider.radius >= endScale){
                //print("destroying");
                Destroy(this.gameObject);
            } else{
                collider.radius = Mathf.Lerp(startScale, endScale,lerpScale);
                lerpScale += Time.deltaTime*lerpMultiplier;
            }
        }
    }
}
