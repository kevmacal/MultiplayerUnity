using System.Collections;
using UnityEngine;

public class Egg : MonoBehaviour
{
    private LanzarEgg lanzarEgg_player;
    private float lifeTime=4.3f;
    private float lifeTimeSuccess=2.0f;
    private bool isCollision = false;
    private bool isHit=false;

    public void Configurar(LanzarEgg player)
    {
        lanzarEgg_player = player;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isCollision) return; //Si ya toco el piso no se procesa más. El huevo ya se "rompio"

        // Validamos con qué chocó
        if (collision.gameObject.CompareTag("Suelo"))
        {
            // Si choca con el suelo, simplemente se rompe
            Debug.Log("El huevo se estrelló contra el suelo.");
        }
        else
        {
            Debug.Log("Huevo golpeó a: " + collision.gameObject.name);
            
            // Ejemplo: si choca con un enemigo
            if (collision.gameObject.CompareTag("Enemigo"))
            {
                Enemy scriptEnemigo = collision.gameObject.GetComponent<Enemy>();
                if (scriptEnemigo != null)
                {     
                    // Hablamos con el jugador para darle su punto
                    if (lanzarEgg_player != null&&scriptEnemigo.IsAlive())
                    {
                        scriptEnemigo.Derrotar();
                        //lanzarEgg_player.GanarPunto();
                        lanzarEgg_player.GetPlayerScore().AddScore();
                        Debug.Log("Ganar punto");
                    }
                }
                // Hacer algo al enemigo
                isHit=true;
            }
        }
        // Marcamos como colisionado y avisamos al jugador para liberar cupo
        isCollision = true;
        if (isHit)
        {
            StartCoroutine(SuccessDestroy());
        }
        else
        {
            StartCoroutine(WaitAndDestroy());
        }       
        
    }
    IEnumerator WaitAndDestroy()
    {
        // El huevo se queda rebotando el tiempo que definas
        yield return new WaitForSeconds(lifeTime);
        SelfDestroy();
    }
    IEnumerator SuccessDestroy()
    {
        // El huevo se queda rebotando el tiempo que definas
        yield return new WaitForSeconds(lifeTimeSuccess);
        SelfDestroy();
    }
    private void SelfDestroy()
    {
        if (lanzarEgg_player != null)
        {
            lanzarEgg_player.SelfDestroyNotify();
        }
        Destroy(gameObject);
    }
}
