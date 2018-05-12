using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerShootPistol : MonoBehaviour
{
    public MultiShooter trigger;
    public MeshRenderer boltMesh;
    public float aniSpeed;
    public Color boltEmissionColor;
    public AudioSource shootSFX;
    public GameObject nonPlayerPistolBullet; // The bullet that is going to be created on the non-player side

    public Material boltMat;
    public Coroutine fireAniRoutine;
    public GameObject playerKart;
    public GameObject nonPlayerKart;

    // Use this for initialization
    void Start()
    {
        boltMat = boltMesh.material;
        playerKart = FindObjectOfType<MirrorGameManager>().playerKart;
        nonPlayerKart = FindObjectOfType<MirrorGameManager>().nonPlayerKart;
    }

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

        GameObject newBullet = Instantiate(nonPlayerPistolBullet, nonPlayerKart.transform.TransformPoint(playerKart.transform.InverseTransformPoint(transform.position + transform.forward * 0.3f)), transform.rotation);
        newBullet.GetComponent<NonPlayerPistolBulletBehavior>().nonPlayerKart = FindObjectOfType<MirrorGameManager>().nonPlayerKart;
        shootSFX.PlayOneShot(shootSFX.clip);

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {
            boltMat.SetColor("_EmissionColor", new Color(Mathf.Lerp(boltEmissionColor.r, 0, t), Mathf.Lerp(boltEmissionColor.g, 0, t), Mathf.Lerp(boltEmissionColor.b, 0, t)));
            yield return null;
        }
    }
}
