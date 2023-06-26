using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoryBoard : MonoBehaviour
{
    int currentSlide = 0;
    [SerializeField] Image[] storySlides;
    [SerializeField] GameObject tutorialGO;

    IEnumerator GoThroughSlides() {

        for(int i = 0; i < storySlides.Length; i++) {
            yield return new WaitForSeconds(3);
            yield return StartCoroutine(FadeSlide(storySlides[i], 1f));
        }


        Image backGround = GetComponent<Image>();
        Color initialColor = backGround.color;
        Color fadeToColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);

        float j = 1f;
        while (j > 0) {
            backGround.color = Color.Lerp(fadeToColor,initialColor, j / 1f);
            yield return null;
            j -= Time.deltaTime;
        }
        tutorialGO.SetActive(true);
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
