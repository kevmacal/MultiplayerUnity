using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using Unity.Services.Economy;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

/// <summary>
/// Inicializa todos los servicios UGS al arrancar la aplicación.
/// Actúa como punto de entrada único (Singleton).
///
/// Analytics 6.x: usa RecordEvent(CustomEvent) en lugar del deprecado CustomData().
/// </summary>
public class NetworkGameBootstrap : MonoBehaviour
{
    public static NetworkGameBootstrap Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            Debug.Log("[UGS] Servicios inicializados correctamente.");

            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"[Auth] Sesión iniciada anónimamente.\n" +
                      $"      Player ID    : {AuthenticationService.Instance.PlayerId}\n" +
                      $"      Access Token : " +
                      $"{AuthenticationService.Instance.AccessToken?[..Math.Min(20, AuthenticationService.Instance.AccessToken.Length)]}...");

            await LoadCloudSaveData();
            await LoadEconomyData();
            LogAnalyticsSessionStart();
        }
        catch (Exception e)
        {
            Debug.LogError($"[UGS] Error crítico durante inicialización: {e.Message}");
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    // CLOUD SAVE
    // ────────────────────────────────────────────────────────────────────────────

    private async Task LoadCloudSaveData()
    {
        try
        {
            var data = await CloudSaveService.Instance.Data.Player
                .LoadAsync(new HashSet<string> { "nivel", "monedas" });

            var nivel   = data.TryGetValue("nivel",   out var n) ? n.Value.GetAs<int>() : 0;
            var monedas = data.TryGetValue("monedas", out var m) ? m.Value.GetAs<int>() : 0;

            Debug.Log($"[CloudSave] Datos cargados:\n" +
                      $"           Nivel   : {nivel}\n" +
                      $"           Monedas : {monedas}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[CloudSave] Sin datos previos o error al cargar: {e.Message}");
        }
    }

    /// <summary>Guarda progreso del jugador en la nube.</summary>
    public async void SaveProgress(int nivel, int monedas)
    {
        try
        {
            await CloudSaveService.Instance.Data.Player
                .SaveAsync(new Dictionary<string, object>
                {
                    { "nivel",   nivel   },
                    { "monedas", monedas }
                });
            Debug.Log($"[CloudSave] Progreso guardado → nivel: {nivel}, monedas: {monedas}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[CloudSave] Error al guardar: {e.Message}");
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    // ECONOMY
    // ────────────────────────────────────────────────────────────────────────────

    private async Task LoadEconomyData()
    {
        try
        {
            var result = await EconomyService.Instance.PlayerBalances.GetBalancesAsync();

            if (result.Balances.Count == 0)
            {
                Debug.Log("[Economy] No hay monedas configuradas para este jugador aún.");
                return;
            }

            foreach (var b in result.Balances)
                Debug.Log($"[Economy] {b.CurrencyId}: {b.Balance}");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Economy] Error al obtener balances: {e.Message}");
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    // ANALYTICS — SDK 6.x
    // CustomData() fue removido. Reemplazado por RecordEvent(CustomEvent).
    // CustomEvent se usa como diccionario con inicialización de colección.
    // ────────────────────────────────────────────────────────────────────────────

    private void LogAnalyticsSessionStart()
    {
        try
        {
            var evt = new CustomEvent("session_start")
            {
                { "player_id", AuthenticationService.Instance.PlayerId },
                { "timestamp", DateTime.UtcNow.ToString("o")           },
                { "platform",  Application.platform.ToString()         }
            };

            AnalyticsService.Instance.RecordEvent(evt);
            AnalyticsService.Instance.Flush();

            Debug.Log("[Analytics] Evento 'session_start' registrado y enviado.");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Analytics] Error al registrar session_start: {e.Message}");
        }
    }

    /// <summary>Registra que el jugador completó un nivel.</summary>
    public void LogLevelCompleted(int nivel, float tiempoSegundos)
    {
        try
        {
            var evt = new CustomEvent("nivel_completado")
            {
                { "nivel",  nivel          },
                { "tiempo", tiempoSegundos }
            };

            AnalyticsService.Instance.RecordEvent(evt);
            AnalyticsService.Instance.Flush();

            Debug.Log($"[Analytics] Evento 'nivel_completado' → nivel: {nivel}, tiempo: {tiempoSegundos}s");
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[Analytics] Error al registrar nivel_completado: {e.Message}");
        }
    }

    // ────────────────────────────────────────────────────────────────────────────
    // RELAY  (llamado desde NetworkUI)
    // ────────────────────────────────────────────────────────────────────────────

    /// <summary>Crea una allocation de Relay y devuelve el Join Code.</summary>
    public async Task<(Allocation allocation, string joinCode)> CreateRelayAllocation(int maxPlayers = 4)
    {
        var allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers);
        var joinCode   = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        Debug.Log($"[Relay] Allocation creada.\n" +
                  $"        AllocationId : {allocation.AllocationId}\n" +
                  $"        Join Code    : {joinCode}\n" +
                  $"        Region       : {allocation.Region}");

        return (allocation, joinCode);
    }

    /// <summary>Une al cliente a una sesión Relay existente usando el Join Code.</summary>
    public async Task<JoinAllocation> JoinRelay(string joinCode)
    {
        var join = await RelayService.Instance.JoinAllocationAsync(joinCode);

        Debug.Log($"[Relay] Unido correctamente.\n" +
                  $"        Join Code    : {joinCode}\n" +
                  $"        AllocationId : {join.AllocationId}");

        return join;
    }
}