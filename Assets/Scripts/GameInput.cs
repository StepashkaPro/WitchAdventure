using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    public static GameInput Instance { get; private set; }

    private PlayerInputActions plauerInputActions;

    public event EventHandler OnPlayerAttack;

    private void Awake() {
        Instance = this;
        
        plauerInputActions = new PlayerInputActions();
        plauerInputActions.Enable();

        plauerInputActions.Combat.Attack.started += PlayerAttack_started;
    }

    private void PlayerAttack_started(InputAction.CallbackContext odj) {
        OnPlayerAttack?.Invoke(this, EventArgs.Empty); // ? = < != null >
    }

    public UnityEngine.Vector2 GetMovementVector() {
        UnityEngine.Vector2 inputVector = plauerInputActions.Player.Move.ReadValue<UnityEngine.Vector2>();

        return inputVector;
    }

    public UnityEngine.Vector3 GetmousePosirion() {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        return mousePos;
    }
}
