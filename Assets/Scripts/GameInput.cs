using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour {
    
    // создаем событие
    public event EventHandler OnInteractAction; 
    public event EventHandler OnInteractAlternateAction;

    private PlayerInputActions playerInputActions;

    private void Awake() {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.Enable();

        playerInputActions.Player.Interacts.performed += Interact_performed;
        playerInputActions.Player.InteractsAlternate.performed += InteractAlternate_performed;
    }

    private void Interact_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        // есть ли слушатели у события (по сути проверка OnInteractAction != null)
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    private void InteractAlternate_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        // есть ли слушатели у события (по сути проверка OnInteractAction != null)
        OnInteractAlternateAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        // Vector2 - двумерный вектор (x, y) - не 3-х мерный, потому что хоть игра и 3д, однако прыжка в ней не будет,
        // то есть перемещение только по двум осям - x и y
        // Player - action map, Move - action
        Vector2 inputVector =  playerInputActions.Player.Move.ReadValue<Vector2>(); 

        // это нужно для того, чтобы по диагонали перс двигался с той же скоростью, что и по прямой
        inputVector = inputVector.normalized; 

        return inputVector;
    }
}
