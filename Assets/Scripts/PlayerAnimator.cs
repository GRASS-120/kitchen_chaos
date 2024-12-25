using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    // для минимизации вероятности опечатки в строке выносим название в переменную
    private const string IS_WALKING = "IsWalking";

    // для подключения Player нужно в unity в объекте со скриптом анимации подключить Player'a
    [SerializeField] private Player player;

    private Animator animator;

    // перед инициализацией скрипта
    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Update() {
        animator.SetBool(IS_WALKING, player.IsWalking());
    }

}
