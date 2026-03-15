using System.ComponentModel;
using JetBrains.Annotations;
using TMPro;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovementMuseoPC : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce;
    [SerializeField] MainCameraObject MainCamera;
    [SerializeField] Canvas ownCanvas;
    public LayerMask capaPlantaBaja;
    public LayerMask capaPiso1;
    public LayerMask capasComunes;
    [SerializeField] Canvas menuCanvas;
    
    Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction menuAction;
    private InputAction lookAction;
    private UIStartPc NUI;
    private string NUINAme;
    private TextMeshProUGUI playernameTMP;
    private bool isGrounded;
    private bool onMenu;
    private Color color;
    private float rotationSpeed = 90f;
    //private Vector3 lastPosition;
    //private float currentSpeed;

    private Color[] colores={Color.blue,Color.red,Color.green, Color.yellow};

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();        
        MainCamera = MainCameraObject.FindFirstObjectByType<MainCameraObject>();
        MainCamera.SetTarget(this.transform);
        MainCamera.BloquearMouse();
        onMenu=false;
        menuCanvas.gameObject.SetActive(false);
        VerPlantaBaja();
        //Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("P1"));
        NUI=UIStartPc.FindFirstObjectByType<UIStartPc>();
        rb=GetComponent<Rigidbody>();
        if (NUI!=null)
        {
            Transform inputField = NUI.transform.GetChild(2);
            TMP_InputField input = inputField.GetComponent<TMP_InputField>();
            playernameTMP=ownCanvas.GetComponentsInChildren<TextMeshProUGUI>()[0];
            string contenido = null;
            if (input != null)
            {                
                contenido = input.text;
                NUINAme=contenido;             
                
            }
            else
            {
                NUINAme="NoName";
                //playernameTMP.text="NoName";
            }
            
        }
        else
        {
            Debug.Log("Null");
        }
        
        moveAction = playerInput.actions["Move"];
        jumpAction=playerInput.actions["Jump"];
        menuAction=playerInput.actions["Menu"];
        lookAction=playerInput.actions["Look"];
        
        //Debug.Log("Inicia");
    }
    void Start() 
    {
        //lastPosition = transform.position;
    }
    public void VerPlantaBaja()
    {
        // La cámara solo mostrará lo Común + Planta Baja
        Camera.main.cullingMask = capasComunes | capaPlantaBaja;
    }
    public void VerPisoUno()
    {
        // La cámara solo mostrará lo Común + Planta Baja
        Camera.main.cullingMask = capasComunes | capaPiso1;
    }

    public void OnEnable()
    {
        ApplyColor(colores[0]);
        ChangeName(NUINAme);
        lookAction.performed+=OnLookInput;     
    }
    private void OnLookInput(InputAction.CallbackContext context)
    {
        // Obtenemos el Vector2 del mouse
        Vector2 value = context.ReadValue<Vector2>();
        
        // Se lo pasamos al script de la cámara que creamos antes
        MainCamera.OnLook(value);
    }
    
    private void ChangeName(string playername)
    {
        Debug.Log(playername);
        playernameTMP.text=playername.ToString();
    }
    private void ApplyColor(Color colorToApply)
    {     
        Debug.Log(colorToApply.ToString());   
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.color=colorToApply;
    }
    private void Jump()
    {
        //Debug.Log("Salta");
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    private void Update()
    {
        var moveInput = moveAction.ReadValue<Vector2>(); 
        float h = moveAction.ReadValue<Vector2>().x;
        float v = moveAction.ReadValue<Vector2>().y;
        if (jumpAction.triggered&& isGrounded)
        {
            Jump();
        }
        if (menuAction.triggered)
        {
            if (onMenu)
            {
                onMenu=false;
                MainCamera.BloquearMouse();
            }
            else
            {
                onMenu=true;
                MainCamera.LiberarMouse();
            }
            menuCanvas.gameObject.SetActive(onMenu);
        }
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 1. ROTACIÓN: La mantenemos igual, pero asegúrate de que rotationSpeed sea alta
        if (Mathf.Abs(input.x) > 0.01f)
        {
            float turnAmount = input.x * rotationSpeed * Time.deltaTime;
            transform.Rotate(0, turnAmount, 0);
        }

        // 2. MOVIMIENTO: Aquí está el truco para no perder velocidad
        // Usamos el valor absoluto del input o simplemente detectamos si hay presión
        if (Mathf.Abs(input.y) > 0.01f)
        {
            // Forzamos a que el multiplicador sea siempre 1 o -1 
            // para que no importe si el joystick está a mitad de camino
            float direction = input.y > 0 ? 1 : -1;

            // Si usas el valor directo (input.y), asegúrate de que no se esté 
            // mezclando con el eje X.
            transform.position += transform.forward * direction * moveSpeed * Time.deltaTime;
        }
        // Cálculo de velocidad: Distancia / Tiempo
        //float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        //currentSpeed = distanceMoved / Time.deltaTime;
        //lastPosition = transform.position;

        // // Solo imprime si te estás moviendo para no llenar la consola
        // if (currentSpeed > 0.1f)
        // {
        //     Debug.Log("Velocidad Real: " + currentSpeed.ToString("F2") + " m/s");
        // }
    }
    private void OnCollisionEnter(Collision collision)
    {
        // Opcional: Revisa si el objeto tiene el tag "Suelo"
        if (collision.gameObject.CompareTag("Suelo"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("P1"))
        {
            //Debug.Log("Piso uno");
            VerPisoUno();
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("PB"))
        {
            //Debug.Log("Planta baja");
            VerPlantaBaja();
        }
    }

    // Se ejecuta cuando dejas de tocar el suelo
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Suelo"))
        {
            isGrounded = false;
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("P1"))
        {
            //Debug.Log("Piso uno");
            VerPisoUno();
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("PB"))
        {
            //Debug.Log("Planta baja");
            VerPlantaBaja();
        }
    }
}
