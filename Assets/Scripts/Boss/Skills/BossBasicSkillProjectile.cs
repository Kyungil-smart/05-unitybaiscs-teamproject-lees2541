using System;
using Boss.VFX;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Boss.Skills
{
	public class BossBasicSkillProjectile : MonoBehaviour
	{
		private Rigidbody rigidbody;
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
			BossVFXManager.Instance.Spawn(VFXType.GroundDustExplosion, transform.position, Quaternion.identity);
			gameObject.SetActive(false);
		}
	}
}