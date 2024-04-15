using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.EventSystems;

public class Recipe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public GameController gameController;
    public int id = 0;
    public string nameText = "";
    public string descriptionText = "";
    public string improvedDescriptionText = "";
    public int currencyA = 0;
    public int currencyB = 0;
    public int lifeTime = 0;
    public int improvedLifeTime = 0;
    public delegate void ImprovedAction(Slot slot);
    public ImprovedAction improvedAction;
    public delegate void StartAction(Slot slot);
    public StartAction startAction;
    public delegate void EndAction(Slot slot);
    public EndAction endAction;
    GameObject tooltipObject = null;

    public List<GameObject> iconsList;
    public List<GameObject> iconsUnknownList;

    public GameObject coinPrefab;
    public GameObject gemPrefab;
    public GameObject coinGemPrefab;

    public bool revealed = false;
    public bool available = false;

    GameObject Icon = null;

    public void Pay()
    {
        gameController.PayRecipe(this);
    }

    public void PayEffect()
    {
        
        gameController.PayRecipe(this);
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetRevealed(bool rev)
    {
        if (rev && revealed) return;

        revealed = rev;

        for (int i = 0; i < iconsList.Count; i++)
        {
            iconsList[i].SetActive((i == id - 1) && revealed);
            if ((i == id - 1) && revealed)
            {
                Icon = iconsList[i];
            }
        }
        for (int i = 0; i < iconsUnknownList.Count; i++)
        {
            iconsUnknownList[i].SetActive((i == id - 1) && !revealed);
            if ((i == id - 1) && !revealed)
            {
                Icon = iconsList[i];
            }
        }

        CheckAndSetAvailable(gameController.currencyA, gameController.currencyB);
    }

    public void setHasEmptySlot(bool hasEmptySlot)
    {
        Icon.GetComponent<SpriteRenderer>().color = hasEmptySlot ? Color.white : new Color(150f / 255f, 150f / 255f, 150f / 255f, 1f);
    }

    public void CheckAndSetAvailable(int cur1, int cur2)
    {
        var newAvailable = cur1 >= currencyA && cur2 >= currencyB;

        if (newAvailable != available)
        {
            available = newAvailable;
        }

        if (!revealed) return;

        if (currencyA > 0 && currencyB > 0)
        {
            var currency1TextComponent = coinGemPrefab.transform.Find("RecipeCurrency1").GetComponent<TMP_Text>();
            currency1TextComponent.text = currencyA.ToString();
            currency1TextComponent.color = gameController.currencyA >= currencyA ? Color.black : new Color(188f/255f, 42f/255f, 39f/255f, 1f);

            var currency2TextComponent = coinGemPrefab.transform.Find("RecipeCurrency2").GetComponent<TMP_Text>();
            currency2TextComponent.text = currencyB.ToString();
            currency2TextComponent.color = gameController.currencyB >= currencyB ? Color.black : new Color(188f/255f, 42f/255f, 39f/255f, 1f);

            coinGemPrefab.SetActive(true);
        }
        else if (currencyA > 0)
        {
            var currency1TextComponent = coinPrefab.transform.Find("RecipeCurrency1").GetComponent<TMP_Text>();
            currency1TextComponent.text = currencyA.ToString();
            currency1TextComponent.color = gameController.currencyA >= currencyA ? Color.black : new Color(188f/255f, 42f/255f, 39f/255f, 1f);

            coinPrefab.SetActive(true);
        }
        else
        {
            var currency2TextComponent = gemPrefab.transform.Find("RecipeCurrency2").GetComponent<TMP_Text>();
            currency2TextComponent.text = currencyB.ToString();
            currency2TextComponent.color = gameController.currencyB >= currencyB ? Color.black : new Color(188f/255f, 42f/255f, 39f/255f, 1f);

            gemPrefab.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!revealed) return;

        var tooltipObject = transform.Find("Tooltip").gameObject;
        tooltipObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!revealed) return;

        var tooltipObject = transform.Find("Tooltip").gameObject;
        tooltipObject.SetActive(false);
    }

    public void InitTooltip()
    {
        var realLifeTimeMult = id == 7 ? gameController.snakeMultLifetime : 1;

        tooltipObject = transform.Find("Tooltip").gameObject;
        tooltipObject.transform.Find("RecipeName").GetComponent<TMP_Text>().text = nameText;
        tooltipObject.transform.Find("RecipeDescription").GetComponent<TMP_Text>().text = descriptionText + "\n\r" + "Lifetime: " + (lifeTime * realLifeTimeMult).ToString();
    }

}
