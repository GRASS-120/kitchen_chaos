using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour {
    
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;

    
    private void Awake() {
        // template становиться невидимым
        // наверное делаем через код, чтобы не было путаницы из-за того, что делаем SetActive(true) с детьми 
        iconTemplate.gameObject.SetActive(false);
    }

    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.OnIngredientAddedEventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        // разрушаем всех детей... перед тем как спавнить иконки новые
        foreach (Transform child in transform) {
            if (child == iconTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (KitchenObjectSO kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
            // если указать transform, то объект будет спавниться как дочерний объект this
            // (в данном случае IconTemplate дочерний компонент PlateIconsUI)
            // если передать null, то объект спавниться как global object
            Transform iconTransform = Instantiate(iconTemplate, transform);
            // так как копии template тоже будут не активны изначально
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);

            // быстрый способ (без создания отдельного скрипта)
            // Transform iconTransform = Instantiate(iconTemplate, transform);
            // iconTransform.Find("Image").GetComponent<>();
        }
    }
}
