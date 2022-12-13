using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class GetLadder : MonoBehaviour
{
    public Ladder ladder;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetRequest(StaticClass.ApiURL));
    }

    /// <summary>
    /// This function do a request get on the server.
    /// This request returns the first 10 higher scores.
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    ladder = new Ladder();
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    ladder = new Ladder();
                    break;
                case UnityWebRequest.Result.Success:
                    ladder = JsonConvert.DeserializeObject<Ladder>(webRequest.downloadHandler.text);
                    break;
            }
            createLadderChildren();
        }
    }

    /// <summary>
    /// This function create all players receive by the request get.
    /// These players are add the current panel.
    /// </summary>
    private void createLadderChildren()
    {
        int pos = 1;
        GameObject playerTemplate = transform.GetChild(0).gameObject;
        GameObject playerInstance;
        foreach (Player player in ladder.ladder)
        {
            playerInstance = Instantiate(playerTemplate, transform);
            playerInstance.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = pos.ToString();
            playerInstance.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = player.name;
            playerInstance.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = player.number_years.ToString();
            pos++;
        }
    }
}

[System.Serializable]
public class Player
{
    public string name;
    public int number_years;
}

[System.Serializable]
public class Ladder
{
    public List<Player> ladder;
}
