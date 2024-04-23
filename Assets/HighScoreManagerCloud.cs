//using Newtonsoft.Json;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Unity.Services.Authentication;
//using Unity.Services.Core;
//using Unity.Services.Leaderboards;
//using UnityEngine;
//using UnityEngine.UI;

//public class HighScoreManagerCloud : MonoBehaviour
//{
//    private const string LeaderboardId = "PoReflexSquares";

//    //private string VersionId { get; set; }
//    //private int Offset { get; set; }
//    //private int Limit { get; set; }
//    //private int RangeLimit { get; set; }
//    //private List<string> FriendIds { get; set; }

//    public GameObject highScorePanel; // Assign in inspector
//    public List<Text> highScoreTexts; // Assign each Text component in inspector

//    private async void Awake()
//    {
//        await UnityServices.InitializeAsync();
//        await SignInAnonymously();
//    }

//    private void Start()
//    {
//        GetScores();
//    }

//    private async Task SignInAnonymously()
//    {
//        AuthenticationService.Instance.SignedIn += () =>
//        {
//            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
//        };
//        AuthenticationService.Instance.SignInFailed += s =>
//        {
//            Debug.Log(s);
//        };

//        await AuthenticationService.Instance.SignInAnonymouslyAsync();
//    }

//    void SignInGoogle()
//    {

//        //AuthenticationService.Instance.SignedIn += () => {
//        //    // Shows how to get a playerID
//        //    Debug.Log($"PlayerID: {AuthenticationService.Instance.PlayerId}");

//        //    // Shows how to get an access token
//        //    Debug.Log($"Access Token: {AuthenticationService.Instance.AccessToken}");

//        //};

//        //AuthenticationService.Instance.SignInFailed += (err) => {
//        //    Debug.LogError(err);
//        //};

//        //AuthenticationService.Instance.SignedOut += () => {
//        //    Debug.Log("Player signed out.");
//        //};

//        //AuthenticationService.Instance.Expired += () =>
//        //{
//        //    Debug.Log("Player session could not be refreshed and expired.");
//        //};
//    }

//    public async void AddScore(float score)
//    {
//        Unity.Services.Leaderboards.Models.LeaderboardEntry scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(LeaderboardId, score);
//        Debug.Log(JsonConvert.SerializeObject(scoreResponse));
//    }

//    public async void GetScores()
//    {
//        Unity.Services.Leaderboards.Models.LeaderboardScoresPage scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId);
//        var highScores = scoresResponse.Results;

//        if (highScores != null && highScores.Count > 0)
//        {
//            highScores = highScores.OrderBy(hs => hs.Score).Take(10).ToList();

//            for (int i = 0; i < highScoreTexts.Count && i < highScores.Count; i++)
//            {
//                // highScoreTexts[i].text = $"{i + 1}. {highScores[i].Name} - {(int)highScores[i].Score}";
//                // highScoreTexts[i].text = $"{i + 1}. {(int)highScores[i].Score}";
//                highScoreTexts[i].text = $"{i + 1}. {highScores[i].PlayerName} - {(int)highScores[i].Score}";
//            }
//        }

//        Debug.Log(JsonConvert.SerializeObject(scoresResponse));
//    }

//    //public async void GetPaginatedScores()
//    //{
//    //    Offset = 10;
//    //    Limit = 10;
//    //    Unity.Services.Leaderboards.Models.LeaderboardScoresPage scoresResponse =
//    //        await LeaderboardsService.Instance.GetScoresAsync(LeaderboardId, new GetScoresOptions { Offset = Offset, Limit = Limit });
//    //    Debug.Log(JsonConvert.SerializeObject(scoresResponse));
//    //}

//    //public async void GetPlayerScore()
//    //{
//    //    Unity.Services.Leaderboards.Models.LeaderboardEntry scoreResponse =
//    //        await LeaderboardsService.Instance.GetPlayerScoreAsync(LeaderboardId);
//    //    Debug.Log(JsonConvert.SerializeObject(scoreResponse));
//    //}

//    //public async void GetVersionScores()
//    //{
//    //    Unity.Services.Leaderboards.Models.LeaderboardVersionScoresPage versionScoresResponse =
//    //        await LeaderboardsService.Instance.GetVersionScoresAsync(LeaderboardId, VersionId);
//    //    Debug.Log(JsonConvert.SerializeObject(versionScoresResponse));
//    //}
//}
