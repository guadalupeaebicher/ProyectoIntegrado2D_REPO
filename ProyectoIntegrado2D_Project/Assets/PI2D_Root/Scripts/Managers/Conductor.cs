using UnityEngine;

public class Conductor : MonoBehaviour
{

    public float songBpm; //Beats per minute
    public float secPerBeat;
    public float songPosition;
    public float songPositionInBeats;
    public float dspSongTime;
    public float firstBeatOffset;
    public AudioSource musicSource;

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
