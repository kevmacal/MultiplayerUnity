using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] float speed;
    [SerializeField] MainCameraObject MainCamera;
    private Vector2 moveInput;
    private Rigidbody rb;
    private NetworkVariable<Color> networkColor = new NetworkVariable<Color>(
        Color.white, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner // El dueño elige su color
    );
    private Color[] colores={Color.green,Color.blue,Color.red};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MainCamera = MainCameraObject.FindFirstObjectByType<MainCameraObject>();
        //MeshRenderer renderer = GetComponent<MeshRenderer>();

        //renderer.material.color = Color.blue;
    }

    void OnMove(InputValue value)
    {
        if (!IsOwner) return;
        moveInput = value.Get<Vector2>();
    }
    // void FixedUpdate()
    // {
    //     // Movimiento físicas
    //     Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
    //     rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime);
    // }

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner) return;
        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y);
        //rb.MovePosition(rb.position + move * speed * Time.fixedDeltaTime); 
        //float h = Input.GetAxis("Horizontal");
        //float v = Input.GetAxis("Vertical");
        transform.Translate(move * speed * Time.fixedDeltaTime);
        MainCamera.transform.position=new Vector3(transform.position.x,transform.position.y+1,transform.position.z-4);
    }
    public override void OnNetworkSpawn()
    {
        networkColor.OnValueChanged += OnColorChanged;
        if (!IsOwner)
        {
            // Desactiva el componente que escucha el teclado si no es mi cápsula
            GetComponent<PlayerInput>().enabled = false;
            //GetComponent<Renderer>().material.color=Color.gray;
            ApplyColor(networkColor.Value);
        }
        else
        {
            GetComponent<Renderer>().material.color=Color.blue;
            networkColor.Value = colores[(int)NetworkObjectId%colores.Length];       
        }
    }
    private void OnColorChanged(Color oldColor, Color newColor)
    {
        ApplyColor(newColor);
        //Debug.Log("Ingresa color"+NetworkObjectId+oldColor.ToString()+":"+newColor.ToString()); 
    }

    private void ApplyColor(Color colorToApply)
    {        
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        //renderer.GetPropertyBlock(propBlock);
        //propBlock.SetColor("_Color", colorToApply);
        //renderer.SetPropertyBlock(propBlock);
        renderer.material.color=colorToApply;
    }

    // Limpieza al destruir el objeto
    public override void OnNetworkDespawn()
    {
        networkColor.OnValueChanged -= OnColorChanged;
    }
}
