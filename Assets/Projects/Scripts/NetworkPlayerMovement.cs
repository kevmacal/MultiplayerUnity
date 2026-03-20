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
    public LayerMask capaPlantaBaja;
    public LayerMask capaPiso1;
    public LayerMask capasComunes;
    [SerializeField] Canvas menuCanvas;
    
    Rigidbody rb;
    private PlayerInput playerInput;
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
        MainCamera = MainCameraObject.FindFirstObjectByType<MainCameraObject>();
        onMenu=false;
        menuCanvas.gameObject.SetActive(false);
        CounterCanvas=NetworkCounter.FindFirstObjectByType<NetworkCounter>();
        VerPlantaBaja();
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
                onMenu=false;
            }
            else
            {
                onMenu=true;
            }
            menuCanvas.gameObject.SetActive(onMenu);
        }
        var movement = new Vector3(moveInput.x, 0f, moveInput.y);
        transform.Translate(movement * (moveSpeed * Time.deltaTime), Space.World);
        MainCamera.transform.position=new Vector3(transform.position.x,transform.position.y+5,transform.position.z-7);
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