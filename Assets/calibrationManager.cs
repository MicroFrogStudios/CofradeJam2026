using UnityEngine;

public class calibrationManager : MonoBehaviour
{
    public GameInfoSO gameInfo;

    public GameObject hasMicroUI,noMicroUI;

    public GameObject audioMinigame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
          if ( Microphone.devices.Length > 0)
        {
            ActivateCalibrationUI();
        }
        else
        {
            NoMicroWarning();
        }
    }

    void ActivateCalibrationUI()
    {
        gameInfo.hasMicro = true;
        hasMicroUI.SetActive(true);
        noMicroUI.SetActive(false);
    }

    void NoMicroWarning()
    {
        gameInfo.hasMicro = false;
        hasMicroUI.SetActive(false);
        noMicroUI.SetActive(true);
        audioMinigame.SetActive(false );

    }
}
