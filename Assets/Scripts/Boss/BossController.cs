using System.Collections;
using System.Collections.Generic;
using Boss.Skills;
using UnityEngine;
using UnityEngine.Events;

namespace Boss
{
	public class BossController : MonoBehaviour
	{
		public bool IsCasting { get; private set; }

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
			var components = GetComponents<IBossSkill>();
			foreach (var component in components)
			{
				_skills.Add(component.Type, component);
			}
		}

		public void CastSkill(BossSkillType skillType)
		{
			if (IsCasting) return;

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
			IsCasting = true;
			AttackStarted?.Invoke(currentSkillType);
			yield return currentSkill.StartAttack();

			AttackPerformed?.Invoke(currentSkillType);
			yield return currentSkill.PerformAttack();

			AttackEnded?.Invoke(currentSkillType);
			yield return currentSkill.EndAttack();

			currentSkill = null;
			currentCoroutine = null;
			IsCasting = false;
		}
	}
}