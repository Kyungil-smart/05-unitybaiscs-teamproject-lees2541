using UnityEngine;

public class Side_SpringController : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Rigidbody rb = other.GetComponent<Rigidbody>();
			rb.velocity = Vector3.zero;
			rb.AddForce(Vector3.up * 225, ForceMode.Impulse);
		}
	}
}