using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject leftNotePrefab;
    public GameObject rightNotePrefab;

    [Header("Spawns")]
    public Transform leftSpawn;
    public Transform rightSpawn;

    [Header("Hit Points")]
    public Transform leftHitPoint;
    public Transform rightHitPoint;

    [Header("Timing")]
    public float leadBeats = 4f;
    public float firstBeat = 4f;

    private float nextTargetBeat;

    void Start()
    {
        nextTargetBeat = firstBeat;
    }

    void Update()
    {
        if (Conductor.instance == null)
            return;

        float songBeat = Conductor.instance.songPositionInBeats;

        if (songBeat >= nextTargetBeat - leadBeats)
        {
            SpawnNote(nextTargetBeat);
            nextTargetBeat += 1f;
        }
    }

    void SpawnNote(float targetBeat)
    {
        bool fromLeft = Random.value < 0.5f;

        GameObject prefab = fromLeft ? leftNotePrefab : rightNotePrefab;
        Transform spawn = fromLeft ? leftSpawn : rightSpawn;
        Transform hit = fromLeft ? leftHitPoint : rightHitPoint;

        GameObject noteGO = Instantiate(prefab, spawn.position, Quaternion.identity);
        Note note = noteGO.GetComponent<Note>();


        FacingDirection dir = fromLeft
            ? FacingDirection.Right
            : FacingDirection.Left;

        note.Initialize(
            targetBeat,
            spawn.position,
            hit.position,
            dir
        );
    }
}
