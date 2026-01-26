using UnityEngine;

public enum InteractionType
{
    Press,
    Hold
}

public interface IInteractable
{
    InteractionType InteractType { get; }
    // 상호작용 함수
    public bool OnInteract(GameObject instigator);
    // 상호작용 대사 가져오기
    public string GetInteractPrompt();
}