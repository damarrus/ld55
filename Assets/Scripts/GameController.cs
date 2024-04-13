using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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

    public int improveChance = 5;

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
    int maxSlotCount = 12;

    Dictionary<int, int> RecipesOrder { get; set; }

    public delegate void PayStartEventHandler(Slot slot);
    public static event PayStartEventHandler PayStartEvent;

    public delegate void PayEndEventHandler(Slot slot);
    public static event PayEndEventHandler PayEndEvent;

    void InitConfig()
    {
        RecipesOrder = new Dictionary<int, int>()
        { // recipeSlotID : recipeID
            { 1, 1   }, { 2, 2   },
            { 3, 3   }, { 4, 4   },
            { 5, 5   }, { 6, 6   },
            { 7, 7   }, { 8, 8   },
            { 9, 9   }, { 10, 10 },
            { 11, 11 }, { 12, 12 },
            { 13, 13 }, { 14, 14 },
            {       15, 15       }
        };
    }

    void Start()
    {
        // TODO убрать общий такт. Везде сделать индивидуально
        // TODO вынести параметры отдельно
        // TODO скрывать изначально все рецепты, список id => [ id, id ]

        InitConfig();

        Currency1Component.text = currencyA.ToString();
        Currency2Component.text = currencyB.ToString();

        StartCoroutine(MainCoroutine());

        recipes.Add(InitializeRecipe(1, 3, 0, 5, "Cat", "Increase increment A +2",
            (slot) =>
            {
                slot.increaseLifeTime(slot.lifeTimeMax);
            }, (slot) =>
            {
                UpdateCurrencyIncr(slot.isImproved ? 10 : 5, 0);
            }, (slot) => 
            {
                UpdateCurrencyIncr(slot.isImproved ? -10 : -5, 0);
            }
        ));
        recipes.Add(InitializeRecipe(2, 5, 0, 30, "Greed", "Increase max A/B x2",
            (slot) =>
            {
                slot.increaseLifeTime(slot.lifeTimeMax);
            }, (slot) =>
            {
                UpdateMaxCurrency(slot.isImproved ? 3 : 2, slot.isImproved ? 3 : 2);

            }, (slot) =>
            {
                UpdateMaxCurrency(slot.isImproved ? 3 : 2, slot.isImproved ? 3 : 2, true);
            }
        ));
        recipes.Add(InitializeRecipe(3, 3, 0, 10, "Trader", "Convert 10 A to 1 B",
            (slot) =>
            {

            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var rateA = slot.isImproved ? 8 : 10;
                    var rateB = slot.isImproved ? 4 : 3;
                    if (currencyA >= rateA) UpdateCurrency(-rateA, rateB);

                }, 3));
            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        recipes.Add(InitializeRecipe(4, 0, 1, 10, "Mushroom", "After death gives 10 A",
            (slot) =>
            {
                
            }, (slot) =>
            {

            }, (slot) =>
            {
                UpdateCurrency(slot.isImproved ? 100 : 10, 0);
            }
        ));
        recipes.Add(InitializeRecipe(5, 0, 1, 10, "Druid", "Gives 3 slots",
            (slot) =>
            {

            }, (slot) =>
            {
                var slotsCount = slot.isImproved ? 5 : 3;
                for (int i = 1; i <= slotsCount; i++)
                {
                    var blockedSlot = GetFirstBlockedSlot();
                    if (blockedSlot == null) break;

                    blockedSlot.setActive(true);
                    slot.paramListInt.Add(blockedSlot.id);
                }
            }, (slot) =>
            {
                slot.paramListInt.ForEach(slotId =>
                {
                    var s = slots.Find(slot => slot.id == slotId);
                    s.setActive(false);
                });
                slot.paramListInt = new List<int>();
            }
        ));
        recipes.Add(InitializeRecipe(6, 3, 0, 10, "Glutton", "Convert 1 demon to 1 B",
            (slot) =>
            {

            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var filteredSlots = slots.FindAll(s => s.id != slot.id && s.recipe != null);
                    if (filteredSlots.Count > 0)
                    {
                        System.Random random = new System.Random();
                        int randomIndex = random.Next(0, filteredSlots.Count);
                        filteredSlots[randomIndex].setRecipe(null);
                        UpdateCurrency(0, slot.isImproved ? 3 : 1);
                    }
                    
                }, 3));
            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        recipes.Add(InitializeRecipe(7, 3, 0, 10, "Snake", "Increases life time",
            (slot) =>
            {
                slot.increaseLifeTime(slot.lifeTimeMax);
            }, (slot) =>
            {
                var filteredSlots = slots.FindAll(s => s.id != slot.id && s.recipe != null);
                foreach (var filteredSlot in filteredSlots)
                {
                    filteredSlot.increaseLifeTime(filteredSlot.lifeTimeMax);
                }
                PayEndEvent += SnakePayHandlerMethod;
                if (!slot.isImproved)
                {
                    var activeSlot = GetFirstActiveEmptySlot();
                    if (activeSlot == null) activeSlot = slots[0];

                    activeSlot.setActive(false);
                    slot.paramInt = activeSlot.id;
                }
            }, (slot) =>
            {
                PayEndEvent -= SnakePayHandlerMethod;
                if (!slot.isImproved)
                {
                    var blockedSlot = slots.Find(s => s.id == slot.paramInt);
                    blockedSlot.setActive(true);
                }
            }
        ));
        recipes.Add(InitializeRecipe(8, 3, 0, 10, "Eye", "Gives 1 B when summoned",
            (slot) =>
            {
                slot.increaseLifeTime(slot.lifeTimeMax);
            }, (slot) =>
            {
                if (slot.isImproved)
                {
                    PayStartEvent += EyeImprovedPayHandlerMethod;
                } 
                else
                {
                    PayStartEvent += EyePayHandlerMethod;
                }
            }, (slot) =>
            {
                if (slot.isImproved)
                {
                    PayStartEvent -= EyeImprovedPayHandlerMethod;
                }
                else
                {
                    PayStartEvent -= EyePayHandlerMethod;
                }
            }
        ));
        recipes.Add(InitializeRecipe(9, 3, 0, 10, "Ptero", "Gives x2 A",
            (slot) =>
            {

            }, (slot) =>
            {
                UpdateCurrency(currencyA * (slot.isImproved ? 3 : 2), 0);
            }, (slot) =>
            {

            }
        ));
        recipes.Add(InitializeRecipe(10, 3, 0, 10, "Spider", "Increase increment B +1",
            (slot) =>
            {
                slot.increaseLifeTime(slot.lifeTimeMax);
            }, (slot) =>
            {
                UpdateCurrencyIncr(0, slot.isImproved ? 2 : 1);
            }, (slot) =>
            {
                UpdateCurrencyIncr(0, slot.isImproved ? -2 : -1);
            }
        ));
        recipes.Add(InitializeRecipe(11, 3, 0, 10, "Fire", "Increase increment A/B x2",
            (slot) =>
            {
                slot.increaseLifeTime(slot.lifeTimeMax);
            }, (slot) =>
            {
                UpdateCurrencyMult(slot.isImproved ? 2 : 1, slot.isImproved ? 2 : 1);
            }, (slot) =>
            {
                UpdateCurrencyMult(slot.isImproved ? -2 : -1, slot.isImproved ? -2 : -1);
            }
        ));
        recipes.Add(InitializeRecipe(12, 3, 0, 10, "Octopus", "Gives 1 A for each demons every 2s",
            (slot) =>
            {
                slot.increaseLifeTime(slot.lifeTimeMax);
            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var filteredSlots = slots.FindAll(s => s.id != slot.id && s.recipe != null);
                    UpdateCurrency(1 * filteredSlots.Count, 0);
                }, 2));
            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        recipes.Add(InitializeRecipe(13, 3, 0, 10, "Joker", "Gives 1 A for each demons every 2s",
            (slot) =>
            {
            }, (slot) =>
            {
                improveChance *= slot.isImproved ? 10 : 3;
            }, (slot) =>
            {
                improveChance /= slot.isImproved ? 10 : 3;
            }
        ));
        recipes.Add(InitializeRecipe(14, 3, 0, 25, "CatMother", "Spawn cats every 3s",
            (slot) =>
            {
                slot.isImproved = true;
            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var slot = GetFirstActiveEmptySlot();
                    if (slot != null) slot.setRecipe(recipes[0], slot.isImproved ? 50 : 0);

                }, 3));

            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        recipes.Add(InitializeRecipe(15, 3, 0, 25, "Dragon", "You win!",
            (slot) =>
            {

            }, (slot) =>
            {
                Debug.Log("WIN!");
            }, (slot) =>
            {

            }
        ));


        for (int i = 1; i <= maxSlotCount; i++)
        {
            var slot = slotsContainer.transform.Find("SlotPrefab" + i.ToString()).GetComponent<Slot>();
            slot.id = i;
            if (i > startSlotCount) slot.setActive(false);

            slots.Add(slot);
        }

    }

    public void SnakePayHandlerMethod(Slot slot)
    {
        slot.increaseLifeTime(slot.lifeTimeMax);
    }

    public void EyePayHandlerMethod(Slot slot)
    {
        UpdateCurrency(0, 1);
    }

    public void EyeImprovedPayHandlerMethod(Slot slot)
    {
        UpdateCurrency(0, 3);
    }

    public void PayRecipe(Recipe recipe)
    {
        var slot = GetFirstActiveEmptySlot();
        if (slot != null && recipe.currencyA <= currencyA && recipe.currencyB <= currencyB)
        {
            UpdateCurrency(-recipe.currencyA, -recipe.currencyB);
            PayStartEvent(slot);
            slot.setRecipe(recipe);
            PayEndEvent(slot);
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

    void UpdateCurrencyMult(int deltaMultCurrencyA, int deltaMultCurrencyB)
    {
        currencyAMult += deltaMultCurrencyA;
        currencyBMult += deltaMultCurrencyB;

        UpdateCurrencyText();
    }

    void UpdateMaxCurrency(int deltaMaxCurrency1, int deltaMaxCurrency2, bool division = false)
    {
        if (division)
        {
            currencyAMax /= deltaMaxCurrency1;
            currencyBMax /= deltaMaxCurrency2;

        } else
        {
            currencyAMax *= deltaMaxCurrency1;
            currencyBMax *= deltaMaxCurrency2;
        }
        
        
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
        Recipe.ImprovedAction improvedAction, Recipe.StartAction startAction, Recipe.EndAction endAction)
    {
        var recipeSlotId = RecipesOrder.FirstOrDefault(x => x.Value == id).Key;
        var recipePrefabObject = recipesContainer.transform.Find("RecipePrefab" + recipeSlotId.ToString()).gameObject;

        var recipe = recipePrefabObject.GetComponent<Recipe>();
        recipe.gameController = this;
        recipe.id = id;
        recipe.nameText = nameText;
        recipe.descriptionText = descriptionText;
        recipe.currencyA = currency1;
        recipe.currencyB = currency2;
        recipe.lifeTime = lifeTime;
        recipe.improvedAction = improvedAction;
        recipe.startAction = startAction;
        recipe.endAction = endAction;

        recipe.UpdateText();

        return recipe;
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

    Slot GetFirstBlockedSlot()
    {
        foreach (Slot slot in slots)
        {
            if (!slot.active && slot.recipe != null)
            {
                return slot;
            }
        }

        foreach (Slot slot in slots)
        {
            if (!slot.active)
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
