using System;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Instancia estática (el Singleton) que nos permite e acceso global
    public static AudioManager instance;

    public Audio[] sonidos;

    void Awake()
    {
        //Configuración del Singleton
        //-----------------------------
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            //por si ya existiera un AudioManager, evitamos duplicados por si escalamos el código en alguna dirección que pueda ocurrir esto
            Destroy(gameObject);
            return;
        }

        //Importante si no queremos que se corte el audio entre cambios de escena
        DontDestroyOnLoad(gameObject);


        //Inicialización de los Audios
        //-----------------------------
        foreach (Audio aud in sonidos)
        {
            aud.source = gameObject.AddComponent<AudioSource>();

            //Aquí se asignan los valores configurados al AudioSource real
            aud.source.clip = aud.clip;
            aud.source.volume = aud.volume;
            aud.source.pitch = aud.pitch;
            aud.source.loop = aud.loop;
        } //podría haberle llamado "a" y me habría ahorrado escribir tantas veces "aud", pero este comentario tampoco es eficiente así que...
    }


    //SEÑORAS Y SEÑORES, EL ÚNICO, EL INIGUALABLE... MÉTODO PARA ACTIVAR SONIDOS!!!
    public void Play(string name)
    {
        //Esto hay que testearlo: lo encontré en un proyecto antiguo, lo modifico para esto. Debería buscar en el array el sonido que coincida con el nombre que le pasamos
        Audio aud = Array.Find(sonidos, sonidosParam => sonidosParam.name == name);

        //Y esto por si nos equivocamos con el nombre nos de un aviso
        if (aud == null)
        {
            Debug.LogWarning("AudioManager: Merluzo! no se encontró el sonido con el nombre: " + name);
            return;
        }

        aud.source.Play();
    }
}
