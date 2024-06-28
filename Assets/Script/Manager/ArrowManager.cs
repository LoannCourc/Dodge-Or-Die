using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ArrowManager : MonoBehaviour
{
    public GameObject arrowPrefab; // Le prefab de la flèche
    public float arrowActiveTime = 1.0f; // Durée pendant laquelle la flèche reste active
    
    public GameObject[] arrows; // Tableau des flèches
    private int currentGridSize;
    private float tileSpacing;

    void Start()
    {
        // Initialisation des flèches
        InitializeArrows(3, ScaleManager.Instance.GetTileSpacing()); // Exemple de valeurs, à ajuster selon votre grille
    }

    public void InitializeArrows(int gridSize, float spacing)
    {
        currentGridSize = gridSize;
        tileSpacing = spacing;

        // Désactiver et détruire les anciennes flèches si nécessaire
        if (arrows != null)
        {
            foreach (GameObject arrow in arrows)
            {
                Destroy(arrow);
            }
        }

        // Calculer le nombre de flèches en fonction de la taille de la grille
        int arrowCount = gridSize * 4;
        arrows = new GameObject[arrowCount];

        // Instancier les flèches
        for (int i = 0; i < arrowCount; i++)
        {
            arrows[i] = Instantiate(arrowPrefab, transform);
            arrows[i].SetActive(false); // Désactiver initialement
            arrows[i].tag = "Arrow";
        }

        PositionArrows();
        // Après avoir instancié et positionné toutes les flèches
        ScaleManager.Instance.ScaleArrows(ScaleManager.Instance.CalculateScaleFactor(gridSize));

    }

    private void PositionArrows()
    {
        float halfSize = (currentGridSize - 1) * tileSpacing / 2;

        for (int i = 0; i < currentGridSize; i++)
        {
            // Haut
            arrows[i].transform.position = new Vector3(-halfSize + i * tileSpacing, halfSize + tileSpacing, 0);
            arrows[i].transform.rotation = Quaternion.Euler(0, 0, 180);

            // Bas
            arrows[i + currentGridSize].transform.position = new Vector3(-halfSize + i * tileSpacing, -halfSize - tileSpacing, 0);
            arrows[i + currentGridSize].transform.rotation = Quaternion.Euler(0, 0, 0);

            // Gauche
            arrows[i + currentGridSize * 2].transform.position = new Vector3(-halfSize - tileSpacing, halfSize - i * tileSpacing, 0);
            arrows[i + currentGridSize * 2].transform.rotation = Quaternion.Euler(0, 0, -90);

            // Droite
            arrows[i + currentGridSize * 3].transform.position = new Vector3(halfSize + tileSpacing, halfSize - i * tileSpacing, 0);
            arrows[i + currentGridSize * 3].transform.rotation = Quaternion.Euler(0, 0, 90);
        }
    }

    public void ActivateArrow(int arrowIndex)
    {
        if (arrowIndex >= 0 && arrowIndex < arrows.Length)
        {
            arrows[arrowIndex].SetActive(true);
            StartCoroutine(DeactivateArrowAfterTime(arrowIndex));
        }
        else
        {
            Debug.LogError("Invalid arrowIndex: " + arrowIndex);
        }
    }

    private IEnumerator DeactivateArrowAfterTime(int arrowIndex)
    {
        yield return new WaitForSeconds(arrowActiveTime);
        arrows[arrowIndex].SetActive(false);
    }

    // Méthode pour récupérer la position d'une flèche spécifique en fonction de l'index
    public Vector3 GetArrowPosition(int index)
    {
        if (index >= 0 && index < arrows.Length && arrows[index] != null)
        {
            return arrows[index].transform.position;
        }
        else
        {
            Debug.LogWarning("Arrow position not found for index: " + index);
            return Vector3.zero;
        }
    }
    public float GetArrowRotation(int index)
    {
        if (index >= 0 && index < arrows.Length && arrows[index] != null)
        {
            return arrows[index].transform.eulerAngles.z;
        }
        else
        {
            Debug.LogWarning("Arrow rotation not found for index: " + index);
            return 0f; // Retourne 0 degrés si la position n'est pas trouvée
        }
    }

}
