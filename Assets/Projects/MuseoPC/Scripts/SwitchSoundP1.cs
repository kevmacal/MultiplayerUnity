using Unity.Netcode;
using UnityEngine;

public class SwitchSoundP1 : NetworkBehaviour
{
    [SerializeField] AudioController SoundPB;
    [SerializeField] AudioController SoundP1;
    [SerializeField] AudioController SoundSalaEspecial;
    private void OnTriggerEnter(Collider other)
    {
        NetworkObject playerNetObj = other.GetComponent<NetworkObject>();
        if (playerNetObj == null) return;
        SoundPB.DetenerMusica();
        SoundSalaEspecial.DetenerMusica();
        SoundP1.ReproducirMusica();
    }
}
