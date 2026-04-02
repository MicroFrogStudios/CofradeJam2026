using UnityEngine;

[CreateAssetMenu(fileName = "GameInfoSO", menuName = "Scriptable Objects/GameInfoSO")]
public class GameInfoSO : ScriptableObject
{
    public float maxLoud, minLoud;
    public bool calibrated = false;

    public bool hasMicro;
}
