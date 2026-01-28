using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class BossVFX : MonoBehaviour
{
	private static YieldInstruction lifeYield1s = new WaitForSeconds(1f);

	private IObjectPool<GameObject> pool;
	private VFXType vfxType;
	private ParticleSystem ps;
	private Coroutine lifeCoroutine;

	private bool isInitialized;
	private float lifetimeLimit = 3f;

	public void Initialize(VFXType type, IObjectPool<GameObject> pool)
	{
		vfxType = type;
		this.pool = pool;
		ps = GetComponent<ParticleSystem>();
		isInitialized = true;
	}

	void OnEnable()
	{
		if (!isInitialized) gameObject.SetActive(false);
		if (ps != null)
		{
			ps.Play();
			lifeCoroutine = StartCoroutine(LifeCoroutine());
		}
	}

	private IEnumerator LifeCoroutine()
	{
		float time = 0;
		while (gameObject.activeSelf)
		{
			if (!ps.IsAlive() || lifetimeLimit <= time) break;
			time += 1;
			yield return lifeYield1s;
		}

		lifeCoroutine = null;
		ReturnToPool();
	}

	private void OnDisable()
	{
		if (lifeCoroutine != null)
		{
			StopCoroutine(lifeCoroutine);
			lifeCoroutine = null;
		}
	}

	private void ReturnToPool()
	{
		pool.Release(gameObject);
	}
}