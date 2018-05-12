using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ShootPistol : MonoBehaviour
{
    public MultiShooter trigger;
    public MeshRenderer boltMesh;
    public float aniSpeed;
    public Color boltEmissionColor;
    public AudioSource shootSFX;
    public GameObject pistolBullet;

    public Material boltMat;
    public Coroutine fireAniRoutine;

    // Use this for initialization
    void Start()
    {
        boltMat = boltMesh.material;
    }

    // Update is called once per frame
    //void Update ()
    //   {

    //}

    //public override void StartUsing(GameObject usingObject)
    //{
    //    trigger.shoot();
    //    fireAniRoutine = StartCoroutine(fireAni(1f / aniSpeed));
    //}

    public void shoot()
    {
        trigger.shoot();
        fireAniRoutine = StartCoroutine(fireAni(1f / aniSpeed));
    }

    IEnumerator fireAni(float fadeTime)
    {
        if (fireAniRoutine != null)
        {
            StopCoroutine(fireAniRoutine);
        }

        //if (!GameManager.sPistolBulletIsLaser)
        //{
        //    GameObject newBullet = Instantiate(pistolBullet, transform.position + transform.forward * 0.3f, transform.rotation);
        //    newBullet.GetComponent<PlayerPistolBulletBehavior>().initialVelocity = FindObjectOfType<GameManager>().playerKart.GetComponent<Rigidbody>().velocity;
        //}
        //Instantiate(pistolBullet, transform.position + transform.forward * 0.3f, transform.rotation);
        shootSFX.PlayOneShot(shootSFX.clip);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            boltMat.SetColor("_EmissionColor", new Color(Mathf.Lerp(boltEmissionColor.r, 0, t), Mathf.Lerp(boltEmissionColor.g, 0, t), Mathf.Lerp(boltEmissionColor.b, 0, t)));
            yield return null;
        }
    }
}
