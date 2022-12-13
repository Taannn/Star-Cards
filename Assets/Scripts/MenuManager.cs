using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public Image empireBackground, allianceBackground;
    public GameObject backButton;
    public GameObject playButton;
    public GameObject ladderButton;
    public GameObject quitButton;
    public GameObject confirmNameButton;
    public TMP_InputField inputName;
    public GameObject ladderContainer;
    private Vector2 anchorPointEmpire, anchorPointAlliance, initialAnchorPointEmpire, initialAnchorPointAlliance;
    private float returnSpeed = 0.08f;
    private bool btnPlayClicked = false;
    private bool btnBackClicked = true;

    /// <summary>
    /// Setup the pwo positions available for the buttons to choose a faction.
    /// </summary>
    void Start()
    {
        initialAnchorPointEmpire = empireBackground.rectTransform.position;
        initialAnchorPointAlliance = allianceBackground.rectTransform.position;
        anchorPointEmpire = new Vector2(0, Screen.height/2);
        anchorPointAlliance = new Vector2(Screen.width, Screen.height/2);
    }

    /// <summary>
    /// If we entered a name, an animation is done to show the buttons to choose a faction.
    /// If we go back to the menu, an animation is done to hide these buttons.
    /// </summary>
    void Update()
    {
        if (btnPlayClicked)
        {
            empireBackground.transform.position = Vector2.Lerp(empireBackground.transform.position, anchorPointEmpire, returnSpeed);
            allianceBackground.transform.position = Vector2.Lerp(allianceBackground.transform.position, anchorPointAlliance, returnSpeed);
        }
        else if (btnBackClicked)
        {
            empireBackground.transform.position = Vector2.Lerp(empireBackground.transform.position, initialAnchorPointEmpire, returnSpeed);
            allianceBackground.transform.position = Vector2.Lerp(allianceBackground.transform.position, initialAnchorPointAlliance, returnSpeed);
        }
    }

    /// <summary>
    /// This function change the elements in the scene.
    /// The new shown element is the form to enter a name.
    /// </summary>
    public void GoToFomeNameUser()
    {
        playButton.SetActive(false);
        ladderButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
        inputName.gameObject.SetActive(true);
        confirmNameButton.SetActive(true);
    }

    /// <summary>
    /// This function change the elements in the scene.
    /// The new shown elements are the buttons to lanch a game.
    /// </summary>
    public void GoToScreenFaction()
    {
        if(inputName.text != "")
            StaticClass.NameUser = inputName.text;
        playButton.SetActive(false);
        ladderButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
        inputName.gameObject.SetActive(false);
        confirmNameButton.SetActive(false);
        btnBackClicked = false;
        btnPlayClicked = true;
    }

    /// <summary>
    /// This function change the elements in the scene.
    /// The new shown elements are the menu elements.
    /// </summary>
    public void GoToScreenMenu()
    {
        playButton.SetActive(true);
        ladderButton.SetActive(true);
        quitButton.SetActive(true);
        backButton.SetActive(false);
        ladderContainer.SetActive(false);
        inputName.gameObject.SetActive(false);
        confirmNameButton.SetActive(false);
        btnPlayClicked = false;
        btnBackClicked = true;
    }

    /// <summary>
    /// This function change the elements in the scene.
    /// The new shown elements are the ladder elements.
    /// </summary>
    public void GoToScreenLadder()
    {
        playButton.SetActive(false);
        ladderButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
        ladderContainer.SetActive(true);
    }

    /// <summary>
    /// This function allow to quit the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// This function change the scene.
    /// The scene is load with the influence value that we give.
    /// </summary>
    public void StartGameGood()
    {
        StaticClass.InfluenceValue = 0.9f;
        SceneManager.LoadScene("GameScene");
    }

    /// <summary>
    /// This function change the scene.
    /// The scene is load with the influence value that we give.
    /// </summary>
    public void StartGameBad()
    {
        StaticClass.InfluenceValue = 0.1f;
        SceneManager.LoadScene("GameScene");
    }
}
