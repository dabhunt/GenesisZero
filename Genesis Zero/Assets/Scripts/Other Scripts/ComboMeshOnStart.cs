using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboMeshOnStart : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MeshCombiner combiner = gameObject.AddComponent<MeshCombiner>();
        combiner.CreateMultiMaterialMesh = true;
        combiner.DeactivateCombinedChildrenMeshRenderers = true;
        combiner.CombineMeshes(true);
        gameObject.GetComponent<MeshRenderer>().receiveShadows = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
