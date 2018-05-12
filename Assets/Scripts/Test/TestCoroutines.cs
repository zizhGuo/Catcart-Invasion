using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCoroutines : MonoBehaviour
{
    public bool runNewCoroutine;
    public List<Coroutine> runningCoroutines = new List<Coroutine>();
    public int runningCoroutineCount;
    public bool stopAll;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (runNewCoroutine)
        {
            runNewCoroutine = false;
            runningCoroutines.Add(StartCoroutine(testCoroutine()));
        }

        if (stopAll)
        {
            stopAll = false;
            StopAllPlayLineCoroutines();
        }

        runningCoroutineCount = runningCoroutines.Count;
    }

    public void StopAllPlayLineCoroutines()
    {
        for (int i = runningCoroutines.Count - 1; i > -1; i--)
        {
            StopCoroutine(runningCoroutines[i]);
            runningCoroutines.Remove(runningCoroutines[i]);
        }
    }

    public IEnumerator testCoroutine()
    {
        yield return new WaitForSeconds(1);
    }
}
