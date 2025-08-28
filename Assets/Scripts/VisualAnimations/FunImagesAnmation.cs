using UnityEngine;

public class FunImagesAnmation : MonoBehaviour
{
    private float delayBetweenAnmations = 0.2f;
    private Animator[] animators;
    private void Awake()
    {
        animators = new Animator[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            animators[i] = transform.GetChild(i).GetComponent<Animator>();
            animators[i].speed = 0;
        }
    }
    private void OnEnable()
    {
        StartCoroutine(PlayWithDelay());
    }
    void OnDisable()
{
    foreach (var anim in animators)
        anim.speed = 0; // pause all
}
    System.Collections.IEnumerator PlayWithDelay()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            animators[i].speed = 1;
            yield return new WaitForSeconds(delayBetweenAnmations);
        }
    }
}
