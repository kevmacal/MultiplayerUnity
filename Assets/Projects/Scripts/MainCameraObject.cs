using UnityEngine;
using UnityEngine.InputSystem;

public class MainCameraObject : MonoBehaviour
{
    private Transform target; 
    [SerializeField] private Vector3 offset = new Vector3(0, 4, -6); // Altura y distancia
    [SerializeField] private float smoothSpeed = 10f;

    // Esta es la función que conectarás a tu evento C# de Input
    public void OnLook(Vector2 value)
    {
        // El evento nos entrega cuánto se movió el mouse en este frame
        
        //lookInput = value;
    }
    void Start()
    {
        // Bloqueamos el mouse en el centro de la pantalla
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Calculamos la posición deseada BASADA en la rotación del jugador
        // TransformPoint calcula automáticamente dónde es "atrás y arriba" 
        // según hacia dónde mire la cápsula.
        Vector3 desiredPosition = target.TransformPoint(offset);

        // 2. Movemos la cámara suavemente
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        // 3. Hacemos que la cámara mire hacia el frente del jugador
        // Le sumamos un poco de altura al "target.position" para que no mire a los pies
        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
    public void BloquearMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void LiberarMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
