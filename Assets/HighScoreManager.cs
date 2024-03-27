using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class HighScoreManager : MonoBehaviour
{
    public GameObject highScorePanel; // Assign in inspector
    public List<Text> highScoreTexts; // Assign each Text component in inspector
    private readonly string apiUrl = "https://poshared.azurewebsites.net/highscores/top"; // Your API URL

    [System.Obsolete]
    private void Start()
    {
        _ = StartCoroutine(FetchHighScores());
    }

    [System.Obsolete]
    public IEnumerator FetchHighScores()
    {
        using UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl);
        // Request and wait for the desired page.
        yield return webRequest.SendWebRequest();

        if (webRequest.isNetworkError)
        {
            Debug.LogError("Error: " + webRequest.error);
        }
        else
        {
            ProcessHighScores(webRequest.downloadHandler.text);
        }
    }

    private void ProcessHighScores(string json)
    {
        // Using Newtonsoft.Json to directly deserialize the top-level JSON array
        List<HighScore> highScores = JsonConvert.DeserializeObject<List<HighScore>>(json);

        if (highScores != null && highScores.Count > 0)
        {
            highScores = highScores.OrderBy(hs => hs.Score).Take(5).ToList();

            for (int i = 0; i < highScoreTexts.Count && i < highScores.Count; i++)
            {
                highScoreTexts[i].text = $"{i + 1}. {highScores[i].Name} - {(int)highScores[i].Score}";
            }
        }


    }

    // Wrapper class for deserialization
    [System.Serializable]
    private class HighScoreWrapper
    {
        public HighScore[] highScores;
    }
}
