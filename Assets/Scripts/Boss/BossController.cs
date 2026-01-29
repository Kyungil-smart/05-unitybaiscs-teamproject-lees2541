using System.Collections;
using System.Collections.Generic;
using Boss.Skills;
using UnityEngine;
using UnityEngine.Events;

namespace Boss
{
	public class BossController : MonoBehaviour
	{
		public UnityEvent<BossSkillType> AttackStarted;
		public UnityEvent<BossSkillType> AttackPerformed;
		public UnityEvent<BossSkillType> AttackEnded;

		private readonly Dictionary<BossSkillType, IBossSkill> _skills = new();
		private Coroutine currentCoroutine;
		private IBossSkill currentSkill;
		BossSkillType currentSkillType;

		private void Awake()
		{
			RegisterSkillComponents();
		}

		private void RegisterSkillComponents()
		{
			_skills.Add(BossSkillType.BasicCast, GetComponent<BossBasicSkill>());
		}

		public void CastSkill(BossSkillType skillType)
		{
			if (currentCoroutine != null)
			{
				StopCoroutine(currentCoroutine);
			}

			currentSkillType = skillType;
			currentSkill = _skills[currentSkillType];
			if (currentSkill == null)
			{
				Debug.LogError($"Skill {currentSkillType} not found");
				return;
			}

			currentCoroutine = StartCoroutine(SkillCoroutine());
		}

		private IEnumerator SkillCoroutine()
		{
			AttackStarted?.Invoke(currentSkillType);
			yield return currentSkill.StartAttack();

			AttackPerformed?.Invoke(currentSkillType);
			yield return currentSkill.PerformAttack();

			AttackEnded?.Invoke(currentSkillType);
			yield return currentSkill.EndAttack();

			currentSkill = null;
			currentCoroutine = null;
		}
	}
}