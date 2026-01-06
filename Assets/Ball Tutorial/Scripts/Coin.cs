using Unity.InferenceEngine;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static int best = 0;
    public static int score = 0;
    [SerializeField] private float rotateSpeed = 30f;
    [SerializeField] private LayerMask layerName;

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    void Start()
    {
        if (Physics.CheckSphere(transform.position, 0.4f, LayerMask.GetMask("Default")))
        {
            Debug.Log($"destroy");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        transform.localEulerAngles = new Vector3
        (
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            transform.localEulerAngles.z + Time.deltaTime * rotateSpeed
        );
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"{LayerMask.NameToLayer("Player") == other.gameObject.layer}");
        if (LayerMask.NameToLayer("Player") == other.gameObject.layer)
        {
            score++;
            gameObject.SetActive(false);
        }
    }
}
