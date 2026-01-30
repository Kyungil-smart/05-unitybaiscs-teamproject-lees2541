using UnityEngine;
using UnityEngine.Events;

public class DefaultInteractable : MonoBehaviour, IInteract
{
	[field: SerializeField] public UnityEvent Interacted { get; set; }
	[field: SerializeField] public UnityEvent PlayerLockStarted { get; set; }
	[field: SerializeField] public UnityEvent PlayerLockEnded { get; set; }

	public void OnInteractFocusEnter()
	{
		PlayerLockStarted?.Invoke();
	}

	public void OnInteractFocusExit()
	{
		PlayerLockEnded?.Invoke();
	}
}