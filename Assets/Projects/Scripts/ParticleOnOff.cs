using UnityEngine;

public class ParticleOnOff : MonoBehaviour
{
    [SerializeField] ParticleSystem ps;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag=="Player")
        {
            //Debug.Log("Iniciar particulas");
            ps.Play();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.tag=="Player")
        {
            //Debug.Log("Detener particulas");
            ps.Stop();
        }
    }

}
