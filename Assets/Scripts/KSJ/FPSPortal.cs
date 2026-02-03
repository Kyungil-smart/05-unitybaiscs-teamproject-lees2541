using UnityEngine;
using UnityEngine.SceneManagement;

public class FPSPortal : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			SceneManager.LoadScene("SideViewScene");
		}
	}
}