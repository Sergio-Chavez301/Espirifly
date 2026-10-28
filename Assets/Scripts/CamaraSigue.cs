using UnityEngine;

public class CamaraSigue : MonoBehaviour
{
    public Transform objetivo;
    public float suavizado = 6f;
    public float minX = 10f, maxX = 10f, minY = 5.625f, maxY = 34.375f;

    void LateUpdate()
    {
        if (objetivo == null) return;
        Vector3 p = transform.position;
        float x = Mathf.Clamp(objetivo.position.x, minX, maxX);
        float y = Mathf.Clamp(objetivo.position.y, minY, maxY);
        p.x = Mathf.Lerp(p.x, x, suavizado * Time.deltaTime);
        p.y = Mathf.Lerp(p.y, y, suavizado * Time.deltaTime);
        transform.position = p;
    }
}