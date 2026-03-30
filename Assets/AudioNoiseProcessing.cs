using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class AudioNoiseProcessing : MonoBehaviour
{


    public GameObject voiceLevelMarker;

    public AudioSource audioSource;
    public float updateStep = 0.1f;
    private string CurrentMicroDevice;
    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;

    public float GameVoiceLoudness;
    public float grav = 0.00005f;
    public float descendSpeed = 0;

    public float maxLoud, minLoud;
    public float targetLoudness;
    public float bufferLoudness;

    public int underLoudMistakes = 0;
    public int overLoudMistakes = 0;
    public float graceTime = 1f;

    [HideInInspector]
    public bool startedRecording = false;
    [HideInInspector]
    public bool calibrating = true;

    private bool firstLoop = true;

    private int beingQuiet, beingLoud;

    void Awake()
    {

        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
       
        CurrentMicroDevice = Microphone.devices[0];

        audioSource.clip = Microphone.Start(CurrentMicroDevice, false, 300, AudioSettings.outputSampleRate / 2);
        Debug.Log(AudioSettings.outputSampleRate);
        clipSampleData = new float[AudioSettings.outputSampleRate / 10];
        audioSource.Play();
        StartCoroutine(StartCalibration());

    }
    
    IEnumerator StartCalibration()
    {

        yield return new WaitForSeconds(10);
        //audioSource.Stop();
        //Microphone.End(CurrentMicroDevice);
        calibrating = false;
        targetLoudness = (maxLoud - minLoud) / 2 + minLoud;
        bufferLoudness = (maxLoud - minLoud) / 10;
        StartCoroutine(delayedStart());

    }

    IEnumerator delayedStart()
    {
        yield return new WaitForSeconds(2);
        startGameRecording();
    }

    void startGameRecording()
    {

        //audioSource.clip = Microphone.Start(CurrentMicroDevice, false, 300, AudioSettings.outputSampleRate / 2);

        Debug.Log(audioSource.clip);
        Debug.Log("Max loud: " + maxLoud + " || min loud: " + minLoud);
        //clipSampleData = new float[AudioSettings.outputSampleRate / 10];
        audioSource.volume = 0f;
        //audioSource.Play();
        startedRecording = true;
    }



    void Update()
    {

        if (calibrating)
            CalibratingLoudness();


        if (!startedRecording)
            return;


        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            

            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //The more samplse you get, the more accurate and smooth the reading, but also more costly.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= AudioSettings.outputSampleRate / 10; //Average of the sampled time



            if (GameVoiceLoudness < clipLoudness) 
            { 
                GameVoiceLoudness = clipLoudness;
                descendSpeed = 0f;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                GameVoiceLoudness += 0.005f;
                descendSpeed = 0f;
            }

            
            float normLoud = GameVoiceLoudness / (maxLoud - minLoud);
            bool tooLoud = normLoud > targetLoudness + bufferLoudness;
            bool tooQuiet = normLoud < targetLoudness - bufferLoudness;

            if (tooQuiet)
            {
                beingQuiet++;
            }

            if (tooLoud)
                beingLoud++;

            voiceLevelMarker.transform.SetLocalPositionAndRotation(new Vector3(0,Mathf.Clamp(normLoud,0,1),0), Quaternion.identity);

            descendSpeed += grav;
            GameVoiceLoudness -= descendSpeed;
        }

    }


    void CalibratingLoudness()
    {
        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {


            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //The more samplse you get, the more accurate and smooth the reading, but also more costly.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= AudioSettings.outputSampleRate / 10; //Average of the sampled time

           
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
