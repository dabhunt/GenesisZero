using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomMeshCombiner : MonoBehaviour
{
    Dictionary<string, List<GameObject>> combineDict;
    Dictionary<string, Material> uniqueMaterials;
    private MeshFilter ParentMesh;
    private void Start()
    {
        uniqueMaterials = new Dictionary<string, Material>();
        //for each unique material, it will be stored inside the dictionary as it's name string, and it stores a list of combineI
        combineDict = new Dictionary<string, List<GameObject>>();
        if (this.gameObject.GetComponent<MeshFilter>() == null)
            this.gameObject.AddComponent<MeshFilter>();
        if (this.gameObject.GetComponent<MeshRenderer>() == null)
            this.gameObject.AddComponent<MeshRenderer>();
        //RecursiveMeshCombine(this.transform);
        ParentMesh = GetComponent<MeshFilter>();
        //AdvancedMerge();
        RecursiveMeshCombine(this.transform);
    }
    /*This function will recursively check all children of the parent object this is placed onto at the start of the game for identical materials
     * if the material is the same, it will combine meshes with the children
     */
    private void RecursiveMeshCombine(Transform t)
    {

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        
        for (int i = 0; i < meshFilters.Length; i++)
        {
            //Material parentMat = GetComponent<MeshRenderer>().material;
            MeshRenderer curRender = meshFilters[i].gameObject.GetComponent<MeshRenderer>();
            if (curRender != null && curRender.sharedMaterial != null)
            {
                //if our combineMesh dictionary already has this material stored
                if (combineDict.ContainsKey(curRender.sharedMaterial.name))
                {//get the gameObject list associated with that material, and Add the current renderer's gameObject
                    combineDict[curRender.sharedMaterial.name].Add(curRender.gameObject);
                }
                else //if the material does not yet exist in the dictionary
                {
                    List<GameObject> combineParent = new List<GameObject>();
                    combineParent.Add(meshFilters[i].gameObject);
                    uniqueMaterials.Add(curRender.sharedMaterial.name, curRender.sharedMaterial);
                    meshFilters[i].gameObject.SetActive(false);
                    combineDict.Add(curRender.sharedMaterial.name, combineParent);
                }
            }
        }
        //loop through all the items in the dictionary, and combine meshes for the list of objects using the same material
        foreach (var item in combineDict)
        {
            Material mat = uniqueMaterials[item.Key];
            MergeMesh(mat , item.Value.ToArray());
        }

        //CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        ////loop through all the gameobjects in the list associated with a specific material
        //for (int i = 0; i < item.Value.Count; i++)
        //{
        //    combine[i].mesh = item.Value[i].GetComponent<MeshFilter>().sharedMesh;
        //    combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
        //    if (i > 0)//destroy the meshfilter component on that gameobject
        //        Destroy(meshFilters[i]); 
        //}
        ////for the first item in the Gameobject list, we make it the "parent" holder of the combined mesh for that type of material
        //item.Value[0].GetComponent<MeshFilter>().mesh = new Mesh();
        //item.Value[0].GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        //item.Value[0].GetComponent<MeshRenderer>().sharedMaterial = item.Key;
        //item.Value[0].gameObject.SetActive(true);
        ////item.Value[0].transform.localScale = Vector3.one;
        ////item.Value[0].transform.rotation = Quaternion.identity;
        //transform.position = Vector3.zero;

        //for (int o = 0; o < transform.childCount; o++)
        //{
        //    GameObject.Destroy(transform.GetChild(o).gameObject);
        //}

        //foreach (Transform child in t)
        //    {
        //        if (child.gameObject.GetComponent<MeshRenderer>() != null)
        //        {
        //            child.gameObject.GetComponent<MeshRenderer>().material = mat;
        //        }
        //        Renderer r = child.gameObject.GetComponent<Renderer>();
        //        if (child.childCount > 0)
        //        {
        //            RecursiveMeshCombine(child, mat);
        //        }
        //    }

    }
    private void MergeMesh( Material mat, GameObject[] objs)
    {
        List<MeshFilter> meshFilters = new List<MeshFilter>();
        for (int i = 0; i < objs.Length; i++)
        {
            print(objs[i] +" has material: "+ mat);
            meshFilters.AddRange(objs[i].GetComponentsInChildren<MeshFilter>());
        }
        CombineInstance[] combine = new CombineInstance[meshFilters.Count];
        for (int i = 1; i < meshFilters.Count; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }
        objs[0].GetComponent<MeshFilter>().mesh = new Mesh();
        objs[0].GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        objs[0].gameObject.SetActive(true);
        //print("obj[0] =" + objs[0]);
        print(objs.Length + " meshes combined");
        //objs[0].transform.localScale = Vector3.one;
        //objs[0].transform.rotation = Quaternion.identity;
        //objs[0].transform.position = Vector3.zero;
        //starting at 1 so we leave 1 gameobject with the mesh not destroyed
        for (int i = 1; i < objs.Length; i++)
        {
            GameObject.Destroy(objs[i].gameObject.GetComponent<MeshRenderer>());
            GameObject.Destroy(objs[i].gameObject.GetComponent<MeshFilter>());
        }
    }
    public void AdvancedMerge()
    {
        // All our children (and us)
        MeshFilter[] filters = GetComponentsInChildren<MeshFilter>();

        // All the meshes in our children (just a big list)
        List<Material> materials = new List<Material>();
        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>(); // <-- you can optimize this
        foreach (MeshRenderer renderer in renderers)
        {
            if (renderer.transform == transform)
                continue;
            Material[] localMats = renderer.sharedMaterials;
            foreach (Material localMat in localMats)
                if (!materials.Contains(localMat))
                    materials.Add(localMat);
        }

        // Each material will have a mesh for it.
        List<Mesh> submeshes = new List<Mesh>();
        foreach (Material material in materials)
        {
            // Make a combiner for each (sub)mesh that is mapped to the right material.
            List<CombineInstance> combiners = new List<CombineInstance>();
            foreach (MeshFilter filter in filters)
            {
                if (filter.transform == transform) continue;
                // The filter doesn't know what materials are involved, get the renderer.
                MeshRenderer renderer = filter.GetComponent<MeshRenderer>();  // <-- (Easy optimization is possible here, give it a try!)
                if (renderer == null)
                {
                    Debug.LogError(filter.name + " has no MeshRenderer");
                    continue;
                }

                // Let's see if their materials are the one we want right now.
                Material[] localMaterials = renderer.sharedMaterials;
                for (int materialIndex = 0; materialIndex < localMaterials.Length; materialIndex++)
                {
                    if (localMaterials[materialIndex] != material)
                        continue;
                    // This submesh is the material we're looking for right now.
                    CombineInstance ci = new CombineInstance();
                    ci.mesh = filter.sharedMesh;
                    ci.subMeshIndex = materialIndex;
                    ci.transform = Matrix4x4.identity;
                    combiners.Add(ci);
                }
            }
            // Flatten into a single mesh.
            Mesh mesh = new Mesh();
            mesh.CombineMeshes(combiners.ToArray(), true);
            submeshes.Add(mesh);
        }

        // The final mesh: combine all the material-specific meshes as independent submeshes.
        List<CombineInstance> finalCombiners = new List<CombineInstance>();
        foreach (Mesh mesh in submeshes)
        {
            CombineInstance ci = new CombineInstance();
            ci.mesh = mesh;
            ci.subMeshIndex = 0;
            ci.transform = Matrix4x4.identity;
            finalCombiners.Add(ci);
        }
        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(finalCombiners.ToArray(), false);
        ParentMesh.sharedMesh = finalMesh;

    }
}
