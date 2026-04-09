using UnityEngine;

public class AudioController : MonoBehaviour
{
    private AudioSource miSonido;

    void Awake()
    {
        miSonido = GetComponent<AudioSource>();
    }

    // Función para iniciar el sonido
    public void ReproducirMusica()
    {
        if (!miSonido.isPlaying) // Solo si no se está reproduciendo ya
        {
            miSonido.Play();
        }
    }

    // Función para detener el sonido
    public void DetenerMusica()
    {
        if (miSonido.isPlaying)
        {
            miSonido.Stop();
        }
    }
    
    // Función para pausar
    public void PausarMusica()
    {
        miSonido.Pause();
    }
}
