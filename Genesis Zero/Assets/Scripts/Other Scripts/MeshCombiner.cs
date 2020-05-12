using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshCombiner : MonoBehaviour
{
    private void Start()
    {
        //RecursiveMeshCombine(this.transform);
    }
    /*This function will recursively check all children of the parent object this is placed onto at the start of the game for identical materials
     * if the material is the same, it will combine meshes with the children
     * 
     */
    private void RecursiveMeshCombine(Transform t, Material mat)
        {

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 1;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }
        transform.GetComponent<MeshFilter>().mesh = new Mesh();
        transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;

        for (int o = 0; o < transform.childCount; o++)
        {
            GameObject.Destroy(transform.GetChild(o).gameObject);
        }

        foreach (Transform child in t)
            {
                if (child.gameObject.GetComponent<MeshRenderer>() != null)
                {
                    child.gameObject.GetComponent<MeshRenderer>().material = mat;
                }
                Renderer r = child.gameObject.GetComponent<Renderer>();
                if (child.childCount > 0)
                {
                    RecursiveMeshCombine(child, mat);
                }
            }
        }
    }
}
