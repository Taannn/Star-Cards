using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
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
    private float returnSpeed = 0.01f;
    private bool btnPlayClicked = false;
    private bool btnBackClicked = true;

    void Start()
    {
        initialAnchorPointEmpire = empireBackground.rectTransform.position;
        initialAnchorPointAlliance = allianceBackground.rectTransform.position;
        anchorPointEmpire = new Vector2(0, Screen.height/2);
        anchorPointAlliance = new Vector2(Screen.width, Screen.height / 2);
    }

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

    public void GoToFomeNameUser()
    {
        playButton.SetActive(false);
        ladderButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
        inputName.gameObject.SetActive(true);
        confirmNameButton.SetActive(true);
    }

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

    public void GoToScreenLadder()
    {
        playButton.SetActive(false);
        ladderButton.SetActive(false);
        quitButton.SetActive(false);
        backButton.SetActive(true);
        ladderContainer.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartGameAlliance()
    {
        StaticClass.InfluenceValue = 0.9f;
        SceneManager.LoadScene("GameScene");
    }

    public void StartGameEmpire()
    {
        StaticClass.InfluenceValue = 0.1f;
        SceneManager.LoadScene("GameScene");
    }
}
