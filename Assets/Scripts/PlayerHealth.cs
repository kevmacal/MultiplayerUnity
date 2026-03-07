using Unity.Netcode;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{

    private NetworkVariable<float> m_Health = new NetworkVariable<float>(
        100.0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        m_Health.OnValueChanged += OnHealthChanged;
        //UpdateScoreUI(m_Score.Value);

        if (IsOwner)
        {
            //m_AddScoreAction.performed += OnAddScorePerformed;
            //m_AddScoreAction.Enable();
        }
    }
    private void OnHealthChanged(float oldValue, float newvalue)
    {
        
    }
}
