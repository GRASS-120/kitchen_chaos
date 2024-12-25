using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// добавляет в контекстное меню при создании объекта
[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject {
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
