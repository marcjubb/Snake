using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using static System.Net.Mime.MediaTypeNames;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using Text = UnityEngine.UI.Text;
public class GameManager : MonoBehaviour
{
    private int highScore;
    public GameObject doubleScoreTimer;
    private int scoreMultiplier = 1;
    private bool doubleScoreActive = false;
    private static GameManager _instance;
    public List<Achievement> achievements = new List<Achievement>();


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        InitializeAchievements();
        LoadAchievements();
        LoadHighScore();
    }


    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();
            }
            return _instance;
        }
    }

    /*   public GameObject gameOverText;
       public GameObject resetText;*/
    public TextMeshProUGUI achievementNotificationText;
    private bool gameEnded = false;

    public delegate void ScoreUpdated(int newScore);
    public event ScoreUpdated OnScoreUpdated;
    private int currentScore = 0;

    void Start()
    {
        /*gameOverText.SetActive(false);*/
        /* resetText.SetActive(false);*/
       
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }


   
    void Update()
    {
        if (gameEnded)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetGame();
            }
        }
    }

    public GameObject gameOverSquare;
    public GameObject gameOverText;
    public GameObject transitionSquare;
    public void GameOver()
    {
        PauseGame();
        gameEnded = true;

        gameOverText.SetActive(true);
        gameOverSquare.SetActive(true);
    }

    public void ResetGame()
    {
        gameOverText.SetActive(false);
        gameOverSquare.SetActive(false);
     
        UnpauseGame();
    }



    public bool IsGameOver()
    {
        return gameEnded;
    }

  

    public int GetCurrentScore()
    {
        return currentScore;
    }

    public void UpdateHighScore(int newScore)
    {
        ProfileManager profileManager = ProfileManager.Instance;
        Profile selectedProfile = profileManager.profiles[profileManager.SelectedProfileIndex];



        if (newScore > selectedProfile.highScore)
        {
            selectedProfile.highScore = newScore;
            profileManager.SaveProfiles();
        }
    }

    public void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }



    private void InitializeAchievements()
    {
        achievements = new List<Achievement>
        {
            new Achievement("fast_start", "Reach a score of 300 within the first 10 seconds"),
       new Achievement("corner_master", "Turn 50 corners without hitting anything"),
       new Achievement("doctor_visit","60s without eating, the doctor is on the way"),
      new Achievement("minimalist", "Reach a score of 1000 with a snake of 6 segments or less")
      
        };
    }

    public void UnlockAchievement(string id)
    {
        Achievement achievement = achievements.Find(a => a.id == id);
        ProfileManager profileManager = ProfileManager.Instance;
        Profile selectedProfile = profileManager.profiles[profileManager.SelectedProfileIndex];

        if (achievement != null && !selectedProfile.unlockedAchievements.Contains(id))
        {
            selectedProfile.unlockedAchievements.Add(id);
            profileManager.SaveProfiles();

            // Display the notification for the unlocked achievement
            achievementNotificationText.text = "Achievement unlocked: " + achievement.description;
            achievementNotificationText.gameObject.SetActive(true);
            StartCoroutine(HideAchievementNotification());
        }
    }


    private IEnumerator HideAchievementNotification()
    {
        yield return new WaitForSeconds(0.18f); // Show the notification for 3 seconds
        achievementNotificationText.gameObject.SetActive(false);
    }

    private void LoadAchievements()
    {
        ProfileManager profileManager = ProfileManager.Instance;
        Profile selectedProfile = profileManager.profiles[profileManager.SelectedProfileIndex];

        foreach (Achievement achievement in achievements)
        {
            achievement.isUnlocked = selectedProfile.unlockedAchievements.Contains(achievement.id);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 1f;
    }

    public void UnpauseGame()
    {
        Time.timeScale = 0.06f;
    }


    public void SetScoreMultiplier(int multiplier)
    {
        scoreMultiplier = multiplier;
    }




    IEnumerator UpdateDoubleScoreTimer(float duration)
    {
        doubleScoreTimer.GetComponent<TextMeshProUGUI>().enabled = true;
        float timeLeft = duration;

        while ( timeLeft > 0)
        {
            timeLeft -= (Time.deltaTime);
            doubleScoreTimer.GetComponent<TextMeshProUGUI>().text = "2x: " + Mathf.RoundToInt((float)(timeLeft*16.666667)) + "s";

            yield return null;
        }

        // Reset score multiplier when the timer reaches zero
        SetScoreMultiplier(1);
        doubleScoreTimer.GetComponent<TextMeshProUGUI>().enabled = false;
    }






    // Add this method to the GameManager script
    public int GetScoreMultiplier()
    {
        return scoreMultiplier;
    }

    // Add these methods to show/hide the double score timer
    public void ShowDoubleScoreTimer(float duration)
    {
        doubleScoreTimer.SetActive(true);
        // Set the timer's text to the duration (you can format it as you like)
        doubleScoreTimer.GetComponent<TextMeshProUGUI>().text = duration.ToString("0.0");
        StartDoubleScoreTimer(0.9f);
    }

    public void HideDoubleScoreTimer()
    {
        doubleScoreTimer.SetActive(false);
    }

    public void StartDoubleScoreTimer(float duration)
    {
        StartCoroutine(UpdateDoubleScoreTimer(duration));
    }


    public void BackToMenu()
    {
        
        Time.timeScale = 1f;
        FindObjectOfType<SceneTransition>().FadeToLevel("menu");
        SaveScoreAndAchievements();
        transitionSquare.SetActive(true);
        gameOverText.SetActive(false);
        gameOverSquare.SetActive(false);
    }


    private void SaveScoreAndAchievements()
    {
        ProfileManager profileManager = ProfileManager.Instance;
        if (profileManager != null)
        {
            int selectedProfileIndex = profileManager.SelectedProfileIndex;
            Profile selectedProfile = profileManager.profiles[selectedProfileIndex];

            if (selectedProfile != null)
            {
                // Save high score
                if (currentScore > selectedProfile.highScore)
                {
                    selectedProfile.highScore = currentScore;
                }

                // Save achievements
                selectedProfile.unlockedAchievements.Clear();
                foreach (Achievement achievement in achievements)
                {
                    if (achievement.isUnlocked)
                    {
                        selectedProfile.unlockedAchievements.Add(achievement.id);
                    }
                }
            }

            profileManager.SaveProfiles();
        }


    }
}


