using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSSceneManager : MonoBehaviour
{
	[SerializeField] private GameObject portal;
	[SerializeField] private int requiredFindCount = 4;
	[SerializeField] private TextUIPanel mainTitle;
	[SerializeField] private TextUIPanel missionTitle;
	[SerializeField] private TextUIPanel missionClearTitle;

	private static int foundObjectCount;
	private static Action countChanged;
	public Transform playerObject;


	private void Start()
	{
		foundObjectCount = 0;
		CheckFoundObjectCountAsync().Forget();
		countChanged += OnCountChanged;
		mainTitle.ActivateOnce();
		missionTitle.Activate();

		countChanged?.Invoke();
	}

	private void Update()
	{
		if (playerObject?.position.y < -5)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
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