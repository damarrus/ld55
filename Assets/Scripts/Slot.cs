using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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
    public GameObject tooltipObject;

    private void Start()
    {
        foreach (var item in GetComponentsInChildren<Canvas>())
        {
            item.worldCamera = Camera.main;
        }
    }

    public void createFlyOut(int number, bool isA)
    {
        var flyOutObject = Instantiate(flyOutPrefab, transform);
        flyOutObject.transform.position = transform.position;
        flyOutObject.transform.localPosition = Vector3.zero;

        var tmpText = flyOutObject.GetComponentInChildren<TMP_Text>();
        tmpText.text = (number > 0 ? "+" : "") + number.ToString();
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
                if (i == rec.id - 1)
                {
                    prefabList[i].transform.Find("upgrade").gameObject.SetActive(isImproved);
                    if (Random.Range(0, 100) < 50)
                    {
                        prefabList[i].transform.localScale = new Vector3(-prefabList[i].transform.localScale.x, prefabList[i].transform.localScale.y, prefabList[i].transform.localScale.z);
                    }
                }
            }

            rec.startAction(this);
            InitTooltip();
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
            var tooltipObject = transform.Find("Canvas").Find("Tooltip").gameObject;
            tooltipObject.SetActive(false);
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

    public void EyePayHandlerMethod(Slot slot)
    {
        var (realDeltaA, realDeltaB) = gameController.UpdateCurrency(0, gameController.eyeSummonedAddB);
        createFlyOut(realDeltaB, false);
    }

    public void EyeImprovedPayHandlerMethod(Slot slot)
    {
        var (realDeltaA, realDeltaB) = gameController.UpdateCurrency(0, gameController.eyeImprovedSummonedAddB);
        createFlyOut(realDeltaB, false);
    }

    public void Die()
    {
        if (recipe != null && recipe.id != 4)
        {
            setRecipe(null);
        }
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (recipe == null) return;
        
        var tooltipObject = transform.Find("Canvas").Find("Tooltip").gameObject;
        tooltipObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        var tooltipObject = transform.Find("Canvas").Find("Tooltip").gameObject;
        tooltipObject.SetActive(false);
    }

    public void InitTooltip()
    {
        tooltipObject.transform.Find("RecipeName").GetComponent<TMP_Text>().text = recipe.nameText;
        if (isImproved) tooltipObject.transform.Find("RecipeName").GetComponent<TMP_Text>().color = Color.green;

        tooltipObject.transform.Find("RecipeDescription").GetComponent<TMP_Text>().text = isImproved
            ? recipe.improvedDescriptionText + "\n\r" + "Lifetime: " + recipe.improvedLifeTime.ToString()
            : recipe.descriptionText + "\n\r" + "Lifetime: " + recipe.lifeTime.ToString();
    }
}
