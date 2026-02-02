using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


public class BossSceneManager : MonoBehaviour
{
	[SerializeField] private Transform mapMinimumPoint;
	[SerializeField] private Transform mapMaximumPoint;
	[SerializeField] private List<BossCameraInfo> _cameras;
	[SerializeField] private PlayerController _player;

	[System.Serializable]
	public struct BossCameraInfo
	{
		public enum BossCameraType
		{
			BossTop,
			PlayerTop
		}

		public BossCameraType cameraType;
		public CinemachineVirtualCameraBase camera;
	}

	public static BossSceneManager Instance { get; private set; }

	public Vector3 MinCoordinate => mapMinimumPoint.position;
	public Vector3 MaxCoordinate => mapMaximumPoint.position;

	private void Awake()
	{
		if (Instance != null) Destroy(Instance.gameObject);
		Instance = this;
	}

	private void OnDestroy()
	{
		Instance = null;
	}

	public void SetCamera(BossCameraInfo.BossCameraType cameraType)
	{
		foreach (var bossCameraInfo in _cameras)
		{
			bossCameraInfo.camera.Priority = bossCameraInfo.cameraType == cameraType ? 10 : 0;
		}
	}
}