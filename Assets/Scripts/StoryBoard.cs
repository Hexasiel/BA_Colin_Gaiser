using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryBoard : MonoBehaviour
{
    int currentSlide = 0;
    [SerializeField] Image[] storySlides;

    IEnumerator GoThroughSlides() {

        for(int i = 0; i < storySlides.Length; i++) {
            yield return new WaitForSeconds(3);
            yield return StartCoroutine(FadeSlide(storySlides[i], 1f));
        }
        gameObject.SetActive(false);
    }

    IEnumerator FadeSlide(Image slide, float time) {
        float i = time;
        while(i > 0) {
            slide.color = new Color(1, 1, 1, i / time);
            yield return null;
            i -= Time.deltaTime;
        }
    }

    private void Start() {
        StartCoroutine(GoThroughSlides());
    }
}
