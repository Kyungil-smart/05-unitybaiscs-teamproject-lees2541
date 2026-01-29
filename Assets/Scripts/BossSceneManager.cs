using UnityEngine;

public class BossSceneManager : MonoBehaviour
{
	[SerializeField] private Transform mapMinimumPoint;
	[SerializeField] private Transform mapMaximumPoint;

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
}