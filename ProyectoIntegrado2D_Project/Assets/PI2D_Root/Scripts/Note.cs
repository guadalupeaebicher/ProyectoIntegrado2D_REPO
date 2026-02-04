using UnityEngine;

public class Note : MonoBehaviour
{
    public float targetBeat;
    public FacingDirection noteDirection;
    [Header("Timing Windows")]
    public float perfectWindow = 0.1f;
    public float goodWindow = 0.25f;
    public float missWindow = 0.4f;

    public bool alreadyHit = false;

    private Vector3 targetPosition;
    private bool initialized = false;

    // Evento para avisar popups
    public event System.Action<HitResult, FacingDirection> OnHitResult;

    public void Initialize(float _targetBeat, Vector3 _targetPosition, FacingDirection _direction)
    {
        targetBeat = _targetBeat;
        targetPosition = _targetPosition;
        noteDirection = _direction;
        initialized = true;
    }

    void Update()
    {
        if (!initialized || alreadyHit)
            return;

        float songBeat = Conductor.instance.songPositionInBeats;

        // Movimiento hacia el hit point
        float speed = 6f;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Miss automático
        if (songBeat > targetBeat + missWindow)
        {
            Miss();
        }
    }

    public HitResult TryHit(FacingDirection playerFacing)
    {
        if (alreadyHit)
            return HitResult.None;

        if (playerFacing != noteDirection)
            return HitResult.WrongSide;

        float songBeat = Conductor.instance.songPositionInBeats;
        float error = Mathf.Abs(songBeat - targetBeat);

        HitResult result = HitResult.None;

        if (error <= perfectWindow) result = HitResult.Perfect;
        else if (error <= goodWindow) result = HitResult.Good;
        else if (error <= missWindow) result = HitResult.Miss;

        if (result != HitResult.None)
            Hit(result);

        return result;
    }

    private void Hit(HitResult result)
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
