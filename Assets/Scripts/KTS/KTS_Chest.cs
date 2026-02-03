using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class KTS_Chest : MonoBehaviour, IInteract
{
	[field: SerializeField] public UnityEvent Interacted { get; set; }
	public GameObject compass;
	public Transform chestDoor;

	private bool interacted = false;

	private void Start()
	{
		compass.SetActive(false);
		Interacted.AddListener(OnInteracted);
	}

	private void OnInteracted()
	{
		if (interacted) return;

		if (KTS_QuarterSceneManager.GameCleared)
		{
			interacted = true;
			StartCoroutine(OpenChest());
		}
	}

	[ContextMenu("Open Chest")]
	public void Test()
	{
		StartCoroutine(OpenChest());
	}

	IEnumerator OpenChest()
	{
		compass.SetActive(true);
		float time = 1;
		while (time > 0)
		{
			time -= Time.deltaTime;
			chestDoor.Rotate(Vector3.right * (Time.deltaTime * -130), Space.Self);
			yield return null;
		}
	}
}