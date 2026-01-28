using UnityEngine;

public class Note : MonoBehaviour
{
    private float targetBeat;          // En qué beat debe llegar la nota
    private Vector3 targetPosition;    // Hacia dónde se mueve (centro)

    private bool initialized = false;

    // Llamado desde NoteSpawner para configurar la nota
    public void Initialize(float _targetBeat, Vector3 _targetPosition)
    {
        targetBeat = _targetBeat;
        targetPosition = _targetPosition;
        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        // Calcula cuánto falta para el beat (suponiendo que Conductor tiene la info)
        float songBeat = Conductor.instance.songPositionInBeats;

        // Tiempo restante para llegar al beat objetivo
        float beatsLeft = targetBeat - songBeat;

        // Mueve la nota hacia el centro según beatsLeft (ajusta velocidad a tu gusto)
        float speed = 5f;

        // Movimiento simple hacia target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        // Opcional: destruir la nota si ya llegó o pasó el beat objetivo
        if (beatsLeft <= 0)
        {
            Destroy(gameObject);
        }
    }
}
