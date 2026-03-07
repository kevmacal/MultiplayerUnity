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
    private int PlayerID;
    private Rigidbody rb;
    private NetworkVariable<Color> networkColor = new NetworkVariable<Color>(
        Color.white, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner // El dueño elige su color
    );
    private NetworkVariable<int> m_Score = new NetworkVariable<int>(
        0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server // Solo el server puede escribir
    );
    private Color[] colores={Color.blue,Color.red,Color.green};
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        MainCamera = MainCameraObject.FindFirstObjectByType<MainCameraObject>();
        //MeshRenderer renderer = GetComponent<MeshRenderer>();

        //renderer.material.color = Color.blue;
    }
    public int GetPlayerID()
    {
        return PlayerID;
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
        MainCamera.transform.position=new Vector3(transform.position.x,transform.position.y+1,transform.position.z-6);
    }
    public override void OnNetworkSpawn()
    {
        networkColor.OnValueChanged += OnColorChanged;
        m_Score.OnValueChanged += OnScoreChanged;
        if (!IsOwner)
        {
            // Desactiva el componente que escucha el teclado si no es mi cápsula
            GetComponent<PlayerInput>().enabled = false;
            //GetComponent<Renderer>().material.color=Color.gray;
            ApplyColor(networkColor.Value);
        }
        else
        {
            //GetComponent<Renderer>().material.color=Color.blue;
            // 1. Buscamos todos los objetos con el tag "Player"
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            int playerIndex = (players.Length - 1) % colores.Length;
            PlayerID=playerIndex;
            //Debug.Log("C:"+playerIndex);
            networkColor.Value = colores[playerIndex];       
        }
    }
    private void OnColorChanged(Color oldColor, Color newColor)
    {
        ApplyColor(newColor);
        //Debug.Log("Ingresa color"+NetworkObjectId+oldColor.ToString()+":"+newColor.ToString()); 
    }
    private void OnScoreChanged(int oldScore, int newScore)
    {
        // Solo imprime si el cambio es en MI propia cápsula
        if (IsOwner) {
            Debug.Log($"[YO SOY ID:{OwnerClientId}] Mi score cambió de {oldScore} a {newScore}");
        } else {
            //Debug.Log($"[OTRO ID:{OwnerClientId}] El score de otro cambió a {newScore}");
        }
        //Debug.Log("Soy: "+PlayerID+"\tEl Score total es: "+m_Score.Value);
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
    private void AddScore(int plusScore)
    {
        m_Score.Value=m_Score.Value+plusScore;
    }
    [Rpc(SendTo.Server)]
    public void CollectItemServerRPC(int plusScore)
    {
        //Debug.Log("Ejecuta solo servidor");
        AddScore(plusScore);
        //GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //foreach (GameObject jug in players){
            //PlayerMovement jugTmp=jug.GetComponent<PlayerMovement>();
            //if (jugTmp.PlayerID==playerID)
           //{
                //jugTmp.m_Score.Value=m_Score.Value+plusScore;
            //}
        //}        
        ShowScoreRPC();
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void ShowScoreRPC()
    {
        Debug.Log($"Un jugador ha sumado puntaje");
    }

    // Limpieza al destruir el objeto
    public override void OnNetworkDespawn()
    {
        networkColor.OnValueChanged -= OnColorChanged;
    }
}
