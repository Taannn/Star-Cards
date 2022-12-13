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
    private Queue<int> lastStories = new Queue<int>();

    /// <summary>
    /// The first card is generated.
    /// The initial position of the card is set.
    /// The influence is set according to our faction choice.
    /// The queue of last stories is set with an id which don't exist
    /// </summary>
    void Start()
    {
        lastStories.Enqueue(0);
        lastStories.Enqueue(0);
        lastStories.Enqueue(0);
        textCard.text = "";
        influenceValueSlider.value = StaticClass.InfluenceValue;
        nameText.text = StaticClass.NameUser;
        cardsInJSON = JsonConvert.DeserializeObject<Stories>(jsonFile.text);
        randomCard();
        anchorPoint = imageCard.rectTransform.position;
    }

    /// <summary>
    /// If we don't drag, the card return to its position.
    /// If we game detect that we lost, the defeat card is generated.
    /// </summary>
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

    /// <summary>
    /// During the drag, if the position card is after the limits, the choice text appear.
    /// Else, the text disapear.
    /// </summary>
    /// <param name="eventData"></param>
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

    /// <summary>
    /// If the the user drop the card after the limit, a new story is generated.
    /// Else, the card return to its position.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        isDrag = false;
        if(eventData.position.x < limitLeft)
        {
            //The story wich have -1 for id is the story to ende the game
            if (currentStory.id == -1) 
            {
                SceneManager.LoadScene("MenuScene");
            }
            else
            {
                this.leftChoice();
            }
        }
        else if (eventData.position.x > limitRight)
        {
            //The story wich have -1 for id is the story to ende the game
            //The score is sed to the server
            if (currentStory.id == -1)
            {
                SceneManager.LoadScene("MenuScene");
                StartCoroutine(Upload());
            }
            else
            {
                this.rightChoice();
            }
        }
        textCard.text = "";
        shadeText.SetActive(false);
    }

    /// <summary>
    /// This function is called when we did the left choice.
    /// This function modify the slider value according to the choice.
    /// Also, a new card is generated.
    /// If the current story have a substory, the current story is modified.
    /// </summary>
    private void leftChoice()
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

    /// <summary>
    /// This function is called when we did the right choice.
    /// This function modify the slider value according to the choice.
    /// Also, a new card is generated.
    /// If the current story have a substory, the current story is modified.
    /// </summary>
    private void rightChoice()
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

    /// <summary>
    /// This function generate a random card.
    /// While the card was generate in the 3 last rounds, another card will be generate
    /// Also, while the card is not able to be with our influence, another card will be generate
    /// </summary>
    private void randomCard()
    {
        StoryCard newStory;
        bool isGood = this.influenceValueSlider.value >= 0.5f;
        do
        {
            newStory = cardsInJSON.stories[UnityEngine.Random.Range(0, cardsInJSON.stories.Count)];
        } while (!(!lastStories.Contains(newStory.id) && (newStory.isGood == null || newStory.isGood == isGood)));
        currentStory = newStory;
        lastStories.Enqueue(currentStory.id);
        lastStories.Dequeue();
        imageCard.sprite = Resources.Load<Sprite>("ImageCard/" + currentStory.imageCharacter);
        storyCard.text = currentStory.story;
    }

    /// <summary>
    /// This function generate the defeatCard whan we lost.
    /// This card allow to end the game and go to menu
    /// </summary>
    private void defeatCard()
    {
        influenceValueSlider.value = 0.5f;
        moneyValueSlider.value = 0;
        populationValueSlider.value = 0;
        militaryValueSlider.value = 0;
        currentStory.id = -1;
        currentStory.leftChoice = "Retour au menu";
        currentStory.rightChoice = "Retour au menu + envoi du score";
        imageCard.sprite = Resources.Load<Sprite>("ImageCard/Crash");
        storyCard.text = "Vous avez perdu, votre empire est mort";
    }

    /// <summary>
    /// This function upload our score on the server with a request post
    /// </summary>
    /// <returns></returns>
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
    public bool? isGood;
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

