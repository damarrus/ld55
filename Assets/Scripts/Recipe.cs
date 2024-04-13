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

    public void SetRevealed()
    {
        if (!revealed)
        {
            revealed = true;
            gameObject.SetActive(true);
        }
    }

    public void CheckAndSetAvailable(int cur1, int cur2)
    {
        var newAvailable = cur1 >= currencyA && cur2 >= currencyB;

        if (newAvailable != available)
        {
            var nameTextComponent = transform.Find("RecipeName").GetComponent<TMP_Text>();
            nameTextComponent.color = newAvailable ? Color.green : Color.red;

            available = newAvailable;
        }
    }

    public void UpdateText()
    {
        var nameTextComponent = transform.Find("RecipeName").GetComponent<TMP_Text>();
        nameTextComponent.text = nameText;
        nameTextComponent.color = Color.red;


        var currency1TextComponent = transform.Find("RecipeCurrency1").GetComponent<TMP_Text>();
        currency1TextComponent.text = currencyA.ToString();

        var currency2TextComponent = transform.Find("RecipeCurrency2").GetComponent<TMP_Text>();
        currency2TextComponent.text = currencyB.ToString();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        var tooltipObject = transform.Find("Tooltip").gameObject;
        tooltipObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var tooltipObject = transform.Find("Tooltip").gameObject;
        tooltipObject.SetActive(false);
    }

    public void InitTooltip()
    {
        tooltipObject = transform.Find("Tooltip").gameObject;
        tooltipObject.transform.Find("RecipeName").GetComponent<TMP_Text>().text = nameText;
        tooltipObject.transform.Find("RecipeDescription").GetComponent<TMP_Text>().text = descriptionText + "\n\r" + "Lifetime: " + lifeTime.ToString();
    }

}
