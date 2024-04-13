using System;
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
    public GameObject recipesContainer;
    public GameObject slotsContainer;
    public GameObject recipePrefab;
    public GameObject slotPrefab;

    int currencyA = 0;
    int currencyB = 0;
    int currencyAMax = 20;
    int currencyBMax = 10;
    int currencyAIncr = 0;
    int currencyBIncr = 0;
    int currencyAMult = 1;
    int currencyBMult = 1;
    int currencyADelta = 0;
    int currencyBDelta = 0;

    List<Recipe> recipes = new List<Recipe>();
    List<Slot> slots = new List<Slot>();

    int startSlotCount = 4;
    int minSlotCount = 1;
    int maxSlotCount = 32;
    int curSlotCount = 0;

    void Start()
    {
        Currency1Component.text = currencyA.ToString();
        Currency2Component.text = currencyB.ToString();

        StartCoroutine(MainCoroutine());

        recipes.Add(InitializeRecipe(1, 3, 0, 5, "Rat", "Increase increment A +2",
            (slot) =>
            {
                slot.lifeTimeLeft *= 2;
            }, (slot) =>
            {
                UpdateCurrencyIncr(2, 0);
            }, (slot) =>
            {

            }, (slot) => 
            {
                UpdateCurrencyIncr(-2, 0);
            }
        ));
        recipes.Add(InitializeRecipe(2, 5, 0, 30, "Bird", "Increase max A +30",
            (slot) =>
            {
                slot.lifeTimeLeft *= 2;
            }, (slot) =>
            {
                UpdateMaxCurrency(30, 0);
            }, (slot) =>
            {

            }, (slot) =>
            {
                UpdateMaxCurrency(-30, 0);
            }
        ));
        recipes.Add(InitializeRecipe(3, 25, 0, 60, "Godzilla", "Increase increment A +2",
            (slot) =>
            {
                slot.lifeTimeLeft *= 2;
            }, (slot) =>
            {
                currencyAIncr += 5;
            }, (slot) =>
            {

            }, (slot) =>
            {
                currencyAIncr -= 5;
            }
        ));
        recipes.Add(InitializeRecipe(4, 3, 0, 25, "RatMother", "Spawn rats every 3s",
            (slot) =>
            {
                slot.lifeTimeLeft *= 2;
            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var slot = GetFirstActiveEmptySlot();
                    if (slot != null) slot.setRecipe(recipes[0]);
                    
                }, 3));

            }, (slot) =>
            {

            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        

        for (int i = 0; i < recipes.Count; i++)
        {
            recipes[i].transform.localPosition = new Vector2(0f, 250f - 100f * i);
            recipes[i].InitTooltip();
        }

        curSlotCount = startSlotCount;

        for (int i = 0; i < maxSlotCount; i++)
        {
            var slot = InitializeSlot();
            slot.transform.localPosition = new Vector2(0f, 250f - 100f * i);
            if (i >= startSlotCount)
            {
                slot.setActive(false);
            }
            slots.Add(slot);
        }

    }

    void Update()
    {
        
    }

    public void PayRecipe(Recipe recipe)
    {
        var slot = GetFirstActiveEmptySlot();
        if (slot != null && recipe.currencyA <= currencyA && recipe.currencyB <= currencyB)
        {
            UpdateCurrency(-recipe.currencyA, -recipe.currencyB);
            slot.setRecipe(recipe);
        }
    }

    void UpdateCurrency(int deltaCurrencyA, int deltaCurrencyB)
    {
        currencyA += deltaCurrencyA;
        if (currencyA > currencyAMax) currencyA = currencyAMax;

        currencyB += deltaCurrencyB;
        if (currencyB > currencyBMax) currencyB = currencyBMax;

        UpdateCurrencyText();

        foreach (var recipe in recipes)
        {
            recipe.CheckAndSetAvailable(currencyA, currencyB);
        }
    }

    void UpdateCurrencyIncr(int deltaIncrCurrencyA, int deltaIncrCurrencyB)
    {
        currencyAIncr += deltaIncrCurrencyA;
        currencyBIncr += deltaIncrCurrencyB;

        UpdateCurrencyText();
    }

    void UpdateMaxCurrency(int deltaMaxCurrency1, int deltaMaxCurrency2)
    {
        currencyAMax += deltaMaxCurrency1;
        currencyBMax += deltaMaxCurrency2;
        
        if (currencyA > currencyAMax) currencyA = currencyAMax;
        if (currencyB > currencyBMax) currencyB = currencyBMax;

        UpdateCurrencyText();

        foreach (var recipe in recipes)
        {
            recipe.CheckAndSetAvailable(currencyA, currencyB);
        }
    }

    void UpdateCurrencyText()
    {
        currencyADelta = currencyAIncr * currencyAMult;
        currencyBDelta = currencyBIncr * currencyBMult;

        if (currencyADelta == 0 && !HasFilledSlot() && currencyA < 3) currencyADelta = 1;

        Currency1Component.text = currencyA.ToString() + "/" + currencyAMax.ToString() + " (+" + currencyADelta + ")";
        Currency2Component.text = currencyB.ToString() + "/" + currencyBMax.ToString() + " (+" + currencyBDelta + ")";
    }

    Recipe InitializeRecipe(
        int id, int currency1, int currency2, int lifeTime, string nameText, string descriptionText,
        Recipe.ImprovedAction improvedAction, Recipe.StartAction startAction, Recipe.DuringAction duringAction, Recipe.EndAction endAction)
    {
        GameObject newItem = Instantiate(recipePrefab, recipesContainer.transform);

        var recipe = newItem.GetComponent<Recipe>();
        recipe.gameController = this;
        recipe.id = id;
        recipe.nameText = nameText;
        recipe.descriptionText = descriptionText;
        recipe.currencyA = currency1;
        recipe.currencyB = currency2;
        recipe.lifeTime = lifeTime;
        recipe.improvedAction = improvedAction;
        recipe.startAction = startAction;
        recipe.duringAction = duringAction;
        recipe.endAction = endAction;

        recipe.UpdateText();

        return recipe;
    }

    Slot InitializeSlot()
    {
        GameObject newItem = Instantiate(slotPrefab, slotsContainer.transform);

        var slot = newItem.GetComponent<Slot>();
        return slot;
    }

    IEnumerator MainCoroutine()
    {
        while (true)
        {
            UpdateCurrency(currencyADelta, currencyBDelta);
            yield return new WaitForSeconds(1f);
        }
    }

    Slot GetFirstActiveEmptySlot()
    {
        foreach (Slot slot in slots)
        {
            if (slot.active && slot.recipe == null)
            {
                return slot;
            }
        }

        return null;
    }

    bool HasFilledSlot()
    {
        foreach (Slot slot in slots)
        {
            if (slot.recipe != null)
            {
                return true;
            }
        }

        return false;
    }

    public static IEnumerator Do(Action action, int time)
    {
        while (true)
        {
            yield return new WaitForSeconds(time);
            action();
        }
    }

}
