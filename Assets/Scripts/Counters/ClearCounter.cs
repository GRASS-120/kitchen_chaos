using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// MonoBehaviour у BaseCounter
public class ClearCounter : BaseCounter {
    [SerializeField] private KitchenObjectSO kitchenObjectSO;


    public override void Interact(Player player) {
        // так как логика наличия kitchen object у counter и у player одинаковая, то код одинаковый по сути для обоих случаев
        if (!HasKitchenObject()) {
            // no kitchen object
            if (player.HasKitchenObject()) {
                // player is carrying something
                player.GetKitchenObject().SetKitchenObjectParent(this);
            } else {
                // player not carrying anything
            }
        } else {
            // kitchen object here
            if (player.HasKitchenObject()) {
                // player is carrying something
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        // то есть сначала сохраняем KitchenObject путем передачи его в функцию AddIngredient,
                        // а затем удаляем его (так как он уже не будет сам по себе существовать)
                        GetKitchenObject().DestroySelf();
                    }
                } else {
                    // player is not carrying plate but something else
                    if (GetKitchenObject().TryGetPlate(out plateKitchenObject)) {  // убрали тип чтобы убрать ошибку
                        // counter is holding a plate
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            // remove player's kitchen object and move it on a plate
                            player.GetKitchenObject().DestroySelf();
                        }
                    }
                }
            } else {
                // player not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }
}
