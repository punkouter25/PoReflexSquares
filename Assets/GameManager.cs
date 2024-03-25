using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] squares;
    public GameObject gameOverModal;
    public Text scoreText;
    public Button startButton; // Reference to the Start button
    public Text countdownTimerText; // For countdown display
    public InputField initialsInputField;

    private bool gameIsActive = false;
    private int currentIndex = -1;
    private readonly List<float> tapTimes = new();
    private float squareActivatedTime;
    private readonly float timeToTap = 2.0f; // Time allowed to tap the square
    private IEnumerator currentCountdownCoroutine; // To keep track of the countdown coroutine
    private bool validScore = false;
    private float averageTime;

    private void Start()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        gameIsActive = false;
        currentIndex = -1;
        tapTimes.Clear();
        gameOverModal.SetActive(false);
        startButton.gameObject.SetActive(true); // Ensure the start button is visible
        countdownTimerText.text = ""; // Clear the countdown timer text
    }

    public void StartGame()
    {
        foreach (GameObject squareObj in squares)
        {
            Square square = squareObj.GetComponent<Square>();
            if (square != null)
            {
                square.ResetSquare(); // Ensure this method resets color and other state
            }
        }

        gameIsActive = true;
        currentIndex = -1;
        gameOverModal.SetActive(false);
        startButton.gameObject.SetActive(false); // Hide the start button during gameplay
        ActivateNextSquareWithDelay();
    }

    private void ActivateNextSquareWithDelay()
    {
        if (currentIndex < squares.Length - 1)
        {
            currentIndex++;
            _ = StartCoroutine(DelaySquareActivation(squares[currentIndex]));

            // After activating the square, start the countdown timer
            if (gameIsActive)
            {
                if (currentCountdownCoroutine != null)
                {
                    StopCoroutine(currentCountdownCoroutine);
                }
                currentCountdownCoroutine = CountdownCoroutine(timeToTap);
                _ = StartCoroutine(currentCountdownCoroutine);
            }
        }
        else
        {
            EndGame(false, false); // Consider what should happen if all squares are successfully tapped
        }
    }

    private IEnumerator DelaySquareActivation(GameObject square)
    {
         // float delay = UnityEngine.Random.Range(0.0f, 0.1f);
          float delay = UnityEngine.Random.Range(0.5f, 1.5f);

        yield return new WaitForSeconds(delay);

        if (gameIsActive)
        {
            squareActivatedTime = Time.time;
            square.GetComponent<Square>().ActivateSquare();
            if (currentCountdownCoroutine != null)
            {
                StopCoroutine(currentCountdownCoroutine);
            }
            currentCountdownCoroutine = CountdownCoroutine(timeToTap);
            _ = StartCoroutine(currentCountdownCoroutine);
        }
    }

    private IEnumerator CountdownCoroutine(float countdownTime)
    {
        float timer = countdownTime;
        while (timer > 0)
        {
            countdownTimerText.text = $"{timer:F1} s";
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;

            // Check if the square was tapped and deactivate the timer if so
            if (!squares[currentIndex].GetComponent<Square>().IsActive)
            {
                yield break; // Exit the coroutine if the square is no longer active
            }
        }
        countdownTimerText.text = "0.0 s";

        // If the square wasn't tapped in time, end the game
        if (gameIsActive && squares[currentIndex].GetComponent<Square>().IsActive)
        {
            Debug.Log("Time's up! Game over.");
            EndGame(earlyTap: false, failed: true);
        }
    }


    public void SquareTapped(int index)
    {
        // Verify if the game is active and if the tapped square is the correct one (the one expected to be tapped next).
        if (gameIsActive)
        {
            Square tappedSquare = squares[index].GetComponent<Square>();

            // Check if the tapped square was active, and if it's the correct one (currentIndex)
            if (tappedSquare.IsActive && index == currentIndex)
            {
                // The tap is valid. Perform the usual logic for a successful tap.
                float reactionTime = (Time.time - squareActivatedTime) * 1000;
                tapTimes.Add(reactionTime);
                tappedSquare.ResetSquare(true, reactionTime);

                // Stop the countdown coroutine and clear the timer text.
                if (currentCountdownCoroutine != null)
                {
                    StopCoroutine(currentCountdownCoroutine);
                    countdownTimerText.text = "";
                }

                // Move to the next square.
                ActivateNextSquareWithDelay();
            }
            else
            {
                // If the tapped square was not active or it wasn't the correct square, it's game over.
                // Whether it was the correct square or not, the game should end since it wasn't active.
                EndGame(earlyTap: true, failed: false);
            }
        }
        else
        {
            // If the game is not active, ignore the tap or consider it as an early tap.
            // You can also use this else block to handle other scenarios if needed.
        }
    }

    void EndGame(bool earlyTap, bool failed)
    {
        Debug.Log("EndGame");
        // Stop the game activity.
        gameIsActive = false;

        // Show the game over modal.
        gameOverModal.SetActive(true);

        // Hide the Start button when the Game Over modal is shown.
        startButton.gameObject.SetActive(false);

        // Determine what text to show based on how the game ended.
        if (earlyTap)
        {
            // Early tap or tap on an inactive square means no score.
            scoreText.text = "GAME OVER: You tapped too soon!";
            initialsInputField.gameObject.SetActive(false); // Hide the input field on early tap
        }
        else if (failed)
        {
            scoreText.text = "GAME OVER: Time's up!";
            initialsInputField.gameObject.SetActive(false); // Show the input field for other failures
        }
        else
        {
            validScore = true;
            // Calculate and display the average reaction time if the game ended normally.
            averageTime = CalculateAverageTime();
            scoreText.text = $"Average Reaction Time: {averageTime:F2} ms";
            initialsInputField.gameObject.SetActive(true); // Ensure the input field is visible for valid game overs
        }

        // Clear the countdown timer text.
        countdownTimerText.text = "";

        // Optionally stop any ongoing coroutines to make sure there are no residual countdowns running.
        if (currentCountdownCoroutine != null)
        {
            StopCoroutine(currentCountdownCoroutine);
        }

        // Clear the times list since the game is over.
        tapTimes.Clear();
    }



    private float CalculateAverageTime()
    {
        if (tapTimes.Count == 0)
        {
            return 0f;
        }

        float total = 0f;
        foreach (float time in tapTimes)
        {
            total += time;
        }
        return total / tapTimes.Count;
    }

    public void ResetGame()
    {
        gameOverModal.SetActive(false);
        startButton.gameObject.SetActive(true);
        validScore = false;
        InitializeGame(); // Reset and prepare for a new game
    }

    public void OnSubmitScoreClicked()
    {
        _ = SubmitScoreAsync(); // Discard the task
    }

    public async Task SubmitScoreAsync()
    {
        Debug.Log("Submit Score");

        string initials = initialsInputField.text;

        //call http post to save score if it is valid score
        if (validScore)
        {
            Debug.Log("Initials: " + initials + " Score: " + averageTime);

            var highScore = new HighScore
            {
                Name = initials,
                Score = Convert.ToDouble(averageTime),
                DateCreated = DateTime.UtcNow,
                AppName = "PoReflexSquares"
                // Set other properties as needed
            };

            await PostHighScoreAsync(highScore, "https://poshared.azurewebsites.net/");
        }
        else
        {
            Debug.Log("Invalid score. No Score saved.");
        }

        gameOverModal.SetActive(false);
        ResetGame();
    }

    public async Task PostHighScoreAsync(HighScore highScore, string apiUrl)
    {
        try
        {
            using (var client = new HttpClient())
            {
                string json = JsonUtility.ToJson(highScore);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{apiUrl}/highscores", content);

                if (response.IsSuccessStatusCode)
                {
                    Debug.Log("High score posted successfully.");
                }
                else
                {
                    Debug.Log($"Failed to post high score. Status code: {response.StatusCode}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Log($"An error occurred while posting the high score: {ex.Message}");
        }
    }
}
