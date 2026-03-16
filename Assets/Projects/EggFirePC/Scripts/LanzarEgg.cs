using UnityEngine;
using UnityEngine.InputSystem;

public class LanzarEgg : MonoBehaviour
{
    [SerializeField] private GameObject proyectilPrefab;
    [SerializeField] private Transform puntoDisparo;
    [SerializeField] private float fuerzaFrontal = 10f;
    [SerializeField] private float fuerzaAscendente = 5f;
    private int eggs=0;
    private int maxEggs=3;


    public void OnFire()
    {
        if (eggs<maxEggs)
        {
            Lanzar();
        }
        else
        {
            Debug.Log("Hay que esperar para recargar");
        }
    }
    void Lanzar()
    {
        //Sumar huevo en lanzamiento
        eggs++;
        GameObject huevo = Instantiate(proyectilPrefab, puntoDisparo.position, puntoDisparo.rotation);
        Egg huevoNuevo=huevo.GetComponent<Egg>();
        huevoNuevo.Configurar(this);

        //Fisicas de lanzamiento
        Rigidbody rb = huevo.GetComponent<Rigidbody>();

        // 1. Calculamos el vector de lanzamiento
        // Combinamos el "adelante" del jugador con un poco de "arriba"
        Vector3 direccionLanzamiento = (transform.forward * fuerzaFrontal) + (transform.up * fuerzaAscendente);

        // 2. Aplicamos la fuerza de una sola vez (Impulse)
        rb.AddForce(direccionLanzamiento, ForceMode.Impulse);        
        
    }
    public void SelfDestroyNotify()
    {
        eggs--;
        if (eggs<0)
        {
            eggs=0;
        }
    }
}
