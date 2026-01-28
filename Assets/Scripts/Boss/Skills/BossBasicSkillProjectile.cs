using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BossBasicSkillProjectile : MonoBehaviour
{
	private Vector3 rotDir;
	private Transform model;

	private void Start()
	{
		model = transform.GetChild(0);
		rotDir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 60;
	}

	private void Update()
	{
		model.Rotate(rotDir * Time.deltaTime);
	}

	[ContextMenu("Test")]
	public void Activate()
	{
		GetComponent<Rigidbody>().useGravity = true;
	}

	private void OnCollisionEnter(Collision other)
	{
		gameObject.SetActive(false);
	}
}