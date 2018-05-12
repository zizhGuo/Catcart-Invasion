using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class KartBasket : MonoBehaviour
{


    public int catCount; // How many cats are currently in the basket
    public List<GameObject> cats; // The array contain's all the cat gameobject currently in the basket

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        catCount = cats.Count;
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Cat") // If it is a cat in the basket
        {
            if(!cats.Contains(other.gameObject)) // If the basket is not already containing the cat
            {
                cats.Add(other.gameObject); // Add this cat to the basket

                if (other.GetComponent<CatStayInKart>() && !other.GetComponent<CatStayInKart>().isInBasket)
                {
                    other.GetComponent<CatStayInKart>().getInCoroutine = StartCoroutine(keepCat(other.GetComponent<CatStayInKart>())); // keep the cat in the basket
                }
                else if ((other.GetComponent<NonPlayerCatStayInBasket>() && !other.GetComponent<NonPlayerCatStayInBasket>().isInBasket))
                {
                    other.GetComponent<NonPlayerCatStayInBasket>().getInCoroutine = StartCoroutine(keepNonPlayerCat(other.GetComponent<NonPlayerCatStayInBasket>())); // keep the cat in the basket
                }
            }

            if (other.GetComponent<CatStayInKart>() && other.GetComponent<CatStayInKart>().isInBasket)
            {
                if (other.GetComponent<VRTK_InteractableObject>().IsGrabbed()) // If the cat is being grabbed by the player
                {
                    other.GetComponent<CatStayInKart>().isInBasket = false;
                    cats.Remove(other.gameObject);
                }
            }

            else if (other.GetComponent<NonPlayerCatStayInBasket>())
            {
                if (other.GetComponent<NonPlayerSideMirror>().playerSideCopy.GetComponent<VRTK_InteractableObject>().IsGrabbed()) // If the cat is being grabbed by the player
                {
                    other.GetComponent<NonPlayerCatStayInBasket>().isInBasket = false;
                    cats.Remove(other.gameObject);
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
                if (other.GetComponent<CatStayInKart>() && other.GetComponent<CatStayInKart>().isInBasket)
                {
                    StopCoroutine(other.GetComponent<CatStayInKart>().getInCoroutine);
                    other.GetComponent<CatStayInKart>().isInBasket = false;
                }
                else
                {
                    StopCoroutine(other.GetComponent<NonPlayerCatStayInBasket>().getInCoroutine);
                    other.GetComponent<NonPlayerCatStayInBasket>().isInBasket = false;
                }

                cats.Remove(other.gameObject);
            }
        }
    }

    public IEnumerator keepCat(CatStayInKart cat)
    {
        yield return new WaitForSeconds(1.5f);
        cat.isInBasket = true;
        cat.positionInBasket = transform.InverseTransformPoint(cat.transform.position);
    }
    public IEnumerator keepNonPlayerCat(NonPlayerCatStayInBasket cat)
    {
        yield return new WaitForSeconds(1.5f);
        cat.isInBasket = true;
    }
}
