using Data;
using UnityEngine;

public class TestItem : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemConfig itemConfig;
    public InteractionType InteractType => InteractionType.Press;

    public string GetInteractPrompt()
    {   // 추후 Localize작업 필요함.
        return $"{itemConfig.interactPrompt}";
    }

    public bool OnInteract(GameObject instigator)
    {
        Debug.Log($"{itemConfig.itemName} 을 주웠습니다.");

        Destroy(gameObject);
        return true;
    }
}