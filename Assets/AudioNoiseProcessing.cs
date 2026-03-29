using UnityEngine;
using UnityEngine.UI;

public class AudioNoiseProcessing : MonoBehaviour
{


    public Text text;
    public Slider slider;
    public AudioSource audioSource;
    public float updateStep = 0.1f;
    private string CurrentMicroDevice;
    private float currentUpdateTime = 0f;

    private float clipLoudness;
    private float[] clipSampleData;

    public float GameVoiceLoudness;
    public float grav = 0.0001f;
    public float descendSpeed = 0;

    // Use this for initialization
    void Awake()
    {

        if (!audioSource)
        {
            audioSource = GetComponent<AudioSource>();
        }
       
        CurrentMicroDevice = Microphone.devices[0];
 
        Debug.Log(Microphone.devices[0]);
        audioSource.clip = Microphone.Start(CurrentMicroDevice, false, 300, AudioSettings.outputSampleRate/2);
        Debug.Log(AudioSettings.outputSampleRate);
        clipSampleData = new float[AudioSettings.outputSampleRate / 10];
        audioSource.Play();

    }

    // Update is called once per frame
    void Update()
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

            text.text = clipLoudness.ToString();
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


            slider.value = GameVoiceLoudness;
            descendSpeed += grav;
            GameVoiceLoudness -= descendSpeed;
        }

    }
}
