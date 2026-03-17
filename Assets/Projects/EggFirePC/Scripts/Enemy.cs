using UnityEngine;

public class Enemy : MonoBehaviour
{
    private bool isDefeated = false;
    [SerializeField] float desplazamientoY;
    public void Derrotar()
    {
        if (isDefeated) return;
        
        isDefeated = true;
        // Rotar 180 grados en el eje Z o X para que quede al revés
        transform.rotation = Quaternion.Euler(180, 0, 0);
        transform.position=new Vector3(transform.position.x,transform.position.y+desplazamientoY,transform.position.z);
        
        //Rigidbody rb = GetComponent<Rigidbody>();
        //if (rb != null) rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
    }
    public bool IsAlive()
    {
        return !isDefeated;
    }
}
