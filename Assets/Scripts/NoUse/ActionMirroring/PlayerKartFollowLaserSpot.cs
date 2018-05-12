using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class PlayerKartFollowLaserSpot : VRTK_Pointer
{
    public float maximumSpeed; //As long as the player is using laser pointer to guide the Kart, the kart will not move faster than this speed
    public float minimumSpeed; //As long as the player is using laser pointer to guide the Kart, the kart will not move slower than this speed
    public float baseAcceleration; //The base acceleration speed
    public float maxAcceleration; //The maximum value for acceleration
    public AudioSource engineSFX;
    public Transform otherController; // The other controller

    public GameObject playerCatKart; // The player side kart
    public GameObject nonPlayerCatKart; // The non-player side kart
    public float dis2speedFactor; //The distance between the pointer and the kart will be multiplied by this number and then apply to the target speed
    public float currentSpeed;
    public float targetSpeed;
    public float acceleration; //The current acceleration speed
    public bool isLaserActive;
    public bool isGrabbed; // If the laser pointer is picked up by the player
    public bool laserPointerReady; // If the laser pointer is ready to be used
    public MirrorGameManager gameManager;
    public Quaternion currentRotation; // The current rotation of the kart
    public Quaternion targetRotation; // The target rotation of the kart

    protected override void Start()
    {
        gameManager = FindObjectOfType<MirrorGameManager>();
        //base.Start();
        if (controller == null)
        {
            controller = GetComponentInParent<VRTK_ControllerEvents>();
        }

        isGrabbed = false;
        isLaserActive = false;
        laserPointerReady = false;
        dis2speedFactor = MirrorGameManager.sSpeedMultiplier / 10f;
        maximumSpeed *= MirrorGameManager.sSpeedMultiplier;
        minimumSpeed *= MirrorGameManager.sSpeedMultiplier;
        maxAcceleration *= MirrorGameManager.sSpeedMultiplier;
        playerCatKart = gameManager.playerKart;
        nonPlayerCatKart = gameManager.nonPlayerKart;
    }

    protected override void Update()
    {
        if (MirrorGameManager.kartMovementInfo == null || transform.Find("LaserPointer_Temp") || transform.Find("LaserPointer"))
        {
            MirrorGameManager.kartMovementInfo = this;
        }

        //Change the engine sound pitch
        engineSFX.pitch = 1f + (currentSpeed / maximumSpeed * 2f);

        if (EnabledPointerRenderer())
        {
            pointerRenderer.InitalizePointer(this, invalidListPolicy, navMeshCheckDistance, headsetPositionCompensation);
            pointerRenderer.UpdateRenderer();
            if (!IsPointerActive())
            {
                bool currentPointerVisibility = pointerRenderer.IsVisible();
                pointerRenderer.ToggleInteraction(currentPointerVisibility);
            }
        }

        if (!isLaserActive && playerCatKart.GetComponent<Rigidbody>().velocity.magnitude != 0)
        {
            laserOffStopKart();
        }

        nonPlayerCatKart.transform.rotation = playerCatKart.transform.rotation;
    }

    public override void PointerEnter(RaycastHit givenHit)
    {
        if (otherController.Find("LaserPointer_Temp") || otherController.Find("LaserPointer"))
        {
            return;
        }

        if (!laserPointerReady)
        {
            return;
        }

        if (givenHit.collider.tag != "CanKartGo" && givenHit.collider.tag != "GroundTrigger") // If the laser is shooting at invalid navigation point
        {
            laserOffStopKart();
            return;
        }
        
        if (givenHit.transform.tag == "IgnoreLaser")
        {

        }

        isLaserActive = true;
        currentSpeed = playerCatKart.GetComponent<Rigidbody>().velocity.magnitude; //Get the current speed of the kart regardless of direction
        acceleration = baseAcceleration + currentSpeed / maximumSpeed * (maxAcceleration - baseAcceleration); //The acceleration will be faster as the velocity go faster
        targetSpeed = Mathf.Clamp(Mathf.Pow(2, (Vector3.Distance((pointerRenderer as VRTK_StraightPointerRenderer).actualCursor.transform.position, playerCatKart.transform.position) * dis2speedFactor)), minimumSpeed, maximumSpeed); //Get the target speed based on the distance between the laser pointer and the kart, regardless of direction

        currentRotation = playerCatKart.transform.rotation;
        playerCatKart.transform.LookAt((pointerRenderer as VRTK_StraightPointerRenderer).actualCursor.transform);
        targetRotation = playerCatKart.transform.rotation;

        playerCatKart.transform.rotation = currentRotation;

        if (playerCatKart.transform.InverseTransformPoint((pointerRenderer as VRTK_StraightPointerRenderer).actualCursor.transform.position).x < 0) // If the laser is on the left side of the kart
        {
            if (currentRotation.eulerAngles.y <= 60 && targetRotation.eulerAngles.y >= 60) // Prevent instant turn speed increase caused by loop back from 0 degree to 360
            {
                playerCatKart.transform.eulerAngles = new Vector3(0, playerCatKart.transform.eulerAngles.y
                                                           - Mathf.Clamp(Mathf.Abs(-(360f - targetRotation.eulerAngles.y) - currentRotation.eulerAngles.y), 0, 30)
                                                           * Time.deltaTime * 5f, 0);
            }

            else
            {
                playerCatKart.transform.eulerAngles = new Vector3(0, playerCatKart.transform.eulerAngles.y
                                                           - Mathf.Clamp(Mathf.Abs(targetRotation.eulerAngles.y - currentRotation.eulerAngles.y), 0, 30)
                                                           * Time.deltaTime * 5f, 0);
            }
        }

        if (playerCatKart.transform.InverseTransformPoint((pointerRenderer as VRTK_StraightPointerRenderer).actualCursor.transform.position).x > 0) // If the laser is on the right side of the kart
        {
            if (currentRotation.eulerAngles.y >= 300 && targetRotation.eulerAngles.y <= 300)
            {
                playerCatKart.transform.eulerAngles = new Vector3(0, playerCatKart.transform.eulerAngles.y
                                                           + Mathf.Clamp(Mathf.Abs((360f + targetRotation.eulerAngles.y) - currentRotation.eulerAngles.y), 0, 30)
                                                           * Time.deltaTime * 5f, 0);
            }

            else
            {
                playerCatKart.transform.eulerAngles = new Vector3(0, playerCatKart.transform.eulerAngles.y
                                                           + Mathf.Clamp(Mathf.Abs(targetRotation.eulerAngles.y - currentRotation.eulerAngles.y), 0, 30)
                                                           * Time.deltaTime * 5f, 0);
            }
        }

        playerCatKart.GetComponent<Rigidbody>().velocity = (playerCatKart.transform.forward.normalized) * currentSpeed; //Turn the kart to where the pointer currently aiming at

        if (currentSpeed < targetSpeed)
        {
            playerCatKart.GetComponent<Rigidbody>().AddForce((playerCatKart.transform.forward.normalized) * acceleration, ForceMode.Acceleration);
        }
        else if (currentSpeed > targetSpeed && Vector3.Angle(playerCatKart.GetComponent<Rigidbody>().velocity, playerCatKart.transform.forward) <= 30)
        {
            playerCatKart.GetComponent<Rigidbody>().AddForce(-(playerCatKart.transform.forward.normalized) * acceleration, ForceMode.Acceleration);
        }
        else
        {

        }
    }

    public override void PointerExit(RaycastHit givenHit)
    {
        if (otherController.Find("LaserPointer_Temp") || otherController.Find("LaserPointer"))
        {
            return;
        }

        isLaserActive = false;
    }

    public void laserOffStopKart()
    {
        if (otherController.Find("LaserPointer_Temp") || otherController.Find("LaserPointer"))
        {
            return;
        }

        currentSpeed = playerCatKart.GetComponent<Rigidbody>().velocity.magnitude; //Get the current speed of the kart regardless of direction.

        if (currentSpeed > 0.1f && Vector3.Angle(playerCatKart.GetComponent<Rigidbody>().velocity, playerCatKart.transform.forward) <= 30)
        {
            playerCatKart.GetComponent<Rigidbody>().AddForce(-(playerCatKart.transform.forward.normalized) * acceleration, ForceMode.Acceleration);
        }
        else
        {
            playerCatKart.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }

    public IEnumerator delayedEnable()
    {
        yield return new WaitForEndOfFrame();
        OnEnable();
        StartCoroutine(readyLaser());
    }

    public IEnumerator delayedDisable()
    {
        yield return new WaitForEndOfFrame();
        OnDisable();
    }

    public void grabLaserPointer()
    {
        StartCoroutine(delayedEnable());
    }

    public void unGrabLaserPointer()
    {
        OnDisable();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    public IEnumerator readyLaser()
    {
        yield return new WaitForEndOfFrame();
        laserPointerReady = true;
    }
}
