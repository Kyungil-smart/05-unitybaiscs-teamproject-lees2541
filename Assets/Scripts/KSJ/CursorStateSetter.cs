using UnityEngine;

public class CursorStateSetter : MonoBehaviour
{
	private void Start()
	{
		CursorLock(true);
	}

	private void CursorLock(bool isLocked)
	{
		Cursor.lockState = isLocked ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !isLocked;
	}
}