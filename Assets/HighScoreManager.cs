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
    /// <summary>
    /// Coroutine to fetch high scores from the server using an HTTP GET request.
    /// </summary>
    /// <returns>An IEnumerator for coroutine compatibility.</returns>
    public IEnumerator FetchHighScores()
    {
        // Create a GET request using the API URL
        using UnityWebRequest webRequest = UnityWebRequest.Get(apiUrl);
        // Send the request and wait for the response
        yield return webRequest.SendWebRequest();

        // Check for network errors
        if (webRequest.isNetworkError)
        {
            // Log error to the console
            Debug.LogError("Error: " + webRequest.error);
        }
        else
        {
            // Process the JSON data received from the server
            ProcessHighScores(webRequest.downloadHandler.text);
        }
    }

    private void ProcessHighScores(string json)
    {
        // Using Newtonsoft.Json to directly deserialize the top-level JSON array
        List<HighScore> highScores = JsonConvert.DeserializeObject<List<HighScore>>(json);

        if (highScores != null && highScores.Count > 0)
        {
            highScores = highScores.OrderBy(hs => hs.Score).Take(10).ToList();

            for (int i = 0; i < highScoreTexts.Count && i < highScores.Count; i++)
            {
                highScoreTexts[i].text = $"{i + 1}. {highScores[i].Name} - {(int)highScores[i].Score} ({highScores[i].DateCreated.ToString("MM/dd/yy")})";
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
