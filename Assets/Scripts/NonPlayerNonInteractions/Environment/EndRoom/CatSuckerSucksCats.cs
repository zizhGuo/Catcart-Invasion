using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CatSuckerSucksCats : MonoBehaviour
{
    public float catSuckUpAcceleration; // How fast the cat should be sucked up
    public EndRoomTrackStopCart catStatusDisplayer; // The gameobject that displays the cat status

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // If a cat is near the sucker's opening, and is released by the player
        if (other.tag == "Cat")
        {
            StartCoroutine(SuckUpCat(other.GetComponent<Rigidbody>()));
        }
    }

    /// <summary>
    /// Suck up a cat
    /// </summary>
    /// <param name="cat"></param>
    /// <returns></returns>
    public IEnumerator SuckUpCat(Rigidbody cat)
    {
        // Adjust cat physics and mirror to be ready to be sucked up
        cat.GetComponent<PlayerSideMirror>().nonPlayerSideCopy.GetComponent<NonPlayerSideMirror>().enabled = false;
        cat.GetComponent<PlayerSideMirror>().enabled = false;
        cat.GetComponent<PlayerCatStayInBasket>().enabled = false;
        cat.mass = 0.001f;
        cat.isKinematic = false;
        cat.useGravity = false;
        cat.velocity = Vector3.zero;
        cat.angularVelocity = Vector3.zero;

        // Suck cat up
        while (cat.transform.position.y < transform.position.y)
        {
            cat.//velocity = (transform.position - cat.transform.position).normalized * catSuckUpAcceleration;
                AddForce((transform.position - cat.transform.position).normalized * catSuckUpAcceleration, ForceMode.Acceleration);

            yield return null;
        }
        while (cat.transform.position.y < 6)
        {
            cat.AddForce(Vector3.up * catSuckUpAcceleration, ForceMode.Acceleration);
            yield return null;
        }

        catStatusDisplayer.ShowCatOnLeaderboard(cat.GetComponent<PlayerCatStayInBasket>());
        
        while (cat.transform.position.y < 20)
        {
            cat.AddForce(Vector3.up * catSuckUpAcceleration, ForceMode.Acceleration);
            yield return null;
        }
        cat.velocity = Vector3.zero;
        cat.isKinematic = true;
    }
}
