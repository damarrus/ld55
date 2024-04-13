using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameController gameController;
    public int id = 0;
    public Recipe recipe = null;
    public int lifeTimeLeft = 0;
    public Coroutine ltCoroutine = null;
    public bool active = true;
    public Coroutine actionCoroutine = null;
    public bool isImproved = false;
    public int paramInt = 0;
    public List<int> paramListInt = new List<int>();

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
        var pentagramTextComponent = transform.Find("Pentagram").GetComponent<TMP_Text>();

        if (!active)
        {
            pentagramTextComponent.text = "";
        }

        if (!active && recipe == null)
        {
            nameTextComponent.text = "Blocked";
            nameTextComponent.color = Color.red;
            lifeTimeTextComponent.text = "";
        }

        if (active)
        {
            nameTextComponent.text = "Empty";
            nameTextComponent.color = Color.white;
            lifeTimeTextComponent.text = "";
            pentagramTextComponent.text = "*";
        }
    }

    public void setRecipe(Recipe rec, int improvedChance = 20)
    {
        var nameTextComponent = transform.Find("RecipeName").GetComponent<TMP_Text>();
        var lifeTimeTextComponent = transform.Find("LifeTime").GetComponent<TMP_Text>();


        if (rec != null )
        {
            nameTextComponent.text = rec.nameText;
            lifeTimeLeft = rec.lifeTime;
            if (Random.Range(0, 100) < improvedChance)
            {
                isImproved = true;
                rec.improvedAction(this);
                nameTextComponent.color = Color.green;
            }
            lifeTimeTextComponent.text = lifeTimeLeft.ToString();
            ltCoroutine = StartCoroutine(lifeTimeCoroutine());
            recipe = rec;
            rec.startAction(this);
        } else
        {
            nameTextComponent.text = "Empty";
            nameTextComponent.color = Color.white;
            lifeTimeTextComponent.text = "";
            
            StopCoroutine(ltCoroutine);
            recipe.endAction(this);
            isImproved = false;
            recipe = rec;
            if (!active)
            {
                setActive(false);
            }
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
