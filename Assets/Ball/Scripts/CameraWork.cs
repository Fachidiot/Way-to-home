using UnityEngine;

public class CameraWork : MonoBehaviour
{
    [SerializeField] private float camSpeed = 5f;
    [SerializeField] private Transform ball;
    [SerializeField] private Vector3 offset;

    void FixedUpdate()
    {
        Vector3 newPos = new Vector3(ball.position.x, ball.position.y, ball.position.z) + offset;
        transform.position = Vector3.Lerp(newPos, transform.position, Time.deltaTime * camSpeed);
    }
}
