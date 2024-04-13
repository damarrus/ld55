using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI Currency1Component;
    public TextMeshProUGUI Currency2Component;
    public GameObject scrollViewContent;
    public GameObject slotsContainer;
    public GameObject recipePrefab;
    public GameObject slotPrefab;

    int currency1 = 0;
    int currency2 = 0;
    int currency1Max = 20;
    int currency2Max = 10;

    List<Recipe> recipes = new List<Recipe>();
    List<Slot> slots = new List<Slot>();

    int minSlotCount = 4;
    int maxSlotCount = 32;

    void Start()
    {
        Currency1Component.text = currency1.ToString();
        Currency2Component.text = currency2.ToString();

        StartCoroutine(IncrementNumber());

        recipes.Add(InitializeRecipe(1, "Bird", 3, 0, 5));
        recipes.Add(InitializeRecipe(2, "Rat", 5, 0, 10));
        recipes.Add(InitializeRecipe(3, "Godzilla", 10, 5, 30));


        for (int i = 0; i < recipes.Count; i++)
        {
            recipes[i].transform.localPosition = new Vector2(0f, 250f - 100f * i);
        }

        for (int i = 0; i < maxSlotCount; i++)
        {
            var slot = InitializeSlot();
            slot.transform.localPosition = new Vector2(0f, 250f - 100f * i);
            if (i >= minSlotCount)
            {
                slot.active = false;
            }
            slots.Add(slot);
        }

    }

    void Update()
    {
        
    }

    public void PayRecipe(Recipe recipe)
    {
        var slot = GetFirstEmptySlot();
        if (slot != null && recipe.currency1 <= currency1 && recipe.currency2 <= currency2)
        {
            UpdateCurrency(-recipe.currency1, -recipe.currency2);
            slot.setRecipe(recipe);
        }
    }

    void UpdateCurrency(int deltaCurrency1, int deltaCurrency2)
    {
        currency1 += deltaCurrency1;
        currency2 += deltaCurrency2;
        UpdateCurrencyText();

        foreach (var recipe in recipes)
        {
            recipe.CheckAndSetAvailable(currency1, currency2);
        }
    }

    void UpdateMaxCurrency(int deltaMaxCurrency1, int deltaMaxCurrency2)
    {
        currency1Max += deltaMaxCurrency1;
        currency2Max += deltaMaxCurrency2;
        
        if (currency1 > currency1Max) currency1 = currency1Max;
        if (currency2 > currency2Max) currency2 = currency2Max;

        UpdateCurrencyText();

        foreach (var recipe in recipes)
        {
            recipe.CheckAndSetAvailable(currency1, currency2);
        }
    }

    void UpdateCurrencyText()
    {
        Currency1Component.text = currency1.ToString() + "/" + currency1Max.ToString();
        Currency2Component.text = currency2.ToString() + "/" + currency2Max.ToString();
    }

    Recipe InitializeRecipe(int id, string nameText, int currency1, int currency2, int lifeTime)
    {
        GameObject newItem = Instantiate(recipePrefab, scrollViewContent.transform);

        var recipe = newItem.GetComponent<Recipe>();
        recipe.gameController = this;
        recipe.id = id;
        recipe.nameText = nameText;
        recipe.currency1 = currency1;
        recipe.currency2 = currency2;
        recipe.lifeTime = lifeTime;

        recipe.UpdateText();

        // Пересчитываем размер содержимого ScrollView
        RectTransform contentRectTransform = scrollViewContent.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, contentRectTransform.sizeDelta.y + newItem.GetComponent<RectTransform>().sizeDelta.y);


        return recipe;
    }

    Slot InitializeSlot()
    {
        GameObject newItem = Instantiate(slotPrefab, slotsContainer.transform);

        var slot = newItem.GetComponent<Slot>();
        return slot;
    }

    IEnumerator IncrementNumber()
    {
        while (true)
        {
            UpdateCurrency(1, 0);            
            yield return new WaitForSeconds(2f);
        }
    }

    Slot GetFirstEmptySlot()
    {
        foreach (Slot slot in slots)
        {
            if (slot.recipe == null)
            {
                return slot;
            }
        }

        return null;
    }
}
