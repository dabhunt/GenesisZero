﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboMeshOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    public bool MultiMaterial = true;
    void Start()
    {
        if (GetComponent<MeshCombiner>() == null)
        {
            MeshCombiner combiner = gameObject.AddComponent<MeshCombiner>();
            combiner.CreateMultiMaterialMesh = MultiMaterial;
            combiner.DestroyCombinedChildren = true;
            combiner.CombineMeshes(true);
            gameObject.GetComponent<MeshRenderer>().receiveShadows = false;
        }
    }
}
