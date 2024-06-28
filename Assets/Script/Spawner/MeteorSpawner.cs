using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class MeteorSpawner : MonoBehaviour
{
    public GameObject meteorPrefab;
    public List<Sprite> meteorSprites;
    public float minSpeed = 3f;
    public float maxSpeed = 6f;
    public float minSpeedRotation = 3f;
    public float maxSpeedRotation = 6f;
    public float spawnInterval = 1f;
    public float meteorLifetime = 10f;

    // Array to store spawn points
    public Transform[] spawnPoints;

    private void Start()
    {
        // Start spawning meteors
        StartCoroutine(SpawnMeteors());
    }

    private IEnumerator SpawnMeteors()
    {
        while (true)
        {
            SpawnMeteor();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnMeteor()
    {
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        GameObject meteor = Instantiate(meteorPrefab, spawnPoint.position, Quaternion.identity, transform);

        // Fix Z position to 0
        meteor.transform.position = new Vector3(meteor.transform.position.x, meteor.transform.position.y, 0f);

        SpriteRenderer spriteRenderer = meteor.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && meteorSprites.Count > 0)
        {
            spriteRenderer.sprite = meteorSprites[Random.Range(0, meteorSprites.Count)];
        }

        Vector3 screenCenter = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, Camera.main.nearClipPlane));
        Vector3 direction = (screenCenter - spawnPoint.position).normalized;

        float randomSpeed = Random.Range(minSpeed, maxSpeed);
        float rotationSpeed = Random.Range(minSpeedRotation, maxSpeedRotation);

        Rigidbody rb = meteor.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = new Vector2(direction.x, direction.y) * randomSpeed;
            rb.angularVelocity = new Vector3(0f, 0f, rotationSpeed);
        }

        Destroy(meteor, meteorLifetime);
    }
}
