using System.Collections;
using UnityEngine;


public class AudioNoiseProcessing : MonoBehaviour
{

    public GameInfoSO gameInfo;
    public GameObject voiceLevelMarker;
    public RectTransform TargetLoudness;
    public AudioSource audioSource;
    public float updateStep = 0.1f;
    private string CurrentMicroDevice;
    private float currentUpdateTime = 0f;

    private float clipLoudness = 0;
    private float[] clipSampleData;
    public int sampleBuffer = 2048;
    public float GameVoiceLoudness;
    private float grav;
    public float gravityPct = .005f;
    public float speedIncrementPct = 0.05f;
    private float gainSpeed = 0;

    public int underLoudMistakes = 0;
    public int overLoudMistakes = 0;
    public float graceTime = 1f;

    [HideInInspector]
    public bool startedRecording = false;
    
    public bool calibrating = false;

    private bool firstLoop = true;

    public int beingQuiet, beingLoud;
    public int errorTolerance = 30;

    public float audioTime = 10f;
    private float audioStart;
    private Vector3 goalPos;
    private void Start()
    {
        goalPos = transform.position;
        clipSampleData = new float[sampleBuffer];
        if (gameInfo.calibrated)
        {
            grav = (gameInfo.maxLoud - gameInfo.minLoud) * gravityPct;
        }

        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (!gameInfo.hasMicro)
            return;

        CurrentMicroDevice = Microphone.devices[0];
        
        string micDevice = Microphone.devices[0];
        Microphone.GetDeviceCaps(micDevice, out int minFreq, out int maxFreq);
        Debug.Log($"Mic supports: {minFreq}Hz - {maxFreq}Hz");
        Debug.Log($"Unity output rate: {AudioSettings.outputSampleRate}");

    }


    void StartCalibration()
    {
        firstLoop = true;
        audioSource.clip = null;
        audioSource.clip = Microphone.Start(CurrentMicroDevice, false, 200, AudioSettings.outputSampleRate);
        Debug.Log(audioSource.clip);
        audioSource.volume = 0.0f;
        Debug.Log(AudioSettings.outputSampleRate / 10);
        audioSource.Play();

        calibrating = true;


       
        

    }

    void EndCalibration()
    {
        audioSource.Stop();
        Microphone.End(CurrentMicroDevice);
        calibrating = false;
        grav = (gameInfo.maxLoud - gameInfo.minLoud) * gravityPct;
        Debug.Log("Max loud: " + gameInfo.maxLoud + " || min loud: " + gameInfo.minLoud);
        gameInfo.calibrated = true;
    }


    public void Calibrate()
    {
       if (calibrating)
            EndCalibration();
       else
            StartCalibration();
    }

    public void StartGameRecording()
    {
        audioSource.clip = null;
        audioSource.clip = Microphone.Start(CurrentMicroDevice, false, 200, AudioSettings.outputSampleRate);
        audioSource.volume = 0f;
        audioSource.Play();
        TargetLoudness.GetComponent<Animation>().Play();
        startedRecording = true;
        audioStart = Time.time;
        GameFlowManager.Instance.idle = false;
        goalPos = transform.position + Vector3.right * 2;
    }


    public void ToggleGameRecording()
    {
        if (!startedRecording && !GameFlowManager.Instance.isWinning)
            StartGameRecording();
    }


    public void EndGameRecording()
    {
        audioSource.Stop();
        Microphone.End(CurrentMicroDevice);
        startedRecording = false;
        GameFlowManager.Instance.startIdleTime = Time.time;
        GameFlowManager.Instance.idle = true;
        beingLoud = 0;
        beingQuiet = 0;
        goalPos = transform.position - Vector3.right * 2;

    }


    void Update()
    {

        if (calibrating) {
            CalibratingLoudness();
            return;
        }

        transform.position = Vector3.Lerp(transform.position, goalPos, .1f);

        if (!startedRecording)
        {
            return;
        }


       

        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            if (gameInfo.hasMicro)
            {
                if (Microphone.GetPosition(CurrentMicroDevice) - sampleBuffer <= 0)
                    return;
                SampleAudio();
            }
           

            if (gameInfo.hasMicro && GameVoiceLoudness < clipLoudness || Input.GetKey(KeyCode.Space)) 
            { 
                
                GameVoiceLoudness += (gameInfo.maxLoud - gameInfo.minLoud) * speedIncrementPct;
                gainSpeed = 0f;
            }
 

            
            float normLoud = (GameVoiceLoudness - gameInfo.minLoud) / (gameInfo.maxLoud - gameInfo.minLoud);
            bool tooLoud = normLoud > TargetLoudness.anchoredPosition.x + TargetLoudness.sizeDelta.x * 0.5f;
            bool tooQuiet = normLoud < TargetLoudness.anchoredPosition.x - TargetLoudness.sizeDelta.x * 0.5f;

            if (tooQuiet)
            {
                voiceLevelMarker.GetComponent<SpriteRenderer>().color = Color.white;
                beingQuiet++;
                if( beingQuiet > 30)
                {
                    underLoudMistakes++;
                    beingQuiet = 0;
                }
            }

            if (tooLoud)
            {
                voiceLevelMarker.GetComponent<SpriteRenderer>().color = Color.red;
                beingLoud++;
                if (beingLoud > 30)
                {
                    overLoudMistakes++;
                    beingLoud = 0;
                    Shush();
                    Debug.Log("SHHH");
                }
            }
                

            if (!tooLoud && !tooQuiet)
            {
                beingLoud = 0;
                beingQuiet = 0;
                voiceLevelMarker.GetComponent<SpriteRenderer>().color = Color.green;
            }


            HandleMistakes();

            voiceLevelMarker.transform.SetLocalPositionAndRotation(new Vector3(0,Mathf.Clamp(normLoud,0,1),-0.01f), Quaternion.identity);

            gainSpeed -= grav;
            GameVoiceLoudness += gainSpeed;
        }

    }

    void Shush()
    {
        if (overLoudMistakes >= 4)
            return;

        for (int i = 0; i < overLoudMistakes*3 && i < GameFlowManager.Instance.shushers.Length; i++) {
            GameFlowManager.Instance.shushers[i].SetTrigger("Callando");
        }

        GameFlowManager.Instance.Shark.SetTrigger("Tiburon_Callando_0" + overLoudMistakes.ToString());

        AudioManager.instance.Play("shhSonido" + overLoudMistakes.ToString());
    }

    void HandleMistakes()
    {
        if (overLoudMistakes > 3) {
            GameFlowManager.Instance.SharkGameOver();
            EndGameRecording();
            GameFlowManager.Instance.phone.gameObject.SetActive(false);
            gameObject.SetActive(false);
            return;
        }

        if (audioTime + audioStart > Time.time)
            return;
        
        EndGameRecording();
        if (underLoudMistakes >= 3)
        {
            GameFlowManager.Instance.phone.AddVoiceMessage(true);
            underLoudMistakes = 0;
            return;
        }
        GameFlowManager.Instance.phone.AddVoiceMessage(false);

    }

    void SampleAudio()
    {

        audioSource.clip.GetData(clipSampleData, Microphone.GetPosition(CurrentMicroDevice) - sampleBuffer); //The more samplse you get, the more accurate and smooth the reading, but also more costly.
        clipLoudness = 0f;
        foreach (var sample in clipSampleData)
        {
            clipLoudness += Mathf.Abs(sample);
        }
        clipLoudness /= 1024; //Average of the sampled time

        clipLoudness = 20 * Mathf.Log10(clipLoudness / 0.1f);
        Debug.Log(clipLoudness);
    }

    void CalibratingLoudness()
    {
        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;

  
            if (Microphone.GetPosition(CurrentMicroDevice) - sampleBuffer <= 0)
                return;

            SampleAudio();

            if (GameVoiceLoudness < clipLoudness)
            {

                GameVoiceLoudness += (gameInfo.maxLoud - gameInfo.minLoud) * speedIncrementPct;
                gainSpeed = 0f;
            }

            float normLoud = (GameVoiceLoudness - gameInfo.minLoud) / (gameInfo.maxLoud - gameInfo.minLoud);
            voiceLevelMarker.transform.SetLocalPositionAndRotation(new Vector3(0, Mathf.Clamp(normLoud, 0, 1), 0), Quaternion.identity);

            gainSpeed -= grav;
            GameVoiceLoudness += gainSpeed;


            if (firstLoop)
            {
                gameInfo.minLoud = clipLoudness;
                gameInfo.maxLoud = clipLoudness;
                firstLoop = false;
            }

            if (clipLoudness < gameInfo.minLoud)
                gameInfo.minLoud = clipLoudness;

            if (clipLoudness > gameInfo.maxLoud)
                gameInfo.maxLoud = clipLoudness;
            grav = (gameInfo.maxLoud - gameInfo.minLoud) * gravityPct;

        }
    }
}
