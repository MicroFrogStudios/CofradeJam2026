using UnityEngine;

public class AudioPruebas : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AudioManager.instance.Play("envioAudio");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AudioManager.instance.Play("envioAudioConfirmado");
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            AudioManager.instance.Play("vibracion");
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            AudioManager.instance.Play("shhSonido");
        }
    }
}
