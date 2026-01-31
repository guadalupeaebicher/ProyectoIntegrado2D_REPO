using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;

    public Transform leftSpawn;
    public Transform rightSpawn;
    public Transform rightHitPoint;
    public Transform leftHitPoint;

    public float spawnBeats = 4f;
    public float songLengthInBeats = 384f;

    private float nextBeat = 0f;

    void Update()
    {
        float songBeat = Conductor.instance.songPositionInBeats;

        if (songBeat >= nextBeat - spawnBeats)
        {
            SpawnNote(nextBeat);
            nextBeat += 1f; // una nota por beat
        }
    }

    void SpawnNote(float targetBeat)
    {
        bool spawnLeft = Random.value < 0.5f;

        Transform spawnPoint = spawnLeft ? leftSpawn : rightSpawn;
        Transform hitPoint = spawnLeft ? leftHitPoint : rightHitPoint;

        GameObject noteGO = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);
        Note noteScript = noteGO.GetComponent<Note>();

        FacingDirection direction = spawnLeft
            ? FacingDirection.Right   // viene desde la izquierda, se golpea mirando derecha
            : FacingDirection.Left;   // viene desde la derecha, se golpea mirando izquierda

        noteScript.Initialize(targetBeat, hitPoint.position, direction);
    }
}
