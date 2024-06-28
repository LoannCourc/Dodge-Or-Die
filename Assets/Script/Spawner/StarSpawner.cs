using UnityEngine;
using DG.Tweening;

public class StarSpawner : MonoBehaviour
{
    public GameObject objectPrefab; // Le prefab de l'objet à instancier
    public int numberOfObjects = 10; // Le nombre d'objets à instancier
    public Vector2 sizeRange = new Vector2(1f, 3f); // La plage de tailles (min et max)
    public Vector2 rotationRange = new Vector2(0f, 90f); // La plage de tailles (min et max)
    public float scaleDuration = 2f; // La durée de l'effet de scaling

    private void Start()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            Quaternion rotation = Quaternion.Euler(0, 0,Random.Range(rotationRange.x, rotationRange.y));
            
            GameObject obj = Instantiate(objectPrefab, GetRandomPosition(), rotation, transform);

            // Définir une taille aléatoire pour l'objet
            float randomScale = Random.Range(sizeRange.x, sizeRange.y);
            obj.transform.localScale = new Vector3(randomScale, randomScale, randomScale);

            // Appliquer l'effet de scaling continu
            ApplyScalingEffect(obj.transform, randomScale);
        }
    }

    private Vector3 GetRandomPosition()
    {
        // Obtenir les limites de l'écran
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        // Générer une position aléatoire dans les limites de l'écran
        float randomX = Random.Range(-screenBounds.x, screenBounds.x);
        float randomY = Random.Range(-screenBounds.y, screenBounds.y);

        return new Vector3(randomX, randomY, 0);
    }

    private void ApplyScalingEffect(Transform objTransform, float initialScale)
    {
        float targetScale = initialScale * 1.2f;

        // Appliquer l'effet de scaling continu
        objTransform.DOScale(targetScale, scaleDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
    }
}