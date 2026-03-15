using Unity.Netcode;
using UnityEngine;

public class NetworkStairs : NetworkBehaviour
{
    [SerializeField]GameObject stairs;


    void Awake()
    {
        if (stairs != null)
        {
            //Debug.Log("Desactivado");
            stairs.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("es null");
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        // Verificar si es un jugador
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        //Verificar que el Objeto no sea Null
        if (playerNetObj == null) return;
        // Verificar si es el jugador LOCAL (IsOwner)
        //if (!playerNetObj.IsOwner) return;
        // Activar canvas SOLO para este jugador
        //Debug.Log("Activar Stairs");
        ShowStairs();
    }
    private void OnTriggerExit(Collider other)
    {
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;

        //if (!playerNetObj.IsOwner) return;

        // Desactivar canvas
        HideStairs();
    }

    private void ShowStairs()
    {
        if (stairs != null)
        {
            stairs.gameObject.SetActive(true);
        }
    }
    private void HideStairs()
    {
        if (stairs!=null)
        {
            stairs.gameObject.SetActive(false);
        }
    }
}
