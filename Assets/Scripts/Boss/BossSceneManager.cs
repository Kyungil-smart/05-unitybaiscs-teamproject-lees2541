using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityChan;
using UnityChan.Combat;

namespace Boss
{
	public class BossSceneManager : MonoBehaviour
	{
		[Header("World")] [SerializeField] private Transform mapMinimumPoint;
		[SerializeField] private Transform mapMaximumPoint;
		[SerializeField] private List<BossCameraInfo> cameras;
		[SerializeField] private GameObject floor;

		[Header("Entity")] [SerializeField] private PlayerController player;
		[SerializeField] private BossBrain boss;

		[Header("UI")] [SerializeField] private Slider hpSlider;
		[SerializeField] private GameObject endPanel;
		[SerializeField] private CanvasGroup gameCanvasGroup;
		[SerializeField] private TextMeshProUGUI clearTimeText;

		[Serializable]
		public struct BossCameraInfo
		{
			public enum BossCameraType
			{
				BossTop,
				PlayerTop,
				BossDie,
				PlayerQuarter
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
			SetCamera(BossCameraInfo.BossCameraType.PlayerQuarter);
			player.GetComponent<HealthSystem>().OnDeath +=
				() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		private void Update()
		{
			if (player.transform.position.y < -50) SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
		}

		private void OnDestroy()
		{
			Instance = null;
		}

		public void GameEnd()
		{
			endPanel.SetActive(true);

			var startedTime = PlayerPrefs.GetFloat("StartTime");
			var clearTime = Time.time - startedTime;
			clearTimeText.text = $"클리어 시간: {(clearTime / 60):00}분 {(clearTime % 60):F2}\n";

			float bestTime = float.MaxValue;
			if (PlayerPrefs.HasKey("BestTime"))
			{
				bestTime = PlayerPrefs.GetFloat("BestTime");
				clearTimeText.text += $"최고 기록: {(bestTime / 60):00}분 {(bestTime % 60):F2}\n";
			}

			if (clearTime < bestTime)
			{
				clearTimeText.text += "신기록 갱신!";
				PlayerPrefs.SetFloat("BestTime", clearTime);
			}
		}

		public void SetCamera(BossCameraInfo.BossCameraType cameraType)
		{
			foreach (var bossCameraInfo in cameras)
			{
				bossCameraInfo.camera.Priority = bossCameraInfo.cameraType == cameraType ? 10 : 0;
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				if (boss.enabled) return;
				StartCoroutine(StartGame());
				GetComponent<Collider>().enabled = false;
			}
		}

		private IEnumerator StartGame()
		{
			player.gameObject.SetActive(false);
			SetCamera(BossCameraInfo.BossCameraType.BossDie);
			yield return new WaitForSeconds(2f);
			Destroy(floor);

			float time = 1;
			while (time > 0)
			{
				time -= Time.deltaTime;
				gameCanvasGroup.alpha = 1 - time;
				yield return null;
			}

			SetCamera(BossCameraInfo.BossCameraType.PlayerTop);
			yield return new WaitForSeconds(1f);

			player.gameObject.SetActive(true);
			gameCanvasGroup.alpha = 1;
			boss.enabled = true;
		}

		public void OnHealthChanged(float current, float max)
		{
			hpSlider.value = current / max;
		}
	}
}