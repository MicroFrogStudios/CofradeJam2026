using UnityEngine;

public class AudioNoiseProcessing : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   
    public AudioSource audioSource;
    public float updateStep = 0.1f;
    public int sampleDataLength = 1024;

    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;

    // Use this for initialization
    void Awake()
    {

        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
        clipSampleData = new float[sampleDataLength];

    }

    void Start()
    {

        string firstDeviceName = Microphone.devices[0];
        Debug.Log(Microphone.devices[0]);
        audioSource.clip = Microphone.Start(firstDeviceName, true, 1, AudioSettings.outputSampleRate);
       
        

    }

    // Update is called once per frame
    void Update()
    {

        currentUpdateTime += Time.deltaTime;
        if (currentUpdateTime >= updateStep)
        {
            currentUpdateTime = 0f;
            audioSource.clip.GetData(clipSampleData, audioSource.timeSamples); //I read 1024 samples, which is about 80 ms on a 44khz stereo clip, beginning at the current sample position of the clip.
            clipLoudness = 0f;
            foreach (var sample in clipSampleData)
            {
                clipLoudness += Mathf.Abs(sample);
            }
            clipLoudness /= sampleDataLength; //clipLoudness is what you are looking for
            Debug.Log(clipLoudness.ToString());
        }

    }
}
