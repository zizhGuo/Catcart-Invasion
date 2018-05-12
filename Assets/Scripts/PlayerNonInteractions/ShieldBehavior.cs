using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ShieldBehavior : MonoBehaviour
{
    public MeshRenderer shieldMesh;
    public float normalAlpha; //The alpha value when the shield is idle
    public float hitAlpha; //The alpha value when the shield is being hit
    public float aniDuration; //Time for the shield to fade back to normal alpha after been hit
    public float shieldEnergyConsump; //How much energy the shield will use when deflecting enemy attack

    public Coroutine hitRoutine;
    public Material shieldMat;
    public VRTK_ControllerActions controllerActions; // The controller it corresponding to

    // Use this for initialization
    void Start()
    {
        shieldMat = shieldMesh.material;
        controllerActions = transform.parent.GetComponentInChildren<VRTK_ControllerActions>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.currentEnergy <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    void OnCollisionEnter(Collision col)
    {
        hitRoutine = StartCoroutine(hitAni(aniDuration));
        GameManager.currentEnergy -= shieldEnergyConsump;

        if (col.collider.tag == "EnemyBullet")
        {
            //float hapticStrength = GameManager.sSlapForceMagnifier / GameManager.sMaxCollisionForce;
            //controllerActions.TriggerHapticPulse(hapticStrength / 30f, 0.1f, 0.01f);
            controllerActions.TriggerHapticPulse(0.6f, 0.1f, 0.2f);
            col.gameObject.GetComponent<NASTaserShotBehavior>().isReflected = true;
        }
    }

    public IEnumerator hitAni(float duration)
    {
        if (hitRoutine != null)
        {
            StopCoroutine(hitRoutine);
        }

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration * 2f)
        {
            shieldMat.SetColor("_Color", new Color(shieldMat.color.r, shieldMat.color.g, shieldMat.color.b, Mathf.Lerp(shieldMat.color.a, hitAlpha, t)));
            yield return null;
        }

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration * 2f)
        {
            shieldMat.SetColor("_Color", new Color(shieldMat.color.r, shieldMat.color.g, shieldMat.color.b, Mathf.Lerp(shieldMat.color.a, normalAlpha, t)));
            yield return null;
        }
    }

    /// <summary>
    /// What to do if the shield is hit
    /// </summary>
    public void BeingHit()
    {
        hitRoutine = StartCoroutine(hitAni(aniDuration));
        GameManager.currentEnergy -= shieldEnergyConsump;
        controllerActions.TriggerHapticPulse(0.6f, 0.1f, 0.2f);
    }
}
