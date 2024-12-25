using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObject {

    public event EventHandler<OnIngredientAddedEventArgs> OnIngredientAdded;
    public class OnIngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO kitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;
    
    private List<KitchenObjectSO> kitchenObjectSOList;

    private void Awake() {
        kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    // ингредиент одного типа может быть только один на тарелке, поэтому и try - то есть не факт, что
    // получиться добавить, так как нельзя добавлять один и тот же ингридиент больше 1 раза
    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) {
            // not valid ingredient
            return false;
        }
        // вообще не уверен что так стоит делать, так как функция делает две вещи сразу - 
        // говорит, можно ли добавить + добавляет если можно... думаю можно это разделить (на будущее)
        if (kitchenObjectSOList.Contains(kitchenObjectSO)) {
            // already have this type
            return false;
        } else {
            kitchenObjectSOList.Add(kitchenObjectSO);

            OnIngredientAdded?.Invoke(this, new OnIngredientAddedEventArgs {
                kitchenObjectSO = kitchenObjectSO
            });

            return true;
        }
        
    }
    
    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return kitchenObjectSOList;
    }
}
