using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class EncenderSalon : NetworkBehaviour
{
    [SerializeField] GameObject sala;
    [SerializeField] Light[] luces;
    [Header("Configuración")]
    [SerializeField] float esperaInicial;
    [SerializeField] float intervaloLuces;
    private void Start()
    {
        foreach (Light luz in luces)
        {
            if (luz != null)
            {
                luz.enabled = false;
            }
        }
    }
    //private bool activado = false; // Para que solo pase una vez
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es un jugador
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        //Verificar que el Objeto no sea Null
        if (playerNetObj == null) return;
        sala.SetActive(true);
        //activado = true;
        StartCoroutine(EncendidoLuces());
    }

    // Se ejecuta cuando algo SALE del trigger
    private void OnTriggerExit(Collider other)
    {
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;

        //if (!playerNetObj.IsOwner) return;

        // Desactivar canvas
        
    }
    IEnumerator EncendidoLuces()
    {
        yield return new WaitForSeconds(esperaInicial);

        // 2. Aparecen las luces una a una
        foreach (Light luz in luces)
        {
            if (luz != null)
            {
                luz.enabled = true;
                yield return new WaitForSeconds(intervaloLuces);
            }
        }
    }
}
