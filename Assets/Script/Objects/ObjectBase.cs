using System;
using UnityEngine;

public class ObjectBase : MonoBehaviour
{
    public int damage = 1; // Taille de l'objet (1 par défaut)

    private bool _alreadyEnteredInAWall = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            if (!_alreadyEnteredInAWall)
            {
                _alreadyEnteredInAWall = true;
            }
            else
            {
                _alreadyEnteredInAWall = false;
                ScoreManager.Instance.IncreaseScore(1);
            }
        }
    }
}