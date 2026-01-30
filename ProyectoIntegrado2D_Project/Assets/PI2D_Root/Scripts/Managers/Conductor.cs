using UnityEngine;

public class Conductor : MonoBehaviour
{
    [Header ("Song Settings")]
    public float songBpm; //Beats per minute
    public float songOffset;
    public AudioSource musicSource;

    [Header("Read-Only")]
    public float secPerBeat;
    public float songPosition; //Segundos
    public float songPositionInBeats; //Beats

    public float dspSongTime;
    public static Conductor instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
        }

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Load the AudioSource component
        musicSource = GetComponent<AudioSource>();
        //Calcular la cantidad de segundos en cada beat
        secPerBeat = 60 / songBpm;
        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        //Determinar cuantos segundos pasaron desde que empezó la canción
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);
        //Determinar cuántos beats desde que empezó la canción
        songPositionInBeats = songPosition / secPerBeat;
    }
}
