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

    public int localLifetimeMultiplier = 1;

    public GameObject flamesRed;
    public GameObject flamesBlack;
    public GameObject destroyFlames;
    public GameObject primePrefab;

    Coroutine toolTipCoroutine = null;
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
        flyOutObject.transform.localPosition = Vector3.zero + new Vector3(0f, 0.3f,0f);

        var tmpText = flyOutObject.GetComponentInChildren<TMP_Text>();
        tmpText.text = (number > 0 ? "+" : "") + number.ToString();
        tmpText.color = isA ? Color.black : new Color(188f/255f, 42f/255f, 39f/255f, 1f);
        StartCoroutine(flyOutCoroutine(flyOutObject));
    }

    IEnumerator flyOutCoroutine(GameObject obj)
    {
        yield return new WaitForSeconds(2f);
        Destroy(obj);
    }

    public void setActive(bool act)
    {
        active = act;
        var canvas = transform.Find("Canvas");
        var nameTextComponent = canvas.Find("RecipeName").GetComponent<TMP_Text>();
        var lifeTimeTextComponent = canvas.Find("LifeTime").GetComponent<TMP_Text>();
        var pentagramObject = transform.Find("PentagramImage").gameObject;

        if (!active)
        {
            pentagramObject.SetActive(false);
        }

        if (!active && recipe == null)
        {
            nameTextComponent.text = "Blocked";
            nameTextComponent.color = Color.red;
            pentagramObject.SetActive(false);
        }

        if (active)
        {
            nameTextComponent.text = "Empty";
            nameTextComponent.color = Color.white;
            lifeTimeTextComponent.text = "";
            pentagramObject.SetActive(true);
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
                nameTextComponent.color = new Color(188f/255f, 42f/255f, 39f/255f, 1f);
            }

            lifeTimeLeft = isImproved ? rec.improvedLifeTime : rec.lifeTime;
            lifeTimeMax = isImproved ? rec.improvedLifeTime : rec.lifeTime;
            updateLifeTimeLeft();

            lifeTimeTextComponent.text = lifeTimeLeft.ToString();
            ltCoroutine = StartCoroutine(lifeTimeCoroutine());
            recipe = rec;

            for (int i = 0; i < prefabList.Count; i++)
            {
                //prefabList[i].SetActive(i == rec.id - 1);
                if (i == rec.id - 1)
                {
                    StartCoroutine(ActivateObjectAfterDelay(prefabList[i], 0.3f));
                    prefabList[i].transform.Find("upgrade").gameObject.SetActive(isImproved);
                    if (isImproved)
                    {
                        flamesRed.SetActive(true);
                        GameObject prime = Instantiate(primePrefab, new Vector3(-3f, 0.25f, 0f), Quaternion.identity );
                        Destroy(prime, 0.75f);
                    }
                    else
                    {
                        flamesBlack.SetActive(true);
                    }

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
            localLifetimeMultiplier = 1;

            for (int i = 0; i < prefabList.Count; i++)
            {
                StartCoroutine(ScaleDownAndRestore(prefabList[i]));
                //prefabList[i].SetActive(false);
            }
            var tooltipObject = transform.Find("Canvas").Find("Tooltip").gameObject;
            tooltipObject.SetActive(false);
            gameController.setHasEmptySlot();
        }
    }
    
    IEnumerator ActivateObjectAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        obj.SetActive(true);
    }
    
    IEnumerator ScaleDownAndRestore(GameObject obj)
    {
        Animator animator = obj.GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }
        Vector3 initialScale = obj.transform.localScale;
        float timer = 0f;
        float duration = 0.2f;
        while (timer + 0.03f < duration)
        {
            float t = timer / duration;
            obj.transform.localScale = Vector3.Lerp(initialScale, Vector3.zero, t);
            timer += Time.deltaTime;
            yield return null;
        }
        obj.transform.localScale = Vector3.zero; 
        
        obj.transform.localScale = initialScale;
        if (animator != null)
        {
            animator.enabled = true;
        }
        obj.SetActive(false);
        destroyFlames.SetActive(true);
    }
    
    public void increaseLifeTime(int newSnakeLifetimeMultiplier)
    {
        var lifeTimeMult = newSnakeLifetimeMultiplier - localLifetimeMultiplier;
        if (lifeTimeMult > 0)
        {
            var lifeTimeDelta = (isImproved ? recipe.improvedLifeTime : recipe.lifeTime) * lifeTimeMult;
            lifeTimeLeft += lifeTimeDelta;
            lifeTimeMax += lifeTimeDelta;
            localLifetimeMultiplier = newSnakeLifetimeMultiplier;
        }

        updateLifeTimeLeft();
    }
    
    public void updateLifeTimeLeft()
    {
        var canvas = transform.Find("Canvas");
        var lifeTimeTextComponent = canvas.Find("LifeTime").GetComponent<TMP_Text>();
        lifeTimeTextComponent.text = gameController.makeTimeString(lifeTimeLeft).ToString();
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

    public void SnakePayHandlerMethod(Slot slot)
    {
        slot.increaseLifeTime(gameController.globalLifetimeMultiplier);
    }

    public void Die()
    {
        if (recipe != null && recipe.id != 4)
        {
            setRecipe(null);
        }
        
    }

    IEnumerator showToolTip()
    {
        yield return new WaitForSeconds(0.5f);
        var tooltipObject = transform.Find("Canvas").Find("Tooltip").gameObject;
        tooltipObject.SetActive(true);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (recipe == null) return;

        toolTipCoroutine = StartCoroutine(showToolTip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (toolTipCoroutine != null)
        {
            StopCoroutine(toolTipCoroutine);
        }

        var tooltipObject = transform.Find("Canvas").Find("Tooltip").gameObject;
        tooltipObject.SetActive(false);
    }

    public void InitTooltip()
    {
        tooltipObject.transform.Find("RecipeName").GetComponent<TMP_Text>().text = isImproved ? recipe.improvedNameText : recipe.nameText;
        if (isImproved)
        {
            tooltipObject.transform.Find("RecipeName").GetComponent<TMP_Text>().color = new Color(188f / 255f, 42f / 255f, 39f / 255f, 1f);
        }
        else
        {
            tooltipObject.transform.Find("RecipeName").GetComponent<TMP_Text>().color = Color.black;
        }

        var realLifeTimeMult = recipe.id == 7
            ? isImproved ? gameController.snakeImprovedMultLifetime : gameController.snakeMultLifetime
            : 1;

        tooltipObject.transform.Find("RecipeDescription").GetComponent<TMP_Text>().text = isImproved
            ? recipe.improvedDescriptionText + "\n\r" + "Lifetime: " + gameController.makeTimeString(recipe.improvedLifeTime * realLifeTimeMult).ToString()
            : recipe.descriptionText + "\n\r" + "Lifetime: " + gameController.makeTimeString(recipe.lifeTime * realLifeTimeMult).ToString();

        if (recipe.id == 4)
        {
            tooltipObject.transform.Find("Button").gameObject.SetActive(false);
        }
    }
}
