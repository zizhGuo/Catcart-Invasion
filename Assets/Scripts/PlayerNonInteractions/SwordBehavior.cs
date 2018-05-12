using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class SwordBehavior : MonoBehaviour
{
    /// <summary>
    /// Deduct the vehical velocity from the sword velocity
    /// </summary>

    public MeshRenderer swordMesh;
    public float normalAlpha; //The alpha value when the shield is idle
    public float hitAlpha; //The alpha value when the shield is being hit
    public float aniDuration; //Time for the shield to fade back to normal alpha after been hit
    public float speedToKillEnemy; //The speed needed for the sword to kill an enemy (if the speed is too small means the player is not really swinging the sword)
    public float enemySlapDestroyDelay; //How long the enemy can last before turns into cat when slapped (So that the enemy can fly away for a moment)

    public Coroutine hitRoutine;
    public Material swordMat;
    public float swordVelocity; //How fast the sword is being swinged
    public Vector3 swordLastPosition; //The position of the sword in the previous frame
    public Vector3 slapForce; //The force the player is slapping the enemy
    public Vector3 triggerSize; //The trigger size for detecting enemy collision will change as the current velocity of the melee changes 
                                //(The faster the melee weapon is moving, the bigger the trigger collider will be to compensate the physics update)
    public float triggerIncreaseFactor; //How much the trigger size will increase with the melee velocity
    public GameObject playerKart;
    public VRTK_ControllerActions controllerActions; // The controller it corresponding to

    // Use this for initialization
    void Start()
    {
        swordMat = swordMesh.material;
        swordLastPosition = transform.position;
        triggerSize.x = 1;
        triggerSize.y = 1;
        triggerSize.z = 1;
        playerKart = FindObjectOfType<GameManager>().playerKart;
        controllerActions = transform.parent.GetComponentInChildren<VRTK_ControllerActions>();
    }

    // Update is called once per frame
    void Update()
    {
        //swordVelocity = Vector3.Distance(transform.position, swordLastPosition) / Time.fixedUnscaledDeltaTime;
        //swordLastPosition = transform.position;
    }

    void FixedUpdate()
    {
        swordVelocity = Vector3.Distance(transform.position, swordLastPosition) / Time.fixedUnscaledDeltaTime - playerKart.GetComponent<Rigidbody>().velocity.magnitude; // This does not have direction
        slapForce = (transform.position - swordLastPosition) / Time.fixedUnscaledDeltaTime - playerKart.GetComponent<Rigidbody>().velocity; // This have direction
        swordLastPosition = transform.position;
        triggerSize.z = 1 + 1 * swordVelocity / Time.fixedUnscaledDeltaTime * triggerIncreaseFactor;
        GetComponent<BoxCollider>().size = triggerSize;
    }

    void OnTriggerEnter(Collider other)
    {
        hitRoutine = StartCoroutine(hitAni(aniDuration));
        //print("swordVelocity: " + swordVelocity + ", slapForce: " + slapForce);

        //Vector3 vrtkVelocity = VRTK_DeviceFinder.GetControllerVelocity(controllerActions.gameObject);
        //float vrtkForce = VRTK_DeviceFinder.GetControllerVelocity(controllerActions.gameObject).magnitude * slapForceMagnifier;
        //print("VRTK_Controller_Velocity: " + vrtkVelocity + ", vrtkForce: " + vrtkForce);
        //float hapticStrength = vrtkForce / maxCollisionForce;
        float hapticStrength = swordVelocity * GameManager.sSlapForceMagnifier / GameManager.sMaxCollisionForce;


        if (other.tag == "EnemyBullet")
        {
            other.gameObject.GetComponent<ProjectileMovement>().isReflected = true;
            controllerActions.TriggerHapticPulse(hapticStrength / 30f, 0.1f, 0.01f);
        }

        else if (other.tag == "Enemy" && slapForce.magnitude >= speedToKillEnemy)
        {
            slapEnemy(other.gameObject);
            controllerActions.TriggerHapticPulse(hapticStrength, 0.2f, 0.01f);
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
            swordMat.SetColor("_Color", new Color(swordMat.color.r, swordMat.color.g, swordMat.color.b, Mathf.Lerp(swordMat.color.a, hitAlpha, t)));
            yield return null;
        }

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / duration * 2f)
        {
            swordMat.SetColor("_Color", new Color(swordMat.color.r, swordMat.color.g, swordMat.color.b, Mathf.Lerp(swordMat.color.a, normalAlpha, t)));
            yield return null;
        }
    }

    public void slapEnemy(GameObject enemy)
    {
        //if (enemy.GetComponent<SimpleEnemyBehavior>().enabled == true) //Only run this if the enemy is not slapped
        //{
        //    enemy.GetComponent<SimpleEnemyBehavior>().isSlapped = true;
        //    enemy.GetComponent<SimpleEnemyBehavior>().enabled = false;
        //    enemy.GetComponent<Rigidbody>().useGravity = true;
        //    enemy.GetComponent<Rigidbody>().mass /= 3f;
        //    enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    enemy.GetComponent<Rigidbody>().AddForce(slapForce + playerKart.GetComponent<Rigidbody>().velocity, ForceMode.Impulse);
        //    Destroy(enemy, enemySlapDestroyDelay);
        //}

        if (enemy.GetComponent<NASCatcherEnemyBehavior>() && enemy.GetComponent<NASCatcherEnemyBehavior>().enabled == true) //Only run this if the enemy is not slapped
        {
            enemy.GetComponent<NASCatcherEnemyBehavior>().isSlapped = true;
            enemy.GetComponent<NASCatcherEnemyBehavior>().enabled = false;
            enemy.GetComponent<Rigidbody>().useGravity = true;
            enemy.GetComponent<Rigidbody>().mass /= 3f;
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            enemy.GetComponent<Rigidbody>().AddForce(slapForce + playerKart.GetComponent<Rigidbody>().velocity, ForceMode.Impulse);
            enemy.GetComponent<SmallEnemyDie>().destroyedByPlayer = true;

            // Attach a new slap audio player to the enemy that's just been slapped
            Instantiate(transform.parent.GetComponent<PlayerMeleeRelatedReferences>().slapEnemySounds, enemy.transform);

            Destroy(enemy, enemySlapDestroyDelay);
        }
        else if (enemy.GetComponent<NASDrillerEnemyBehavior>())
        {
            //print("driller");
            enemy.GetComponent<Rigidbody>().useGravity = true;
            enemy.GetComponent<Rigidbody>().mass /= 3f;
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            enemy.GetComponent<Rigidbody>().AddForce(slapForce + playerKart.GetComponent<Rigidbody>().velocity, ForceMode.Impulse);
            enemy.GetComponent<SmallEnemyDie>().destroyedByPlayer = true;
            //enemy.GetComponent<SmallEnemyDie>().enabled = true;
            enemy.GetComponentInChildren<ShockBehavior>().isSlapped = true;
            Destroy(enemy, enemySlapDestroyDelay);
        }
        else if (enemy.GetComponent<DrillerMissileBehavior>())
        {
            enemy.GetComponent<Rigidbody>().useGravity = true;
            enemy.GetComponent<Rigidbody>().mass /= 3f;
            enemy.GetComponent<Rigidbody>().velocity = Vector3.zero;
            enemy.GetComponent<Rigidbody>().AddForce(slapForce + playerKart.GetComponent<Rigidbody>().velocity, ForceMode.Impulse);
            enemy.GetComponent<SmallEnemyDie>().destroyedByPlayer = true;
            enemy.GetComponent<DrillerMissileBehavior>().enabled = false;
            enemy.GetComponent<MoveWithBoss>().enabled = false;
            Destroy(enemy, enemySlapDestroyDelay);
        }

        ///
        /// Temporary boss defeat behavior
        ///
        else if (enemy.GetComponent<BossGetHit>())
        {
            print("hit boss, " + enemy.name + ", " + Vector3.Distance(transform.position, enemy.transform.position));
            //enemy.GetComponentInParent<BossBehavior>().GetComponent<Rigidbody>().useGravity = false;
            enemy.GetComponentInParent<BossBehavior>().GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            enemy.GetComponentInParent<BossBehavior>().GetComponent<Rigidbody>().mass /= 3f;
            enemy.GetComponentInParent<BossBehavior>().GetComponent<Rigidbody>().velocity = Vector3.zero;
            enemy.GetComponentInParent<BossBehavior>().GetComponent<Rigidbody>().AddForce(slapForce + playerKart.GetComponent<Rigidbody>().velocity, ForceMode.Impulse);

            enemy.GetComponentInParent<BossBehavior>().enabled = false;
        }
    }
}
