using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public void CargarMuseoPC()
    {
        SceneManager.LoadScene("MuseoPC");
    }
    public void CargarMuseoMP()
    {
        SceneManager.LoadScene("MuseoMP");
    }
    public void CargarJuegoPC()
    {
        SceneManager.LoadScene("EggFirePC");
    }
    public void CargarMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }
        SceneManager.LoadScene("MenuInicial");
    }
}
