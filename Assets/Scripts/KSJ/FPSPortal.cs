using UnityEngine;

public class FPSPortal : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			LevelLoader.Instance.LoadLevel("SideViewScene");
		}
	}
}