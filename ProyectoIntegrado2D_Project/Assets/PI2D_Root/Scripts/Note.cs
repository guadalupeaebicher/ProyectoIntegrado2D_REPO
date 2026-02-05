using UnityEngine;

public class Note : MonoBehaviour
{
    // --- Timing ---
    public float targetBeat;
    public FacingDirection noteDirection;

    [Header("Timing Windows")]
    public float perfectWindow = 0.5f;
    public float goodWindow = 0.25f;
    public float missWindow = 0.4f;

    // --- State ---
    public bool alreadyHit = false;
    private bool initialized = false;

    // --- Movement ---
    private Vector3 targetPosition;
    public float moveSpeed = 6f;

    // --- Approach Circle ---
    [Header("Approach Circle")]
    public Transform approachCircle;
    public float approachMaxMultiplier = 2.5f;
    public float approachMinMultiplier = 0.6f; 


    private Vector3 approachBaseScale;
    private float startDistance;

    // --- Events ---
    public event System.Action<HitResult, FacingDirection> OnHitResult;

    // --- Init ---
    public void Initialize(float _targetBeat, Vector3 _targetPosition, FacingDirection _direction)
    {
        targetBeat = _targetBeat;
        targetPosition = _targetPosition;
        noteDirection = _direction;
        initialized = true;

        if (approachCircle != null)
        {
            approachBaseScale = approachCircle.localScale;
            startDistance = Vector3.Distance(transform.position, targetPosition);
            approachCircle.localScale = approachBaseScale * approachMaxMultiplier;
        }
    }

    void Update()
    {
        if (!initialized || alreadyHit)
            return;

        float songBeat = Conductor.instance.songPositionInBeats;

        // --- Move towards hit point ---
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // --- Approach circle scaling ---
        if (approachCircle != null && startDistance > 0f)
        {
            float currentDistance = Vector3.Distance(transform.position, targetPosition);
            float t = Mathf.Clamp01(currentDistance / startDistance);
            float multiplier = Mathf.Lerp(approachMinMultiplier, approachMaxMultiplier, t);
            approachCircle.localScale = approachBaseScale * multiplier;
        }

        // --- Auto miss ---
        if (songBeat > targetBeat + missWindow)
        {
            Miss();
        }
    }

    // --- Hit logic ---
    public HitResult TryHit(FacingDirection playerFacing)
    {
        if (alreadyHit)
            return HitResult.None;

        if (playerFacing != noteDirection)
            return HitResult.WrongSide;

        float songBeat = Conductor.instance.songPositionInBeats;
        float error = Mathf.Abs(songBeat - targetBeat);

        HitResult result = HitResult.None;

        if (error <= perfectWindow)
            result = HitResult.Perfect;
        else if (error <= goodWindow)
            result = HitResult.Good;
        else if (error <= missWindow)
            result = HitResult.Miss;

        if (result != HitResult.None)
            ResolveHit(result);

        return result;
    }

    // --- Results ---
    private void ResolveHit(HitResult result)
    {
        alreadyHit = true;
        OnHitResult?.Invoke(result, noteDirection);
        Destroy(gameObject);
    }

    private void Miss()
    {
        alreadyHit = true;
        OnHitResult?.Invoke(HitResult.Miss, noteDirection);
        Destroy(gameObject);
    }
}
