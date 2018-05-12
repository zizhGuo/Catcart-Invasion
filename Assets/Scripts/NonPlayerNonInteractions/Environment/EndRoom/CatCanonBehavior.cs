using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK.GrabAttachMechanics;

/// <summary>
/// The behavior of cat canon
/// </summary>
public class CatCanonBehavior : MonoBehaviour
{
    public int canonIndex; // The index that determines which cat should it display
    public GameObject leftCat; // The cat that will shoot from the left canon
    public GameObject rightCat; // The cat that will shoot from the right canon
    public float canonForce; // How much force will the cat be shot out
    public AudioSource leftCanonShootSFX; // The SFX played when the left canon shoot
    public AudioSource rightCanonShootSFX; // The SFX played when the right canon shoot

    public bool canonLoaded; // Are the cats set up

    // Use this for initialization
    void Start()
    {
        canonLoaded = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Set up cats when the game is finished
        if (GameManager.gameFinished && !canonLoaded)
        {
            LoadCanonWithCat();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Shoot the cat out of the canons
        ShootCat();
    }

    /// <summary>
    /// Set up the cat for each canon
    /// </summary>
    public void LoadCanonWithCat()
    {
        int remainingCatCount = GameManager.gameManager.nonPlayerCatBasket.GetComponent<NonPlayerKartBasket>().cats.Count;

        // Set up cat colors for the cats that are remaining
        if (canonIndex < remainingCatCount)
        {
            leftCat.GetComponentInChildren<MeshRenderer>().material =
                GameManager.gameManager.nonPlayerCatBasket.GetComponent<NonPlayerKartBasket>().cats[canonIndex].
                GetComponent<NonPlayerSideMirror>().playerSideCopy.
                GetComponent<InteractWithCat>().catColorRenderer.material;
            rightCat.GetComponentInChildren<MeshRenderer>().material =
                GameManager.gameManager.nonPlayerCatBasket.GetComponent<NonPlayerKartBasket>().cats[canonIndex].
                GetComponent<NonPlayerSideMirror>().playerSideCopy.
                GetComponent<InteractWithCat>().catColorRenderer.material;
        }
        // Set up cat colors for the cats that are missing
        else
        {
            leftCat.GetComponentInChildren<MeshRenderer>().material =
                GameManager.missingCats[canonIndex - remainingCatCount].catMat;
            rightCat.GetComponentInChildren<MeshRenderer>().material =
                GameManager.missingCats[canonIndex - remainingCatCount].catMat;
        }
        //leftCat.GetComponentInChildren<MeshRenderer>().material = GameManager.catsInfo.catsInfo[canonIndex].catMat;
        //rightCat.GetComponentInChildren<MeshRenderer>().material = GameManager.catsInfo.catsInfo[canonIndex].catMat;
        canonLoaded = true;
    }

    /// <summary>
    /// Shoot out cat
    /// </summary>
    public void ShootCat()
    {
        // Seperate the cat from the canon
        leftCat.transform.parent = null;
        leftCat.GetComponent<Rigidbody>().isKinematic = false;
        rightCat.transform.parent = null;
        rightCat.GetComponent<Rigidbody>().isKinematic = false;

        // Shoot the cats
        leftCat.GetComponent<Rigidbody>().AddForce(leftCat.transform.forward * canonForce, ForceMode.VelocityChange);
        rightCat.GetComponent<Rigidbody>().AddForce(rightCat.transform.forward * canonForce, ForceMode.VelocityChange);

        // Play canon shoot SFX
        leftCanonShootSFX.Play();
        rightCanonShootSFX.Play();

        this.enabled = false; // Prevent it shoot more than once
    }
}
