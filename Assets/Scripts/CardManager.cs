using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CardManager : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public Slider moneyValueSlider;
    public Slider populationValueSlider;
    public Slider militaryValueSlider;
    public Slider influenceValueSlider;
    public TextAsset jsonFile;
    public Stories cardsInJSON;
    public StoryCard currentStory;
    public Image imageCard;
    public GameObject shadeText;
    public TextMeshProUGUI textCard;
    public TextMeshProUGUI storyCard;
    public TextMeshProUGUI yearsText;
    int numberYears = 0;
    public TextMeshProUGUI nameText;
    private bool isDrag = false;
    private Vector3 anchorPoint;
    public float returnSpeed = 0.1f;
    public float rotationSpeed = 0.03f;
    public float tranlationYPower = 0.2f;
    private double limitLeft = Screen.width * 0.3;
    private double limitRight = Screen.width * 0.7;
    private double limitTextLeft = Screen.width * 0.45;
    private double limitTextRight = Screen.width * 0.55;

    private void Awake()
    {
        influenceValueSlider.value = StaticClass.InfluenceValue;
        nameText.text = StaticClass.NameUser;
        imageCard = GetComponentsInChildren<Image>().First();
    }

    void Start()
    {
        textCard.text = "";
        cardsInJSON = JsonConvert.DeserializeObject<Stories>(jsonFile.text);
        randomCard();
        anchorPoint = imageCard.rectTransform.position;
    }

    void Update()
    {
        if (!isDrag)
        {
            imageCard.transform.position = Vector2.Lerp(imageCard.transform.position, anchorPoint, returnSpeed);
            imageCard.transform.rotation = Quaternion.Lerp(imageCard.transform.rotation, Quaternion.Euler(0, 0, 0), returnSpeed);
        }
        if(populationValueSlider.value <= 0 || militaryValueSlider.value <= 0 || moneyValueSlider.value <= 0
            || populationValueSlider.value >= 1 || militaryValueSlider.value >= 1 || moneyValueSlider.value >= 1)
        {
            defeatCard();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        isDrag = true;
    }

    public void OnDrag(PointerEventData eventData)
    {
        imageCard.transform.position = new Vector2(eventData.position.x, anchorPoint.y - (anchorPoint.y - eventData.position.y) * tranlationYPower);
        imageCard.transform.rotation = Quaternion.Euler(0, 0, (anchorPoint.x - eventData.position.x) * rotationSpeed);
        if (eventData.position.x < limitTextLeft)
        {
            shadeText.SetActive(true);
            textCard.alignment = TextAlignmentOptions.Right;
            textCard.text = currentStory.leftChoice;
        }
        else if (eventData.position.x > limitTextRight)
        {
            shadeText.SetActive(true);
            textCard.alignment = TextAlignmentOptions.Left;
            textCard.text = currentStory.rightChoice;
        }
        else
        {
            shadeText.SetActive(false);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        if(eventData.position.x < limitLeft)
        {
            if (currentStory.id == -1) 
            {
                SceneManager.LoadScene("MenuScene");
            }
            else
            {
                influenceValueSlider.value += (float)currentStory.influenceLeft;
                moneyValueSlider.value += (float)currentStory.moneyLeft;
                populationValueSlider.value += (float)currentStory.populationLeft;
                militaryValueSlider.value += (float)currentStory.militaryLeft;
                if (currentStory.storyLeft != null)
                {
                    currentStory = currentStory.storyLeft;
                    imageCard.sprite = Resources.Load<Sprite>("ImageCard/" + currentStory.imageCharacter);
                    storyCard.text = currentStory.story;
                }
                else
                {
                    randomCard();
                    numberYears++;
                    yearsText.text = numberYears.ToString();
                }
            }
        }
        else if (eventData.position.x > limitRight)
        {
            if (currentStory.id == -1)
            {
                SceneManager.LoadScene("MenuScene");
                StartCoroutine(Upload());
            }
            else
            {
                influenceValueSlider.value += (float)currentStory.influenceRight;
                moneyValueSlider.value += (float)currentStory.moneyRight;
                populationValueSlider.value += (float)currentStory.populationRight;
                militaryValueSlider.value += (float)currentStory.militaryRight;
                if (currentStory.storyRight != null)
                {
                    currentStory = currentStory.storyRight;
                    imageCard.sprite = Resources.Load<Sprite>("ImageCard/" + currentStory.imageCharacter);
                    storyCard.text = currentStory.story;
                }
                else
                {
                    randomCard();
                    numberYears++;
                    yearsText.text = numberYears.ToString();
                }
            }
        }
        textCard.text = "";
        shadeText.SetActive(false);
    }

    private void randomCard()
    {
        StoryCard newStory;
        do
        {
            newStory = cardsInJSON.stories[UnityEngine.Random.Range(0, cardsInJSON.stories.Count)];
        } while (newStory.id == currentStory.id);
        currentStory = newStory;
        imageCard.sprite = Resources.Load<Sprite>("ImageCard/" + currentStory.imageCharacter);
        storyCard.text = currentStory.story;
    }

    private void defeatCard()
    {
        influenceValueSlider.value = 50;
        moneyValueSlider.value = 0;
        populationValueSlider.value = 0;
        militaryValueSlider.value = 0;
        currentStory.id = -1;
        currentStory.leftChoice = "Retour au menu";
        currentStory.rightChoice = "Retour au menu + envoi du score";
        imageCard.sprite = Resources.Load<Sprite>("ImageCard/Crash");
        storyCard.text = "Vous avez perdu, votre royaume est mort";
    }

    IEnumerator Upload()
    {
        WWWForm form = new WWWForm();
        form.AddField("name", StaticClass.NameUser);
        form.AddField("number_years", numberYears);

        using (UnityWebRequest www = UnityWebRequest.Post(StaticClass.ApiURL, form))
        {
            yield return www.SendWebRequest();
        }
    }
}

[System.Serializable]
public class StoryCard
{
    public int id;
    public string story;
    public string leftChoice;
    public string rightChoice;
    public string imageCharacter;
    public double influenceLeft;
    public double moneyLeft;
    public double populationLeft;
    public double militaryLeft;
    public double influenceRight;
    public double moneyRight;
    public double populationRight;
    public double militaryRight;
    public StoryCard storyLeft;
    public StoryCard storyRight;
}

[System.Serializable]
public class Stories
{
    public List<StoryCard> stories;
}

