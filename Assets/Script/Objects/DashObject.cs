using UnityEngine;
using System.Collections;
using DG.Tweening;

public class DashObject : MonoBehaviour
{
    private Vector3 targetPosition;
    private bool isMoving = false;
    private bool hasStopped = false;
    private Vector3 initialDirection;

    public float speed = 5f;        // Vitesse normale de déplacement
    public float chargeSpeed = 10f;     // Vitesse lors de la charge
    public float stopTime = 1f;         // Temps d'arrêt à la position de la flèche
    public float shakeStrength = 0.2f;  // Intensité du tremblement
    public float deathTime = 10f;       // Temps de vie de l'objet avant destruction
    public GameObject trail;

    private Vector3 direction;

    // Initialisez la direction où la bullet doit se déplacer
    public void Initialize(Vector3 dir)
    {
        direction = dir;
    }

    public Vector3 GetDirection()
    {
        return direction;
    }
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    void Start()
    {
        initialDirection = (targetPosition - transform.position).normalized;
        Destroy(gameObject, deathTime); // Détruire l'objet après un certain temps pour éviter les fuites de mémoire
    }

    void Update()
    {
        if (isMoving && !hasStopped)
        {
            MoveToTargetPosition();
        }
    }

    public void SetTargetPosition(Vector3 target)
    {
        targetPosition = target;
        isMoving = true;
    }
    public void SetRotation(float angle)
    {
        // Ajuste la rotation du sprite pour correspondre à l'angle de l'arrow
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void MoveToTargetPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            StartCoroutine(ChargeAndMove());
        }
    }

    private IEnumerator ChargeAndMove()
    {
        // Tremblement pendant la charge
        transform.DOShakePosition(stopTime, shakeStrength);

        yield return new WaitForSeconds(stopTime);
        trail.SetActive(true);
        hasStopped = true;
        AudioManager.Instance.PlaySound("DashSound");
        // Reprendre le mouvement dans la direction initiale à une vitesse accélérée
        Vector3 endPosition = targetPosition + initialDirection * 10f; // Modifier la distance selon les besoins
        float startTime = Time.time;
        float journeyLength = Vector3.Distance(transform.position, endPosition);
        float speed = chargeSpeed;

        while (transform.position != endPosition)
        {
            float distCovered = (Time.time - startTime) * speed;
            float fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, endPosition, fractionOfJourney);
            yield return null;
        }

        // Destruction de l'objet à la fin du mouvement
        Destroy(gameObject);
    }
}
