using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TextUIPanel : MonoBehaviour
{
	private CanvasGroup _canvasGroup;
	private TextMeshProUGUI _text;

	private void Awake()
	{
		Init();
	}

	void Init()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
		_text = GetComponentInChildren<TextMeshProUGUI>();
	}

	public void ActivateOnce()
	{
		Fade().Forget();
	}

	public void Activate()
	{
		FadeIn().Forget();
	}

	public void Deactivate()
	{
		FadeOut().Forget();
	}

	public void SetText(string description)
	{
		_text.text = description;
	}

	async UniTaskVoid Fade()
	{
		await FadeIn();
		await UniTask.Delay(TimeSpan.FromSeconds(3));
		await FadeOut();
	}

	async UniTask FadeIn()
	{
		_canvasGroup.alpha = 0f;
		gameObject.SetActive(true);
		await Tween.Alpha(_canvasGroup, 1f, 1f, ease: Ease.OutCirc);
	}

	async UniTask FadeOut()
	{
		await Tween.Alpha(_canvasGroup, 0f, .3f, ease: Ease.OutCirc);
		gameObject.SetActive(false);
	}
}