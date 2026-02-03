using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;

    public Transform leftSpawn;
    public Transform rightSpawn;
    public Transform leftHitPoint;
    public Transform rightHitPoint;

    public float spawnBeats = 4f; // cuánto antes spawnear la nota antes del target
    private float nextTargetBeat = 0f; // controla el beat de la próxima nota

    void Update()
    {
        float songBeat = Conductor.instance.songPositionInBeats;

        // Si estamos en el momento para spawnear la siguiente nota (spawnBeats antes del hit)
        if (songBeat >= nextTargetBeat - spawnBeats)
        {
            SpawnNote(nextTargetBeat);
            nextTargetBeat += 1f; // siguiente nota 1 beat después (puedes ajustar)
        }
    }

    void SpawnNote(float targetBeat)
    {
        bool fromLeft = Random.value < 0.5f;

        Transform spawnPoint = fromLeft ? leftSpawn : rightSpawn;
        Transform hitPoint = fromLeft ? leftHitPoint : rightHitPoint;

        GameObject noteGO = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);
        Note note = noteGO.GetComponent<Note>();

        FacingDirection direction = fromLeft ? FacingDirection.Left : FacingDirection.Right;

        note.Initialize(targetBeat, hitPoint.position, direction);
    }
}
