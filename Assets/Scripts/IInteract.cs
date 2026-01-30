using UnityEngine;
using UnityEngine.Events;

public interface IInteract
{
	public UnityEvent Interacted { get; set; }

	/// <summary>
	/// 플레이어가 오브젝트에 상호작용 시도 시 호출하는 메서드
	/// </summary>
	public void Interact()
	{
		Interacted?.Invoke();
	}

	/// <summary>
	/// 플레이어가 해당 오브젝트를 주시하고 있을 때(타깃 설정) 호출될 메소드 (선택)
	/// </summary>
	public void OnInteractFocusEnter()
	{
	}

	/// <summary>
	/// 플레이어가 해당 오브젝트를 더 이상 주시하지 않을 때(타깃 X) 호출될 메소드 (선택)
	/// </summary>
	public void OnInteractFocusExit()
	{
	}
}