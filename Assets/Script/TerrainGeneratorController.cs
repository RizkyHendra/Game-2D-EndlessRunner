﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainGeneratorController : MonoBehaviour
{
    [Header("Template")]
    public List<TerrainTemplateController> terrainTemplates;
    public float terrainTemplateWidth;

    [Header("Force Early Template")]
    public List<TerrainTemplateController> earlyTerrainTemplater;

    [Header("Generator Area")]
    public Camera gameCamera;
    public float areaStartOffSet;
    public float areaEndOffset;

    private List<GameObject> spawnedTerrain;
    private float lastGeneretedPositionX;

    private float lastRemovedPositionX;
    private const float debugLineHeight= 10.0f;
    private Dictionary<string, List<GameObject>> pool;
    

    private void Start()
    {
        pool = new Dictionary<string, List<GameObject>>();
        spawnedTerrain = new List<GameObject>();
        lastGeneretedPositionX = GetHorizontalPositionStart();
        lastRemovedPositionX = lastGeneretedPositionX - terrainTemplateWidth;

        foreach (TerrainTemplateController terrain in earlyTerrainTemplater)
        {
            GenerateTerrain(lastGeneretedPositionX, terrain);
            lastGeneretedPositionX += terrainTemplateWidth;
        }
        
     
        while (lastGeneretedPositionX < GetHorizontalPositionEnd())
        {
            GenerateTerrain(lastGeneretedPositionX);
            lastGeneretedPositionX += terrainTemplateWidth;
        }
        
    }

    // Update is called once per frame
    private void Update()
    {
        while (lastGeneretedPositionX < GetHorizontalPositionEnd())
        {
            lastGeneretedPositionX += terrainTemplateWidth;
            GenerateTerrain(lastGeneretedPositionX);
            
        }


        while (lastRemovedPositionX + terrainTemplateWidth < GetHorizontalPositionStart())
        {
         
            RemoveTerrain(lastRemovedPositionX);
            lastRemovedPositionX += terrainTemplateWidth;

        }

    }

    private float GetHorizontalPositionStart()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(0f, 0f)).x + areaStartOffSet;
    }

    private float GetHorizontalPositionEnd()
    {
        return gameCamera.ViewportToWorldPoint(new Vector2(1f, 0f)).x + areaEndOffset;
    }

    private void GenerateTerrain(float posX, TerrainTemplateController forceTerrain = null)
    {
        GameObject newTerrain = null;
        if (forceTerrain == null)
        {
            newTerrain = GenerateFromPool(terrainTemplates[Random.Range(0, terrainTemplates.Count)].gameObject, transform);
        }
        else
        {
            newTerrain = GenerateFromPool(forceTerrain.gameObject, transform);
        }
        newTerrain.transform.position = new Vector2(posX, 0f);
        spawnedTerrain.Add(newTerrain);
    }
    private void RemoveTerrain(float posX)
    {
        GameObject terrainToRemove = null;
        foreach (GameObject item in spawnedTerrain)
        {
            if(item.transform.position.x == posX)
            {
                terrainToRemove = item;
                break;
            }
        }    

        if(terrainToRemove != null)
        {
            spawnedTerrain.Remove(terrainToRemove);
            ReturnToPool(terrainToRemove);
        }
    }

    private GameObject GenerateFromPool(GameObject item, Transform parent)
    {
        if (pool.ContainsKey(item.name))
        {
            if (pool[item.name].Count > 0)
            {
                GameObject newItemFromPool = pool[item.name][0];
                pool[item.name].Remove(newItemFromPool);
                newItemFromPool.SetActive(true);
                return newItemFromPool;
            }
        }
        else
        {
            pool.Add(item.name, new List<GameObject>());
        }
        
        GameObject newItem = Instantiate(item, parent);
        newItem.name = item.name;
        return newItem;
    }

    private void ReturnToPool(GameObject item)
    {
        if(!pool.ContainsKey(item.name))
        {
            Debug.LogError("INVALID POOL ITEM");
        }
        pool[item.name].Add(item);
        item.SetActive(false);
    }
    private void OnDrawGizmos()
    {
        Vector3 areaStartPosition = transform.position;
        Vector3 areaEndPosition = transform.position;

        areaStartPosition.x = GetHorizontalPositionStart();
        areaEndPosition.x = GetHorizontalPositionEnd();

        Debug.DrawLine(areaStartPosition + Vector3.up * debugLineHeight / 2, areaStartPosition + Vector3.down * debugLineHeight / 2, Color.red);
        Debug.DrawLine(areaEndPosition + Vector3.up * debugLineHeight / 2, areaEndPosition + Vector3.down * debugLineHeight / 2, Color.red);
    }
}
