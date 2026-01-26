using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Playables;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameObject[] playerableCharacters;
    [SerializeField] private int playerIndex = 0;

    private PlayerInputs playerInputs;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerInputs = GetComponent<PlayerInputs>();

        if (playerableCharacters.Length > 0)
        {
            for (int i = 0; i < playerableCharacters.Length; ++i)
                ChangeToFollower(i);
            ChangeToMain(0);
            prevIndex = playerIndex;
        }
    }

    public void SetMainAnimation(bool isSprint, bool isCrouched)
    {
        foreach (var player in playerableCharacters)
        {
            if (player == playerableCharacters[playerIndex]) continue;
            player.GetComponent<AIFollowerMovement>().SetAnimation(isSprint, isCrouched);
        }
    }

    void ChangeToMain(int index)
    {
        playerIndex = index;
        playerableCharacters[index].GetComponent<PlayerMovement>().enabled = true;
        playerableCharacters[index].GetComponent<AIFollowerMovement>().enabled = false;
        playerableCharacters[index].GetComponent<NavMeshAgent>().enabled = false;
        playerableCharacters[index].transform.rotation = quaternion.identity;
        foreach (var player in playerableCharacters)
        {
            if (player == playerableCharacters[playerIndex]) continue;
            player.GetComponent<AIFollowerMovement>().ChangeTarget(playerableCharacters[index].transform);
        }
    }

    void ChangeToFollower(int index)
    {
        playerableCharacters[index].GetComponent<PlayerMovement>().enabled = false;
        playerableCharacters[index].GetComponent<AIFollowerMovement>().enabled = true;
        playerableCharacters[index].GetComponent<NavMeshAgent>().enabled = true;
        playerableCharacters[index].GetComponentInChildren<Animator>().transform.rotation = quaternion.identity;
    }

    int prevIndex = 0;
    void Update()
    {
        if (playerInputs.change)
        {
            if (playerIndex >= playerableCharacters.Length - 1)
                playerIndex = -1;
            ++playerIndex;
            playerInputs.change = false;
        }

        if (prevIndex != playerIndex)
        {
            for (int i = 0; i < playerableCharacters.Length; ++i)
                ChangeToFollower(i);
            ChangeToMain(playerIndex);
            prevIndex = playerIndex;
        }
    }
}
