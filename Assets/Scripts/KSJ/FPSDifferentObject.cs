using System;
using UnityEngine;
using UnityEngine.Events;

public class FPSDifferentObject : MonoBehaviour, IInteract
{
	[field: SerializeField] public UnityEvent Interacted { get; set; }

	private GameObject destroyEffectPrefab;
	private Outline outline;

	private void Awake()
	{
		outline = GetComponent<Outline>();
	}

	private void Start()
	{
		destroyEffectPrefab = Resources.Load<GameObject>("VFX_ObjectDestroy");
		Interacted.AddListener(OnInteracted);
	}

	void OnInteracted()
	{
		if (!enabled || !gameObject.activeSelf) return;
		// 이펙트 생성
		ParticleSystem effect = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity)
			.GetComponent<ParticleSystem>();
		Destroy(effect.gameObject, 3); // 이펙트 Destroy 예약

		FPSSceneManager.OnDifferentObjectFound();

		gameObject.SetActive(false);
	}


	/// <summary>
	/// 플레이어가 해당 오브젝트를 주시하고 있을 때(타깃 설정) 호출될 메소드 (선택)
	/// </summary>
	public void OnInteractFocusEnter()
	{
		outline.enabled = true;
	}

	/// <summary>
	/// 플레이어가 해당 오브젝트를 더 이상 주시하지 않을 때(타깃 X) 호출될 메소드 (선택)
	/// </summary>
	public void OnInteractFocusExit()
	{
		outline.enabled = false;
	}
}