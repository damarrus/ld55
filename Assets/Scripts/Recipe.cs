using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class Recipe : MonoBehaviour
{
    public GameController gameController;
    public int id = 0;
    public string nameText = "";
    public int currency1 = 0;
    public int currency2 = 0;
    public int lifeTime = 0;

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

    public void CheckAndSetAvailable(int cur1, int cur2)
    {
        var newAvailable = cur1 >= currency1 && cur2 >= currency2;

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
        currency1TextComponent.text = currency1.ToString();

        var currency2TextComponent = transform.Find("RecipeCurrency2").GetComponent<TMP_Text>();
        currency2TextComponent.text = currency2.ToString();
    }
}
