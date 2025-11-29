using UnityEngine;
using System.Collections;

public class Plant : MonoBehaviour
{
    [SerializeField] private float growDuration = 2.0f;
    [SerializeField] private Vector3 targetScale = Vector3.one;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(GrowRoutine());
    }

    private IEnumerator GrowRoutine()
    {
        float timer = 0f;
        while (timer < growDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / growDuration;
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, progress);
            yield return null;
        }
        transform.localScale = targetScale;
    }
}
