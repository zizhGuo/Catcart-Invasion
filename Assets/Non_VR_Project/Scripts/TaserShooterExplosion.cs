using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaserShooterExplosion : MonoBehaviour
{
    public ParticleSystem ExplosionParticleSystem;
    [SerializeField] bool isNonVRplayer = false;

    void Start()
    {
        if (FindObjectOfType<CameraMoving>())
        {
            isNonVRplayer = true;
        }
    }

    void Update()
    {

    }
    private void OnDestroy()
    {
        if (isNonVRplayer)
        {
            ParticleSystem explosionEffect = Instantiate(ExplosionParticleSystem) as ParticleSystem;
            explosionEffect.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            explosionEffect.transform.position = gameObject.transform.position;
            //short pieces = (short)Mathf.Max(8, (50.0f * 3 * 3));
            //explosionEffect.emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0.0f, pieces) }, 1);
            explosionEffect.Play();
            //Destroy(explosionEffect.gameObject);
            Debug.Log("OnDestroy() Called!");
        }
    }
}
