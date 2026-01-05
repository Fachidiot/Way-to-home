using UnityEngine;

public class Coin : MonoBehaviour
{
    public static int score = 0;
    [SerializeField] private float rotateSpeed = 30f;
    [SerializeField] private LayerMask layerName;

    void Update()
    {
        transform.localEulerAngles = new Vector3
        (
            transform.localEulerAngles.x,
            transform.localEulerAngles.y + Time.deltaTime * rotateSpeed,
            transform.localEulerAngles.z
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (0 != other.gameObject.layer.CompareTo(layerName))
        {
            score++;
            Destroy(gameObject);
        }
    }
}
