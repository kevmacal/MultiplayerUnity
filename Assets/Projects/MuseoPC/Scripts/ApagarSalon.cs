using Unity.Netcode;
using UnityEngine;

public class ApagarSalon : NetworkBehaviour
{
    [SerializeField] GameObject sala;
    [SerializeField] Light[] luces;
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es un jugador
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        //Verificar que el Objeto no sea Null
        if (playerNetObj == null) return;
        //sala.SetActive(true);
    }

    // Se ejecuta cuando algo SALE del trigger
    private void OnTriggerExit(Collider other)
    {
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;

        //if (!playerNetObj.IsOwner) return;

        // Desactivar canvas
        sala.SetActive(false);
        ApagarLuces();
    }
    private void ApagarLuces()
    {
        foreach (Light luz in luces)
        {
            if (luz != null)
            {
                luz.enabled = false;
            }
        }
    }
}
