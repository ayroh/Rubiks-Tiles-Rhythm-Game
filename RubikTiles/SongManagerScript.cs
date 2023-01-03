using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SongManagerScript : MonoBehaviour {

    [SerializeField] private PuzzleManagerScript puzzleManager;

    [Header("Song")]
    [SerializeField] private UnityEngine.UI.Slider songSlider;
    [SerializeField] private SongSO songSO;
    private AudioSource audioSource;
    private bool isRecording = false;
    private bool isPlaying = false;

    private List<float> rhythmTimer;
    private List<float> rhythmTimerTimed;
    private int currentTile = 0;

    int bpm = 139;

    //the current position of the song (in seconds)
    float songPosition;

    //the current position of the song (in beats)
    float songPosInBeats;

    //the duration of a beat
    float secPerBeat;

    //how much time (in seconds) has passed since the song started
    float dsptimesong;

    void Start() {
        rhythmTimer = new List<float>();

        //calculate how many seconds is one beat
        //we will see the declaration of bpm later
        secPerBeat = 60f / bpm;

        //record the time when the song starts

        //start the song
        audioSource = GetComponent<AudioSource>();
    }

    void Update() {
        if (!isPlaying && !isRecording)
            return;

        songPosition = (float)(AudioSettings.dspTime - dsptimesong);

        if (isPlaying && songPosition >= rhythmTimerTimed[currentTile]) {
            puzzleManager.NextMove(currentTile);
            if (++currentTile == rhythmTimerTimed.Count)
                isPlaying = false;
        }
        else if(isRecording && Input.GetKeyDown(KeyCode.Space)) 
            rhythmTimer.Add(songPosition);

        songSlider.value = songPosition / audioSource.clip.length;
    }

    public void PrintElapsedTime() => print("songpos: " + (float)(AudioSettings.dspTime - dsptimesong));

    public void RecordButton() {
        if (isRecording)
            rhythmTimer.Add(songPosition);
    }

    public void RecordSong() {
        isRecording = true;
        dsptimesong = (float)AudioSettings.dspTime;
        songSlider.value = songSlider.minValue;
        audioSource.Play();
    }

    public void StopRecording() {
        isRecording = false;
        audioSource.Stop();
        System.Text.StringBuilder strBuild = new();
        seriList<float> seri = new();
        seri.rhtyhm = new();
        for (int i = 0; i < rhythmTimer.Count; ++i)
            seri.rhtyhm.Add(rhythmTimer[i]);
        System.IO.File.WriteAllText(Application.persistentDataPath + "/song", JsonUtility.ToJson(seri));
    }

    [System.Serializable]
    public class seriList<T> {
        public List<T> rhtyhm;
    }


    public void GetSongTiles() {
        string json;
        try {
            json = System.IO.File.ReadAllText(Application.persistentDataPath + "/song");
        }
        catch(System.IO.FileNotFoundException e) {
            json = null;
        }
        if (!string.IsNullOrEmpty(json)) {
            seriList<float> seri = new();
            seri = JsonUtility.FromJson<seriList<float>>(json);
            if (rhythmTimer == null)
                rhythmTimer = new();
            else
                rhythmTimer.Clear();
            for (int i = 0; i < seri.rhtyhm.Count; ++i)
                rhythmTimer.Add(seri.rhtyhm[i]);
            rhythmTimerTimed = puzzleManager.SetSongTiles(rhythmTimer);
        }
        if (rhythmTimerTimed == null) {
            for (int i = 0; i < songSO.songRhythm.Count; ++i)
                rhythmTimer.Add(songSO.songRhythm[i]);
            rhythmTimerTimed = puzzleManager.SetSongTiles(rhythmTimer);
        }
    }

    public void StartSong() {

        songSlider.value = songSlider.minValue;
        isPlaying = true;
        dsptimesong = (float)AudioSettings.dspTime;
        audioSource.Play();
    }

    public void PauseSong() {
        isPlaying = false;
        audioSource.Pause();
    }
}
