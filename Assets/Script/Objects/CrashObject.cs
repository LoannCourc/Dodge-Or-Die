using System;
using UnityEngine;

public class CrashObject : MonoBehaviour
{
    public float speed;
    private bool isDestroyedNormally = true; // Marqueur pour indiquer si l'objet est détruit normalement

    private void Start()
    {
        Destroy(gameObject, speed);
    }

    private void OnDestroy()
    {
        if (isDestroyedNormally)
        {
            ScoreManager.Instance.IncreaseScore(1);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    // Méthode appelée lors d'une collision avec le joueur
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isDestroyedNormally = false; // Marquer que l'objet est désactivé par collision avec le joueur
        }
    }
    
}