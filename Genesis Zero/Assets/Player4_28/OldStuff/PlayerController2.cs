using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController2 : MonoBehaviour
{
    //public variables
    [Header("TransformComponents")]
    public float speed;
    public float jumpheight;
    public float rotationspeed;
    public LayerMask mouseaimmask;
    public float timescale = 1;


    [Header("Final IK Stuff")]
    public Transform aimobject;

    //public GameObject tempobject;
  ///  [Header("IK Stuff")]
   // public Vector3 lookatweights;
    public float maxarmdistance;
   // public float armheight;
  //  public Transform armrotcenter;
  //  public float DistanceToGround = 1f;
  //  public float footraycastlength = 3f;
  //  public LayerMask walkableLayer;
    public float MaskOffset = 0f;

  //  [Header("GunandHandIk")]
    //public Transform lefthandikttarget;
    //public Transform righthandikttarget;
   // public Transform gunobject;
    //public Vector2 gunoffset;

    //private variables
    bool isGrounded;
    float horizontalinput;
    Animator ani;
    Rigidbody rb;
    Vector2 targetvelocity;
    Vector3 aimvector;
    Camera main;
    GameObject mousecolliderlayer;
    Vector3 targetposition;
    public float GroundCheckDistance;
    public float WallCheckDistance;

    //animator specific
    float leftFoot_Weight;
    float rightFoot_Weight;
    Transform leftFoot;
    Transform rightFoot;
    Transform rightShoulder;
    Transform leftShoulder;
    Transform rightHand;
    Vector3 leftFoot_pos;
    Vector3 rightFoot_pos;
    Quaternion leftFoot_rot;
    Quaternion rightFoot_rot;



    // Start is called before the first frame update
    void Start()
    {
        // Cursor.visible = false;
        //intialize variables
        rb = GetComponent<Rigidbody>();
        ani = GetComponent<Animator>();
        main = Camera.main;
        initializemousecollider();

        rightHand= ani.GetBoneTransform(HumanBodyBones.RightHand);
        leftFoot = ani.GetBoneTransform(HumanBodyBones.LeftFoot);
        rightFoot = ani.GetBoneTransform(HumanBodyBones.RightFoot);
        leftShoulder = ani.GetBoneTransform(HumanBodyBones.LeftShoulder);
        rightShoulder = ani.GetBoneTransform(HumanBodyBones.RightShoulder);



    }

    void initializemousecollider() //creates a collider on runtime for the player aim to work with, parents it to the player as well
    {

        mousecolliderlayer = new GameObject("mousecolliderlayer");
        
        mousecolliderlayer.layer = Mathf.RoundToInt(Mathf.Log(mouseaimmask.value, 2)); //LayerMask.NameToLayer(mouseaimmask);

        //  mousecolliderlayer.transform.SetParent(transform);
        mousecolliderlayer.transform.position = transform.position;
        BoxCollider bc = mousecolliderlayer.AddComponent<BoxCollider>();
        bc.isTrigger = true;
        bc.size = new Vector3(500, 500);


    }

    public float originoffset;
    Vector3 originofray;
    void WallCheck()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        rayOrigin.y = rayOrigin.y + originoffset;

        originofray = rayOrigin;
        

        if (Physics.SphereCast(rayOrigin, WallCheckDistance, Vector3.right, out hit, WallCheckDistance) &&rb.velocity.x>=0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
        if (Physics.SphereCast(rayOrigin, WallCheckDistance, Vector3.left, out hit, WallCheckDistance) && rb.velocity.x <= 0)
        {
            rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(originofray, WallCheckDistance);
    }
    void GroundCheck()
    {
        RaycastHit hit;
        Vector3 rayOrigin = transform.position;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, GroundCheckDistance))
        {
            ani.ResetTrigger("JumpPressed");
            isGrounded = true;
            rb.rotation = Quaternion.Euler(hit.normal);
        }
        else isGrounded = false;
        ani.SetBool("isGrounded", isGrounded);
       

       
    }

    // Update is called once per frame
    void Update()
    {
       
      
        GetInputs();


    }

    private void FixedUpdate()
    {
        GroundCheck();
       
        // AlignRotation();

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            ani.SetTrigger("JumpPressed");
            //rb.AddForce(Vector2.up * jumpheight, ForceMode.Impulse);
            rb.velocity = new Vector3(rb.velocity.x, jumpheight, rb.velocity.z);
        }

        mousecolliderlayer.transform.position = transform.position;

        // tempobject.transform.position = aimvector;

        rb.velocity = new Vector3(horizontalinput * speed, rb.velocity.y, 0);

        if (FacingRight == 1)
        {
            float y = Mathf.Lerp(transform.rotation.eulerAngles.y, 90, rotationspeed);
            rb.rotation = Quaternion.Euler(new Vector3(0, y, 0));

        }
        if (FacingRight == 2)
        {
            float y = Mathf.Lerp(transform.rotation.eulerAngles.y, 270, rotationspeed);
            rb.rotation = Quaternion.Euler(new Vector3(0, y, 0));
            //rb.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(new Vector3(0, 270, 0)), rotationspeed);
        }
        WallCheck();
        // rb.MoveRotation(Quaternion.Euler(new Vector3(0, 90 * Mathf.Sign(aimvector.x - transform.position.x), 0)));
    }

    private int FacingRight
    {
        get
        {
            Debug.Log("aimvector-transform.position.x " + (aimvector.x - transform.position.x));
            // Debug.Log("Sign aimvector-transform= "+Mathf.Sign(aimvector.x - transform.position.x));
            float facing = (aimvector.x - transform.position.x);
            return facing >= .3f ? 1 : facing <= -.3f ? 2 : 3;
        }
    }//returns a value of 1-3 based on 

    private int FacingSign
    {
        get
        {
            Vector3 perp = Vector3.Cross(transform.forward, Vector3.forward);
            float dir = Vector3.Dot(perp, transform.up);
            return dir > 0f ? -1 : dir < 0f ? 1 : 0;
        }
    }


    



    void GetInputs()
    {
        horizontalinput = Input.GetAxis("Horizontal");
        ani.SetFloat("Speed", FacingSign * horizontalinput);




        //get mouseinput
        Ray ray = main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseaimmask))
        {
            // Debug.Log("hitpoint= " + hit.point);
            aimvector = hit.point;

        }
        Vector3 armpos;
        // Debug.Log(transform.position + (Vector3.up * 1.375f));
        armpos = transform.position + (Vector3.up * 1.375f);

       
        //targetposition = aimvector - armpos;
        targetposition = (((aimvector - armpos).normalized) * 10f)+armpos;

        aimobject.position = targetposition;//aimvector;
        //targetposition = armpos + Vector3.ClampMagnitude(targetposition, maxarmdistance);
        
        targetposition.z += MaskOffset;
        
        //tempobject.transform.position = targetposition;

    }



}
