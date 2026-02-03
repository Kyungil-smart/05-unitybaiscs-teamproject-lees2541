using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class FPSSceneManager : MonoBehaviour
{
	[SerializeField] private GameObject portal;
	[SerializeField] private int requiredFindCount = 4;
	[SerializeField] private TextUIPanel mainTitle;
	[SerializeField] private TextUIPanel missionTitle;
	[SerializeField] private TextUIPanel missionClearTitle;

	private static int foundObjectCount;
	private static Action countChanged;

	private void Start()
	{
		CheckFoundObjectCountAsync().Forget();
		countChanged += OnCountChanged;
		mainTitle.ActivateOnce();
		missionTitle.Activate();

		countChanged?.Invoke();
	}

	private void OnDestroy()
	{
		countChanged -= OnCountChanged;
	}

	void OnCountChanged()
	{
		missionTitle.SetText($"{foundObjectCount}/{requiredFindCount}");
	}

	async UniTask CheckFoundObjectCountAsync()
	{
		await UniTask.WaitUntil(() => foundObjectCount >= requiredFindCount);
		portal.SetActive(true);
		missionTitle.Deactivate();
		missionClearTitle.ActivateOnce();
		GetComponent<AudioSource>().Play();
	}

	public static void OnDifferentObjectFound()
	{
		foundObjectCount++;
		countChanged?.Invoke();
	}
}