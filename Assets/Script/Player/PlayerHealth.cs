using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public List<GameObject> hearths;

    public GameObject deathPanel;
    public GameObject panelPause;
    
    [Header("Damage Effects")]
    public ParticleSystem damageParticles; 
    
    public float durationHearthEffect;
    public float scaleHearthEffect;
    
    private Camera _mainCamera;
    private int _currentHealth;

    private void Start()
    {
        _currentHealth = maxHealth;
        _mainCamera = Camera.main;
        InitializeHeartAnimations();
    }

    private void InitializeHeartAnimations()
    {
        foreach (GameObject heart in hearths)
        {
            // Appliquer une animation de yoyo qui modifie le scale en Y
            heart.transform.DOScaleY(scaleHearthEffect, durationHearthEffect) // Modifiez les valeurs selon l'effet désiré
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth < hearths.Count)
        {
            hearths[_currentHealth].SetActive(false);
        }
        
        if (_mainCamera != null)
        {
            _mainCamera.transform.DOShakePosition(0.5f, 0.5f, 20, 90, false, true);
        }
        if (damageParticles != null)
        {
            ParticleSystem particles = Instantiate(damageParticles, transform.position, Quaternion.identity, transform);
            Destroy(particles.gameObject, particles.main.duration);
        }
        
        if (_currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other != null)
        {
            int damage = other.gameObject.GetComponent<ObjectBase>().damage;
            TakeDamage(damage);
            AudioManager.Instance.PlaySound("HitSound");
            other.gameObject.SetActive(false);
        }
    }

    private IEnumerator Die()
    {
        AudioManager.Instance.PlaySound("PlayerDeath");
        AudioManager.Instance.StopSound("MainMusic");
        GameManager.Instance.EndGame();
        deathPanel.SetActive(true);
        panelPause.SetActive(false);
        gameObject.SetActive(false);
        yield return new WaitForSeconds(0.5f);
        Debug.Log("Time off");
        Time.timeScale = 0f;
    }
}
