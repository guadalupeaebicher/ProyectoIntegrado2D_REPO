using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    public int score;
    public int combo;
    public int maxCombo;

    public int perfects;
    public int goods;
    public int misses;
    public int wrongs;

    public int perfectScore = 1000;
    public int goodScore = 500;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void RegisterHit(HitResult result)
    {
        switch (result)
        {
            case HitResult.Perfect:
                perfects++;
                combo++;
                score += perfectScore * combo;
                break;

            case HitResult.Good:
                goods++;
                combo++;
                score += goodScore * combo;
                break;

            case HitResult.Miss:
                misses++;
                combo = 0;
                break;

            case HitResult.WrongSide:
                wrongs++;
                combo = 0;
                break;
        }

        if (combo > maxCombo)
            maxCombo = combo;
    }

    public LevelResult GetFinalResult()
    {
        LevelResult result = new LevelResult();

        result.score = score;
        result.maxCombo = maxCombo;

        result.perfects = perfects;
        result.goods = goods;
        result.misses = misses;
        result.wrongs = wrongs;

        int totalHits = perfects + goods + misses + wrongs;

        float accuracy = 0f;
        if (totalHits > 0)
        {
            accuracy = (perfects + goods * 0.5f) / totalHits;
        }

        result.accuracy = accuracy;
        result.rank = CalculateRank(accuracy);

        return result;
    }

    private Rank CalculateRank(float accuracy)
    {
        if (accuracy >= 0.95f) return Rank.S;
        if (accuracy >= 0.85f) return Rank.A;
        if (accuracy >= 0.70f) return Rank.B;
        if (accuracy >= 0.55f) return Rank.C;
        return Rank.D;
    }
}
