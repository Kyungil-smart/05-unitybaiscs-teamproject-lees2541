using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class BossVFXManager : MonoBehaviour
{
	[System.Serializable]
	public struct PoolConfig
	{
		public VFXType vfxType;
		public GameObject prefab;
	}

	public static BossVFXManager Instance { get; private set; }

	[SerializeField] private List<PoolConfig> poolConfigs;

	private Dictionary<VFXType, IObjectPool<GameObject>> pools = new();
	private Dictionary<VFXType, GameObject> prefabs = new();

	private void Awake()
	{
		Instance = this;

		foreach (var config in poolConfigs)
		{
			if (config.vfxType == VFXType.None) continue;
			prefabs.Add(config.vfxType, config.prefab);

			var pool = new ObjectPool<GameObject>(
				createFunc: () => CreateNewObject(config.vfxType),
				actionOnGet: OnGetFromPool,
				actionOnRelease: OnReleaseToPool,
				actionOnDestroy: OnDestroyObject
			);
			pools.Add(config.vfxType, pool);

			// 미리 로딩
			GameObject[] prewarmArray = new GameObject[6];
			for (int i = 0; i < 6; i++)
			{
				prewarmArray[i] = pool.Get();
			}

			for (int i = 0; i < 6; i++)
			{
				pool.Release(prewarmArray[i]);
			}
		}
	}

	private GameObject CreateNewObject(VFXType type)
	{
		GameObject obj = Instantiate(prefabs[type], transform);
		var pooledObj = obj.GetComponent<BossVFX>();
		pooledObj.Initialize(type, pools[type]);
		return obj;
	}

	private void OnGetFromPool(GameObject obj) => obj.SetActive(true);
	private void OnReleaseToPool(GameObject obj) => obj.SetActive(false);
	private void OnDestroyObject(GameObject obj) => Destroy(obj);

	public void Spawn(VFXType type, Vector3 position, Quaternion rotation)
	{
		if (!pools.ContainsKey(type)) return;

		GameObject obj = pools[type].Get();
		obj.transform.SetPositionAndRotation(position, rotation);
	}
}