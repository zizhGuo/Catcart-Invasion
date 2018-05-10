using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BeamCollision : MonoBehaviour
{
    public float actualBulletEffectTime;            //The time duration that the bullet is actually effective and can kill the enemy.
    public float bulletStartTime;                   //The start time when this bullet spawn.
    public AudioClip hitPlayerSFX;                  // The sound effect to be played when the player is shot by the enemy
    public AudioClip catCartNoEnergyLine;           // The line CatCart will say when the player's energy become 0 because of the shooter
    public GameObject enemyDrainsEnergyParticle;    // The particle effect that plays when the player is hit by the enemy

    public bool Reflect = false;
    private BeamLine BL;

    public GameObject HitEffect = null;

    private bool bHit = false;

    private BeamParam BP;

    public List<string> shootables;
    public LayerMask shootLayer;

    public bool effective;                          // Make the beam uneffective after dealing damage to player (only use on enemy's attack)
    public newNASTaserShooterEnemyBehavior taserBeamOwner; // The taser drone that shot this beam

    public static bool multiIsShot = false;
    
    // Use this for initialization
    void Start()
    {
        BL = (BeamLine)this.gameObject.transform.Find("BeamLine").GetComponent<BeamLine>();
        BP = this.transform.root.gameObject.GetComponent<BeamParam>();
        bulletStartTime = Time.time;
        effective = true;
    }

    // Update is called once per frame
    void Update()
    {
        //RayCollision
        RaycastHit hit;
        int layerMask = ~(1 << LayerMask.NameToLayer("NoBeamHit") | 1 << 2);
        if (HitEffect != null && !bHit &&
            Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity, shootLayer))
        {
            GameObject hitobj = hit.collider.gameObject;
            if (Mathf.Abs(hit.distance - BL.GetNowLength()) < 0.5f)
            {
                BL.StopLength(hit.distance);
                bHit = true;

                Quaternion Angle;
                //Reflect to Normal
                if (Reflect)
                {
                    Angle = Quaternion.LookRotation(Vector3.Reflect(transform.forward, hit.normal));
                }
                else
                {
                    Angle = Quaternion.AngleAxis(180.0f, transform.up) * this.transform.rotation;
                }
                GameObject obj = (GameObject)Instantiate(HitEffect, this.transform.position + this.transform.forward * hit.distance, Angle);
                obj.GetComponent<BeamParam>().SetBeamParam(BP);
                obj.transform.localScale = this.transform.localScale;

                if (hitobj.GetComponent<PlayerInfo>() && effective) // If it shot at player head
                {
                    effective = false;
                    PlayerInfo player = hitobj.GetComponent<PlayerInfo>();
                    taserBeamOwner.shotHit = true;
                    taserBeamOwner.energyChargeCount++;
                    multiIsShot = true;

                    // Plays the player get hit sound effetc
                    //AudioSource.PlayClipAtPoint(hitPlayerSFX, GameManager.gameManager.catCartSFX.position);

                    // Instantiate draining energy particle effect if the player has the Catilizer in the left or right hand
                    if (GameManager.gameManager.playerUI.leftHandWeapon.activeInHierarchy)
                    {
                        GameObject newDrainParticle = Instantiate(enemyDrainsEnergyParticle, GameManager.sLeftController.transform);
                        newDrainParticle.GetComponentInChildren<particleAttractorLinear>().target = taserBeamOwner.transform;
                    }
                    else if (GameManager.gameManager.playerUI.rightHandWeapon.activeInHierarchy)
                    {
                        GameObject newDrainParticle = Instantiate(enemyDrainsEnergyParticle, GameManager.sRightController.transform);
                        newDrainParticle.GetComponentInChildren<particleAttractorLinear>().target = taserBeamOwner.transform;
                    }

                    // If this shot completely drains player's energy, then disable the energy recharge
                    if (GameManager.currentEnergy - taserBeamOwner.energyDrainPerShot <= 0)
                    {
                        GameManager.currentEnergy = 0;
                        GameManager.canRechargeEnergy = false;
                        print("disable energy recharge");

                        if (Time.time - GameManager.lastTimeNoEnergyVoicePlayed > catCartNoEnergyLine.length + 10)
                        {
                            GameManager.gameManager.catCartVoiceOver.PlayOneShot(catCartNoEnergyLine); // Play CatCart no energy line
                            GameManager.lastTimeNoEnergyVoicePlayed = Time.time;
                        }
                    }
                    else
                    {                     
                        GameManager.currentEnergy -= taserBeamOwner.energyDrainPerShot;
                    }

                    //if (!player.isInvulnerable) // If the player is not invulnerable
                    //{
                    //    player.effectiveShots.Add(Time.time); // Add current shot to the list for all the shots player've taken
                    //    effective = false;

                    //    if (player.effectiveShots.Count >= player.continuousShotToDisarm) // If player've taken enough continuous shots
                    //    {
                    //        player.lastDisarmTime = Time.time;
                    //        player.isInvulnerable = true;

                    //        if (GameManager.sLeftController.GetComponent<HandPickItems>().currentItemName == "PlayerWeapon")
                    //        {
                    //            GameManager.sLeftController.GetComponent<HandPickItems>().dropItem();
                    //        }
                    //        if (GameManager.sRightController.GetComponent<HandPickItems>().currentItemName == "PlayerWeapon")
                    //        {
                    //            GameManager.sRightController.GetComponent<HandPickItems>().dropItem();
                    //        }
                    //    }
                    //}

                    return;
                }
                else if (hitobj.GetComponent<ShieldBehavior>() && effective) // If it shot at player shield
                {
                    hitobj.GetComponent<ShieldBehavior>().BeingHit();
                    effective = false;

                    return;
                }
            }
            //print("find" + hit.collider.gameObject.name);
            if (shootables.Contains(hitobj.tag) && Time.time - bulletStartTime <= actualBulletEffectTime)
            {
                if (hitobj.name == "ChaserEnemy_Temp") // If hit chaser enemy
                {
                    hitobj.GetComponent<NASChaserEnemyBehavior>().lastTimeBeenHit = Time.time;
                    hitobj.GetComponent<NASChaserEnemyBehavior>().beenhit = true;
                }
                else if (hitobj.name == "Body")
                {
                    return;
                }
                else if (hitobj.name == "ContainerTrigger")
                {
                    hitobj.GetComponent<CatContainerTrigger>().isShot = true;
                }
                else if (hitobj.name == "Bullseye") // If hit tutorial bullseye
                {
                    hitobj.GetComponent<GateButtonBullseyeTrigger>().isShot = true;
                }
                else if (hitobj.tag == "EnemyShield")   //If it hits the enemy's shield
                {
                    effective = false;
                }
                else
                {
                    if (hitobj.GetComponent<BossGetHit>() && hitobj.GetComponentInParent<BossBehavior>() && hitobj.name != "Body") // If it is a weak point
                    {
                        hitobj.GetComponentInParent<BossBehavior>().bossHealth--;
                        hitobj.GetComponentInParent<BossBehavior>().turnOnShield();
                        hitobj.GetComponentInParent<BossMovement>().chargeBack();
                    }

                    if (hitobj.GetComponent<SmallEnemyDie>())
                    {
                        hitobj.GetComponent<SmallEnemyDie>().destroyedByPlayer = true;
                    }

                    Destroy(hitobj);
                }
            }
        }
        /*
		if(bHit && BL != null)
		{
			BL.gameObject.renderer.material.SetFloat("_BeamLength",HitTimeLength / BL.GetNextLength() + 0.05f);
		}
		*/
    }

    private void OnDestroy()
    {
        if (taserBeamOwner != null)
        {
            taserBeamOwner.currentShot = null;
        }
    }
}
