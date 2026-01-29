using System.Collections.Generic;
using Boss.Skills;
using UnityEngine;

namespace Boss
{
	public class BossAnimator : MonoBehaviour
	{
		enum SkillState
		{
			Started,
			Performed,
			Ended
		}

		private Dictionary<(BossSkillType, SkillState), int> paramsHashDict = new()
		{
			{ (BossSkillType.BasicCast, SkillState.Started), Animator.StringToHash("BasicSkillStarted") },
			{ (BossSkillType.BasicCast, SkillState.Performed), Animator.StringToHash("BasicSkillPerformed") },
			{ (BossSkillType.BasicCast, SkillState.Ended), Animator.StringToHash("BasicSkillEnded") },
		};

		private Animator animator;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		public void OnAttackStarted(BossSkillType skillType) => SetAnimation(skillType, SkillState.Started);
		public void OnAttackPerformed(BossSkillType skillType) => SetAnimation(skillType, SkillState.Performed);
		public void OnAttackEnded(BossSkillType skillType) => SetAnimation(skillType, SkillState.Ended);

		private void SetAnimation(BossSkillType skillType, SkillState skillState)
		{
			if (!paramsHashDict.TryGetValue((skillType, skillState), out var hash)) return;
			animator.SetTrigger(hash);
		}
	}
}