using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject cubo1;

    void Start()
    {
        cubo1.gameObject.SetActive(false);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            cubo1.gameObject.SetActive(true);
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            cubo1.gameObject.SetActive(false);
        }
    }
}
