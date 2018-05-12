using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NASDrillerEnemyBehavior : MonoBehaviour
{
    /// <summary>
    /// Driller's behavior:
    /// 1. Drive parallel to the player on the ground for a few seconds
    /// 2. Rotate (turn while keep driving) towards its destined position
    /// 3. Jump forward (towards the destined position) and dig into the ground
    /// 4. Raise it's scope above ground and make the scope constantly looking at the player (only rotate the Y axis)
    /// 5. Move and align with the player on the desired axis (x or z)
    /// 6. When it reaches the destined position, stop and retract the scope and turn on the area hint so player with HUD can see its area
    /// 7. If the player reaches its detecting range then come out of ground and release shockwave to temporarily slow or stop player's kart
    /// </summary>

    public GameObject drillerBody; // The body of the driller vehicle
    public GameObject drillerScope; // The scope on the driller
    public GameObject areaHint; // The hint showing the driller detecting range if the player is wearing HUD
    public float diggingAltitude; // What's the driller enemy's Y axis when it is underground
    public float distanceAheadKart; // How far away from the player's kart the driller enemy will moving at
    public float detectingRange; // How close the player's kart have to be to trigger the driller
    public float timeTillJumpIntoGround; // How long the driller will drive on the ground before it jump and dig into the ground
    public float shockLastingTime; // How long will a single shock last
    public GameObject shockAttack; // The shock area the driller is going to release
    public float shockDelay; // How much time the attack will wait to activate after it is triggered (so the player has some time to respond)
    public float moveSpeedAboveGround; // How fast the driller will move relate to its destined position above ground
    public float moveSpeedDuringJump; // How fast the driller will move relate to its destined position during jump
    public float moveSpeedUnderGround; // How fast the driller will move relate to its destined position underground
    public Vector3 initialDirection; // Where the driller will initially look at
    public string matchingAxis; // Which axis will the driller try to match with the player's kart
    public GameObject digInParticle; // The particle effect when the driller drone dig into the ground

    public GameObject playerKart; // The player side kart
    public bool isInGround; // If the driller is in ground
    public bool isInPosition; // If the driller reached its position and deployed
    public Vector3 beforeJumpRotation; // Rotation before jump
    public Vector3 afterJumpRotation; // Rotation after jump
    public bool jumped; // Has the driller jumped

    // Use this for initialization
    void Start()
    {
        playerKart = GameManager.gameManager.playerKart;
        shockAttack.GetComponent<ShockBehavior>().shockLastingTime = shockLastingTime;
        shockAttack.GetComponent<ShockBehavior>().shockDelay = shockDelay;
        GetComponentInChildren<DrillerAreaHintAnimation>(true).range = detectingRange;

        isInGround = false;
        isInPosition = false;
        jumped = false;

        transform.eulerAngles = initialDirection;
    }

    // Update is called once per frame
    void Update()
    {
        //if (timeTillJumpIntoGround > 0) // If the driller is still waiting to jump into ground
        //{
        //    GetComponent<Rigidbody>().velocity = transform.forward * moveSpeedAboveGround;
        //    timeTillJumpIntoGround -= Time.deltaTime;

        //    if (timeTillJumpIntoGround <= 0)
        //    {
        //        StartCoroutine(JumpIntoGround());
        //    }

        //    return;
        //}
        if(Vector3.Distance(transform.position, playerKart.transform.position) >= 500)
        {
            return;
        }

        if (Vector3.Distance(transform.position, playerKart.transform.position) < 500 && !jumped)
        {
            jumped = true;
            StartCoroutine(JumpIntoGround());
        }

        if (transform.position.y <= diggingAltitude && !isInGround) // While jumping, if the driller is getting below the ground
        {
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.position = new Vector3(transform.position.x, diggingAltitude, transform.position.z);
            isInGround = true;
            Instantiate(digInParticle, transform.position, transform.rotation);
        }

        if (isInGround && !isInPosition) // If the driller is already in ground
        {
            drillerScope.SetActive(true);

            if (drillerScope.transform.localPosition.y < 0.76f) // Raise the scope
            {
                drillerScope.transform.localPosition = new Vector3(0, drillerScope.transform.localPosition.y + Time.deltaTime, 0);
            }

            transform.LookAt(playerKart.transform.position + playerKart.transform.forward * distanceAheadKart + Vector3.up * diggingAltitude);
            //GetComponent<Rigidbody>().velocity = transform.forward * moveSpeedUnderGround;

            if(matchingAxis == "x")
            {
                transform.position += Vector3.right * Mathf.Sign(playerKart.transform.position.x - transform.position.x) * (moveSpeedUnderGround * Time.deltaTime);
            }
            else if(matchingAxis == "z")
            {
                transform.position += Vector3.forward * Mathf.Sign(playerKart.transform.position.z - transform.position.z) * (moveSpeedUnderGround * Time.deltaTime);
            }

            drillerScope.transform.LookAt(playerKart.transform); // Make the scope look at player's kart
            drillerScope.transform.localEulerAngles = new Vector3(0, drillerScope.transform.localEulerAngles.y, 0);
        }

        if (isInGround && !isInPosition) // If it reaches the destined position
        {
            if ((matchingAxis == "x" && playerKart.transform.position.x - transform.position.x <= 0.2f) ||
               (matchingAxis == "z" && playerKart.transform.position.z - transform.position.z <= 0.2f))
            {
                isInPosition = true;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                areaHint.SetActive(true);
                shockAttack.SetActive(true);
            }
        }

        if (isInPosition)
        {
            //if (drillerScope.transform.localPosition.y > -0.26f) // Lower the scope
            //{
            //    drillerScope.transform.localPosition = new Vector3(0, drillerScope.transform.localPosition.y - Time.deltaTime, 0);
            //}
            //else
            //{
            //    if (drillerScope.activeInHierarchy)
            //    {
            //        drillerScope.SetActive(false);
            //    }

            //    //transform.eulerAngles = -initialDirection;
            //    //GetComponent<Rigidbody>().velocity = transform.forward * moveSpeedUnderGround;
            //}

            drillerScope.transform.LookAt(playerKart.transform); // Make the scope look at player's kart
            drillerScope.transform.localEulerAngles = new Vector3(0, drillerScope.transform.localEulerAngles.y, 0);
            transform.LookAt(playerKart.transform.position);
        }

        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0); // Keep it parallel to the ground
    }

    public IEnumerator JumpIntoGround()
    {
        //bool turnRight = true;
        Quaternion oldRotation = transform.rotation; // Get the starting rotation
        beforeJumpRotation = oldRotation.eulerAngles;

        transform.LookAt(playerKart.transform.position + playerKart.transform.forward * distanceAheadKart);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        Quaternion newRotation = transform.rotation; // Get the end rotation
        afterJumpRotation = newRotation.eulerAngles;

        transform.rotation = oldRotation;

        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1)
        {
            //if (turnRight)
            //{
            //    transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg * Mathf.LerpAngle(oldRotation.y, newRotation.y, t), 0);
            //}
            //else
            //{
            //    transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg * Mathf.LerpAngle(oldRotation.y, newRotation.y, t), 0);
            //}

            //GetComponent<Rigidbody>().MoveRotation(newRotation);
            //transform.eulerAngles = new Vector3(0, Mathf.Rad2Deg * Mathf.LerpAngle(oldRotation.y, newRotation.y, t), 0);
            transform.rotation = Quaternion.Slerp(oldRotation, newRotation, t);
            GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeedDuringJump, ForceMode.Acceleration);
            yield return null;
        }

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().useGravity = true;
        GetComponent<Rigidbody>().AddForce(transform.forward * moveSpeedDuringJump, ForceMode.VelocityChange);
        GetComponent<Rigidbody>().AddForce(transform.up * moveSpeedDuringJump, ForceMode.VelocityChange);
    }
}
