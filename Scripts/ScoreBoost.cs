using UnityEngine;

public class ScoreBoost : MonoBehaviour
{
    public float duration = 15f;

    private Snake snake;
    private GameManager gameManager;
    private bool isActive = false;
    private float timeActive = 0f;

    void Start()
    {
        snake = FindObjectOfType<Snake>();
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        if (isActive)
        {
            timeActive += Time.deltaTime;
            if (timeActive >= duration)
            {
                Deactivate();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Activate();
            gameObject.SetActive(false);
        }
    }

    void Activate()
    {
        if (!isActive)
        {
            snake.SetScoreMultiplier(2);
            gameManager.ShowDoubleScoreTimer(duration);
            isActive = true;
            timeActive = 0f;
        }
    }

    void Deactivate()
    {
        if (isActive)
        {
            snake.SetScoreMultiplier(1);
            gameManager.HideDoubleScoreTimer();
            isActive = false;
        }
    }
}
