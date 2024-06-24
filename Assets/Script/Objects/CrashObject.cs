using UnityEngine;

public class CrashObject : MonoBehaviour
{
    public float deathTime = 5f;
    public float speed;
    private void Start()
    {
        Destroy(gameObject, deathTime);
    }
    
    public void SetSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}