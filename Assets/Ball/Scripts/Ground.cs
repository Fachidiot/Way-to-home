using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private float maxAngle = 0.3f;
    [SerializeField] private float rotateSpeed = 5f;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Vector3 coinSpawnStartPosition;
    [SerializeField] private int coinSpawnPadding;
    [SerializeField] private Vector3 coinSpawnEndPosition;

    void Start()
    {
        var distance = (coinSpawnStartPosition - coinSpawnEndPosition).magnitude;
        for (int i = (int)coinSpawnStartPosition.x; i < distance; i += coinSpawnPadding)
        {
            Vector3 spawnPos = new Vector3(
                coinSpawnStartPosition.x + Random.Range(-3.0f, 3.0f),
                coinSpawnStartPosition.y,
                coinSpawnStartPosition.z + i);
            var go = Instantiate(coinPrefab, transform);
            go.transform.localPosition = spawnPos;
        }
    }

    void Update()
    {
        Rotate(-Input.GetAxis("Horizontal"));

        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (Input.mousePosition.x < Screen.width / 2)
        //         z = 1;
        //     else
        //         z = -1;
        //     transform.localEulerAngles += new Vector3(0, 0, z * Time.deltaTime);
        // }
    }

    void Rotate(float angle)
    {
        float localAngle = transform.localEulerAngles.z;
        if (localAngle > 180)
            localAngle -= 360;

        Debug.Log($"{angle} & {transform.localEulerAngles.z} -> {localAngle}");

        if (angle > 0 && localAngle > maxAngle)
            return; // 양수일때 -> z + @
        if (angle < 0 && localAngle < -maxAngle)
            return; // 음수일때 -> z - @

        transform.localEulerAngles = new Vector3(
            transform.localEulerAngles.x,
            transform.localEulerAngles.y,
            transform.localEulerAngles.z + (angle * Time.deltaTime * rotateSpeed));
    }
}
