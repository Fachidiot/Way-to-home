using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool crouch;
    public bool sprint;
    public bool interact;
    public bool interactFocus;
    [HideInInspector]
    public InputAction interactValue;
    public bool change;

    public bool escape;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    void OnEnable()
    {
        interactValue = GetComponent<PlayerInput>().actions.FindAction("Interact");
    }

#if ENABLE_INPUT_SYSTEM
    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        if (interactFocus) return;
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnCrouch(InputValue value)
    {
        if (interactFocus) return;
        CrouchInput();
    }

    public void OnInteract(InputValue value)
    {
        InteractInput(value.isPressed);
    }

    public void OnChange(InputValue value)
    {
        if (interactFocus) return;
        ChangeInput(value.isPressed);
    }

    public void OnEscape(InputValue value)
    {
        EscapeInput(value.isPressed);
    }
#endif


    public void MoveInput(Vector2 newMoveDirection)
    {
        if (interactFocus)
            move = Vector2.zero;
        else
            move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        if (crouch)
            crouch = false;
        else
            jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)    // Default
    {
        sprint = newSprintState;
        if (sprint)
            crouch = false;
    }

    public void InteractInput(bool newInteractState)
    {
        interact = newInteractState;
    }

    public void CrouchInput()   // Toggle
    {
        if (sprint)
            return;
        crouch = !crouch;
    }

    public void ChangeInput(bool newChangeState)
    {
        change = newChangeState;
    }

    private void EscapeInput(bool newEscapeState)
    {
        escape = newEscapeState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}
