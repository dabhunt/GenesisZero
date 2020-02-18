using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAimFootIK : MonoBehaviour
{
   
    Transform Chest;
    Transform RightShoulder;
    Transform LeftShoulder;
    private Transform Target;
    public Vector3 ChestOffset;
    public LayerMask LayerMask;
    public Quaternion OriginalChestRotation;
   
    [Range(0,1)]
    public float DistanceToGround;
   

    public Animator Anim;
    // Start is called before the first frame update
    void Start()
    {
        Target = new GameObject().transform;
        Chest = Anim.GetBoneTransform(HumanBodyBones.Chest);
        OriginalChestRotation = Chest.rotation;

     
        
    }
    private void Update()
    {
        Vector2 mouseposition = Input.mousePosition;
        Target.position = Camera.main.ScreenToWorldPoint(new Vector3(mouseposition.x,mouseposition.y,35));  //this is currently a hardcoded distance away from the camera but if we calculate the distance for z we could do animated movements in the z for the camera
       
    }


    private void LateUpdate()
    {


        if (Anim.GetBool("isRolling") == true)
        {
            Chest.rotation = OriginalChestRotation;
            Anim.SetLayerWeight(1, 0);
        }
        else
        {
            Anim.SetLayerWeight(1, 1);
            Chest.LookAt(Target);
            Chest.rotation = Chest.rotation * Quaternion.Euler(ChestOffset);

        }

    }

    private void OnAnimatorIK(int layerIndex)//it places the feet down 
    {
        if (Anim)
        {
            Anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            Anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);
            Anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            Anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

            RaycastHit hit;
            RaycastHit righthit;


            //left foot
            Ray ray = new Ray(Anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up,Vector3.down);

            if(Physics.Raycast(ray,out hit, DistanceToGround + 1f,LayerMask))
            {
                if (hit.transform.tag == "Ground")
                {
                    Vector3 footposition = hit.point;
                    footposition.y += DistanceToGround;
                    var forward = Anim.GetBoneTransform(HumanBodyBones.LeftFoot).forward;
                    Anim.SetIKPosition(AvatarIKGoal.LeftFoot, footposition);
                    Anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward,hit.normal));
                }
            }


            //right foot
             Ray rightray = new Ray(Anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);

            if (Physics.Raycast(rightray, out righthit, DistanceToGround + 1f, LayerMask))
            {
                if (righthit.transform.tag == "Ground")
                {
                    Vector3 footposition = righthit.point;
                    footposition.y += DistanceToGround;
                    var forward=Anim.GetBoneTransform(HumanBodyBones.RightFoot).forward;
                    Anim.SetIKPosition(AvatarIKGoal.RightFoot, footposition);
                    Anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forward, righthit.normal));
                }
            }


        }
    }

}
