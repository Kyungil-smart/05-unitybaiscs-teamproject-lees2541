using System;
using System.Collections.Generic;
using Boss;
using Cinemachine;
using TMPro;
using UnityChan;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BossSceneManager : MonoBehaviour
{
	[Header("World")] [SerializeField] private Transform mapMinimumPoint;
	[SerializeField] private Transform mapMaximumPoint;
	[SerializeField] private List<BossCameraInfo> _cameras;
	[SerializeField] private PlayerController _player;

	[Header("UI")] [SerializeField] private Slider hpSlider;
	[SerializeField] private GameObject endPanel;
	[SerializeField] private TextMeshProUGUI clearTimeText;

	[Serializable]
	public struct BossCameraInfo
	{
		public enum BossCameraType
		{
			BossTop,
			PlayerTop,
			BossDie
		}

		public BossCameraType cameraType;
		public CinemachineVirtualCameraBase camera;
	}

	public static BossSceneManager Instance { get; private set; }

	public Vector3 MinCoordinate => mapMinimumPoint.position;
	public Vector3 MaxCoordinate => mapMaximumPoint.position;

	public void OnHpChanged(int current) => hpSlider.value = (float)current / BossStat.defaultHP;
	public void OnEndBtnClicked() => SceneManager.LoadScene(0);

	private void Awake()
	{
		if (Instance != null) Destroy(Instance.gameObject);
		Instance = this;
	}

	private void Start()
	{
		SetCamera(BossCameraInfo.BossCameraType.PlayerTop);
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void GameEnd()
	{
		endPanel.SetActive(true);

		float defaultBest = 0;
		var clearTime = 532.432f; // TODO
		float bestTime = float.MaxValue;
		clearTimeText.text = $"클리어 시간: {(clearTime / 60):00}분 {(clearTime % 60):F2}\n";

		if (PlayerPrefs.HasKey("Time"))
		{
			bestTime = PlayerPrefs.GetFloat("Time");
			clearTimeText.text += $"최고 기록: {(bestTime / 60):00}분 {(bestTime % 60):F2}\n";
			if (clearTime < bestTime) clearTimeText.text += "신기록 갱신!";
		}

		if (clearTime < bestTime)
		{
			PlayerPrefs.SetFloat("Time", clearTime);
		}
	}

	public void SetCamera(BossCameraInfo.BossCameraType cameraType)
	{
		foreach (var bossCameraInfo in _cameras)
		{
			bossCameraInfo.camera.Priority = bossCameraInfo.cameraType == cameraType ? 10 : 0;
		}
	}
}