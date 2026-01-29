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

        GameObject note = Instantiate(notePrefab, spawnPoint.position, Quaternion.identity);

        Note noteScript = note.GetComponent<Note>();
        Transform hitPoint = spawnLeft ? leftHitPoint : rightHitPoint;
        noteScript.Initialize(targetBeat, hitPoint.position);

    }
}
