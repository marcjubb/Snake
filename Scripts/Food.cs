
using UnityEngine;

public class Food : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RandomPosition();
    }

    void RandomPosition()
    {
        Vector2 randomPosition;
        do
        {
            int x = Random.Range(-7, 6);
            int y = Random.Range(-7, 6);
            randomPosition = new Vector2(x, y);
        } while (IsObstacleAtPosition(randomPosition));

        transform.position = randomPosition;
    }

    bool IsObstacleAtPosition(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Obstacle"))
            {
                return true;
            }
        }
        return false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        RandomPosition();
    }
}
