using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Use to change scene when the CatCart enters endroom triggers
/// </summary>
public class DetectCatCartTriggerEvent : MonoBehaviour
{
    public string sceneName; // The name of the scene to be changed

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("CatKartBase"))
        {
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }
    }
}
