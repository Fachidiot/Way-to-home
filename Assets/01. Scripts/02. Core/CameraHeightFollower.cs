using UnityEngine;

public class CameraHeightFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float followSpeed = 1.5f;

    float yPos;

    void Update()
    {
        if (target.position.y != yPos)
        {
            yPos = target.position.y;
        }

        if (transform.position.y != yPos)
        {
            Vector3 targetPos = new Vector3(transform.position.x, yPos, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSpeed);
        }
    }
}
