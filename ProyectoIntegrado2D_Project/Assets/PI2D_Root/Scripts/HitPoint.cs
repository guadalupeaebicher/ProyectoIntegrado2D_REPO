using UnityEngine;

public class HitPoint : MonoBehaviour
{
    public FacingDirection requiredFacing;

    private Note currentNote;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            currentNote = other.GetComponent<Note>();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Note"))
        {
            if (currentNote == other.GetComponent<Note>())
                currentNote = null;
        }
    }

    public HitResult TryHit(FacingDirection playerFacing)
    {
        if (currentNote == null)
            return HitResult.Miss;

        if (playerFacing != requiredFacing)
            return HitResult.WrongSide;

        HitResult result = currentNote.TryHit(playerFacing);

        if (result != HitResult.None)
            currentNote = null;

        return result;
    }
}
