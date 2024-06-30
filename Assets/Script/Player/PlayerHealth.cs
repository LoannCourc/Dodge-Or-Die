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
        UpdateHeartsDisplay();
    }

    private void InitializeHeartAnimations()
    {
        foreach (GameObject heart in hearths)
        {
            heart.transform.DOScaleY(scaleHearthEffect, durationHearthEffect)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }

    private void UpdateHeartsDisplay()
    {
        for (int i = 0; i < hearths.Count; i++)
        {
            hearths[i].SetActive(i < _currentHealth);
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
    
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        _currentHealth = Mathf.Max(0, _currentHealth); // Assure that health does not go below zero
        UpdateHeartsDisplay();

        if (_mainCamera != null)
        {
            Vector3 originalPosition = _mainCamera.transform.position; // Sauvegarder la position originale
            _mainCamera.transform.DOShakePosition(0.5f, 0.5f, 20, 90, false, true)
                .OnComplete(() => _mainCamera.transform.position = originalPosition); // Réinitialiser la position après le shake
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
        else
        {
            AudioManager.Instance.PlaySound("HitSound");
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
        Time.timeScale = 0f;
    }

    public void Heal(int amount)
    {
        _currentHealth += amount;
        _currentHealth = Mathf.Min(_currentHealth, maxHealth); // Assure that health does not exceed maximum
        UpdateHeartsDisplay();
    }
}
