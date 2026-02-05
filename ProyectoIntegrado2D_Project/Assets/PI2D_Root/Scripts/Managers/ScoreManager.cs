using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score;
    public int combo;

    public int perfectScore = 1000;
    public int goodScore = 500;

    void Awake()
    {
        instance = this;
    }

    public void RegisterHit(HitResult result)
    {
        switch (result)
        {
            case HitResult.Perfect:
                combo++;
                score += perfectScore * combo;
                break;

            case HitResult.Good:
                combo++;
                score += goodScore * combo;
                break;

            case HitResult.Miss:
            case HitResult.WrongSide:
                combo = 0;
                break;
        }
    }
}
