using UnityEngine;
using UnityEngine.Events;

public class FPSDifferentObject : MonoBehaviour, IInteract
{
	[field: SerializeField] public UnityEvent Interacted { get; set; }
	private GameObject destroyEffectPrefab;

	private void Start()
	{
		destroyEffectPrefab = Resources.Load<GameObject>("VFX_ObjectDestroy");
		Interacted.AddListener(OnInteracted);
	}

	void OnInteracted()
	{
		// 이펙트 생성
		ParticleSystem effect = Instantiate(destroyEffectPrefab, transform.position, Quaternion.identity)
			.GetComponent<ParticleSystem>();
		Destroy(effect.gameObject, 3); // 이펙트 Destroy 예약

		FPSSceneManager.OnDifferentObjectFound();

		Destroy(gameObject);
	}
}