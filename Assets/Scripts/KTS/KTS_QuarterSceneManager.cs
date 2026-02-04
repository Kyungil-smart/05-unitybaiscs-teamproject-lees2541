using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using UnityChan;
using UnityChan.Combat;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KTS_QuarterSceneManager : MonoBehaviour
{
	public static bool GameCleared;
	private static int foundKeyCount;
	private static Action changed;

	[SerializeField] private PlayerController playerController;
	[SerializeField] private int requiredKeyCount = 6;
	[SerializeField] private TextUIPanel startTitle;
	[SerializeField] private TextUIPanel missionTest;
	[SerializeField] private TextUIPanel objectTitle;

	private void Start()
	{
		foundKeyCount = 0;
		playerController.GetComponent<HealthSystem>().OnDeath +=
			() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		changed = UpdateUI;

		startTitle.ActivateOnce();
		missionTest.Activate();

		CheckKey().Forget();
		changed?.Invoke();
	}

	private void UpdateUI()
	{
		missionTest.SetText($"{foundKeyCount}/{requiredKeyCount}");
	}

	private async UniTaskVoid CheckKey()
	{
		await UniTask.WaitUntil(() => foundKeyCount >= requiredKeyCount);

		GameCleared = true;
		objectTitle.ActivateOnce();
		missionTest.Deactivate();
	}

	public static void FoundKey()
	{
		foundKeyCount++;
		changed?.Invoke();
	}
}