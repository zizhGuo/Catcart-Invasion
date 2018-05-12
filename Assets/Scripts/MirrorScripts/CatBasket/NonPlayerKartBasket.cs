using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NonPlayerKartBasket : MonoBehaviour
{
    /// <summary>
    /// This is in charge of the basket logic
    /// </summary>

    public Text catCountText; // The text shows the count of cats
    public AudioClip catGetInBasketSFX; // The sound effect to be played when the cat is getting into the cat basket

    public int catCount; // How many cats are currently in the basket
    public List<GameObject> cats; // The array contain's all the cat gameobject currently in the basket
    public NonPlayerKartBasket nonPlayerBasket; // The non-player mirror of the basket

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        catCount = cats.Count;
        catCountText.text = "X " + cats.Count;

        for(int i = 0; i < cats.Count; i++)
        {
            if(cats[i] == null)
            {
                cats.RemoveAt(i);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Cat") // If it is a cat in the basket
        {
            if (!cats.Contains(other.gameObject)) // If the basket is not already containing the cat
            {
                cats.Add(other.gameObject); // Add this cat to the basket
                //print("non player cat in basket, " + other.name + "at time: " + Time.timeSinceLevelLoad);

                // Play the cat get into basket sound effect
                AudioSource.PlayClipAtPoint(catGetInBasketSFX, GameManager.gameManager.catCartSFX.position);

                other.GetComponent<NonPlayerCatStayInBasket>().getInCoroutine = StartCoroutine(keepNonPlayerCat(other.GetComponent<NonPlayerCatStayInBasket>())); // keep the cat in the basket
            }
            else if (cats.Contains(other.gameObject))
            {
                if (!other.GetComponent<NonPlayerCatStayInBasket>().isInBasket)
                {
                    other.GetComponent<NonPlayerCatStayInBasket>().getInCoroutine = StartCoroutine(keepNonPlayerCat(other.GetComponent<NonPlayerCatStayInBasket>())); // keep the cat in the basket
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Cat")
        {
            if (cats.Contains(other.gameObject))
            {
                StopCoroutine(other.GetComponent<NonPlayerCatStayInBasket>().getInCoroutine);
                other.GetComponent<NonPlayerCatStayInBasket>().isInBasket = false;
                //print("non player cat out basket, " + other.GetComponent<NonPlayerCatStayInBasket>().isInBasket);

                //other.GetComponent<SphereCollider>().enabled = false;

                cats.Remove(other.gameObject);
            }
        }
    }

    public IEnumerator keepNonPlayerCat(NonPlayerCatStayInBasket cat)
    {
        yield return new WaitForSeconds(1.5f);
        cat.isInBasket = true;
        //cat.GetComponent<SphereCollider>().enabled = true;s
    }
}
