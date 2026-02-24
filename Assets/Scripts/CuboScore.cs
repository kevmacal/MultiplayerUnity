using Unity.Netcode;
using UnityEngine;

public class CuboScore : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player==null)
            {
                return;
            }
            if (player.IsOwner)
            {
                Debug.Log("A Player enter to area");
                player.CollectItemServerRPC(1);
            }                       
        }        
    }    

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.gameObject.GetComponent<PlayerMovement>();
            if (player==null)
            {
                return;
            }
            Debug.Log("A Player exit area");
        }
    }
    private void OnEnable()
    {
        
    }
    private void OnDisable()
    {
        
    }
}
