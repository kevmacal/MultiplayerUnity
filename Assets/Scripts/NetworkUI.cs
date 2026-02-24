using Unity.Netcode;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    //private bool isStarted=false;
    public void startHost()
    {
        NetworkManager.Singleton.StartHost();
        GetComponent<Canvas>().enabled = false;
        //Debug.Log("Boton StartHost"); 
    }
    public void startClient()
    {
        NetworkManager.Singleton.StartClient();
        GetComponent<Canvas>().enabled = false;
        //Debug.Log("Boton StartClient");
    }
}
