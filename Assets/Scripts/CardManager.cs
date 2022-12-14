using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
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
    public TextMeshProUGUI nameCharacter;
    public TextMeshProUGUI yearsText;
    public TextMeshProUGUI revolutionsText;
    int numberYears = 0;
    int numberRevolutions = 0;
    public TextMeshProUGUI nameText;
    private bool isDrag = false;
    private bool rotate = false;
    private Vector3 anchorPoint;
    public float returnSpeed = 0.1f;
    public float returnSpeedSliders = 0.5f;
    public float rotationSpeed = 0.03f;
    public float tranlationYPower = 0.2f;
    private double limitTextLeft = Screen.width * 0.45;
    private double limitTextRight = Screen.width * 0.55;
    private Queue<int> lastStories = new Queue<int>();
    private float elapsed = 0;
    private float elapsedDrag = 0;
    private float influenceValue = 0.5f;
    private float moneyValue = 0.5f;
    private float populationValue = 0.5f;
    private float militaryValue = 0.5f;

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
        if(Input.touchCount == 2 && numberRevolutions > 0 && this.currentStory.id != -1)
        {
            isDrag = false;
            Vector3 diff = Input.GetTouch(1).position - Input.GetTouch(0).position;
            float angle = (Mathf.Atan2(diff.y, diff.x));
            shadeText.SetActive(false);
            imageCard.transform.rotation = Quaternion.Euler(0f, 0f, Mathf.Rad2Deg * angle * 4);
        }
        if (!isDrag && !rotate)
        {
            imageCard.transform.position = Vector2.Lerp(imageCard.transform.position, anchorPoint, returnSpeed);
            imageCard.transform.rotation = Quaternion.Lerp(imageCard.transform.rotation, Quaternion.Euler(0, 0, 0), returnSpeed);
        }
        if(populationValue <= 0 || militaryValue <= 0 || moneyValue <= 0
            || populationValue >= 1 || militaryValue >= 1 || moneyValue >= 1)
        {
            defeatCard();
        }
        if (rotate)
        {
            elapsed += Time.deltaTime; // Time.deltaTime return the number of seconds elapsed from last frame, usually ~1/60s
            if (elapsed > 3)
            {
                elapsed = 0;
                makeRevolution();
                rotate = false;
            }
            imageCard.transform.Rotate(0, 0f, 20);
        }
        moneyValueSlider.value = Mathf.Lerp(moneyValueSlider.value, moneyValue, returnSpeedSliders);
        influenceValueSlider.value = Mathf.Lerp(influenceValueSlider.value, influenceValue, returnSpeedSliders);
        populationValueSlider.value = Mathf.Lerp(populationValueSlider.value, populationValue, returnSpeedSliders);
        militaryValueSlider.value = Mathf.Lerp(militaryValueSlider.value, militaryValue, returnSpeedSliders);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(!rotate)
            isDrag = true;
    }

    /// <summary>
    /// During the drag, if the position card is after the limits, the choice text appear.
    /// Else, the text disapear.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(PointerEventData eventData)
    {
        if (isDrag)
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
    }

    /// <summary>
    /// If the the user drop the card after the limit, a new story is generated.
    /// Else, the card return to its position.
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            isDrag = false;
            if (eventData.position.x < limitTextLeft)
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
            else if (eventData.position.x > limitTextRight)
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
        else if(numberRevolutions > 0)
        {
            rotate = true;
        }
    }

    /// <summary>
    /// This function is called when we did the left choice.
    /// This function modify the slider value according to the choice.
    /// Also, a new card is generated.
    /// If the current story have a substory, the current story is modified.
    /// All ten years, a new revolution is available.
    /// </summary>
    private void leftChoice()
    {
        influenceValue += (float)currentStory.influenceLeft;
        moneyValue += (float)currentStory.moneyLeft;
        populationValue += (float)currentStory.populationLeft;
        militaryValue += (float)currentStory.militaryLeft;
        if (currentStory.storyLeft != null)
        {
            currentStory = currentStory.storyLeft;
            imageCard.sprite = Resources.Load<Sprite>("ImageCard/" + currentStory.imageCharacter);
            storyCard.text = currentStory.story;
            nameCharacter.text = currentStory.imageCharacter;
        }
        else
        {
            randomCard();
            numberYears++;
            if(numberYears % 10 == 0)
            {
                numberRevolutions++;
                revolutionsText.text = numberRevolutions.ToString();
            }
            yearsText.text = numberYears.ToString();
        }
    }

    /// <summary>
    /// This function is called when we did the right choice.
    /// This function modify the slider value according to the choice.
    /// Also, a new card is generated.
    /// If the current story have a substory, the current story is modified.
    /// All ten years, a new revolution is available.
    /// </summary>
    private void rightChoice()
    {
        influenceValue += (float)currentStory.influenceRight;
        moneyValue += (float)currentStory.moneyRight;
        populationValue += (float)currentStory.populationRight;
        militaryValue += (float)currentStory.militaryRight;
        if (currentStory.storyRight != null)
        {
            currentStory = currentStory.storyRight;
            imageCard.sprite = Resources.Load<Sprite>("ImageCard/" + currentStory.imageCharacter);
            storyCard.text = currentStory.story;
            nameCharacter.text = currentStory.imageCharacter;
        }
        else
        {
            randomCard();
            numberYears++;
            if (numberYears % 10 == 0)
            {
                numberRevolutions++;
                revolutionsText.text = numberRevolutions.ToString();
            }
            yearsText.text = numberYears.ToString();
        }
    }

    private void makeRevolution() 
    {
        numberRevolutions--;
        revolutionsText.text = numberRevolutions.ToString();
        if(influenceValue > 0.5f)
        {
            influenceValue = 0.2f;
            militaryValue = 0.7f;
            populationValue = 0.3f;
        }
        else
        {
            influenceValue = 0.8f;
            militaryValue = 0.3f;
            populationValue = 0.7f;
        }
        randomCard();
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
        nameCharacter.text = currentStory.imageCharacter;
    }

    /// <summary>
    /// This function generate the defeatCard whan we lost.
    /// This card allow to end the game and go to menu
    /// </summary>
    private void defeatCard()
    {
        influenceValue = 0.5f;
        moneyValue = 0;
        populationValue = 0;
        militaryValue = 0;
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
    public string imageCharacter;
    public string story;
    public string leftChoice;
    public double influenceLeft;
    public double moneyLeft;
    public double populationLeft;
    public double militaryLeft;
    public StoryCard storyLeft;
    public string rightChoice;
    public double influenceRight;
    public double moneyRight;
    public double populationRight;
    public double militaryRight;
    public StoryCard storyRight;
}

[System.Serializable]
public class Stories
{
    public List<StoryCard> stories;
}

