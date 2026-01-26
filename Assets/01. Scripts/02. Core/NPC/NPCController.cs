using Data;
using NUnit.Framework;
using Unity.Cinemachine;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private NPCDialogue dialogue;
    [SerializeField] private CinemachineCamera cinemachineCamera;
    public InteractionType InteractType => InteractionType.Hold;

    private bool isFocus = false;

    public string GetInteractPrompt()
    {
        return dialogue.interactPrompt;
    }

    public bool OnInteract(GameObject instigator)
    {
        if (isFocus)
            EndConversation();
        else
            StartConversation(instigator);
        return isFocus;
    }

    void StartConversation(GameObject instigator)
    {
        isFocus = true;
        cinemachineCamera.Priority = 10;
        Debug.Log($"{dialogue.npcName} 와(과) 대화를 시작합니다.");

    }

    void EndConversation()
    {
        isFocus = false;
        cinemachineCamera.Priority = 0;
        Debug.Log($"{dialogue.npcName} 와(과) 대화를 종료합니다.");
    }
}