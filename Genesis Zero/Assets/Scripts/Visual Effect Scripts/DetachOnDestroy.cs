using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetachOnDestroy : MonoBehaviour
{

    public GameObject Detachment;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnDestroy()
    {
        Detachment.transform.parent = null;
    }
}
