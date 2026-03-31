using System.Collections;
using UnityEngine;


public class AudioNoiseProcessing : MonoBehaviour
{


    public GameObject voiceLevelMarker;
    public Transform TargetLoudness;
    public AudioSource audioSource;
    public float updateStep = 0.1f;
    private string CurrentMicroDevice;
    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;
    public int sampleBuffer = 2048;
    public float GameVoiceLoudness;
    private float grav;
    public float gravityPct = .005f;
    public float speedIncrementPct = 0.05f;
    private float gainSpeed = 0;

    public float maxLoud, minLoud;

    public int underLoudMistakes = 0;
    public int overLoudMistakes = 0;
    public float graceTime = 1f;

    [HideInInspector]
    public bool startedRecording = false;
    
    public bool calibrating = false;

    private bool firstLoop = true;

    public int beingQuiet, beingLoud;
    public int errorTolerance = 30;
    

    private void Awake()
    {

        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
       
        CurrentMicroDevice = Microphone.devices[0];
        clipSampleData = new float[sampleBuffer];

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
        grav = (maxLoud - minLoud) * gravityPct;
        Debug.Log("Max loud: " + maxLoud + " || min loud: " + minLoud);
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

    }


    public void ToggleGameRecording()
    {
        if (Microphone.IsRecording(CurrentMicroDevice))
            EndGameRecording();
        else
            StartGameRecording();
    }


    public void EndGameRecording()
    {
        audioSource.Stop();
        Microphone.End(CurrentMicroDevice);
    }


    void Update()
    {

        if (calibrating) {
            CalibratingLoudness();
            return;
        }


        if (!Microphone.IsRecording(CurrentMicroDevice))
        {
            return;
        }




        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            if (Microphone.GetPosition(CurrentMicroDevice) - sampleBuffer <= 0)
                return;
            SampleAudio();

            if (GameVoiceLoudness < clipLoudness || Input.GetKey(KeyCode.Space)) 
            { 
                
                GameVoiceLoudness += (maxLoud - minLoud) * speedIncrementPct;
                gainSpeed = 0f;
            }
 

            
            float normLoud = (GameVoiceLoudness - minLoud) / (maxLoud - minLoud);
            bool tooLoud = normLoud > TargetLoudness.localPosition.y + TargetLoudness.localScale.y * 0.5f;
            bool tooQuiet = normLoud < TargetLoudness.localPosition.y - TargetLoudness.localScale.y * 0.5f;

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
                }
            }
                

            if (!tooLoud && !tooQuiet)
            {
                beingLoud = 0;
                beingQuiet = 0;
                voiceLevelMarker.GetComponent<SpriteRenderer>().color = Color.green;
            }

            

            voiceLevelMarker.transform.SetLocalPositionAndRotation(new Vector3(0,Mathf.Clamp(normLoud,0,1),0), Quaternion.identity);

            gainSpeed -= grav;
            GameVoiceLoudness += gainSpeed;
        }

    }


    void SampleAudio()
    {
        //audioSource.GetOutputData(clipSampleData, 0);
        
        //audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //The more samplse you get, the more accurate and smooth the reading, but also more costly.
        //audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //The more samplse you get, the more accurate and smooth the reading, but also more costly.
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
           

            if (firstLoop)
            {
                minLoud = clipLoudness;
                maxLoud = clipLoudness;
                firstLoop = false;
            }

            if (clipLoudness < minLoud)
                minLoud = clipLoudness;

            if (clipLoudness > maxLoud)
                maxLoud = clipLoudness;

        }
    }
}
