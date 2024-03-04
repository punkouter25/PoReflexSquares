using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject[] squares;
    public GameObject gameOverModal;
    public Text scoreText;
    public Button startButton; // Reference to the Start button
    public Text countdownTimerText; // For countdown display

    private bool gameIsActive = false;
    private int currentIndex = -1;
    private List<float> tapTimes = new List<float>();
    private float squareActivatedTime;
    private float timeToTap = 2.0f; // Time allowed to tap the square
    private IEnumerator currentCountdownCoroutine; // To keep track of the countdown coroutine

    void Start()
    {
        InitializeGame();
    }

    void InitializeGame()
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

    void ActivateNextSquareWithDelay()
    {
        if (currentIndex < squares.Length - 1)
        {
            currentIndex++;
            StartCoroutine(DelaySquareActivation(squares[currentIndex]));
        }
        else
        {
            EndGame(false, false); // Consider what should happen if all squares are successfully tapped
        }
    }

    IEnumerator DelaySquareActivation(GameObject square)
    {
        float delay = Random.Range(0.5f, 1.5f);
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
            StartCoroutine(currentCountdownCoroutine);
        }
    }

    IEnumerator CountdownCoroutine(float countdownTime)
    {
        float timer = countdownTime;
        while (timer > 0)
        {
            countdownTimerText.text = $"{timer:F1} s";
            yield return new WaitForSeconds(0.1f);
            timer -= 0.1f;
        }
        countdownTimerText.text = "0.0 s";
    }

    public void SquareTapped(int index)
    {
        if (!gameIsActive || index != currentIndex) return;

        float reactionTime = (Time.time - squareActivatedTime) * 1000;
        tapTimes.Add(reactionTime);
        squares[currentIndex].GetComponent<Square>().ResetSquare(true, reactionTime);
        if (currentCountdownCoroutine != null)
        {
            StopCoroutine(currentCountdownCoroutine);
            countdownTimerText.text = ""; // Optionally clear the countdown text
        }
        ActivateNextSquareWithDelay();
    }

    void EndGame(bool earlyTap, bool failed)
    {
        gameIsActive = false;
        gameOverModal.SetActive(true);
        startButton.gameObject.SetActive(false); // Hide the Start button when Game Over modal is shown

        if (failed)
        {
            scoreText.text = "GAME OVER: Time's up!";
        }
        else
        {
            float averageTime = CalculateAverageTime();
            scoreText.text = $"Average Reaction Time: {averageTime:F2} ms";
        }

      //  startButton.gameObject.SetActive(true); // Show start button when game ends
        countdownTimerText.text = ""; // Clear countdown timer text
    }

    float CalculateAverageTime()
    {
        if (tapTimes.Count == 0) return 0f;
        float total = 0f;
        foreach (float time in tapTimes)
        {
            total += time;
        }
        return total / tapTimes.Count;
    }

    // Uncomment and adjust this method as necessary based on your modal "OK" button setup
    // public void OnGameOverOKButtonClicked()
    // {
    //     gameOverModal.SetActive(false); // Hide the Game Over modal
    //     InitializeGame(); // Reset the game to its initial state
    // }

    public void ResetGame()
    {
        gameOverModal.SetActive(false);
        startButton.gameObject.SetActive(true);
        InitializeGame(); // Reset and prepare for a new game
    }
}
