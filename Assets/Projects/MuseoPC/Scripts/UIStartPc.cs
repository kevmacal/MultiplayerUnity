using UnityEngine;

public class UIStartPc : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform spawnPoint;
    public void startPlayer()
    {
        //NetworkManager.Singleton.StartHost();
        GetComponent<Canvas>().enabled = false;
        SpawnPlayer();
        //Debug.Log("Boton StartHost"); 
    }
    public void SpawnPlayer()
    {
        // 1. Instanciamos el prefab en la posición y rotación del SpawnPoint
        GameObject newPlayer = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);

        // 2. Opcional: Si quieres pasarle el nombre que guardaste antes:
        // var data = FindObjectOfType<PlayerDataGlobals>(); 
        // newPlayer.GetComponent<TuScriptDeJugador>().ConfigurarNombre(data.playerName);
        
        Debug.Log("Jugador spawneado en: " + spawnPoint.position);
    }
}
