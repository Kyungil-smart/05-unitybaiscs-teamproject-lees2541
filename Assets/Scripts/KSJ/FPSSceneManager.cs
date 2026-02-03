using System.Collections;
using UnityEngine;

public class FPSSceneManager : MonoBehaviour
{
    [SerializeField] private GameObject portal;
    [SerializeField] private int requiredFindCount = 4;

    private static int foundObjectCount;

    private void Start()
    {
        StartCoroutine(Loop());
    }

    IEnumerator Loop()
    {
        while (foundObjectCount < requiredFindCount)
        {
            yield return new WaitForFixedUpdate();
        }

        portal.SetActive(true);
    }

    public static void OnDifferentObjectFound()
    {
        foundObjectCount++;
    }
}