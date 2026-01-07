using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public static Ground GroundInstance;
    public bool isBall;
    [SerializeField] private float maxAngle = 0.3f;
    [SerializeField] private float rotateSpeed = 5f;

    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Vector3 coinSpawnStartPosition;
    [SerializeField] private float coinSpawnPadding;
    [SerializeField] private Vector3 coinSpawnEndPosition;

    [SerializeField] private Transform leftPos;
    Vector3 leftrot;
    [SerializeField] private Transform rightPos;
    Vector3 rightrot;

    [SerializeField] private int specialCoinCount = 3;
    int[] resetCoins;

    void Start()
    {
        GroundInstance = this;
        leftrot = leftPos.localEulerAngles;
        rightrot = rightPos.localEulerAngles;
        resetCoins = new int[specialCoinCount];
        CreateCoin();
    }

    private List<GameObject> coins;

    List<GameObject> temp = new List<GameObject>();
    public void ResetCoin()
    {
        if (temp != coins)
        {
            foreach (var go in coins)
            {
                if (go)
                    temp.Add(go);
            }
            coins = temp;
        }

        if (coins.Count > 0)
        {
            for (int i = 0; i < resetCoins.Length; i++)
                coins[resetCoins[i]].GetComponent<Coin>().resetable = false;

            for (int i = 0; i < resetCoins.Length; i++)
            {
                resetCoins[i] = RandomInt(0, coins.Count, specialCoinCount);
                coins[resetCoins[i]].GetComponent<Coin>().resetable = true;
            }

            foreach (var go in coins)
            {
                if (!go.activeSelf)
                    go.SetActive(true);
            }
        }
    }

    int[] randList;
    int index = 0;
    int RandomInt(int min, int max, int length)
    {
        if (randList == null || randList.Length != length || index + 1 >= length)
        {
            index = 0;
            randList = new int[length];
        }

        int value = Random.Range(min, max);
        while (true)
        {
            if (randList.Contains(value))
                value = Random.Range(min, max);
            else
            {
                randList[index++] = value;
                break;
            }
        }
        return value;
    }

    public void CreateCoin()
    {
        coins = new List<GameObject>();
        for (float x = coinSpawnStartPosition.x; x < coinSpawnEndPosition.x; x += coinSpawnPadding)
        {
            for (float z = coinSpawnStartPosition.z; z < coinSpawnEndPosition.z; z += coinSpawnPadding)
            {
                Vector3 spawnPos = new Vector3(x, coinSpawnStartPosition.y, z);
                var go = Instantiate(coinPrefab, transform);
                go.transform.localPosition = spawnPos;
                coins.Add(go);
            }
        }
    }

    void Update()
    {
        if (isBall)
            Rotate(-Input.GetAxis("Horizontal"));
        else
            Pinball(-Input.GetAxis("Horizontal"));

        // if (Input.GetMouseButtonDown(0))
        // {
        //     if (Input.mousePosition.x < Screen.width / 2)
        //         z = 1;
        //     else
        //         z = -1;
        //     transform.localEulerAngles += new Vector3(0, 0, z * Time.deltaTime);
        // }
    }

    void Pinball(float angle)
    {
        if (angle > 0 && leftPos.localEulerAngles.y <= 350)
        {
            leftPos.localEulerAngles = new Vector3(
                leftPos.localEulerAngles.x,
                leftPos.localEulerAngles.y - Time.deltaTime * 200,
                leftPos.localEulerAngles.z);
            leftPos.GetComponentInChildren<Obstacle>().collide = true;
        }
        else
        {
            leftPos.localEulerAngles = leftrot;
            leftPos.GetComponentInChildren<Obstacle>().collide = false;
        }

        // Debug.Log($"angle : {angle}\nLeft : {leftPos.localEulerAngles.y} / Right : {rightPos.localEulerAngles.y}");
        if (angle < 0 && rightPos.localEulerAngles.y - 360 < 0 && rightPos.localEulerAngles.y < 350)
        {
            rightPos.localEulerAngles = new Vector3(
                rightPos.localEulerAngles.x,
                rightPos.localEulerAngles.y + Time.deltaTime * 200,
                rightPos.localEulerAngles.z
            );
            rightPos.GetComponentInChildren<Obstacle>().collide = true;
        }
        else
        {
            rightPos.localEulerAngles = rightrot;
            rightPos.GetComponentInChildren<Obstacle>().collide = false;
        }
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
