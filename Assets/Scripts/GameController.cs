using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public TextMeshProUGUI Currency1Component;
    public TextMeshProUGUI Currency2Component;
    public GameObject recipesContainer;
    public GameObject slotsContainer;
    public GameObject recipePrefab;
    public GameObject slotPrefab;

    public int improveChance = 0;

    public int currencyA = 0;
    public int currencyB = 0;

    int currencyAMult = 1;
    int currencyBMult = 1;

    List<Recipe> recipes = new List<Recipe>();
    List<Slot> slots = new List<Slot>();

    
    int maxSlotCount = 12;
    public int globalLifetimeMultiplier = 1;

    Dictionary<int, int> RecipesOrder { get; set; }
    Dictionary<int, List<int>> RecipesReveal { get; set; }
    List<int> RecipesRevealedOnStart { get; set; }

    public delegate void PayStartEventHandler(Slot slot);
    public event PayStartEventHandler PayStartEvent;

    public delegate void PayEndEventHandler(Slot slot);
    public event PayEndEventHandler PayEndEvent;
    [Space]
    [Space]
    public int currencyABaseMax = 20;
    public int currencyBBaseMax = 10;
    public int startSlotCount = 4;
    public int baseImproveChance = 5;
    [Space]
    [Space]
    public int catId = 1;
    public string catName = "Cat";
    public string catDescription = "Increase increment A +2";
    public string catImprovedDescription = "Increase increment A +2";
    public int catPriceA = 3;
    public int catPriceB = 0;
    public int catLifeTime = 5;
    public int catImprovedLifeTime = 10;
    public int catAddA = 1;
    public int catImprovedAddA = 2;
    public int catAddDelay = 1;
    [Space]
    public int greedId = 2;
    public string greedName = "Greed";
    public string greedDescription = "Increase max A/B x2";
    public string greedImprovedDescription = "Increase max A/B x2";
    public int greedPriceA = 10;
    public int greedPriceB = 0;
    public int greedLifeTime = 5;
    public int greedImprovedLifeTime = 10;
    public int greedMultMaxA = 2;
    public int greedImprovedMultMaxA = 3;
    [Space]
    public int traderId = 3;
    public string traderName = "Trader";
    public string traderDescription = "Convert 10 A to 1 B";
    public string traderImprovedDescription = "Convert 10 A to 1 B";
    public int traderPriceA = 10;
    public int traderPriceB = 0;
    public int traderLifeTime = 30;
    public int traderImprovedLifeTime = 60;
    public int traderRateA = 10;
    public int traderRateB = 1;
    public int traderImprovedRateA = 8;
    public int traderImprovedRateB = 2;
    public int traderConvertDelay = 3;
    [Space]
    public int mushroomId = 4;
    public string mushroomName = "Mushroom";
    public string mushroomDescription = "After death gives 10 A";
    public string mushroomImprovedDescription = "After death gives 10 A";
    public int mushroomPriceA = 0;
    public int mushroomPriceB = 1;
    public int mushroomLifeTime = 60;
    public int mushroomImprovedLifeTime = 60;
    public int mushroomEndAddA = 20;
    public int mushroomImprovedEndAddA = 100;
    [Space]
    public int druidId = 5;
    public string druidName = "Druid";
    public string druidDescription = "Gives 3 slots";
    public string druidImprovedDescription = "Gives 3 slots";
    public int druidPriceA = 10;
    public int druidPriceB = 1;
    public int druidLifeTime = 60;
    public int druidImprovedLifeTime = 60;
    public int druidAddSlots = 3;
    public int druidImprovedAddSlots = 5;
    [Space]
    public int gluttonId = 6;
    public string gluttonName = "Glutton";
    public string gluttonDescription = "Convert 1 demon to 1 B";
    public string gluttonImprovedDescription = "Convert 1 demon to 1 B";
    public int gluttonPriceA = 0;
    public int gluttonPriceB = 2;
    public int gluttonLifeTime = 30;
    public int gluttonImprovedLifeTime = 30;
    public int gluttonConvertAddB = 1;
    public int gluttonImprovedConvertAddB = 3;
    public int gluttonConvertAddDelay = 3;
    [Space]
    public int snakeId = 7;
    public string snakeName = "Snake";
    public string snakeDescription = "Increases life time";
    public string snakeImprovedDescription = "Increases life time";
    public int snakePriceA = 0;
    public int snakePriceB = 10;
    public int snakeLifeTime = 60;
    public int snakeImprovedLifeTime = 120;
    public int snakeMultLifetime = 2;
    public int snakeImprovedMultLifetime = 3;
    [Space]
    public int eyeId = 8;
    public string eyeName = "Eye";
    public string eyeDescription = "Gives 1 B when summoned";
    public string eyeImprovedDescription = "Gives 1 B when summoned";
    public int eyePriceA = 0;
    public int eyePriceB = 10;
    public int eyeLifeTime = 30;
    public int eyeImprovedLifeTime = 30;
    public int eyeSummonedAddB = 1;
    public int eyeImprovedSummonedAddB = 3;
    [Space]
    public int pteroId = 9;
    public string pteroName = "Ptero";
    public string pteroDescription = "Gives x2 A";
    public string pteroImprovedDescription = "Gives x2 A";
    public int pteroPriceA = 0;
    public int pteroPriceB = 5;
    public int pteroLifeTime = 3;
    public int pteroImprovedLifeTime = 3;
    public int pteroMultInstantlyAdd = 2;
    public int pteroImprovedMultInstantlyAdd = 3;
    [Space]
    public int spiderId = 10;
    public string spiderName = "Spider";
    public string spiderDescription = "Increase increment B +1";
    public string spiderImprovedDescription = "Increase increment B +1";
    public int spiderPriceA = 0;
    public int spiderPriceB = 10;
    public int spiderLifeTime = 10;
    public int spiderImprovedLifeTime = 20;
    public int spiderAddB = 1;
    public int spiderImprovedAddB = 2;
    public int spiderAddDelay = 2;
    [Space]
    public int fireId = 11;
    public string fireName = "Fire";
    public string fireDescription = "Increase increment A/B x2";
    public string fireImprovedDescription = "Increase increment A/B x2";
    public int firePriceA = 40;
    public int firePriceB = 40;
    public int fireLifeTime = 15;
    public int fireImprovedLifeTime = 15;
    public int fireMultAddAB = 2;
    public int fireImprovedMultAddAB = 3;
    [Space]
    public int octopusId = 12;
    public string octopusName = "Octopus";
    public string octopusDescription = "Gives 1 A for each demons every 2s";
    public string octopusImprovedDescription = "Gives 1 A for each demons every 2s";
    public int octopusPriceA = 20;
    public int octopusPriceB = 0;
    public int octopusLifeTime = 10;
    public int octopusImprovedLifeTime = 20;
    public int octopusAddAForEach = 1;
    public int octopusAddForEachDelay = 2;
    [Space]
    public int jokerId = 13;
    public string jokerName = "Joker";
    public string jokerDescription = "Increase chance for improved summon";
    public string jokerImprovedDescription = "Increase chance for improved summon";
    public int jokerPriceA = 5;
    public int jokerPriceB = 5;
    public int jokerLifeTime = 20;
    public int jokerImprovedLifeTime = 20;
    public int jokerMultChanceToImprove = 3;
    public int jokerImprovedMultChanceToImprove = 10;
    [Space]
    public int motherCatId = 14;
    public string motherCatName = "MotherCat";
    public string motherCatDescription = "Spawn cats every 5s";
    public string motherCatImrpovedDescription = "Spawn cats every 5s";
    public int motherCatPriceA = 100;
    public int motherCatPriceB = 0;
    public int motherCatLifeTime = 30;
    public int motherCatImprovedLifeTime = 30;
    public int motherCatImprovedChanceToImprove = 50;
    public int motherCatSummonDelay = 5;
    [Space]
    public int dragonId = 15;
    public string dragonName = "Dragon";
    public string dragonDescription = "You win";
    public string dragonImprovedDescription = "You win";
    public int dragonPriceA = 10;
    public int dragonPriceB = 0;
    public int dragonLifeTime = 0;
    public int dragonImprovedLifeTime = 0;

    void InitConfig()
    {
        RecipesOrder = new Dictionary<int, int>()
        { // recipeSlotID : recipeID
            { 1, 1   }, { 2, 3   },
            { 3, 5   }, { 4, 4   },
            { 5, 7   }, { 6, 10  },
            { 7, 14  }, { 8, 6   },
            { 9, 2   }, { 10, 9  },
            { 11, 13 }, { 12, 11 },
            { 13, 12 }, { 14, 8 },
            {       15, 15       }
        };

        RecipesRevealedOnStart = new List<int>() { 1, 3, 15 };
        RecipesReveal = new Dictionary<int, List<int>>()
        {
            { 1,  new List<int>() {  } }, 
            { 2,  new List<int>() { 13, 11, 12, 8 } }, 
            { 3,  new List<int>() { 5, 4 } },
            { 4,  new List<int>() { 7, 10 } },
            { 5,  new List<int>() { 7, 10 } },
            { 6,  new List<int>() { 2, 9 } },
            { 7,  new List<int>() { 14, 6 } },
            { 8,  new List<int>() {  } },
            { 9,  new List<int>() {  } },
            { 10, new List<int>() { 14, 16 } },
            { 11, new List<int>() {  } },
            { 12, new List<int>() {  } },
            { 13, new List<int>() {  } },
            { 14, new List<int>() { 2, 9 } },
            { 15, new List<int>() {  } },
        };
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            UpdateCurrency(9999, 9999);
        }
    }

    void Start()
    {
        improveChance = baseImproveChance;
        InitConfig();

        Currency1Component.text = currencyA.ToString();
        Currency2Component.text = currencyB.ToString();

        StartCoroutine(MainCoroutine());

        recipes.Add(InitializeRecipe(catId, catPriceA, catPriceB, catLifeTime, catImprovedLifeTime, catName, catDescription, catImprovedDescription,
            (slot) =>
            {

            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var deltaA = slot.isImproved ? catImprovedAddA : catAddA;
                    var (realDeltaA, realDeltaB) = UpdateCurrency(deltaA, 0);
                    slot.createFlyOut(realDeltaA, true);
                }, catAddDelay));
            }, (slot) => 
            {
                StopCoroutine(slot.actionCoroutine);
            } 
        ));
        recipes.Add(InitializeRecipe(greedId, greedPriceA, greedPriceB, greedLifeTime, greedImprovedLifeTime, greedName, greedDescription, greedImprovedDescription,
            (slot) =>
            {
            }, (slot) =>
            {
                UpdateMaxCurrency(slot.isImproved ? greedImprovedMultMaxA : greedMultMaxA, slot.isImproved ? greedImprovedMultMaxA : greedMultMaxA);

            }, (slot) =>
            {
                UpdateMaxCurrency(slot.isImproved ? greedImprovedMultMaxA : greedMultMaxA, slot.isImproved ? greedImprovedMultMaxA : greedMultMaxA, true);
            }
        ));
        recipes.Add(InitializeRecipe(traderId, traderPriceA, traderPriceB, traderLifeTime, traderImprovedLifeTime, traderName, traderDescription, traderImprovedDescription,
            (slot) =>
            {

            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var rateA = slot.isImproved ? traderImprovedRateA : traderRateA;
                    var rateB = slot.isImproved ? traderImprovedRateB : traderRateB;
                    if (currencyA >= rateA)
                    {
                        var (realDeltaA, _) = UpdateCurrency(-rateA, 0, false);
                        var (__, realDeltaB) = UpdateCurrency(0, rateB);

                        // TODO ������� ��������
                        //slot.createFlyOut(-realDeltaA, true);
                        slot.createFlyOut(realDeltaB, false);
                    }

                }, traderConvertDelay));
            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        recipes.Add(InitializeRecipe(mushroomId, mushroomPriceA, mushroomPriceB, mushroomLifeTime, mushroomImprovedLifeTime, mushroomName, mushroomDescription, mushroomImprovedDescription,
            (slot) =>
            {
                
            }, (slot) =>
            {

            }, (slot) =>
            {
                var deltaA = slot.isImproved ? mushroomImprovedEndAddA : mushroomEndAddA;
                var (realDeltaA, realDeltaB) = UpdateCurrency(deltaA, 0);
                slot.createFlyOut(realDeltaA, true);
            }
        ));
        recipes.Add(InitializeRecipe(druidId, druidPriceA, druidPriceB, druidLifeTime, druidImprovedLifeTime, druidName, druidDescription, druidImprovedDescription,
            (slot) =>
            {

            }, (slot) =>
            {
                var slotsCount = slot.isImproved ? druidImprovedAddSlots : druidAddSlots;
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
        recipes.Add(InitializeRecipe(gluttonId, gluttonPriceA, gluttonPriceB, gluttonLifeTime, gluttonImprovedLifeTime, gluttonName, gluttonDescription, gluttonImprovedDescription,
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
                        var deltaB = slot.isImproved ? gluttonImprovedConvertAddB : gluttonConvertAddB;
                        var (realDeltaA, realDeltaB) = UpdateCurrency(0, deltaB);
                        slot.createFlyOut(realDeltaB, false);
                    }
                    
                }, gluttonConvertAddDelay));
            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        recipes.Add(InitializeRecipe(snakeId, snakePriceA, snakePriceB, snakeLifeTime, snakeImprovedLifeTime, snakeName, snakeDescription, snakeImprovedDescription,
            (slot) =>
            {
            }, (slot) =>
            {
                globalLifetimeMultiplier *= slot.isImproved ? snakeImprovedMultLifetime : snakeMultLifetime;
                var filteredSlots = slots.FindAll(s => s.recipe != null);

                foreach (var filteredSlot in filteredSlots)
                {
                    filteredSlot.increaseLifeTime(globalLifetimeMultiplier);
                }
                PayEndEvent += slot.SnakePayHandlerMethod;
            }, (slot) =>
            {
                globalLifetimeMultiplier /= slot.isImproved ? snakeImprovedMultLifetime : snakeMultLifetime;
                PayEndEvent -= slot.SnakePayHandlerMethod;
            }
        ));
        recipes.Add(InitializeRecipe(eyeId, eyePriceA, eyePriceB, eyeLifeTime, eyeImprovedLifeTime, eyeName, eyeDescription, eyeImprovedDescription,
            (slot) =>
            {
            }, (slot) =>
            {
                if (slot.isImproved)
                {
                    PayStartEvent += slot.EyeImprovedPayHandlerMethod;
                } 
                else
                {
                    PayStartEvent += slot.EyePayHandlerMethod;
                }
            }, (slot) =>
            {
                if (slot.isImproved)
                {
                    PayStartEvent -= slot.EyeImprovedPayHandlerMethod;
                }
                else
                {
                    PayStartEvent -= slot.EyePayHandlerMethod;
                }
            }
        ));
        recipes.Add(InitializeRecipe(pteroId, pteroPriceA, pteroPriceB, pteroLifeTime, pteroImprovedLifeTime, pteroName, pteroDescription, pteroImprovedDescription,
            (slot) =>
            {

            }, (slot) =>
            {
                var deltaA = currencyA * (slot.isImproved ? pteroImprovedMultInstantlyAdd : pteroMultInstantlyAdd);
                var (realDeltaA, realDeltaB) = UpdateCurrency(deltaA, 0);
                slot.createFlyOut(realDeltaA, true);
            }, (slot) =>
            {

            }
        ));
        recipes.Add(InitializeRecipe(spiderId, snakePriceA, snakePriceB, spiderLifeTime, spiderImprovedLifeTime, spiderName, spiderDescription, spiderImprovedDescription,
            (slot) =>
            {

            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var deltaB = slot.isImproved ? spiderImprovedAddB : spiderAddB;
                    var (realDeltaA, realDeltaB) = UpdateCurrency(0, deltaB);
                    slot.createFlyOut(realDeltaB, false);
                }, spiderAddDelay));
            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        recipes.Add(InitializeRecipe(fireId, firePriceA, firePriceB, fireLifeTime, fireImprovedLifeTime, fireName, fireDescription, fireImprovedDescription,
            (slot) =>
            {
            }, (slot) =>
            {
                UpdateCurrencyMult(slot.isImproved ? fireImprovedMultAddAB : fireMultAddAB, slot.isImproved ? fireImprovedMultAddAB : fireMultAddAB);
            }, (slot) =>
            {
                UpdateCurrencyMult(slot.isImproved ? fireImprovedMultAddAB : fireMultAddAB, slot.isImproved ? fireImprovedMultAddAB : fireMultAddAB, true);
            }
        ));
        recipes.Add(InitializeRecipe(octopusId, octopusPriceA, octopusPriceB, octopusLifeTime, octopusImprovedLifeTime, octopusName, octopusDescription, octopusImprovedDescription,
            (slot) =>
            {
            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var filteredSlots = slots.FindAll(s => s.id != slot.id && s.recipe != null);
                    var deltaA = octopusAddAForEach * filteredSlots.Count;
                    var (realDeltaA, realDeltaB) = UpdateCurrency(deltaA, 0);
                    slot.createFlyOut(realDeltaA, true);
                }, octopusAddForEachDelay));
            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        recipes.Add(InitializeRecipe(jokerId, jokerPriceA, jokerPriceB, jokerLifeTime, jokerImprovedLifeTime, jokerName, jokerDescription, jokerImprovedDescription,
            (slot) =>
            {
            }, (slot) =>
            {
                improveChance *= slot.isImproved ? jokerImprovedMultChanceToImprove : jokerMultChanceToImprove;
            }, (slot) =>
            {
                improveChance /= slot.isImproved ? jokerImprovedMultChanceToImprove : jokerMultChanceToImprove;
            }
        ));
        recipes.Add(InitializeRecipe(motherCatId, motherCatPriceA, motherCatPriceB, motherCatLifeTime, motherCatImprovedLifeTime, motherCatName, motherCatDescription, motherCatImrpovedDescription,
            (slot) =>
            {
                slot.isImproved = true;
            }, (slot) =>
            {
                slot.actionCoroutine = StartCoroutine(Do(() =>
                {
                    var slot = GetFirstActiveEmptySlot();
                    if (slot != null) slot.setRecipe(recipes[0], slot.isImproved ? motherCatImprovedChanceToImprove : 0);

                }, motherCatSummonDelay));

            }, (slot) =>
            {
                StopCoroutine(slot.actionCoroutine);
            }
        ));
        recipes.Add(InitializeRecipe(dragonId, dragonPriceA, dragonPriceB, dragonLifeTime, dragonImprovedLifeTime, dragonName, dragonDescription, dragonImprovedDescription,
            (slot) =>
            {

            }, (slot) =>
            {
                Debug.Log("WIN!");
                SceneManager.LoadScene("WinScene");
            }, (slot) =>
            {

            }
        ));


        for (int i = 1; i <= maxSlotCount; i++)
        {
            var slot = slotsContainer.transform.Find("SlotPrefab" + i.ToString()).GetComponent<Slot>();
            slot.id = i;
            slot.gameController = this;
            if (i > startSlotCount) slot.setActive(false);

            slots.Add(slot);
        }

    }

    public void PayRecipe(Recipe recipe)
    {
        var slot = GetFirstActiveEmptySlot();
        if (slot != null && recipe.revealed && recipe.currencyA <= currencyA && recipe.currencyB <= currencyB)
        {
            UpdateCurrency(-recipe.currencyA, -recipe.currencyB, false);

            PayStartEvent?.Invoke(slot);
            slot.setRecipe(recipe);
            PayEndEvent?.Invoke(slot);

            RecipesReveal.TryGetValue(recipe.id, out var revealedRecipes);
            foreach (var revealedRecipeId in revealedRecipes)
            {
                recipes.Find(r => r.id == revealedRecipeId).SetRevealed(true);
            }
        }
    }

    public (int,int) UpdateCurrency(int deltaCurrencyA, int deltaCurrencyB, bool needMult = true)
    {
        var deltaCurrencyAMulted = needMult ? deltaCurrencyA * currencyAMult : deltaCurrencyA;
        var deltaCurrencyBMulted = needMult ? deltaCurrencyB * currencyBMult : deltaCurrencyB;

        currencyA += deltaCurrencyAMulted;
        if (currencyA > currencyABaseMax) currencyA = currencyABaseMax;

        currencyB += deltaCurrencyBMulted;
        if (currencyB > currencyBBaseMax) currencyB = currencyBBaseMax;

        UpdateCurrencyText();

        foreach (var recipe in recipes)
        {
            recipe.CheckAndSetAvailable(currencyA, currencyB);
        }

        return (deltaCurrencyAMulted, deltaCurrencyBMulted);
    }

    void UpdateCurrencyMult(int deltaMultCurrencyA, int deltaMultCurrencyB, bool division = false)
    {
        if (division)
        {
            currencyAMult /= deltaMultCurrencyA;
            currencyBMult /= deltaMultCurrencyB;
        }
        else
        {
            currencyAMult *= deltaMultCurrencyA;
            currencyBMult *= deltaMultCurrencyB;
        }

        UpdateCurrencyText();
    }

    void UpdateMaxCurrency(int deltaMaxCurrency1, int deltaMaxCurrency2, bool division = false)
    {
        if (division)
        {
            currencyABaseMax /= deltaMaxCurrency1;
            currencyBBaseMax /= deltaMaxCurrency2;

        } else
        {
            currencyABaseMax *= deltaMaxCurrency1;
            currencyBBaseMax *= deltaMaxCurrency2;
        }
        
        
        if (currencyA > currencyABaseMax) currencyA = currencyABaseMax;
        if (currencyB > currencyBBaseMax) currencyB = currencyBBaseMax;

        UpdateCurrencyText();

        foreach (var recipe in recipes)
        {
            recipe.CheckAndSetAvailable(currencyA, currencyB);
        }
    }

    void UpdateCurrencyText()
    {
        Currency1Component.text = currencyA.ToString() + "/" + currencyABaseMax.ToString();
        Currency2Component.text = currencyB.ToString() + "/" + currencyBBaseMax.ToString();
    }

    Recipe InitializeRecipe(
        int id, int currencyA, int currencyB, int lifeTime, int improvedLifeTime, string nameText, string descriptionText, string improvedDescriptionText,
        Recipe.ImprovedAction improvedAction, Recipe.StartAction startAction, Recipe.EndAction endAction)
    {
        var recipeSlotId = RecipesOrder.FirstOrDefault(x => x.Value == id).Key;
        var recipePrefabObject = recipesContainer.transform.Find("RecipePrefab" + recipeSlotId.ToString()).gameObject;

        var recipe = recipePrefabObject.GetComponent<Recipe>();
        recipe.gameController = this;
        recipe.id = id;
        recipe.nameText = nameText;
        recipe.descriptionText = descriptionText;
        recipe.improvedDescriptionText = improvedDescriptionText;
        recipe.currencyA = currencyA;
        recipe.currencyB = currencyB;
        recipe.lifeTime = lifeTime;
        recipe.improvedLifeTime = improvedLifeTime;
        recipe.improvedAction = improvedAction;
        recipe.startAction = startAction;
        recipe.endAction = endAction;
        recipe.InitTooltip();
        if (RecipesRevealedOnStart.Contains(id))
        {
            recipe.SetRevealed(true);
        } else
        {
            recipe.SetRevealed(false);
        }

        recipe.CheckAndSetAvailable(currencyA, currencyB);

        return recipe;
    }

    IEnumerator MainCoroutine()
    {
        while (true)
        {
            if (!HasFilledSlot() && currencyA < 3) {
                UpdateCurrency(1, 0, false);
            }
            
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

    public void togglePause()
    {
        Time.timeScale = Time.timeScale == 0f ? 1f : 0f;
    }

    public string makeTimeString(int t)
    {
        string minutes = ((int)(t / 60)).ToString();
        string seconds = (t % 60).ToString();
        if(seconds.Length == 1)
        {
            seconds = "0" + seconds;
        }
        return minutes + ":" + seconds;
    }

}
