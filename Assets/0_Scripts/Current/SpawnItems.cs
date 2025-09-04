using UnityEngine;
using System.Collections;
// Spawning of items
public class SpawnItems : MonoBehaviour {
    public GameObject spawnItemPrefab;
    public int spawnCount = 5;

    public Transform[] spawnAreas;

    public Transform spawnContainer; // Objects become child of this parent container

    public float spawnSequenceInterval = 1.5f;

    public int batch = 5;

    private void Start() {
        StartCoroutine(SpawnSequence(batch)); // ~25 capsules at start
    }
    void Update() {
        /*
        if (Input.GetKeyDown(KeyCode.L)) {
            SpawnItemsAtRandomLocations();
        }
        */
    }

    void SpawnItemsAtRandomLocations() {
        if (spawnItemPrefab == null || spawnAreas.Length == 0 || spawnContainer == null) {
            Debug.LogWarning("Spawn prefab, spawn areas, or spawn container not assigned!");
            return;
        }

        for (int i = 0; i < spawnCount; i++) {
            // Pick a random spawn area
            Transform area = spawnAreas[Random.Range(0, spawnAreas.Length)];

            // Get a random point inside that area's bounds
            Vector3 randomPos = GetRandomPointInArea(area);

            // Spawn the item as a child of spawnContainer
            Instantiate(spawnItemPrefab, randomPos, Quaternion.identity, spawnContainer);
        }
    }

    Vector3 GetRandomPointInArea(Transform area) {
        Vector3 center = area.position;
        Vector3 halfExtents = area.localScale * 0.5f;

        float x = Random.Range(-halfExtents.x, halfExtents.x);
        float z = Random.Range(-halfExtents.z, halfExtents.z);

        return new Vector3(center.x + x, center.y, center.z + z);
    }
    
    IEnumerator SpawnSequence(int batchCount) {
        for(int i = 0; i< batchCount; i++) {
            SpawnItemsAtRandomLocations();
            yield return new WaitForSeconds(spawnSequenceInterval);
        }
        
    }
}
