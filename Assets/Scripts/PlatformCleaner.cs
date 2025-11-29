using UnityEngine;

public class PlatformCleaner : MonoBehaviour
{
    public float destroyOffset = 15f;

    void Update()
    {
        if (transform.position.y < Camera.main.transform.position.y - destroyOffset)
        {
            Destroy(gameObject);
        }
    }
}