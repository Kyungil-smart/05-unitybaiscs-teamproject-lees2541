using UnityEngine;
using Random = UnityEngine.Random;

public class BossBasicSkillProjectile : MonoBehaviour
{
	private Rigidbody rigidbody;
	private Transform model;
	private Vector3 rotDir;

	private void Awake()
	{
		rigidbody = GetComponent<Rigidbody>();
		model = transform.GetChild(0);
		rotDir = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 60;
	}

	private void Update()
	{
		model.Rotate(rotDir * Time.deltaTime);
	}

	public void Activate()
	{
		rigidbody.useGravity = true;
	}

	private void OnCollisionEnter(Collision other)
	{
		gameObject.SetActive(false);
	}
}