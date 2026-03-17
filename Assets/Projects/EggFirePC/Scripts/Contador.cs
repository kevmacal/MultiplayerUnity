using TMPro;
using UnityEngine;

public class Contador : MonoBehaviour
{
    [SerializeField] private float tiempoRestante = 12f;
    [SerializeField] private TextMeshProUGUI textoContador;
    [SerializeField] private TextMeshProUGUI textoInstrucciones;
    private bool cuentaActiva = true;
    private bool startGame=false;
    void Update()
    {
        if (cuentaActiva&&startGame)
        {
            if (tiempoRestante > 0)
            {
                tiempoRestante -= Time.deltaTime; // Resta el tiempo real entre frames
                ActualizarContador(tiempoRestante);
            }
            else
            {
                tiempoRestante = 0;
                cuentaActiva = false;
                GameOver();
            }
        }
    }
    void ActualizarContador(float tiempo)
    {
        //Formatea el tiempo para que no muestre 1000 decimales
        //"0" significa que solo muestra el entero.
        textoContador.text = tiempo.ToString("F0"); 
        
        //Si queda poco tiempo, ponerlo en rojo
        if (tiempo <= 3) { textoContador.color = Color.red; }
    }
    void GameOver()
    {
        Debug.Log("¡Se acabó el tiempo!");
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        textoInstrucciones.text="Juego terminado";
        // SceneManager.LoadScene("Menu");
    }
    public void StartGame()
    {
        startGame=true;
        textoInstrucciones.text="";
    }
}
