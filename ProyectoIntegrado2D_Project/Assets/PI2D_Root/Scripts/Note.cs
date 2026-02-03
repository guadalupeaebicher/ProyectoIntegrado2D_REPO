using UnityEngine;

public class Note : MonoBehaviour
{
    //Config
    public float targetBeat;                 // Beat exacto del hit
    public FacingDirection noteDirection;      // Lado desde el que viene

    [Header("Timing Windows")]
    public float perfectWindow = 0.1f;
    public float goodWindow = 0.25f;
    public float missWindow = 0.4f;

    //Movimiento
    private Vector3 targetPosition;
    public Transform approachCircle;

    private float startDistance;
    private bool initialized = false;
    private Vector3 approachStartScale;

    public bool alreadyHit = false;

    //Initialize
    public void Initialize(float _targetBeat, Vector3 _targetPosition, FacingDirection _direction)
    {
        targetBeat = _targetBeat;
        targetPosition = _targetPosition;
        noteDirection = _direction;

        initialized = true;

        startDistance = Vector3.Distance(transform.position, targetPosition);

        if (approachCircle != null)
            approachStartScale = approachCircle.localScale;
    }

    void Update()
    {
        if (!initialized || alreadyHit)
            return;

        float songBeat = Conductor.instance.songPositionInBeats;
        float beatsLeft = targetBeat - songBeat;

        //Movimiento
        float speed = 6f;
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        //Approach circle
        if (approachCircle != null)
        {
            float currentDistance = Vector3.Distance(transform.position, targetPosition);
            float t = Mathf.Clamp01(1f - (currentDistance / startDistance));
            approachCircle.localScale = Vector3.Lerp(approachStartScale, Vector3.one, t);
        }

        //Miss automático
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

        if (error <= perfectWindow)
        {
            Hit();
            return HitResult.Perfect;
        }
        else if (error <= goodWindow)
        {
            Hit();
            return HitResult.Good;
        }
        else if (error <= missWindow)
        {
            Miss();
            return HitResult.Miss;
        }

        return HitResult.None;
    }

    //Results
    private void Hit()
    {
        alreadyHit = true;
        Destroy(gameObject);
    }

    private void Miss()
    {
        alreadyHit = true;
        Destroy(gameObject);
    }
}

//Enums
public enum HitResult
{
    None,
    Perfect,
    Good,
    Miss,
    WrongSide
}

public enum FacingDirection
{
    Left,
    Right
}
