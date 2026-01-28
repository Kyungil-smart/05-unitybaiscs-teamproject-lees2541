using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossController : MonoBehaviour
{
	public bool EnableDebugUI = true;

	public UnityEvent<BossSkillType> AttackStarted;
	public UnityEvent<BossSkillType> AttackPerformed;
	public UnityEvent<BossSkillType> AttackEnded;

	private Coroutine skillCoroutine;
	private IBossSkill skill;

	private readonly Dictionary<BossSkillType, IBossSkill> _skills = new();

	private void Awake()
	{
		_skills.Add(BossSkillType.BasicCast, GetComponent<BossBasicSkill>());
	}

	IEnumerator CastBasicAttack()
	{
		skill = _skills[BossSkillType.BasicCast];
		
		AttackStarted?.Invoke(BossSkillType.BasicCast);
		yield return skill.StartAttack();
	}

	private void OnGUI()
	{
		if (!EnableDebugUI) return;
		if (skillCoroutine != null) return;

		if (GUILayout.Button("Attack(Basic)"))
		{
			skillCoroutine = StartCoroutine(CastBasicAttack());
		}
	}
}