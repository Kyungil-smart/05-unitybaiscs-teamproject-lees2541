using UnityEngine;
using UnityChan;

public class Side_RespawnController : MonoBehaviour
{
	private GameObject[] items;
	public Vector3 respawnPos = new Vector3(0, 3, 4);

	private void Start()
	{
		items = GameObject.FindGameObjectsWithTag("AddJumpChance");
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			Debug.Log("Dead!");

			PlayerController SetPosition = other.GetComponent<PlayerController>();
			if (SetPosition != null)
			{
				SetPosition.enabled = false;
				other.transform.position = respawnPos;
				SetPosition.enabled = true;
			}
			else
			{
				other.transform.position = respawnPos;
			}

			RespawnItem();
		}
	}

	private void RespawnItem()
	{
		Debug.Log("Respawn Item");
		foreach (var item in items)
		{
			if (item != null)
				item.SetActive(true);
		}
	}
}