using UnityEngine;

[System.Serializable]
public class Audio
{
    //Aquí todos los atributos que puede tener la clase Audio: nombre, archivo, control de volumen (slider), control de tono/velocidad (slider), loopeable

    public string name;
    public AudioClip clip;

    [Range(0f, 1f)]
    public float volume = 1f;

    [Range(0.1f, 3f)]
    public float pitch = 1f;

    public bool loop;

    //Oculto esto porque el AudioManager lo asigna de forma automática

    [HideInInspector]
    public AudioSource source;
}
