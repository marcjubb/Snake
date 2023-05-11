using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    public GameObject[] items;
    private GameObject currentItem;
    public float spawnInterval = 0.4f;
    private List<GameObject> spawnedItems = new List<GameObject>();
    private float timeSinceLastSpawn;

    void Start()
    {
        SpawnItem();
    }

    void Update()
    {
        // If there is no current item, or the current item has been consumed, spawn a new one
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn >= spawnInterval)
        {
            if (currentItem == null)
            {
                SpawnItem();
            }
        }
        
    }

    void SpawnItem()
    {
        int itemIndex = UnityEngine.Random.Range(0, items.Length);
        Vector2 spawnPosition = RandomPosition();

        // Instantiate new item and set it as the current item
        currentItem = Instantiate(items[itemIndex], spawnPosition, Quaternion.identity);
        spawnedItems.Add(currentItem);
        timeSinceLastSpawn = 0f;
        spawnInterval = 2f;
    }

    Vector2 RandomPosition()
    {
        Vector2 randomPosition;
        do
        {
            int x = UnityEngine.Random.Range(-7, 6);
            int y = UnityEngine.Random.Range(-7, 6);
            randomPosition = new Vector2(x, y);
        } while (IsObstacleAtPosition(randomPosition));

        return randomPosition;
    }

    bool IsObstacleAtPosition(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }
        return false;
    }

    public void Reset()
    {
        // Destroy all spawned items
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }

        // Clear the list
        spawnedItems.Clear();
    }
}
