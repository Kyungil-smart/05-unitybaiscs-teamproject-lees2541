using System.Collections;
using UnityChan;
using UnityChan.Combat;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KTS_QuarterSceneManager : MonoBehaviour
{
	public static bool GameCleared = false;
	private static int foundKeyCount = 0;

	[SerializeField] private PlayerController playerController;
	[SerializeField] private int requiredKeyCount = 6;

	private void Start()
	{
		playerController.GetComponent<HealthSystem>().OnDeath +=
			() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

		StartCoroutine(Loop());
	}

	private IEnumerator Loop()
	{
		while (foundKeyCount < requiredKeyCount)
		{
			yield return null;
		}

		GameCleared = true;
	}

	public static void FoundKey()
	{
		foundKeyCount++;
	}
}