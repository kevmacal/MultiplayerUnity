using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

public class NetworkArtPiece : NetworkBehaviour
{
    [Header("Art Information")]
    [SerializeField] private string m_Title;
    [SerializeField] private string m_Artist;
    [SerializeField] private string m_Description;
    [SerializeField] private Light artPieceLight;

    [Header("References")]
    [SerializeField] private Canvas m_InfoCanvas;
    [SerializeField] private TextMeshProUGUI m_TitleText;
    [SerializeField] private TextMeshProUGUI m_ArtistText;
    [SerializeField] private TextMeshProUGUI m_DescriptionText;
    [SerializeField] AudioController SoundCry;

    private void Start()
    {
        // Configurar textos del canvas
        if (m_TitleText != null) m_TitleText.text = m_Title;
        if (m_ArtistText != null) m_ArtistText.text = $"Autor: {m_Artist}";
        if (m_DescriptionText != null) m_DescriptionText.text = m_Description;

        // Asegurar que canvas inicia desactivado
        if (m_InfoCanvas != null)
        {
            m_InfoCanvas.gameObject.SetActive(false);
            artPieceLight.enabled=false;
        }
    }

    // Se ejecuta cuando algo ENTRA al trigger
    private void OnTriggerEnter(Collider other)
    {
        // Verificar si es un jugador
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        //Verificar que el Objeto no sea Null
        if (playerNetObj == null) return;
        // Verificar si es el jugador LOCAL (IsOwner)
        //if (!playerNetObj.IsOwner) return;
        // Activar canvas SOLO para este jugador
        SoundCry.ReproducirMusica();
        ShowInfoPanel();
    }

    // Se ejecuta cuando algo SALE del trigger
    private void OnTriggerExit(Collider other)
    {
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;

        //if (!playerNetObj.IsOwner) return;

        // Desactivar canvas
        HideInfoPanel();
    }

    private void ShowInfoPanel()
    {
        if (m_InfoCanvas != null)
        {
            artPieceLight.enabled=true;
            m_InfoCanvas.gameObject.SetActive(true);
            Debug.Log($"[Local] Viendo: {m_Title}");
        }
    }

    private void HideInfoPanel()
    {
        if (m_InfoCanvas != null)
        {
            artPieceLight.enabled=false;
            m_InfoCanvas.gameObject.SetActive(false);
            Debug.Log($"[Local] Dejó de ver: {m_Title}");
        }
    }
}
