using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour, IKitchenObjectParent {
    // { get; set; } - автоматически создаются геттер и сеттер (можно сделать их приватными)
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    // чтобы передать некоторые параметры при вызове ивента, нужно расширить базовый класс EventArgs
    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }
    
    // выделяем настройку скорости перемещения в отдельную переменную и делаем ее приватной  и сериалезируемой (либо делать public, но лучше так)
    // для того, чтобы ее можно было изменять в самом редакторе (выбрать game object). можно это делать во время работы игры
    [SerializeField] private float moveSpeed = 7f; 
    [SerializeField] private float rotateSpeed = 10f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;  // сначала переменную нужно создать в редакторе или нет??? или сразу в коде???
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    // реализация паттерна singleton 
    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one player instance!");
        }
        Instance = this;
    }

    // начинать прослушивание событий нужно в Start
    private void Start() {
        gameInput.OnInteractAction += GameInput_OnInteractionAction;
        gameInput.OnInteractAlternateAction += GameInput_OnInteractionAlternateAction;
    }

    // прослушиваем событие
    private void GameInput_OnInteractionAction(object sender, System.EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnInteractionAlternateAction(object sender, System.EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    // каждое обновление кадра?
    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        // заново создаем эти переменные, так как в HandleMovement мы вычисляем положение для движения
        // и постоянно у нас меняется moveDirection + нам нужно просто знать направление в конкретный
        // момент времени
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        float interactDistance = 2f;
  
        // то есть если вектор не нулевой (то есть если персонаж двигается), мы сохраняем его в lastInteractDirection
        // если вектор нулевой, то все равно в lastInteractDirection будет сохранен последний вектор направления движения
        // => не обязательно двигаться чтобы raycastHit.transform видел объект
        if (moveDirection != Vector3.zero) {
            lastInteractDirection = moveDirection;
        }

        // есть несколько видов Raycast функции и в обычном виде она возвращает bool переменную (задел или нет)
        // а нам нужно точно знать с чем игрок взаимодействует. в raycastHit сохраняется результат функции
        // layer mask позволяет
        if (Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance, countersLayerMask)){
            // 1 способ получить компоненту
            // если объект перед нами - столешниц, и если она не выбрана, то записываем эту столешницу в selectedCounter
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (baseCounter != selectedCounter) {
                    SetSelectedCounter(baseCounter);
                } 
            } else {
                SetSelectedCounter(null);
            }

            // 2 способ получить компоненту 
            // ClearCounter clearCounter = raycastHit.transform.GetComponent<ClearCounter>();
            // if (clearCounter != null){}
        } else {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        // для изменения позиции нужен в 3д игре 3д вектор. и его следует делать именно в моменте сложения,
        // так как нужно разделять логику расчета скорости и логику сложения!
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        // raycast пуляет луч перед объектом и возвращает true/false в зависимости от того
        // есть ли другой объект перед ним или нет
        // bool canMove = !Physics.Raycast(transform.position, moveDirection, playerSize);
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = .7f;  //! не очень нравиться что мы задаем хит бокс игрока в программе
        float playerHeight = 2f;   //! мб это можно будет переделать и сделать хитбокс так же как и у других объектов?
        // CapsuleCast я так понимаю кидает не луч, а форму капсулу для проверки (вокруг объекта?)
        bool canMove = !Physics.CapsuleCast(
            transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirection, moveDistance
        );

        // движение по диагонали при коллизии
        if (!canMove) {
            // не может двигаться прямо, только по диагонали (по оси x)
            // нужно нормализовать по той же причине что и для обычной ходьбы - при диагональном
            // перемещении скорость ниже, а должна быть такой же как и у других направлений
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0, 0).normalized;
            canMove = moveDirection.x != 0 & !Physics.CapsuleCast(
            transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionX, moveDistance
            );

            if (canMove) {
                moveDirection = moveDirectionX;
            } else {
                // cannot move only the x
                // attmept only z movement
                Vector3 moveDirectionZ = new Vector3(0, 0, moveDirection.z).normalized;
                canMove = moveDirection.z != 0 & !Physics.CapsuleCast(
                transform.position, transform.position + Vector3.up * playerHeight, playerRadius, moveDirectionZ, moveDistance
                );

                if (canMove) {
                    // can move only on the Z
                    moveDirection = moveDirectionZ;
                } else {
                    // cannot move in any directino
                }
            }
        }

        if (canMove) {
            // умножаем на Time.deltaTime (количество секунд, прошедших с прошлого кадра)
            // для того, чтобы скорость не зависела от частоты кадров! (в методе Update)
            // то есть чтобы при разном количестве fps скорость была одна и та же
            transform.position += moveDirection * moveDistance;
        }
        

        isWalking = moveDirection != Vector3.zero;  // Vector3.zero = (0, 0, 0)

        // есть много способов сделать повороты: right - для 2д полезно, forward - для поворотов в направлении ходьбы
        // LookAt = поворачивает на конкретный объект
        
        // для добавления анимациям плавности используется интерполяция (определение в obsidian)
        // slerp - обычно используется для разворотов поворотов
        // lerp - для всего остального
        // ? видимо если передаем нулевой вектор, то он не поворачивается
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter selectedCounter) {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}
