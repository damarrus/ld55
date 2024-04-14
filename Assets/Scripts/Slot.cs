using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Slot : MonoBehaviour
{
    public GameController gameController;
    public GameObject flyOutPrefab;

    public int id = 0;
    public Recipe recipe = null;
    public int lifeTimeLeft = 0;
    public int lifeTimeMax = 0;
    public Coroutine ltCoroutine = null;
    public bool active = true;
    public Coroutine actionCoroutine = null;
    public bool isImproved = false;
    public int paramInt = 0;
    public List<int> paramListInt = new List<int>();

    public List<GameObject> prefabList;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void createFlyOut(int number, bool isA)
    {
        var flyOutObject = Instantiate(flyOutPrefab, transform);
        flyOutObject.transform.position = transform.position;
        var tmpText = flyOutObject.GetComponentInChildren<TMP_Text>();
        tmpText.text = number.ToString();
        tmpText.color = isA ? Color.green : Color.blue;
        StartCoroutine(flyOutCoroutine(flyOutObject));
    }

    IEnumerator flyOutCoroutine(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }

    public void setActive(bool act)
    {
        active = act;
        var canvas = transform.Find("Canvas");
        var nameTextComponent = canvas.Find("RecipeName").GetComponent<TMP_Text>();
        var lifeTimeTextComponent = canvas.Find("LifeTime").GetComponent<TMP_Text>();
        var pentagramTextComponent = canvas.Find("Pentagram").GetComponent<TMP_Text>();

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

    public void setRecipe(Recipe rec, int improvedChance = -1)
    {
        var canvas = transform.Find("Canvas");
        var nameTextComponent = canvas.Find("RecipeName").GetComponent<TMP_Text>();
        var lifeTimeTextComponent = canvas.Find("LifeTime").GetComponent<TMP_Text>();

        if (rec != null )
        {
            nameTextComponent.text = rec.nameText;
            improvedChance = improvedChance == -1 ? gameController.improveChance : improvedChance;
            
            if (Random.Range(0, 100) < improvedChance)
            {
                isImproved = true;
                rec.improvedAction(this);
                nameTextComponent.color = Color.green;
            }
            increaseLifeTime(isImproved ? rec.improvedLifeTime : rec.lifeTime);
            lifeTimeTextComponent.text = lifeTimeLeft.ToString();
            ltCoroutine = StartCoroutine(lifeTimeCoroutine());
            recipe = rec;

            for (int i = 0; i < prefabList.Count; i++)
            {
                prefabList[i].SetActive(i == rec.id - 1);
            }

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
            lifeTimeLeft = 0;
            lifeTimeMax = 0;

            for (int i = 0; i < prefabList.Count; i++)
            {
                prefabList[i].SetActive(false);
            }
        }
    }

    public void increaseLifeTime(int lifeTimeDelta)
    {
        lifeTimeLeft += lifeTimeDelta;
        lifeTimeMax += lifeTimeDelta;
        updateLifeTimeLeft();
    }
    
    public void updateLifeTimeLeft()
    {
        var canvas = transform.Find("Canvas");
        var lifeTimeTextComponent = canvas.Find("LifeTime").GetComponent<TMP_Text>();
        lifeTimeTextComponent.text = lifeTimeLeft.ToString();
    }

    IEnumerator lifeTimeCoroutine()
    {
        while (lifeTimeLeft > 0)
        {
            updateLifeTimeLeft();
            yield return new WaitForSeconds(1f);

            --lifeTimeLeft;
        }
        setRecipe(null);
    }
}
