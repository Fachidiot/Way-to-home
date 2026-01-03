using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float max = 3.5f;
    public float speed = 0.1f;

    float delta;

    private void Start()
    {
        Application.targetFrameRate = 60;
        delta = speed;
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x + delta, 0, transform.position.z);
        if (transform.position.x < -max)
            delta = speed;
        else if (transform.position.x > max)
            delta = -speed;
    }
}
