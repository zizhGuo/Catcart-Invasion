using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMController : MonoBehaviour
{
    public float fadeInDuration; // The duration of the fade in
    public float fadeOutDuration; // The duration of the fade out
    public float timeTillFadeIn; // The time to wait for next fade in after the previous BGM is completely fade out
    public Transform alleyBGMplayLocation; // Where to start play the alley BGM
    public Transform finalBGMplayLocation; // Where to start play the final BGM
    public AudioClip finalBGMIntro;
    public AudioClip finalBGMLoop;
    public AudioClip alleyBGMIntro;
    public AudioClip alleyBGMLoop;
    public AudioClip beginningBGMIntro;
    public AudioClip beginningBGMLoop;
    public AudioClip tutorialBGMIntro;
    public AudioClip tutorialBGMLoop;
    public AudioClip catCartAlleyFinishVoiceOverLine; // The line that CatCart will say after coming out of the alley

    private AudioSource audioSource;
    public int bgmTracker; // The counter that keeps track of the BGM that's been played
    public static BGMController bgmController; // The static reference of this bgm controller
    public float defaultVolume; // The default volume of the bgm player

    // Use this for initialization
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        bgmTracker = 0;
        bgmController = this;
        defaultVolume = audioSource.volume;

        // If skip tutorial then play the beginning BGM
        if (GameManager.gameManager.skipTutorial)
        {
            StartCoroutine(PlayBGM(beginningBGMIntro, beginningBGMLoop));
        }
        else
        {
            StartCoroutine(PlayBGM(tutorialBGMIntro, tutorialBGMLoop));
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Play the alley bgm
        if (bgmTracker == 0 &&
            Vector3.Distance(GameManager.gameManager.playerKart.transform.position, alleyBGMplayLocation.position) < 10)
        {
            bgmTracker++;
            StartCoroutine(ChangeBGM(alleyBGMIntro, alleyBGMLoop));
        }

        // Play the final bgm
        if (bgmTracker == 1 &&
            Vector3.Distance(GameManager.gameManager.playerKart.transform.position, finalBGMplayLocation.position) < 10)
        {
            if (!GameManager.gameManager.catCartVoiceOver.isPlaying) // Play the CatCart line when the alley is cleared
            {
                GameManager.gameManager.catCartVoiceOver.PlayOneShot(catCartAlleyFinishVoiceOverLine);
            }
            bgmTracker++;
            StartCoroutine(ChangeBGM(finalBGMIntro, finalBGMLoop));
        }
    }

    /// <summary>
    /// Start new BGM
    /// </summary>
    /// <param name="introClip"></param>
    /// <returns></returns>
    public IEnumerator PlayBGM(AudioClip introClip, AudioClip loopClip)
    {
        audioSource.clip = introClip;
        audioSource.loop = false;
        audioSource.Play();

        StartCoroutine(LoopBGM(loopClip)); // Cue the loop BGM

        // Fade in the next sound track
        for (float t = 0; t < 1; t += Time.deltaTime / fadeInDuration)
        {
            audioSource.volume = Mathf.Lerp(0, defaultVolume, t);
            yield return null;
        }
        audioSource.volume = defaultVolume;
    }

    /// <summary>
    /// Change the bgm
    /// </summary>
    /// <param name="introClip"></param>
    /// <returns></returns>
    public IEnumerator ChangeBGM(AudioClip introClip, AudioClip loopClip)
    {
        // Fade out the previous sound track
        for (float t = 0; t < 1; t += Time.deltaTime / fadeOutDuration)
        {
            audioSource.volume = Mathf.Lerp(defaultVolume, 0, t);
            yield return null;
        }
        audioSource.volume = 0;

        yield return new WaitForSecondsRealtime(timeTillFadeIn); // Wait for the next bgm

        audioSource.Stop(); // Stop the current BGM

        // Start the next BGM
        StartCoroutine(PlayBGM(introClip, loopClip));
    }

    /// <summary>
    /// Play the loop BGM
    /// </summary>
    /// <param name="clip"></param>
    /// <returns></returns>
    public IEnumerator LoopBGM(AudioClip clip)
    {
        yield return new WaitForSecondsRealtime(audioSource.clip.length); // Wait for the intro to finish
        //print("real time: " + Time.realtimeSinceStartup + ", game time: " + Time.time + ", " + audioSource.clip.length);

        audioSource.Stop();
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Function to start playing the beginning BGM music. Begins with the intro and goes into the loopable music.
    void PlayBeginningBGM()
    {
        audioSource.Stop();
        audioSource.clip = beginningBGMIntro;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(PlayBeginningBGMLoop());

    }

    IEnumerator PlayBeginningBGMLoop()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        //yield return new WaitForSeconds(53.5f);

        audioSource.Stop();
        audioSource.clip = beginningBGMLoop;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Function to start playing the tutorial BGM music. Begins with the intro and goes into the loopable music.
    void PlayTutorialBGM()
    {
        audioSource.Stop();
        audioSource.clip = tutorialBGMIntro;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(PlayTutorialBGMLoop());

    }

    IEnumerator PlayTutorialBGMLoop()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        //yield return new WaitForSeconds(53.5f);

        audioSource.Stop();
        audioSource.clip = tutorialBGMLoop;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Function to start playing the final BGM music. Begins with the intro and goes into the loopable music.
    void PlayFinalBGM()
    {
        audioSource.Stop();
        audioSource.clip = finalBGMIntro;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(PlayFinalBGMLoop());

    }

    IEnumerator PlayFinalBGMLoop()
    {
        yield return new WaitForSeconds(audioSource.clip.length);
        //yield return new WaitForSeconds(53.5f);

        audioSource.Stop();
        audioSource.clip = finalBGMLoop;
        audioSource.loop = true;
        audioSource.Play();
    }

    // Function to start playing the alley BGM music. Begins with the intro and goes into the loopable music.
    void PlayAlleyBGM()
    {
        audioSource.Stop();
        audioSource.clip = alleyBGMIntro;
        audioSource.loop = false;
        audioSource.Play();
        StartCoroutine(PlayAlleyBGMLoop());

    }

    IEnumerator PlayAlleyBGMLoop()
    {
        yield return new WaitForSeconds(audioSource.clip.length);

        audioSource.Stop();
        audioSource.clip = alleyBGMLoop;
        audioSource.loop = true;
        audioSource.Play();
    }
}
