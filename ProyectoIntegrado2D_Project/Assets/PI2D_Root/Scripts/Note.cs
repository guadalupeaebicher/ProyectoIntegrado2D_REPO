using UnityEngine;

public class Note : MonoBehaviour
{
    // -------- Timing --------
    public float targetBeat;
    public FacingDirection noteDirection;

    [Header("Timing Windows")]
    public float perfectWindow = 0.08f;
    public float goodWindow = 0.18f;
    public float missWindow = 0.30f;

    // -------- Damage --------
    public int missDamage = 1;
    public int wrongSideDamage = 1;

    // -------- Beat Movement --------
    [Header("Beat Movement")]
    public float leadBeats = 4f;

    private float spawnBeat;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private bool initialized;
    public bool alreadyHit;

    // -------- Anchor --------
    [Header("Hit Anchor")]
    public Transform hitAnchor;

    private PlayerHealth playerHealth;

    // ================= INIT =================
    public void Initialize(
        float _targetBeat,
        Vector3 _startPosition,
        Vector3 _targetPosition,
        FacingDirection _direction)
    {
        targetBeat = _targetBeat;
        noteDirection = _direction;

        startPosition = _startPosition;
        targetPosition = _targetPosition;

        spawnBeat = targetBeat - leadBeats;
        transform.position = startPosition;

        playerHealth = FindAnyObjectByType<PlayerHealth>();

        initialized = true;
    }

    // ================= UPDATE =================
    void Update()
    {
        if (!initialized || alreadyHit)
            return;

        if (Conductor.instance == null)
            return;

        float songBeat = Conductor.instance.songPositionInBeats;

        // ---- Beat based interpolation ----
        float t = Mathf.InverseLerp(spawnBeat, targetBeat, songBeat);
        t = Mathf.Clamp01(t);

        Vector3 anchorOffset = hitAnchor != null ? hitAnchor.localPosition : Vector3.zero;
        Vector3 correctedTarget = targetPosition - anchorOffset;

        transform.position = Vector3.Lerp(startPosition, correctedTarget, t);

        // ✅ AUTO MISS CORREGIDO
        // deja existir el rango GOOD antes de destruir la nota
        if (songBeat > targetBeat + missWindow + goodWindow)
        {
            Miss();
        }
    }

    // ================= HIT =================
    public HitResult TryHit(FacingDirection playerFacing)
    {
        if (alreadyHit)
            return HitResult.None;

        if (playerFacing != noteDirection)
        {
            ApplyDamage(wrongSideDamage);
            ResolveHit(HitResult.WrongSide);
            return HitResult.WrongSide;
        }

        float songBeat = Conductor.instance.songPositionInBeats;
        float error = Mathf.Abs(songBeat - targetBeat);

        HitResult result;

        if (error <= perfectWindow)
            result = HitResult.Perfect;
        else if (error <= goodWindow)
            result = HitResult.Good;
        else if (error <= missWindow)
            result = HitResult.Miss;
        else
            return HitResult.None;

        if (result == HitResult.Miss)
            ApplyDamage(missDamage);

        ResolveHit(result);
        return result;
    }

    // ================= DAMAGE =================
    private void ApplyDamage(int dmg)
    {
        if (dmg <= 0)
            return;

        if (playerHealth == null)
            playerHealth = FindAnyObjectByType<PlayerHealth>();

        if (playerHealth != null)
            playerHealth.TakeDamage(dmg);
    }

    // ================= RESOLVE =================
    private void ResolveHit(HitResult result)
    {
        alreadyHit = true;
        ScoreManager.instance?.RegisterHit(result);
        Destroy(transform.root.gameObject);
    }

    // ================= MISS =================
    private void Miss()
    {
        if (alreadyHit)
            return;

        alreadyHit = true;
        ScoreManager.instance?.RegisterHit(HitResult.Miss);
        ApplyDamage(missDamage);
        Destroy(transform.root.gameObject);
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (hitAnchor != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(hitAnchor.position, 0.05f);
        }
    }
#endif
}
