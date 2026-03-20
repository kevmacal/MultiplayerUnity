using TMPro;
using Unity.Netcode;
using UnityEngine;

public class NetworkCounter : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI counter;
    private int count;
    void Awake()
    {
        count=0;
        counter.text=$"{count}";
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void AddCounterRpc()
    {
        count++;
        counter.text=$"{count}";
    }
    [Rpc(SendTo.ClientsAndHost)]
    public void ReduceCounterRpc()
    {
        count--;
        counter.text=$"{count}";
    }
}
