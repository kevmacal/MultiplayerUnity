using Unity.Netcode;
using UnityEngine;

public class NetworkManagerUI : MonoBehaviour
{
    void OnGUI()
    {
        // Crea un área pequeña en la esquina superior izquierda
        GUILayout.BeginArea(new Rect(10, 10, 200, 200));

        // Solo mostramos los botones si NO estamos conectados aún
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUILayout.Button("Iniciar HOST (Editor)"))
            {
                NetworkManager.Singleton.StartHost();
            }

            if (GUILayout.Button("Iniciar CLIENTE (Virtual)"))
            {
                NetworkManager.Singleton.StartClient();
            }
        }

        GUILayout.EndArea();
    }
}
