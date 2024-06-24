using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 5f;
    public float deathTime = 5f;
    private Vector3 direction;

    public void Start()
    {
        Destroy(gameObject, deathTime);
    }

    public void SetDirection(Vector3 newDirection)
    {
        // Ajuster la direction pour qu'elle soit sur l'axe X ou Y uniquement
        if (Mathf.Abs(newDirection.x) > Mathf.Abs(newDirection.y))
        {
            direction = new Vector3(Mathf.Sign(newDirection.x), 0, 0);
        }
        else
        {
            direction = new Vector3(0, Mathf.Sign(newDirection.y), 0);
        }
    }

    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
    
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}