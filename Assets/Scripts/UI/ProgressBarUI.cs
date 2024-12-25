using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject hasProgressGameObject;
    private IHasProgress hasProgress;

    private void Start() {
        // то есть можно не только классы считывать так, но и интерфейсы
        hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (hasProgress == null) {
            Debug.LogError("game object " + hasProgressGameObject + " does not have a component that implements IHasProgress!");
        }

        hasProgress.OnProgressChanged += HasProgress_OnPressChanged;

        barImage.fillAmount = 0f;

        // скрывать нужно только ПОСЛЕ подписки на событие
        Hide();
    }

    private void HasProgress_OnPressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e) {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 0f || e.progressNormalized == 1f) {
            Hide();
        } else {
            Show();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}
