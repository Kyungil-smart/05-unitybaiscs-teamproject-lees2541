using UnityEngine;
using UnityEngine.Events;

public interface IInteract
{
	public UnityEvent Interacted { get; set; }

	public void Interact()
	{
		Interacted?.Invoke();
	}
}
