using System;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI de conexión multijugador.
/// - Botón Host: crea allocation Relay, inicia como servidor, muestra Join Code.
/// - Botón Client: lee el Join Code del input field, se une a la sesión.
/// - Ambos botones se desactivan después de conectar.
///
/// Unity 6: RelayServerData(JoinAllocation, string) fue removido.
/// Tanto host como cliente usan AllocationUtils.ToRelayServerData().
/// </summary>
public class NetworkUIRelay : MonoBehaviour
{
    [Header("Botones de conexión")]
    [SerializeField] private Button btnHost;
    [SerializeField] private Button btnClient;

    [Header("Join Code")]
    [SerializeField] private TMP_InputField joinCodeInput;   // el cliente escribe aquí
    [SerializeField] private TMP_Text       joinCodeDisplay; // el host ve el código aquí

    [Header("Estado")]
    [SerializeField] private TMP_Text statusText;

    // ────────────────────────────────────────────────────────────────────────────

    private void Start()
    {
        btnHost  .onClick.AddListener(OnHostClicked);
        btnClient.onClick.AddListener(OnClientClicked);

        joinCodeInput  .gameObject.SetActive(true);
        joinCodeDisplay.gameObject.SetActive(false);

        SetStatus("Listo. Elige Host o Client.");
    }

    // ────────────────────────────────────────────────────────────────────────────
    // HOST
    // ────────────────────────────────────────────────────────────────────────────

    private async void OnHostClicked()
    {
        DisableButtons();
        SetStatus("Creando sesión Relay...");

        try
        {
            var (allocation, joinCode) = await NetworkGameBootstrap.Instance.CreateRelayAllocation(maxPlayers: 4);

            // Unity 6: AllocationUtils.ToRelayServerData() para el host (Allocation)
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(allocation, "dtls"));

            NetworkManager.Singleton.StartHost();

            joinCodeInput  .gameObject.SetActive(false);
            joinCodeDisplay.gameObject.SetActive(true);
            joinCodeDisplay.text = joinCode;

            SetStatus("Hosting activo. Esperando jugadores...");
            Debug.Log($"[NetworkUI] Host iniciado. Join Code: {joinCode}");

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        }
        catch (Exception e)
        {
            EnableButtons();
            SetStatus($"Error al crear host: {e.Message}");
            Debug.LogError($"[NetworkUI] Host error: {e}");
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    // CLIENT
    // ────────────────────────────────────────────────────────────────────────────

    private async void OnClientClicked()
    {
        var code = joinCodeInput.text.Trim().ToUpper();

        if (string.IsNullOrEmpty(code))
        {
            SetStatus("Ingresa un Join Code válido.");
            return;
        }

        DisableButtons();
        SetStatus($"Conectando con código {code}...");

        try
        {
            var join = await NetworkGameBootstrap.Instance.JoinRelay(code);

            // Unity 6: AllocationUtils.ToRelayServerData() para el cliente (JoinAllocation)
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetRelayServerData(AllocationUtils.ToRelayServerData(join, "dtls"));

            NetworkManager.Singleton.StartClient();

            joinCodeInput  .gameObject.SetActive(false);
            joinCodeDisplay.gameObject.SetActive(false);

            SetStatus("Conectado como cliente.");
            Debug.Log("[NetworkUI] Cliente conectado al host.");
        }
        catch (Exception e)
        {
            EnableButtons();
            SetStatus($"Error al conectarse: {e.Message}");
            Debug.LogError($"[NetworkUI] Client error: {e}");
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    // CALLBACKS
    // ────────────────────────────────────────────────────────────────────────────

    private void OnClientConnected(ulong clientId)
    {
        var total = NetworkManager.Singleton.ConnectedClients.Count;
        SetStatus($"Jugadores conectados: {total}");
        Debug.Log($"[NetworkUI] Cliente conectado — ID: {clientId} | Total: {total}");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }

    // ────────────────────────────────────────────────────────────────────────────
    // HELPERS
    // ────────────────────────────────────────────────────────────────────────────

    private void DisableButtons()
    {
        btnHost  .interactable = false;
        btnClient.interactable = false;
    }

    private void EnableButtons()
    {
        btnHost  .interactable = true;
        btnClient.interactable = true;
    }

    private void SetStatus(string msg)
    {
        if (statusText != null)
            statusText.text = msg;
    }
}