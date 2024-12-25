using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [CreateAssetMenu()] - убрали, так как нам нужен только один RecipeListSO
public class RecipeListSO : ScriptableObject {
    public List<RecipeSO> recipeSOList;
}
