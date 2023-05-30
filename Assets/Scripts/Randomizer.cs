using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Randomizer class responsible for enemy spawning and prop type selection
/// </summary>
public class Randomizer : MonoBehaviour
{
    public Material[] grassMaterials;
    public Material[] treeMaterials;
    public Mesh[] grassMeshes;
    public Mesh[] treeMeshes;
    public GameObject grassContainer;
    public GameObject treeContainer;
    public GameObject[] spawnPoints;
    public GameObject[] enemyPrefabs;

    void Start()
    {
        int randomIndex = Random.Range(0, grassMaterials.Length);
        
        MeshRenderer[] meshRenderers = grassContainer.GetComponentsInChildren<MeshRenderer>();
        MeshFilter[] meshFilters = grassContainer.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter filter in meshFilters)
        {
            filter.mesh = grassMeshes[randomIndex];
        }

        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.material = grassMaterials[randomIndex];
        } 

        meshRenderers = treeContainer.GetComponentsInChildren<MeshRenderer>();
        meshFilters = treeContainer.GetComponentsInChildren<MeshFilter>();

        foreach (MeshFilter filter in meshFilters)
        {
            filter.mesh = treeMeshes[randomIndex];
        } 

        foreach (MeshRenderer renderer in meshRenderers)
        {
            renderer.material = treeMaterials[randomIndex];
        }

        foreach (GameObject spawnPosition in spawnPoints)
        {
            int rand = Random.Range(0, enemyPrefabs.Length);
            GameObject enemyPrefab = enemyPrefabs[rand];

            Instantiate(enemyPrefab, spawnPosition.transform.position, Quaternion.identity);
        }

        foreach (GameObject enemyObject in enemyPrefabs)
        {
            Destroy(enemyObject);
        }
    }
}
