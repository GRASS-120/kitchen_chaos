using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {
    // а зачем мы делали синглтон (причиной было то, что не хотели прокидывать Player
    // в SelectedCounterVisual, однако мы все равно что-то сюда прокидываем...)

    [SerializeField] private BaseCounter baseCounter;
    // в unity нужно прокинуть mesh сюда, саму модель
    [SerializeField] private GameObject[] visualGameObjectArray;
    // переделали под массив, так как внутри модели для selected может быть еще множество моделей (как в container counter например)
    
    // использование синглтона (то есть мы не создаем экземпляр класса => не нужно прокидывать через юнити
    // экземпляр класса Player, а доступ к классу - а почему нельзя так вызвать класс и создать его экземпляр???)
    private void Start() {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
        if (e.selectedCounter == baseCounter) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(false);
        }
    }

}
