using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;

    public Transform leftSpawn;
    public Transform rightSpawn;
    public Transform leftHitPoint;
    public Transform rightHitPoint;

    public float spawnBeats = 4f;
    private float nextTargetBeat = 0f;

    void Update()
    {
        float songBeat = Conductor.instance.songPositionInBeats;

        if (songBeat >= nextTargetBeat - spawnBeats)
        {
            SpawnNote(nextTargetBeat);
            nextTargetBeat += 1f;
        }
    }

    void SpawnNote(float targetBeat)
    {
        bool fromLeft = Random.value < 0.5f;

        Transform spawnPoint = fromLeft ? leftSpawn : rightSpawn;
        Transform hitPoint = fromLeft ? leftHitPoint : rightHitPoint;

        GameObject noteGO = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);
        Note note = noteGO.GetComponent<Note>();

        // SOLO las notas del LEFT SPAWN se voltean para mirar a la derecha
        if (fromLeft) // fromLeft = true significa que viene del left spawn
        {
            // Cambiar escala en X a negativa para voltear horizontalmente
            Vector3 currentScale = noteGO.transform.localScale;
            noteGO.transform.localScale = new Vector3(
                -Mathf.Abs(currentScale.x),  // X negativo (voltea horizontalmente)
                currentScale.y,              // Y se mantiene igual
                currentScale.z               // Z se mantiene igual
            );

            // Si prefieres mantener el valor absoluto del scale original:
            // noteGO.transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), 
            //                                           currentScale.y, 
            //                                           currentScale.z);
        }
        // Las notas del right spawn (fromLeft = false) se quedan con su escala original
        // mirando a la izquierda (como el prefab original)

        FacingDirection direction = fromLeft ? FacingDirection.Right : FacingDirection.Left;
        note.Initialize(targetBeat, hitPoint.position, direction);
    }
}
    
