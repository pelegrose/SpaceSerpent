using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ReplayButtonScript : MonoBehaviour

{
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float maxVolume;
    [SerializeField] private AudioSource buttonClick;
    [SerializeField] private AudioSource buttonHover;
    [SerializeField] private GameObject playButton;
    //    private EventTrigger.Entry entry;



    // Start is called before the first frame update

    void Start()
    {
        StartCoroutine(StartFade(GetComponent<AudioSource>(), fadeInDuration, maxVolume));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            playButton.GetComponent<Animator>().SetTrigger("PressedPlay");
        }
    }


    public static IEnumerator StartFade(AudioSource audioSource, float duration, float targetVolume)
    {
        float currentTime = 0;
        float start = audioSource.volume;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            audioSource.volume = Mathf.Lerp(start, targetVolume, currentTime / duration);
            yield return null;
        }
        yield break;
    }


    public void OnMouseEnter(BaseEventData eventData)
    {
        playButton.GetComponent<Animator>().SetBool("onHover", true);
        buttonHover.Play();
        Debug.Log("entered");
    }

    public void OnMouseExit(BaseEventData eventData)
    {
        playButton.GetComponent<Animator>().SetBool("onHover", false);
        Debug.Log("exited");
    }

    public void OnMouseClick(BaseEventData eventData)
    {
        playButton.GetComponent<Animator>().SetTrigger("PressedPlay");
        buttonClick.Play();
        Debug.Log("clicked");
        SceneManager.LoadScene("title");
    }
}
