namespace VRTK.GrabAttachMechanics
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ItemLeaveHand : VRTK_FixedJointGrabAttach
    {
        /// <summary>
        /// This script is attached to any holdable items
        /// </summary>
        public string itemName; // The name of the item

        // Use this for initialization
        void Start()
        {
            itemName = gameObject.name;
        }

        //Update is called once per frame
        void Update()
        {

        }

        public override bool StartGrab(GameObject grabbingObject, GameObject givenGrabbedObject, Rigidbody givenControllerAttachPoint)
        {

            if (base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint))
            {
                grabbingObject.GetComponent<HandPickItems>().currentItem = givenGrabbedObject;
                grabbingObject.GetComponent<HandPickItems>().currentItemName = itemName;

                if (itemName == "CatPawStick") // The cat-paw stick doesn't need a mirror so it doesn't need to be disabled when grabbed
                {
                    //print("stick");
                    //GetComponent<Rigidbody>().isKinematic = true;
                    //base.StartGrab(grabbingObject, givenGrabbedObject, givenControllerAttachPoint);
                    return true;
                }
                else
                {
                    // Disable any colliders in the non-player copy
                    foreach (Collider c in GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponentsInChildren<Collider>())
                    {
                        c.enabled = false;
                    }

                    transform.parent = grabbingObject.transform;
                    gameObject.SetActive(false);
                }

                return true;
            }

            return false;
        }

        public override void StopGrab(bool applyGrabbingObjectVelocity)
        {
            //print("Stop grab");
            if (itemName == "CatPawStick")
            {
                base.StopGrab(applyGrabbingObjectVelocity);
                return;
            }

            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            transform.parent = null;

            base.StopGrab(applyGrabbingObjectVelocity);
        }

        protected override void DestroyJoint(bool withDestroyImmediate, bool applyGrabbingObjectVelocity)
        {
            controllerAttachJoint.connectedBody = null;

            Destroy(controllerAttachJoint);
        }
    }
}