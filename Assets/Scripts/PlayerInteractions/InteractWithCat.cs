namespace VRTK.GrabAttachMechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class InteractWithCat : VRTK_FixedJointGrabAttach
    {
        /// <summary>
        /// Clip1: Picked Up By Player // Lower Volumn, need to make louder
        /// Clip2: Touched By Player
        /// Clip3: Dropped By Player
        /// Clip4: Hard Collision
        /// Clip5: Just Been Caught
        /// Clip6: Stay Been Caught
        /// </summary>

        public AudioSource meowSound;
        public AudioClip[] meowClips;
        public float hardCollisionForceToMeow; // If the cat is been collided with a force above this value then it is going to meow hard
        public float softCollisionForceToMeow; // If the cat is been collided with a force above this value then it is going to meow soft
        public string catName; // The name of the cat
        public MeshRenderer catColorRenderer; // The mesh renderer for the cat's skin

        public bool isCaught; // If the cat is being caught
        public float lastCaughtMeow; // The last time when the cat meowed while it is caught
        public float lastTouchMeow; // The last time when the cat meowed while it is touched
        public float lastCollisionMeow; // The last time when the cat meowed while it is collided
        public GameObject playerHead;
        public Coroutine catTurningCoroutine; // Cat turning animation
        public PlayerUI playerUI; // The player's HUD UI
        public GameObject grabbingController; // The controller that is grabbing / was grabbing the cat
        public PhysicMaterial catPhysicsMaterial; // The physics material used for the cat

        // Use this for initialization
        void Start()
        {
            meowSound = GetComponent<AudioSource>();
            playerHead = FindObjectOfType<GameManager>().playerHead;
            playerUI = FindObjectOfType<GameManager>().playerUI;
            catPhysicsMaterial = GetComponent<Collider>().material;

            lastCaughtMeow = 0;
            lastTouchMeow = 0;
            lastCollisionMeow = -1;
        }

        // Update is called once per frame
        void Update()
        {
            // Cat behavior when it is caught
            if (isCaught)
            {
                transform.LookAt(playerHead.transform);
                lastCaughtMeow = Time.time;

                if (Time.time - lastCaughtMeow >= 1.5)
                {
                    meowSound.PlayOneShot(meowClips[5]);
                    lastCaughtMeow = Time.time;
                }
            }

            // Cat behavior when it is touched
            if (GetComponent<VRTK_InteractableObject>().IsTouched() && !isCaught)
            {
                GetComponent<VRTK_InteractableObject>().GetTouchingObjects()[0].GetComponent<VRTK_ControllerActions>().TriggerHapticPulse(0.01f, 0.02f, 0.01f);

                if (Time.time - lastTouchMeow >= 1.5)
                {
                    meowSound.PlayOneShot(meowClips[1]);
                    lastTouchMeow = Time.time;

                    //if (!GetComponentInParent<VRTK_InteractableObject>().IsGrabbed() && catTurningCoroutine == null)
                    //{
                    //    catTurningCoroutine = StartCoroutine(lookAtPlayerWhenTouched());
                    //}
                }
            }

            // Cat behavior when it moved abruptly
            if (GetComponent<PlayerSideMirror>().lastRelativeAcceleration.magnitude >= hardCollisionForceToMeow && Time.time - lastCollisionMeow >= 1 && !isCaught)
            {
                //print(GetComponent<PlayerSideMirror>().lastRelativeAcceleration.magnitude);

                meowSound.PlayOneShot(meowClips[3]);
                lastCollisionMeow = Time.time;
            }

            // Cat behavior when it is looked at
            if (GetComponentInChildren<CheckIfGazed>().isBeingGazed && catTurningCoroutine == null)
            {
                if (!GetComponent<VRTK_InteractableObject>().IsTouched() &&
                    !GetComponentInParent<VRTK_InteractableObject>().IsGrabbed())
                {
                    catTurningCoroutine = StartCoroutine(lookAtPlayerWhenTouched());
                }
            }
        }

        public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
        {
            if (base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint))
            {
                meowSound.PlayOneShot(meowClips[0]);

                GetComponent<PlayerCatStayInBasket>().isInBasket = false;

                foreach (PlayerCatStayInBasket c in GameManager.gameManager.cats)
                {
                    c.GetComponent<Collider>().material = null; // Change the cat's bounceness to 0 when it is being grabbed
                }

                // Display cat's name on player's HUD
                grabbingController = grabbingObject;
                if (grabbingController.name == "LeftController")
                {
                    playerUI.leftHandCat.SetActive(true);
                    playerUI.leftHandCatName.text = catName;
                }
                else
                {
                    playerUI.rightHandCat.SetActive(true);
                    playerUI.rightHandCatName.text = catName;
                }

                return true;
            }

            return false;
        }

        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            base.StopGrab(applyGrabbingObjectVelocity);

            meowSound.PlayOneShot(meowClips[2]);

            foreach (PlayerCatStayInBasket c in GameManager.gameManager.cats)
            {
                c.GetComponent<Collider>().material = catPhysicsMaterial; // Change the cat's bounceness back when it is being grabbed
            }

            // Remove cat's name on player's HUD
            if (grabbingController.name == "LeftController")
            {
                playerUI.leftHandCat.SetActive(false);
            }
            else
            {
                playerUI.rightHandCat.SetActive(false);
            }
        }

        public IEnumerator lookAtPlayerWhenTouched()
        {
            Quaternion oldRotation = transform.rotation;

            transform.LookAt(playerHead.transform);

            Quaternion newRotation = transform.rotation;
            transform.rotation = oldRotation;

            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 0.5f)
            {
                transform.rotation = Quaternion.Slerp(oldRotation, newRotation, t);
                GetComponent<PlayerSideMirror>().nonPlayerSideCopy.transform.rotation = transform.rotation;

                yield return null;
            }

            catTurningCoroutine = null;
        }

        //void OnCollisionEnter(Collision collision)
        //{

        //}

        //void OnCollisionStay(Collision collision)
        //{
        //    print(collision.relativeVelocity.magnitude);

        //    if (collision.relativeVelocity.magnitude >= hardCollisionForceToMeow) // Hard collision
        //    {
        //        meowSound.PlayOneShot(meowClips[3]);
        //    }
        //    else if (collision.relativeVelocity.magnitude >= softCollisionForceToMeow) // Soft collision
        //    {

        //    }
        //}
    }
}