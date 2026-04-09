using System;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class NetworkPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce;
    [SerializeField] MainCameraObject MainCamera;
    [SerializeField] Canvas ownCanvas;
    [SerializeField] private Transform cameraAnchor;
    [SerializeField] private float sensibilidad = 1f;
    public LayerMask capaPlantaBaja;
    public LayerMask capaPiso1;
    public LayerMask capasComunes;
    [SerializeField] Canvas menuCanvas;
    
    Rigidbody rb;
    private Camera ChildCam;
    private Vector2 inputMouse;
    private float rotacionX = 0f;
    private float rotacionY = 0f;
    private PlayerInput playerInput;
    private InputAction cameraLook;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction menuAction;
    private NetworkUI NUI;
    private string NUINAme;
    private TextMeshProUGUI playernameTMP;
    private bool isGrounded;
    private bool onMenu;
    private NetworkCounter CounterCanvas;
    private NetworkVariable<Color> networkColor = new NetworkVariable<Color>(
        Color.white, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner // El dueño elige su color
    );
    private NetworkVariable<FixedString32Bytes> networkPlayerName=new NetworkVariable<FixedString32Bytes>(
        "NoName",
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner
    );

    private Color[] colores={Color.blue,Color.red,Color.green, Color.yellow};

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();              
        //MainCamera = MainCameraObject.FindFirstObjectByType<MainCameraObject>();
        onMenu=false;
        menuCanvas.gameObject.SetActive(false);
        CounterCanvas=NetworkCounter.FindFirstObjectByType<NetworkCounter>();
        //VerPlantaBaja();
        //Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("P1"));
        NUI=NetworkUI.FindFirstObjectByType<NetworkUI>();
        rb=GetComponent<Rigidbody>();
        if (NUI!=null)
        {
            Transform inputField = NUI.transform.GetChild(2);
            TMP_InputField input = inputField.GetComponent<TMP_InputField>();
            string contenido = null;
            if (input != null)
            {
                contenido = input.text;
                NUINAme=contenido;
                playernameTMP=ownCanvas.GetComponentsInChildren<TextMeshProUGUI>()[0];
                
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
        cameraLook=playerInput.actions["Look"];       
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
    public void OnJump(InputValue value)
    {
        if (!IsOwner) return;
        if (value.isPressed)
        {
            Debug.Log("¡Intentando saltar!");
        }
    }

    public override void OnNetworkSpawn()
    {
        networkColor.OnValueChanged += OnColorChanged;
        networkPlayerName.OnValueChanged+=OnNameChanged;
        CounterCanvas.AddCounterRpc();
        if (!IsOwner)
        {            
            // Disable input for remote players
            playerInput.enabled = false;
            ApplyColor(networkColor.Value);
            ChangeName(networkPlayerName.Value);
        }
        else
        {
            GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");
            if (mainCam != null)
            {
                // Convertimos la cámara en hija del ancla dinámicamente
                mainCam.transform.SetParent(cameraAnchor);
                
                // Reseteamos posición local (el offset lo da el ancla o lo pones aquí)
                mainCam.transform.localPosition = new Vector3(0, 2.5f, -4f); // 4 metros atrás
                mainCam.transform.localRotation =  Quaternion.Euler(20f,0f,0f);
                
                Cursor.lockState = CursorLockMode.Locked;
            }

            // Camera ChildCam = Camera.main;
            // if (ChildCam != null)
            // {
            //     // 2. La hacemos hija de este objeto
            //     ChildCam.transform.SetParent(this.transform);

            //     // 3. La reseteamos a la altura de los ojos
            //     ChildCam.transform.localPosition = new Vector3(0, 2.5f, -4.0f); 
            //     ChildCam.transform.localRotation = Quaternion.Euler(20f,0f,0f);
                
            //     // 4. Activamos el script de rotación de mouse
            //     // cam.GetComponent<MouseLook>().enabled = true;
            // }
            Camera.main.cullingMask = capasComunes | capaPlantaBaja;            
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            int playerIndex = (players.Length - 1) % colores.Length;
            networkColor.Value = colores[playerIndex];
            networkPlayerName.Value=NUINAme;
        }
    }
    public override void OnNetworkDespawn()
    {
        CounterCanvas.ReduceCounterRpc();
        base.OnNetworkDespawn();
        if (!IsOwner)
        {
            //Nothing
        }
        else
        {
            
        }
    }
    private void OnDisable()
    {
        // Solo si este es mi jugador local
        if (IsOwner) 
        {
            Camera cam = Camera.main;
            if (cam != null && cam.transform.parent == cameraAnchor.transform)
            {
                // La soltamos antes de que este objeto desaparezca
                cam.transform.SetParent(null);
                
                // Opcional: Mandar la cámara a una posición segura o de "Espectador"
                cam.transform.position = new Vector3(0, 10, 0); 
            }
        }
    }
    private void OnColorChanged(Color oldColor, Color newColor)
    {
        ApplyColor(newColor);
        //Debug.Log("Ingresa color"+NetworkObjectId+oldColor.ToString()+":"+newColor.ToString()); 
    }
    private void OnNameChanged(FixedString32Bytes oldName, FixedString32Bytes newName)
    {
        ChangeName(newName);
    }
    private void ChangeName(FixedString32Bytes playername)
    {
        playernameTMP.text=playername.ToString();
    }
    private void ApplyColor(Color colorToApply)
    {        
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        renderer.material.color=colorToApply;
    }
    [Rpc(SendTo.ClientsAndHost)]
    private void JumpServerRpc()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    public void OnLook(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;
        
        // Leemos el movimiento del mouse (Delta)
        inputMouse = context.ReadValue<Vector2>();
        Debug.Log(inputMouse);
    }
    private void Update()
    {
        if (!IsOwner) return;

        var moveInput = moveAction.ReadValue<Vector2>();        
        if (jumpAction.triggered&& isGrounded)
        {
            JumpServerRpc();
        }
        if (menuAction.triggered)
        {
            if (onMenu)
            {
                Debug.Log("MenuDesactivado");
                onMenu=false;
                Cursor.lockState = CursorLockMode.Locked; // Bloquea el mouse
                Cursor.visible = false;
                //this.enabled = true;
            }
            else
            {
                Debug.Log("MenuActivado");
                onMenu=true;
                Cursor.lockState = CursorLockMode.None; // Libera el mouse
                Cursor.visible = true;
                //this.enabled = false;
                
            }
            menuCanvas.gameObject.SetActive(onMenu);
        }
        var movement = new Vector3(moveInput.x, 0f, moveInput.y);
        
        //if (Mathf.Abs(moveInput.y) > 0.01f)
        //{
            // Forzamos a que el multiplicador sea siempre 1 o -1 
            // para que no importe si el joystick está a mitad de camino
            //float direction = moveInput.y > 0 ? 1 : -1;

            // Si usas el valor directo (input.y), asegúrate de que no se esté 
            // mezclando con el eje X.
            //transform.position += transform.forward * direction * moveSpeed * Time.deltaTime;
        //}
        transform.Translate(movement * (moveSpeed * Time.deltaTime), Space.Self);
        //MainCamera.transform.position=new Vector3(transform.position.x,transform.position.y+5,transform.position.z-7);
    }
    void LateUpdate() // LateUpdate es mejor para cámaras
    {
        if (!IsOwner) return;
        inputMouse=cameraLook.ReadValue<Vector2>();
        rotacionY += inputMouse.x * sensibilidad;
        //rotacionX -= inputMouse.y * sensibilidad;
        //rotacionX = Mathf.Clamp(rotacionX, -20f, 60f);
        //if (Mathf.Abs(rotacionY) > 0.01f)
        //{
            //float turnAmount = rotacionY * sensibilidad * Time.deltaTime;
            //transform.Rotate(0, turnAmount, 0);
        //}
        if (Mathf.Abs(inputMouse.x) > 0.01f)
        {
            float turnAmount = inputMouse.x * 200 * Time.deltaTime;
            transform.Rotate(0, turnAmount, 0);
        }
        //transform.rotation=Quaternion.Euler(0f, rotacionY, 0f);

        // Rotamos el ancla (que ahora tiene a la cámara como hija)
        //cameraAnchor.localRotation = Quaternion.Euler(0f, rotacionY, 0f);
        inputMouse=Vector2.zero;
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
            Debug.Log("Piso uno");
            VerPisoUno();
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("PB"))
        {
            Debug.Log("Planta baja");
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
            Debug.Log("Piso uno");
            VerPisoUno();
        }
        if (collision.gameObject.layer == LayerMask.NameToLayer("PB"))
        {
            Debug.Log("Planta baja");
            VerPlantaBaja();
        }
    }
}