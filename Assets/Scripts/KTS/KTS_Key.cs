using UnityEngine;

public class KTS_Key : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		KTS_QuarterSceneManager.FoundKey();
		Destroy(gameObject);
	}
}