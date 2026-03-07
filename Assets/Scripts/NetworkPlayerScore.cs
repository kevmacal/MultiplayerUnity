using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;
using TMPro;

[RequireComponent(typeof(PlayerInput))]
public class NetworkPlayerScore : NetworkBehaviour
{
//     Declarar la variable
// En PlayerHealth.cs
// , declarar NetworkVariable<float> m_Health
// Igual que m_Score
// , pero float
//  y valor inicial 100f
// Suscribirse en OnNetworkSpawn() 
// m_Health.OnValueChanged += OnHealthChanged;
// Igual que con 
// m_Score.OnValueChanged

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI m_ScoreText;

    private PlayerInput m_PlayerInput;
    private InputAction m_AddScoreAction;

    private NetworkVariable<int> m_Score = new NetworkVariable<int>(
        0,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Awake()
    {
        m_PlayerInput = GetComponent<PlayerInput>();
        m_AddScoreAction = m_PlayerInput.actions["TakeDamage"];
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_Score.OnValueChanged += OnScoreChanged;
        UpdateScoreUI(m_Score.Value);

        if (IsOwner)
        {
            m_AddScoreAction.performed += OnAddScorePerformed;
            m_AddScoreAction.Enable();
        }
    }

    public override void OnNetworkDespawn()
    {
        m_Score.OnValueChanged -= OnScoreChanged;
        m_AddScoreAction.performed -= OnAddScorePerformed;
        m_AddScoreAction.Disable();
    }

    private void OnAddScorePerformed(InputAction.CallbackContext ctx)
        => RequestAddScoreRpc(10);

    [Rpc(SendTo.Server)]
    private void RequestAddScoreRpc(int amount)
        => m_Score.Value += amount;

    private void OnScoreChanged(int previousValue, int newValue)
        => UpdateScoreUI(newValue);

    private void UpdateScoreUI(int value)
    {
        if (m_ScoreText != null)
            m_ScoreText.text = $"Score: {value}";
    }
}