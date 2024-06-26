using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public List<GameObject> hearths;

    public GameObject deathPanel;
    public GameObject panelPause;
    
    private int _currentHealth;

    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        hearths[_currentHealth].SetActive(false);
        
        // Vérifier si le joueur est mort
        if (_currentHealth <= 0)
        {
            Die();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // Vérifiez si le collider appartient à un objet avec lequel le joueur doit interagir
        if (other != null) // Assurez-vous que vos objets ont le tag "Obstacle"
        {
            // Récupérer les dégâts depuis l'objet s'il y a un script approprié
            int damage = other.gameObject.GetComponent<ObjectBase>().damage; // Exemple de dégâts par défaut

            // Appliquer des dégâts au joueur
            TakeDamage(damage);
            
            other.gameObject.SetActive(false);
        }
    }
    private void Die()
    {
        AudioManager.Instance.StopSound("MainMusic");
        // Logique de mort du joueur
        deathPanel.SetActive(true);
        panelPause.SetActive(false);
        Time.timeScale = 0f;
        // Ici, vous pouvez ajouter d'autres actions comme la réinitialisation du jeu, l'affichage d'un écran de fin, etc.
    }

    // Méthode pour restaurer la santé du joueur (par exemple, pour les soins)
    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
    }
}