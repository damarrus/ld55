using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameController gameController;
    public Recipe recipe = null;
    public int lifeTimeLeft = 0;
    public Coroutine coroutine = null;
    public bool active = true;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setActive(bool act)
    {
        active = act;
        var nameTextComponent = transform.Find("RecipeName").GetComponent<TMP_Text>();
        var lifeTimeTextComponent = transform.Find("LifeTime").GetComponent<TMP_Text>();

        if (!active && recipe == null)
        {
            nameTextComponent.text = "Blocked";
            lifeTimeTextComponent.text = "";
        }

        if (active)
        {
            nameTextComponent.text = "Empty";
            lifeTimeTextComponent.text = "";
        }
    }

    public void setRecipe(Recipe rec)
    {
        recipe = rec;

        var nameTextComponent = transform.Find("RecipeName").GetComponent<TMP_Text>();
        var lifeTimeTextComponent = transform.Find("LifeTime").GetComponent<TMP_Text>();


        if (recipe != null )
        {
            nameTextComponent.text = rec.nameText;
            lifeTimeLeft = rec.lifeTime;
            lifeTimeTextComponent.text = lifeTimeLeft.ToString();
            coroutine = StartCoroutine(lifeTimeCoroutine());
        } else
        {
            nameTextComponent.text = "Empty";
            lifeTimeTextComponent.text = "";
            if (!active)
            {
                setActive(false);
            }

            StopCoroutine(coroutine);
        }
    }

    IEnumerator lifeTimeCoroutine()
    {
        while (lifeTimeLeft > 0)
        {
            var lifeTimeTextComponent = transform.Find("LifeTime").GetComponent<TMP_Text>();
            lifeTimeTextComponent.text = lifeTimeLeft.ToString();
            yield return new WaitForSeconds(1f);
            

            --lifeTimeLeft;
        }
        setRecipe(null);
    }
}