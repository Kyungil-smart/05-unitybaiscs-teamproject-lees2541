using UnityEngine;

public class KTS_CameraController : MonoBehaviour
{
	[SerializeField] private Vector3 cameraOffset;
	[SerializeField] private Transform target;
	private Transform cameraTransform;

	private void Start()
	{
		cameraTransform = Camera.main.transform;
	}

	private void LateUpdate()
	{
		if (target == null) return;
		cameraTransform.position = target.position + cameraOffset;
	}
}