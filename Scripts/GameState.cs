using UnityEngine;
[System.Serializable]

public class GameState
{
    public Vector3[] snakePositions;
    public int score;
    public Quaternion headRotation;
    public Vector2 direction; // Add this line

    public GameState(Vector3[] snakePositions, int score, Quaternion headRotation, Vector2 direction) // Add direction parameter
    {
        this.snakePositions = snakePositions;
        this.score = score;
        this.headRotation = headRotation;
        this.direction = direction; // Add this line
    }
}
