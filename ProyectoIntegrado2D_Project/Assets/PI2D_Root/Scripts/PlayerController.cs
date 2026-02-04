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
    public GameObject wrongSidePopupPrefab;

    private bool isFacingRight = true;
    private PlayerInputActions controls;

    private void Awake()
    {
        controls = new PlayerInputActions();

        // --- Flip ---
        controls.Gameplay.Flip.performed += ctx =>
        {
            if (ctx.control.name == "leftArrow" || ctx.control.name == "dpadLeft")
                FaceRight();
            else if (ctx.control.name == "rightArrow" || ctx.control.name == "dpadRight")
                FaceLeft();
        };

        // --- Attack ---
        controls.Gameplay.Attack.performed += ctx =>
        {
            TryHitNote();
        };
    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    // --- Flip ---
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

    // --- Hit logic ---
    private void TryHitNote()
    {
        FacingDirection playerFacing = isFacingRight ? FacingDirection.Right : FacingDirection.Left;

        foreach (var note in Object.FindObjectsByType<Note>(FindObjectsSortMode.None))
        {
            if (note.alreadyHit)
                continue;

            // Suscribimos el popup al evento de la nota
            note.OnHitResult -= ShowHitPopup; // evitamos doble suscripción
            note.OnHitResult += ShowHitPopup;

            HitResult result = note.TryHit(playerFacing); // solo golpea si mirás hacia la nota
            if (result != HitResult.None)
                break; // solo una nota por input
        }
    }

    // --- Popups ---
    private void ShowHitPopup(HitResult result, FacingDirection noteDirection)
    {
        GameObject prefabToSpawn = null;

        switch (result)
        {
            case HitResult.Perfect: prefabToSpawn = perfectPopupPrefab; break;
            case HitResult.Good: prefabToSpawn = goodPopupPrefab; break;
            case HitResult.Miss: prefabToSpawn = missPopupPrefab; break;
            case HitResult.WrongSide: prefabToSpawn = wrongSidePopupPrefab; break;
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

