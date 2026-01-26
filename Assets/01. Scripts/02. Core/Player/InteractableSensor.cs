using UnityEngine;

public class InteractableSensor : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0);
    [SerializeField] private float detectRadius = 3.0f;
    [SerializeField] private LayerMask interactLayer;
    [SerializeField] private float holdTimeout;
    [Space(10)]
    [SerializeField] private bool debug;

    private IInteractable currentInteractable;
    private IInteractable lastFixInteractable;
    private PlayerInputs inputs;
    private bool isInteractFocus = false;
    private float holdDelta;

    void Start()
    {
        inputs = GameManager.Instance.GetComponent<PlayerInputs>();
    }

    void Update()
    {
        DetectInteractables();
        HandleInput();
    }

    void HandleInput()
    {
        if (null == currentInteractable)
        {
            holdDelta = 0;
            return;
        }

        if (inputs.interact)
        {
            if (currentInteractable.InteractType == InteractionType.Press)
            {
                currentInteractable.OnInteract(gameObject);
                inputs.interact = false;
            }
            else
            {   // Hold 시 (대화및 컷씬용 Interactions)
                bool isHold = inputs.interactValue.IsPressed();
                if (isHold)
                {
                    holdDelta += Time.deltaTime;
                    if (holdDelta >= holdTimeout && !isInteractFocus)
                    {
                        isInteractFocus = currentInteractable.OnInteract(gameObject);
                        inputs.interact = false;
                        lastFixInteractable = currentInteractable;
                    }
                }
            }
        }
        else
            holdDelta = 0;

        if (inputs.escape && isInteractFocus)
        {   // Hold 시 (대화및 컷씬용 Interactions)
            if (null != lastFixInteractable)    // 플레이어가 움직이면서 Interact시 감지가 안되는 버그.
                isInteractFocus = lastFixInteractable.OnInteract(gameObject);
            inputs.escape = false;
        }

        if (inputs.interactFocus != isInteractFocus)
            inputs.interactFocus = isInteractFocus;
    }

    void DetectInteractables()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position + offset, detectRadius, interactLayer);

        IInteractable closestInteratable = null;
        float minDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            IInteractable interactable = collider.GetComponent<IInteractable>();

            if (null != interactable)
            {
                float dist = Vector3.Distance(transform.position + offset, collider.transform.position);
                if (minDistance > dist)
                {
                    minDistance = dist;
                    closestInteratable = interactable;
                }
            }
        }

        if (currentInteractable != closestInteratable)
        {
            currentInteractable = closestInteratable;
            UpdateInteractUI();
        }
        else
        {   // 같은 Interactable인 상황.
            if (isInteractFocus)
                UpdateInteractUI();

        }
    }

    void UpdateInteractUI()
    {
        if (null == currentInteractable || isInteractFocus)
        {
            UIManager.Instance.InGameUI.SetInteract("");
            return;
        }
        UIManager.Instance.InGameUI.SetInteract(currentInteractable.GetInteractPrompt());
        // Debug.Log($"[InteractableSensor] : UI표시 -> {currentInteractable.GetInteractPrompt()}");
    }

    void OnDrawGizmos()
    {
        if (!debug) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + offset, detectRadius);
    }
}
