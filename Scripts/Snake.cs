using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;


public class Snake : MonoBehaviour
{
    int score = 0;
    public TextMeshProUGUI scoreText;
    private bool isPaused = false;
    Vector2 direction;
    public GameObject segmentB;
    public GameObject segmentR;
    public GameObject segmentW;
    
    List<GameObject> segments = new List<GameObject>();
    List<GameObject> temporarySegments = new List<GameObject>();

    GameManager gameManager;
    private List<GameState> gameStates = new List<GameState>();

    private float stateSaveInterval = 0.06f;
    private float timeSinceLastSave = 0f;
    bool hasTurnedThisTick = false;
    private float moveSpeed = 0.06f; // Add this line
    private float timeSinceLastMove = 0f; // Add this line
    private bool gameEnded = false;
    private float timeSinceLastFood = 0f;

    private int successfulTurns = 0;
    private float startTime;


    private ItemSpawner itemSpawner;

    void Start()
    {

        startTime = Time.time;
        successfulTurns = 0;

        ProfileManager.Instance.LoadProfiles();
        Profile currentPlayerProfile = ProfileManager.Instance.profiles[ProfileManager.Instance.SelectedProfileIndex];
       
        UpdateScoreText();


        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            UnityEngine.Debug.LogError("GameManager not found");
        }

        itemSpawner = FindObjectOfType<ItemSpawner>(); // Add this line
        if (itemSpawner == null)
        {
            UnityEngine.Debug.LogError("ItemSpawner not found");
        }
        Reset();
    }

    public void Reset()
    {
        transform.position = new Vector2(2, -1); // Change initial position of head
        transform.rotation = Quaternion.Euler(0, 0, -90);
        direction = Vector2.right;
        Time.timeScale = 0.06f;
        score = 0;
        UpdateScoreText();
        gameEnded = false;
        ResetSegments();
        gameManager.ResetGame();
        if (itemSpawner != null)
            {
                itemSpawner.Reset();
            }


    }



    private void CheckAchievements()
    {
        float elapsedTime = (Time.time - startTime) / Time.timeScale;
        if (score >= 300 && elapsedTime <= 10 && gameEnded == false)
        {
            gameManager.UnlockAchievement("fast_start");
        }
        if (score >= 1000 && segments.Count <= 5 && gameEnded == false)
        {
            gameManager.UnlockAchievement("minimalist");
        }
    }


    private void ApplyTimeScale()
    {
        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 0.06f;
        }
    }

    void ResetSegments()
    {
        for (int i = 1; i < segments.Count; i++)
        {
            Destroy(segments[i].gameObject);
        }

        segments.Clear();
        segments.Add(gameObject);

        for (int i = 1; i < 2; i++)
        {
            Vector3 lastSegmentPosition = segments[segments.Count - 1].transform.position;
            Vector3 newPosition = new Vector3(lastSegmentPosition.x - 0.5f, lastSegmentPosition.y, lastSegmentPosition.z); // Update this line

            GameObject newSegment;

            switch (SpriteManager.Instance.currentColor)
            {
                case "Blue":
                    newSegment = Instantiate(segmentB, newPosition, Quaternion.identity);
                    break;
                case "Red":
                    newSegment = Instantiate(segmentR, newPosition, Quaternion.identity);
                    break;
                case "White":
                    newSegment = Instantiate(segmentW, newPosition, Quaternion.identity);
                    break;
                default:
                    newSegment = Instantiate(segmentB, newPosition, Quaternion.identity);
                    break;
            }

            segments.Add(newSegment);
        }
        
        direction = Vector2.right;
        transform.rotation = Quaternion.Euler(0, 0, -90);
    }

    void Grow()
    {
        Vector3 lastSegmentPosition = segments[segments.Count - 1].transform.position;
        Vector3 newPosition = lastSegmentPosition - (Vector3)(direction * 0.5f); // Calculate the new position based on the direction

        GameObject newSegment;

        switch (SpriteManager.Instance.currentColor)
        {
            case "Blue":
                newSegment = Instantiate(segmentB, newPosition, Quaternion.identity);
                break;
            case "Red":
                newSegment = Instantiate(segmentR, newPosition, Quaternion.identity);
                break;
            case "White":
                newSegment = Instantiate(segmentW, newPosition, Quaternion.identity);
                break;
            default:
                newSegment = Instantiate(segmentB, newPosition, Quaternion.identity);
                break;
        }

        segments.Add(newSegment);
        temporarySegments.Add(newSegment);

        score = score + 100 * gameManager.GetScoreMultiplier();
        CheckAchievements();
        UpdateScoreText();
    }






    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            TogglePause();
        }

        GetUserInput();
        if (!isPaused)
        {

            timeSinceLastMove += Time.deltaTime;
            timeSinceLastSave += Time.deltaTime;
            timeSinceLastFood += Time.deltaTime;

            if (timeSinceLastFood >= 3.6 && gameEnded == false)
            {
                GameManager.Instance.UnlockAchievement("doctor_visit");
            }
           

            if (timeSinceLastSave >= stateSaveInterval)
            {

                SaveGameState();
                timeSinceLastSave = 0f;
            }
        }
        else if (isPaused && Input.GetKeyDown(KeyCode.O)) // Move the Rewind() call here
        {
            Rewind();
        }
    }



    private void SaveGameState()
    {
        Vector3[] snakePositions = GetSnakePositions();
        gameStates.Add(new GameState(snakePositions,score, transform.rotation, direction)); // Add direction as a parameter
    }


    public Vector3[] GetSnakePositions()
    {
        Vector3[] positions = new Vector3[segments.Count];
        for (int i = 0; i < segments.Count; i++)
        {
            positions[i] = segments[i].transform.position;
        }
        return positions;
    }

    public void SetSnakePositions(Vector3[] positions)
    {
        for (int i = 0; i < positions.Length; i++)
        {
            segments[i].transform.position = positions[i];
        }
    }

    public void Rewind()
    {
        if (gameStates.Count > 1)
        {
            GameState previousState = gameStates[gameStates.Count - 2];
            gameStates.RemoveRange(gameStates.Count - 2, 2);

            // Remove excess segments
            while (segments.Count > previousState.snakePositions.Length)
            {
                GameObject lastSegment = segments[segments.Count - 1];
                segments.RemoveAt(segments.Count - 1);
                temporarySegments.RemoveAt(temporarySegments.Count - 1);
                Destroy(lastSegment);
            }

            SetSnakePositions(previousState.snakePositions);
            transform.rotation = previousState.headRotation;
            score = previousState.score;
            UpdateScoreText();
            direction = previousState.direction; // Add this line


        }
    }






    void GetUserInput()
    {
       
        if (!hasTurnedThisTick && Input.GetKeyDown(KeyCode.W) && direction != Vector2.down)
        {
            direction = Vector2.up;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            hasTurnedThisTick = true;
            successfulTurns++;
        }
        else if (!hasTurnedThisTick && Input.GetKeyDown(KeyCode.S) && direction != Vector2.up)
        {
            direction = Vector2.down;
            transform.rotation = Quaternion.Euler(0, 0, 180);
            hasTurnedThisTick = true;
            successfulTurns++;
        }
        else if (!hasTurnedThisTick && Input.GetKeyDown(KeyCode.D) && direction != Vector2.left)
        {
            direction = Vector2.right;
            transform.rotation = Quaternion.Euler(0, 0, -90);
            hasTurnedThisTick = true;
            successfulTurns++;
        }
        else if (!hasTurnedThisTick && Input.GetKeyDown(KeyCode.A) && direction != Vector2.right)
        {
            direction = Vector2.left;
            transform.rotation = Quaternion.Euler(0, 0, 90);
            hasTurnedThisTick = true;
            successfulTurns++;
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.ResetGame();
            Reset();
        }
       
        if (successfulTurns == 50 && gameEnded == false)
        {
            GameManager.Instance.UnlockAchievement("corner_master");
        }
    }

    void FixedUpdate()
    {
        if (gameEnded )
        {
            return;
        }

        hasTurnedThisTick = false;
        bool obstacleOrWallAhead = CheckNextMoveObstacle();
        if (!obstacleOrWallAhead)
        {
            MoveSegments();
            MoveSnake();
        }

    }




    void MoveSegments()
    {
        for (int i = segments.Count - 1; i > 0; i--)
        {
            segments[i].transform.position = segments[i - 1].transform.position;
        }
    }

    void MoveSnake()
    {
        Vector2 currentPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 newPosition = currentPosition + direction;

        if (segments.Find(segment => ((Vector2)segment.transform.position) == newPosition))
        {
            Time.timeScale = 0f;
            gameManager.GameOver();
            return;
        }

        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }


    bool CheckNextMoveObstacle()
    {
        Vector2 currentPosition = transform.position;
        Vector2 nextPos = currentPosition + direction;
        RaycastHit2D hit = Physics2D.Raycast(nextPos, direction, 0.04f);

        if (hit.collider != null && (hit.collider.CompareTag("Obstacle")))
        {
            if ( score > ProfileManager.Instance.profiles[ProfileManager.Instance.SelectedProfileIndex].highScore)
            {
                ProfileManager.Instance.UpdateHighScore(score);
                ProfileManager.Instance.UpdateAchievements(gameManager.achievements);
            }
            gameManager.GameOver();
            gameEnded = true;
            return true;
        }

        return false;
    }






    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            gameManager.GameOver();

            timeSinceLastFood = 0f;
            successfulTurns = 0;
        }
        else if (other.CompareTag("Food"))
        {
            Grow();
            
            timeSinceLastFood = 0f;


        }
        else if (other.CompareTag("DoubleScore")) // Add this block for the DoubleScore item
        {
            Destroy(other.gameObject);
            SetScoreMultiplier(2);
             gameManager.ShowDoubleScoreTimer(0.9f);
             StartCoroutine(ResetScoreMultiplier(0.9f));
           
      

        }
    }

    // Add this Coroutine to reset the score multiplier after a given duration
    IEnumerator ResetScoreMultiplier(float duration)
    {
        yield return new WaitForSeconds(duration);
        SetScoreMultiplier(1);
        gameManager.HideDoubleScoreTimer();
    }



    public void TogglePause()
    {
        isPaused = !isPaused;
        ApplyTimeScale(); // Add this line
    }
    public void SetScoreMultiplier(int multiplier)
    {
        gameManager.SetScoreMultiplier(multiplier);
    }

    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

}