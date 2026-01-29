using UnityEngine;

public class Note : MonoBehaviour
{
    private float targetBeat;          // En qué beat debe llegar la nota
    private Vector3 targetPosition;    // Hacia dónde se mueve (centro)
    public Transform approachCircle;

    float startDistance;
    bool Initialized = false;
    private Vector3 approachStartScale;

    // Llamado desde NoteSpawner para configurar la nota
    public void Initialize(float _targetBeat, Vector3 _targetPosition)
    {
        targetBeat = _targetBeat;
        targetPosition = _targetPosition;
        Initialized = true;

        startDistance = Vector3.Distance(transform.position, targetPosition);

        if (approachCircle != null )
            approachStartScale = approachCircle.localScale;
    }

    void Update()
    {
        if (!Initialized)
            return;

        // Calcula cuánto falta para el beat
        float songBeat = Conductor.instance.songPositionInBeats;

        // Tiempo restante para llegar al beat.
        float beatsLeft = targetBeat - songBeat;


        // Mueve la nota hacia el centro según beatsLeft.
        float speed = 6f;

        // Movimiento hacia el target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (approachCircle != null)
        {
            float currentDistance = Vector3.Distance(transform.position, targetPosition);

            float t = Mathf.Clamp01(1f - (currentDistance / startDistance));

            approachCircle.localScale = Vector3.Lerp(approachStartScale, Vector3.one, t);
        }

        // Destruir la nota una vez llega al centro
        if (beatsLeft <= 0)
        {
            Destroy(gameObject);
        }
    }
}
