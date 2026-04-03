using UnityEngine;

[CreateAssetMenu(fileName = "GameInfoSO", menuName = "Scriptable Objects/GameInfoSO")]
public class GameInfoSO : ScriptableObject
{

    public float maxLoud = 1f;
    public float minLoud = 0f;

    public bool calibrated = false;

    public bool hasMicro;
}
