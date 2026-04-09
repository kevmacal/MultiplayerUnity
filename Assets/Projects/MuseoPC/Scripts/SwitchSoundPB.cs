using Unity.Netcode;
using UnityEngine;

public class SwitchSoundPB : NetworkBehaviour
{
    [SerializeField] AudioController SoundPB;
    [SerializeField] AudioController SoundP1;
    [SerializeField] AudioController SoundSalaEspecial;
    private void OnTriggerEnter(Collider other)
    {
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;
        SoundP1.DetenerMusica();
        SoundSalaEspecial.DetenerMusica();
        SoundPB.ReproducirMusica();
    }
}
