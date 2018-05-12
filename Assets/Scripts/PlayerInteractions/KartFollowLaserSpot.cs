using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class KartFollowLaserSpot : VRTK_Pointer
{
    public GameObject catKart;
    public float maximumSpeed; //As long as the player is using laser pointer to guide the Kart, the kart will not move faster than this speed
    public float minimumSpeed; //As long as the player is using laser pointer to guide the Kart, the kart will not move slower than this speed
    public float baseAcceleration; //The base acceleration speed
    public float maxAcceleration; //The maximum value for acceleration
    public AudioSource engineSFX;
    public Transform otherController; // The other controller
    public float minSpeedWhenShocked; // What's the minimum speed the kart will go when it is shocked
    public float maxReverseSpeed; // How fast the cart can move backward

    public float dis2speedFactor; //The distance between the pointer and the kart will be multiplied by this number and then apply to the target speed
    public float currentSpeed;
    public float targetSpeed;
    public float acceleration; // The current acceleration speed
    public bool isLaserActiveAndGuiding; // If the laser pointer is active and currently guiding the movement of the cart
    public bool isLaserActive; // If the laser pointer is active
    public bool isGrabbed; // If the laser pointer is picked up by the player
    public bool laserPointerReady; // If the laser pointer is ready to be used
    public GameManager gameManager;
    public Quaternion currentRotation; // The current rotation of the kart
    public Quaternion targetRotation; // The target rotation of the kart
    public bool isKartShocked; // If the kart is affected by the driller enemy
    public float lastShockedTime; // When is the kart last get shocked (note that each new shock to the kart should reset this time)
    public float shockLastingTime; // How long will the shock last
    public bool beingDraggedByChaser; // If the cart is being dragged by chaser enemy
    public bool isGoingForward; // If the direction of the velocity and cart's forward direction is the same
    public float actualLaserPointerDisplacement; // The real distance between the laser pointer and the cart
    public bool canDrive; // Can the laser drive the cart now?
    public float rotatingSpeed; // How fast the cart is rotating

    protected override void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        //base.Start();
        if (controller == null)
        {
            controller = GetComponentInParent<VRTK_ControllerEvents>();
        }

        isGrabbed = false;
        isLaserActiveAndGuiding = false;
        laserPointerReady = false;
        beingDraggedByChaser = false;
        canDrive = true;
        //dis2speedFactor = GameManager.sSpeedMultiplier / 10f;
        //maximumSpeed *= GameManager.sSpeedMultiplier;
        //minimumSpeed *= GameManager.sSpeedMultiplier;
        //maxAcceleration *= GameManager.sSpeedMultiplier;
        if (GameManager.gameManager.skipTutorial) // Automatically setup the speed if we skip tutorial
        {
            dis2speedFactor = gameManager.speedMultiplier / 10f;
        }
        else
        {
            dis2speedFactor = gameManager.speedMultiplier / 3f;
        }

        maximumSpeed *= gameManager.speedMultiplier;
        minimumSpeed *= gameManager.speedMultiplier;
        baseAcceleration *= gameManager.speedMultiplier;
        maxAcceleration *= gameManager.speedMultiplier;
    }

    protected override void Update()
    {
        // If the speed multiplier changed
        if (GameManager.sSpeedMultiplier != GameManager.gameManager.speedMultiplier)
        {
            maximumSpeed *= (GameManager.sSpeedMultiplier / gameManager.speedMultiplier);
            minimumSpeed *= (GameManager.sSpeedMultiplier / gameManager.speedMultiplier);
            baseAcceleration *= (GameManager.sSpeedMultiplier / gameManager.speedMultiplier);
            maxAcceleration *= (GameManager.sSpeedMultiplier / gameManager.speedMultiplier);

            // Also need to change it for the other hand
            otherController.GetComponent<KartFollowLaserSpot>().maximumSpeed *= (GameManager.sSpeedMultiplier / gameManager.speedMultiplier);
            otherController.GetComponent<KartFollowLaserSpot>().minimumSpeed *= (GameManager.sSpeedMultiplier / gameManager.speedMultiplier);
            otherController.GetComponent<KartFollowLaserSpot>().baseAcceleration *= (GameManager.sSpeedMultiplier / gameManager.speedMultiplier);
            otherController.GetComponent<KartFollowLaserSpot>().maxAcceleration *= (GameManager.sSpeedMultiplier / gameManager.speedMultiplier);

            GameManager.gameManager.speedMultiplier = GameManager.sSpeedMultiplier;

            dis2speedFactor = gameManager.speedMultiplier / 10f;
            otherController.GetComponent<KartFollowLaserSpot>().dis2speedFactor = gameManager.speedMultiplier / 10f;
        }

        if (GameManager.kartMovementInfo == null || transform.Find("LaserPointer_Temp") || transform.Find("LaserPointer"))
        {
            GameManager.kartMovementInfo = this;
        }

        //Change the engine sound pitch
        if (transform.Find("LaserPointer_Temp") || transform.Find("LaserPointer"))
        {
            engineSFX.pitch = 1f + (currentSpeed / 60 * 2f);
        }

        decideVelocityDir();

        calculateCurrentSpeed();

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

        if (!isLaserActiveAndGuiding && currentSpeed != 0)
        {
            laserOffStopKart(true);
        }
    }

    public void CallSubscribeActivationButton()
    {
        base.SubscribeActivationButton();
    }

    public override void PointerEnter(RaycastHit givenHit)
    {
        isLaserActive = true;

        if (!canDrive)
        {
            return;
        }

        //print(givenHit.transform.name + ": pointer enter");
        if (otherController.Find("LaserPointer_Temp") || otherController.Find("LaserPointer"))
        {
            //print(transform.name + ": other controller has laser");
            return;
        }

        if (!laserPointerReady)
        {
            //print(transform.name + ": laserPointer not Ready");
            return;
        }

        if (Time.time - lastShockedTime >= shockLastingTime) // If the shock effect is over
        {
            isKartShocked = false;
        }

        //base.PointerEnter(givenHit);
        //if (givenHit.transform.tag == "IgnoreLaser")
        //{

        //}

        isLaserActiveAndGuiding = true;

        actualLaserPointerDisplacement = Vector3.Distance((pointerRenderer as VRTK_StraightPointerRenderer).actualCursor.transform.position, catKart.transform.position);

        //acceleration = baseAcceleration + currentSpeed / maximumSpeed * (maxAcceleration - baseAcceleration);

        if (isKartShocked) // If the cart is shocked by driller enemy's attack
        {
            if (targetSpeed > minSpeedWhenShocked)
            {
                targetSpeed = minSpeedWhenShocked;
            }
        }
        else if (!isKartShocked)
        {
            targetSpeed = Mathf.Clamp(Mathf.Pow(2, (actualLaserPointerDisplacement * dis2speedFactor)), minimumSpeed, maximumSpeed); //Get the target speed based on the distance between the laser pointer and the kart, regardless of direction
        }

        kartTurn(); // Make the cart turn

        catKart.GetComponent<Rigidbody>().velocity = (catKart.transform.forward.normalized) * currentSpeed; //Turn the kart to where the pointer currently aiming at

        if (!isGoingForward) // If the cart is going backward because bouncing back by collision
        {
            acceleration = Mathf.Clamp((baseAcceleration + currentSpeed) / baseAcceleration, baseAcceleration * 0.5f, baseAcceleration);
            //acceleration = baseAcceleration * 2f;
            catKart.GetComponent<Rigidbody>().AddForce((catKart.transform.forward.normalized) * acceleration, ForceMode.Acceleration);
        }
        else
        {
            if (givenHit.collider.tag != "CanKartGo" && givenHit.collider.tag != "GroundTrigger") // If the laser is shooting at invalid navigation point
            {
                //print(transform.name + ": laser off, hit " + givenHit.collider.name);
                laserOffStopKart(false);
                return;
            }

            if (isKartShocked) // If the cart is shocked
            {
                acceleration = Mathf.Clamp(baseAcceleration * currentSpeed / maximumSpeed * 3f, baseAcceleration, baseAcceleration * 3f); //The acceleration will be faster as the velocity go faster
            }
            else if (currentSpeed <= maximumSpeed) // If the cart is going under maximum speed
            {
                if (currentSpeed < targetSpeed) // If the cart is going under target speed
                {
                    acceleration = Mathf.Clamp(baseAcceleration +
                                               (baseAcceleration * (Mathf.Clamp(currentSpeed, 0, maximumSpeed / 2f) / (maximumSpeed / 2f)) * 2f) -
                                               (baseAcceleration * ((Mathf.Clamp(currentSpeed, maximumSpeed / 2f, maximumSpeed) - (maximumSpeed / 2f)) / (maximumSpeed / 2f)) * 3f)
                                               , 0, maxAcceleration); //The acceleration will be faster as the velocity go faster, but will slow down as the cart is reaching top speed
                }
                else if (currentSpeed > targetSpeed) // If the cart is going above target speed
                {
                    acceleration = Mathf.Clamp(baseAcceleration * currentSpeed / maximumSpeed * 3f, baseAcceleration, baseAcceleration * 3f); //The acceleration will be faster as the velocity go faster
                }
            }
            else if (currentSpeed >= maximumSpeed && Mathf.Pow(2, (actualLaserPointerDisplacement * dis2speedFactor)) <= currentSpeed) // If the cart is going above maximum speed and above target speed
            {
                //print("slow down");
                acceleration = Mathf.Clamp(baseAcceleration * currentSpeed / maximumSpeed * 3f, baseAcceleration, baseAcceleration * 3f); //The acceleration will be faster as the velocity go faster
                                                                                                                                          //Mathf.Clamp(baseAcceleration +
                                                                                                                                          //                           (baseAcceleration * (Mathf.Clamp(currentSpeed, 0, maximumSpeed / 2f) / (maximumSpeed / 2f)) * 2f)
                                                                                                                                          //                           , 0, maxAcceleration); //The acceleration will be faster as the velocity go faster

                //print(">max");
            }
            else if (currentSpeed > maximumSpeed) // If the cart is going above maximum speed and under target speed
            {
                acceleration = baseAcceleration * ((currentSpeed - maximumSpeed) / (maximumSpeed)) * 3f;
            }

            if (currentSpeed < targetSpeed) // If current speed is smaller than target speed
            {
                catKart.GetComponent<Rigidbody>().AddForce((catKart.transform.forward.normalized) * acceleration, ForceMode.Acceleration);
            }
            else if (currentSpeed > targetSpeed && isGoingForward) // If current speed is greater than target speed and the cart is not going backward
            {
                //print(Vector3.Angle(catKart.GetComponent<Rigidbody>().velocity, catKart.transform.forward));
                catKart.GetComponent<Rigidbody>().AddForce(-(catKart.transform.forward.normalized) * acceleration, ForceMode.Acceleration);
            }
        }
    }

    public override void PointerExit(RaycastHit givenHit)
    {
        isLaserActive = false;

        if (otherController.Find("LaserPointer_Temp") || otherController.Find("LaserPointer"))
        {
            return;
        }

        isLaserActiveAndGuiding = false;
    }

    public void calculateCurrentSpeed() // Get the current speed of the cart regardless of direction
    {
        if (isGoingForward)
        {
            currentSpeed = catKart.GetComponent<Rigidbody>().velocity.magnitude;
        }
        else
        {
            currentSpeed = Mathf.Clamp(-catKart.GetComponent<Rigidbody>().velocity.magnitude, -maxReverseSpeed, 0);
        }
    }

    public void decideVelocityDir() // If the cart is going forward or backward
    {
        if (Vector3.Angle(catKart.GetComponent<Rigidbody>().velocity, catKart.transform.forward) < 90)
        {
            isGoingForward = true;
        }
        else
        {
            isGoingForward = false;
        }
    }

    public void kartTurn()
    {
        currentRotation = catKart.transform.rotation;
        catKart.transform.LookAt((pointerRenderer as VRTK_StraightPointerRenderer).actualCursor.transform);
        targetRotation = catKart.transform.rotation;
        catKart.transform.rotation = currentRotation;

        catKart.GetComponent<Rigidbody>().angularVelocity = Vector3.zero; // So that the kart won't turning like crazy after hitting obstacles

        if (catKart.transform.InverseTransformPoint((pointerRenderer as VRTK_StraightPointerRenderer).actualCursor.transform.position).x < 0) // If the laser is on the left side of the kart
        {
            if (currentRotation.eulerAngles.y <= 60 && targetRotation.eulerAngles.y >= 60) // Prevent instant turn speed increase caused by loop back from 0 degree to 360
            {
                catKart.transform.eulerAngles = new Vector3(0, catKart.transform.eulerAngles.y
                                                           - Mathf.Clamp(Mathf.Abs(-(360f - targetRotation.eulerAngles.y) - currentRotation.eulerAngles.y), 0, 30)
                                                           * Time.deltaTime * 5f, 0);
                // Calculate how much the cart turned in this frame
                rotatingSpeed = Mathf.Clamp(Mathf.Abs(-(360f - targetRotation.eulerAngles.y) - currentRotation.eulerAngles.y), 0, 30);
            }

            else
            {
                catKart.transform.eulerAngles = new Vector3(0, catKart.transform.eulerAngles.y
                                                           - Mathf.Clamp(Mathf.Abs(targetRotation.eulerAngles.y - currentRotation.eulerAngles.y), 0, 30)
                                                           * Time.deltaTime * 5f, 0);
                // Calculate how much the cart turned in this frame
                rotatingSpeed = Mathf.Clamp(Mathf.Abs(targetRotation.eulerAngles.y - currentRotation.eulerAngles.y), 0, 30);
            }
        }

        if (catKart.transform.InverseTransformPoint((pointerRenderer as VRTK_StraightPointerRenderer).actualCursor.transform.position).x > 0) // If the laser is on the right side of the kart
        {
            if (currentRotation.eulerAngles.y >= 300 && targetRotation.eulerAngles.y <= 300)
            {
                catKart.transform.eulerAngles = new Vector3(0, catKart.transform.eulerAngles.y
                                                           + Mathf.Clamp(Mathf.Abs((360f + targetRotation.eulerAngles.y) - currentRotation.eulerAngles.y), 0, 30)
                                                           * Time.deltaTime * 5f, 0);
                // Calculate how much the cart turned in this frame
                rotatingSpeed = Mathf.Clamp(Mathf.Abs((360f + targetRotation.eulerAngles.y) - currentRotation.eulerAngles.y), 0, 30);
            }

            else
            {
                catKart.transform.eulerAngles = new Vector3(0, catKart.transform.eulerAngles.y
                                                           + Mathf.Clamp(Mathf.Abs(targetRotation.eulerAngles.y - currentRotation.eulerAngles.y), 0, 30)
                                                           * Time.deltaTime * 5f, 0);
                // Calculate how much the cart turned in this frame
                rotatingSpeed = Mathf.Clamp(Mathf.Abs(targetRotation.eulerAngles.y - currentRotation.eulerAngles.y), 0, 30);
            }
        }
    }

    public void laserOffStopKart(bool stopRotation)
    {
        if (stopRotation)
        {
            catKart.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        //print("laser off");
        if (otherController.Find("LaserPointer_Temp") || otherController.Find("LaserPointer")) // If the laser pointer is on the other controller
        {
            return;
        }

        //if (beingDraggedByChaser)
        //{
        //    return;
        //}

        //currentSpeed = catKart.GetComponent<Rigidbody>().velocity.magnitude; //Get the current speed of the kart regardless of direction.

        if (isKartShocked) // If the cart is shocked
        {
            acceleration = Mathf.Clamp(baseAcceleration * currentSpeed / maximumSpeed * 3f, baseAcceleration, baseAcceleration * 3f); //The acceleration will be faster as the velocity go faster
        }
        else
        {
            acceleration = baseAcceleration;
        }

        if (currentSpeed > 0.1f) // If the cart is moving
        {
            catKart.GetComponent<Rigidbody>().AddForce(-Mathf.Sign(currentSpeed) * catKart.transform.forward.normalized * acceleration, ForceMode.Acceleration);

            //if (isKartShocked)
            //{
            //    print("laser off, shocked: " + acceleration + currentSpeed);
            //}
        }
        else
        {
            catKart.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        //if (currentSpeed > acceleration)
        //{
        //    catKart.GetComponent<Rigidbody>().AddForce(-(catKart.transform.forward.normalized) * acceleration, ForceMode.Acceleration);
        //}
        //else
        //{
        //    catKart.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //}
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
        //print("grab");
        //OnEnable();
        StartCoroutine(delayedEnable());
    }

    public void unGrabLaserPointer()
    {
        //print("ungrab");
        OnDisable();
        //StartCoroutine(delayedDisable());
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
