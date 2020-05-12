using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtMouse : MonoBehaviour
{
    Camera main;
    private Vector3 aimvector;
    public LayerMask mouseaimmask;
    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
    }

    private void LateUpdate()
    {
      

        Ray ray = main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mouseaimmask))
        {
            // Debug.Log("hitpoint= " + hit.point);
            aimvector = hit.point;

        }

        // Vector3 difference = aimvector - transform.position;
        // float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        // transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        transform.LookAt(aimvector);
        //transform.LookAt(new Vector3(transform.x, transform.position.y, aimvector.z));

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
