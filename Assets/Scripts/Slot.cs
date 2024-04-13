using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameController gameController;
    public Recipe recipe = null;
    public int lifeTimeLeft = 0;
    public Coroutine ltCoroutine = null;
    public bool active = true;
    public Coroutine actionCoroutine = null;
    public bool isImproved = false;


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
        var nameTextComponent = transform.Find("RecipeName").GetComponent<TMP_Text>();
        var lifeTimeTextComponent = transform.Find("LifeTime").GetComponent<TMP_Text>();


        if (rec != null )
        {
            



            nameTextComponent.text = rec.nameText;
            lifeTimeLeft = rec.lifeTime;
            if (Random.Range(0, 100) < 20)
            {
                isImproved = true;
                rec.improvedAction(this);
                nameTextComponent.color = Color.green;
            }
            lifeTimeTextComponent.text = lifeTimeLeft.ToString();
            ltCoroutine = StartCoroutine(lifeTimeCoroutine());
            rec.startAction(this);
        } else
        {
            nameTextComponent.text = "Empty";
            nameTextComponent.color = Color.white;
            lifeTimeTextComponent.text = "";
            if (!active)
            {
                setActive(false);
            }

            StopCoroutine(ltCoroutine);
            recipe.endAction(this);
            isImproved = false;
        }

        recipe = rec;
    }

    IEnumerator lifeTimeCoroutine()
    {
        while (lifeTimeLeft > 0)
        {
            var lifeTimeTextComponent = transform.Find("LifeTime").GetComponent<TMP_Text>();
            lifeTimeTextComponent.text = lifeTimeLeft.ToString();
            yield return new WaitForSeconds(1f);
            recipe.duringAction(this);

            --lifeTimeLeft;
        }
        setRecipe(null);
    }
}
