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
        }
        for (int i = 0; i < iconsUnknownList.Count; i++)
        {
            iconsUnknownList[i].SetActive((i == id - 1) && !revealed);
        }

        CheckAndSetAvailable(gameController.currencyA, gameController.currencyB);
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
            currency1TextComponent.color = gameController.currencyA >= currencyA ? Color.white : Color.red;

            var currency2TextComponent = coinGemPrefab.transform.Find("RecipeCurrency2").GetComponent<TMP_Text>();
            currency2TextComponent.text = currencyB.ToString();
            currency2TextComponent.color = gameController.currencyB >= currencyB ? Color.white : Color.red;

            coinGemPrefab.SetActive(true);
        }
        else if (currencyA > 0)
        {
            var currency1TextComponent = coinPrefab.transform.Find("RecipeCurrency1").GetComponent<TMP_Text>();
            currency1TextComponent.text = currencyA.ToString();
            currency1TextComponent.color = gameController.currencyA >= currencyA ? Color.white : Color.red;

            coinPrefab.SetActive(true);
        }
        else
        {
            var currency2TextComponent = gemPrefab.transform.Find("RecipeCurrency2").GetComponent<TMP_Text>();
            currency2TextComponent.text = currencyB.ToString();
            currency2TextComponent.color = gameController.currencyB >= currencyB ? Color.white : Color.red;

            gemPrefab.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var tooltipObject = transform.Find("Tooltip").gameObject;
        tooltipObject.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var tooltipObject = transform.Find("Tooltip").gameObject;
        tooltipObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void InitTooltip()
    {
        tooltipObject = transform.Find("Tooltip").gameObject;
        tooltipObject.transform.Find("RecipeName").GetComponent<TMP_Text>().text = nameText;
        tooltipObject.transform.Find("RecipeDescription").GetComponent<TMP_Text>().text = descriptionText + "\n\r" + "Lifetime: " + lifeTime.ToString();
    }

}
