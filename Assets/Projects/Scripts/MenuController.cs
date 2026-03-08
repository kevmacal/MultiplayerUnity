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
}
