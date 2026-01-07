using UnityEngine;

public class Coin : MonoBehaviour
{
    public static int best = 0;
    public static int score = 0;
    public bool resetable = false;
    [SerializeField] private float rotateSpeed = 30f;
    [SerializeField] private LayerMask layerName;

    public Material normalMaterial;
    public Material specialMaterial;

    public bool debug = false;

    void OnDrawGizmos()
    {
        if (debug)
            Gizmos.DrawSphere(transform.position, 0.5f);
    }

    void ChangeMaterial()
    {
        if (resetable)
            GetComponent<MeshRenderer>().material = specialMaterial;
        else
            GetComponent<MeshRenderer>().material = normalMaterial;
    }

    void Start()
    {
        if (Physics.CheckSphere(transform.position, 0.4f, LayerMask.GetMask("Default")))
        {
            // Debug.Log($"맵과 충돌되는 코인 destroy");
            Destroy(gameObject);
        }

        transform.localEulerAngles = new Vector3
        (
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            transform.localEulerAngles.z + Random.Range(0f, 60f)
        );
    }

    void Update()
    {
        transform.localEulerAngles = new Vector3
        (
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            transform.localEulerAngles.z + Time.deltaTime * rotateSpeed * (resetable ? -2 : 1)
        );
        ChangeMaterial();
    }

    void OnTriggerEnter(Collider other)
    {
        // Debug.Log($"{LayerMask.NameToLayer("Player") == other.gameObject.layer}");
        if (LayerMask.NameToLayer("Player") == other.gameObject.layer)
        {
            score++;
            gameObject.SetActive(false);

            if (resetable)
                Ground.GroundInstance.ResetCoin();
        }
    }
}
