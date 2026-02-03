using Boss.VFX;
using UnityChan.Combat;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boss.Skills
{
	public class BossBasicSkillProjectile : MonoBehaviour
	{
		public float damage = 25;
		private new Rigidbody rigidbody;
		private Transform model;
		private Vector3 rotDir;

		private void Awake()
		{
			rigidbody = GetComponent<Rigidbody>();
			model = transform.GetChild(0);
		}

		private void OnEnable()
		{
			rotDir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 60;
			rigidbody.useGravity = false;
			rigidbody.isKinematic = true;
		}

		private void Update()
		{
			model.Rotate(rotDir * Time.deltaTime);
		}

		public void Activate()
		{
			rigidbody.isKinematic = false;
			rigidbody.useGravity = true;
		}

		private void OnCollisionEnter(Collision other)
		{
			if (other.gameObject.CompareTag("Player"))
			{
				other.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage, gameObject);
			}
			else
			{
				Collider[] hits = Physics.OverlapSphere(transform.position, 1f);
				foreach (var col in hits)
				{
					if (col.TryGetComponent<IDamageable>(out var damageable))
					{
						damageable.TakeDamage(damage, gameObject);
					}
				}
			}

			BossVFXManager.Instance.Spawn(VFXType.GroundDustExplosion, transform.position, Quaternion.identity);
			gameObject.SetActive(false);
		}
	}
}