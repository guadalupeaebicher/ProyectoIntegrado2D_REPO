using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Hit Settings")]
    public Transform hitPoint;          // Punto donde detectamos notas
    public float hitRange = 1f;         // Radio para detectar notas

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

        controls.Gameplay.Flip.performed += ctx =>
        {
            // Detectar qué tecla fue para hacer flip
            if (ctx.control.name == "leftArrow" || ctx.control.name == "dpadLeft")
                FaceLeft();
            else if (ctx.control.name == "rightArrow" || ctx.control.name == "dpadRight")
                FaceRight();
        };

        controls.Gameplay.Attack.performed += ctx =>
        {
            TryHitNote();
        };
    }

    private void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }

    private void FaceRight()
    {
        if (!isFacingRight)
        {
            isFacingRight = true;
            FlipSprite();
        }
    }

    private void FaceLeft()
    {
        if (isFacingRight)
        {
            isFacingRight = false;
            FlipSprite();
        }
    }

    private void FlipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1f;
        transform.localScale = scale;
    }

    private void TryHitNote()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPoint.position, hitRange);
        foreach (var hit in hits)
        {
            Note note = hit.GetComponent<Note>();
            if (note != null)
            {
                FacingDirection playerFacing = isFacingRight ? FacingDirection.Right : FacingDirection.Left;
                HitResult result = note.TryHit(playerFacing);
                if (result != HitResult.None)
                {
                    Debug.Log("Hit: " + result);
                    ShowHitPopup(result);
                }
            }
        }
    }

    private void ShowHitPopup(HitResult result)
    {
        GameObject prefabToSpawn = null;

        switch (result)
        {
            case HitResult.Perfect:
                prefabToSpawn = perfectPopupPrefab;
                break;
            case HitResult.Good:
                prefabToSpawn = goodPopupPrefab;
                break;
            case HitResult.Miss:
                prefabToSpawn = missPopupPrefab;
                break;
            case HitResult.WrongSide:
                prefabToSpawn = wrongSidePopupPrefab;
                break;
        }

        if (prefabToSpawn != null)
        {
            Instantiate(prefabToSpawn, hitPoint.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (hitPoint != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(hitPoint.position, hitRange);
        }
    }
}
