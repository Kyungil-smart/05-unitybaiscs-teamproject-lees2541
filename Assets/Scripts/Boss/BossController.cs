using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class BossController : MonoBehaviour
{
	public UnityEvent<BossSkillType> AttackStarted;
	public UnityEvent<BossSkillType> AttackPerformed;
	public UnityEvent<BossSkillType> AttackEnded;
	public bool EnableDebugUI = true;

	private readonly Dictionary<BossSkillType, IBossSkill> _skills = new();
	private Coroutine skillCoroutine;
	private IBossSkill skill;

	private void Awake()
	{
		_skills.Add(BossSkillType.BasicCast, GetComponent<BossBasicSkill>());
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			BossVFXManager.Instance.Spawn(VFXType.GroundDustExplosion,
				new Vector3(Random.Range(-5, 5), 0.01f, Random.Range(-5, 5)), Quaternion.identity);
		}
	}

	public void CastBasicAttack()
	{
		if (skillCoroutine != null)
		{
			StopCoroutine(skillCoroutine);
		}

		skillCoroutine = StartCoroutine(BasicAttackCoroutine());
	}

	IEnumerator BasicAttackCoroutine()
	{
		var skillType = BossSkillType.BasicCast;
		skill = _skills[skillType];

		AttackStarted?.Invoke(skillType);
		yield return skill.StartAttack();

		AttackPerformed?.Invoke(skillType);
		yield return skill.PerformAttack();

		AttackEnded?.Invoke(skillType);
		yield return skill.EndAttack();

		skillCoroutine = null;
		skill = null;
	}

	private void OnGUI()
	{
		if (!EnableDebugUI) return;
		if (skillCoroutine != null) return;

		if (GUILayout.Button("Attack(Basic)"))
		{
			CastBasicAttack();
		}
	}
}