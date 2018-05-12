using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMove : MonoBehaviour
{
    public Vector3 moveSpeed;

    public Transform kartPosi;
    public Vector3 newPosi;
    public Rigidbody thisRigid;
    public float initialSpeed;

    // Use this for initialization
    void Start()
    {
        kartPosi = FindObjectOfType<SimpleSpawnEnemy>().transform;
        //playerUI = GameManager.playerUI;
        thisRigid = GetComponent<Rigidbody>();
        initialSpeed = moveSpeed.z;
    }

    // Update is called once per frame
    void Update()
    {
        moveSpeed.z = initialSpeed + Mathf.Pow((Time.time / 50f), 0.25f);
        thisRigid.velocity = moveSpeed;
        newPosi = transform.position;
        newPosi.x = kartPosi.position.x;
        transform.position = newPosi;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.collider.tag == "Player")
        {
            GameManager.gameLostProc();
        }

        if( col.collider.tag == "Enemy")
        {
            GameManager.score--;
            Destroy(col.gameObject);
        }
    }
}
