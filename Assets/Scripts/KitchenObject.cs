using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObject : MonoBehaviour {
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    private IKitchenObjectParent kitchenObjectParent;

    public KitchenObjectSO GetKitchenObjectSO() {
        return kitchenObjectSO;
    }

    // а можно это сделать через {get;set;}?
    public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent) {  // то есть я передаю экземпляр класса, который исполняет интерфейс
        // то есть если KitchenObject уже где-то установлен, то убираем его с этого clearCounter и назначаем новый
        if (this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearKitchenObject();
        }
        this.kitchenObjectParent = kitchenObjectParent;

        if (kitchenObjectParent.HasKitchenObject()){
            Debug.Log("!!!");
        }

        kitchenObjectParent.SetKitchenObject(this);  // this - передаем текущий экземпляр KitchenObject

        // перемещаем наш KitchenObject в другой clearCounter (как дочерний компонент)
        transform.parent = kitchenObjectParent.GetKitchenObjectFollowTransform();
        // располагаем томат в позиции (0, 0, 0) относительно counterTopPoint
        transform.localPosition = Vector3.zero;
    }

    public IKitchenObjectParent GetKitchenObjectParent() {
        return kitchenObjectParent;
    }

    public void DestroySelf() {
        kitchenObjectParent.ClearKitchenObject();
        Destroy(gameObject);
    }

    public static KitchenObject SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent) {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
           
        // так как kitchen object спавниться при взаимодействии (то есть его изначально нет на сцене), то чтобы
        // получить доступ к скрипту KitchenObject (а он привзязан к Tomato, CheeseBlock...), нужно использовать
        // GetComponent<KitchenObject>
        // ? или че? почему я не могу просто ссылку на объект дать??? ааа, или мне нужен конкретный kitchenObject, а не
        // ? в целом класс? или че бляяяяяя

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        kitchenObject.SetKitchenObjectParent(kitchenObjectParent);

        return kitchenObject;
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        // тут проверка на то, является ли экземпляр класса KitchenObject экземпляром PlateKitchenObject, и если да
        // то возвращаем этот KitchenObject как PlateKitchenObject чтобы был доступ к AddIngredient,
        // так как KitchenObject не имеет этой функции
        if (this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        } else {
            plateKitchenObject = null;
            return false;
        }
    }
}
