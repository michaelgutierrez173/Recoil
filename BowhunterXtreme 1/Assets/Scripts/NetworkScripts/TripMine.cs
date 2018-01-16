using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.Networking;

public class TripMine : NetworkBehaviour
{
    public GameObject explosion;


    void Start()
    {
        if (!isServer)
            return;
    }


    void Death()
    {
        GameObject go = Instantiate(explosion, transform.position, Quaternion.identity);
        NetworkServer.Spawn(go);
        NetworkServer.Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (!isServer)
        {
            return;
        }
        if (col.collider.tag == "Player" || col.collider.tag == "Bullet")
        {
            Death();
        }
    }
}


