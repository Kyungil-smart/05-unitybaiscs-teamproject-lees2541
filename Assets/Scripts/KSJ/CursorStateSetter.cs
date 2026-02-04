using System;
using UnityEngine;

public class CursorStateSetter : MonoBehaviour
{
	private static CursorStateSetter instance;
	private bool isLocked = true;

	private void Awake()
	{
		if (instance != null) Destroy(gameObject);
		else
		{
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}

	private void Start()
	{
		CursorLock(isLocked);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftAlt))
		{
			CursorLock(!isLocked);
			isLocked = !isLocked;
		}
	}

	private void CursorLock(bool locked)
	{
		Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
		Cursor.visible = !locked;
	}
}