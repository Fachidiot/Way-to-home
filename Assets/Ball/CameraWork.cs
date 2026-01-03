using UnityEngine;

public class CameraWork : MonoBehaviour
{
    public Transform ball;
    public Vector3 offset;

    void Update()
    {
        transform.position = new Vector3(ball.position.x, ball.position.y, ball.position.z) + offset;
    }
}
