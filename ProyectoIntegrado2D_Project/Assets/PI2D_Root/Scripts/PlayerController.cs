using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Popup Points")]
    public Transform leftPopupPoint;
    public Transform rightPopupPoint;

    [Header("Popups")]
    public GameObject perfectPopupPrefab;
    public GameObject goodPopupPrefab;
    public GameObject missPopupPrefab;

    // ===== NUEVO =====
    [Header("Animation")]
    public Animator animator;
    // =================

    private bool isFacingRight = true;
    private PlayerInputActions controls;

    private void Awake()
    {
        controls = new PlayerInputActions();

        controls.Gameplay.Flip.performed += ctx =>
        {
            if (ctx.control.name == "leftArrow" || ctx.control.name == "dpadLeft")
                FaceRight();
            else if (ctx.control.name == "rightArrow" || ctx.control.name == "dpadRight")
                FaceLeft();
        };

        controls.Gameplay.Attack.performed += ctx =>
        {
            TryHitNote();
        };
    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    private void FaceLeft()
    {
        if (isFacingRight)
        {
            isFacingRight = false;
            FlipSprite();
        }
    }

    private void FaceRight()
    {
        if (!isFacingRight)
        {
            isFacingRight = true;
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (isFacingRight ? 1f : -1f);
        transform.localScale = scale;
    }

    private void TryHitNote()
    {
        FacingDirection facing = isFacingRight
            ? FacingDirection.Right
            : FacingDirection.Left;

        Note bestNote = null;
        float bestError = float.MaxValue;
        float beat = Conductor.instance.songPositionInBeats;

        foreach (var note in Object.FindObjectsByType<Note>(FindObjectsSortMode.None))
        {
            if (note.alreadyHit)
                continue;

            float error = Mathf.Abs(beat - note.targetBeat);

            if (error <= note.missWindow && error < bestError)
            {
                bestError = error;
                bestNote = note;
            }
        }

        if (bestNote == null)
        {
            ShowHitPopup(HitResult.Miss, facing);
            PlayHitAnimation(); 
            return;
        }

        HitResult result = bestNote.TryHit(facing);

        if (result != HitResult.None)
        {
            ShowHitPopup(result, bestNote.noteDirection);
            PlayHitAnimation(); 
        }
    }
   private void PlayHitAnimation()
{
    if (animator != null)
    {
        animator.ResetTrigger("Hit"); // seguridad
        animator.SetTrigger("Hit");
    }
}

    private void ShowHitPopup(HitResult result, FacingDirection noteDirection)
    {
        GameObject prefabToSpawn = null;

        switch (result)
        {
            case HitResult.Perfect: prefabToSpawn = perfectPopupPrefab; break;
            case HitResult.Good: prefabToSpawn = goodPopupPrefab; break;
            case HitResult.Miss: prefabToSpawn = missPopupPrefab; break;
        }

        if (prefabToSpawn != null)
        {
            Transform spawnPoint = noteDirection == FacingDirection.Right
                ? rightPopupPoint
                : leftPopupPoint;

            GameObject popup = Instantiate(prefabToSpawn, spawnPoint.position, Quaternion.identity);
            Destroy(popup, 1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (leftPopupPoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(leftPopupPoint.position, 0.25f);
        }

        if (rightPopupPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(rightPopupPoint.position, 0.25f);
        }
    }
}
