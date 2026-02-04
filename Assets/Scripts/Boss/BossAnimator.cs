using System;
using System.Collections.Generic;
using Boss.Skills;
using UnityEngine;

namespace Boss
{
	public class BossAnimator : MonoBehaviour
	{
		private static readonly int LaserDirection = Animator.StringToHash("LaserDirection");

		enum SkillState
		{
			Started,
			Performed,
			Ended
		}

		private Dictionary<(BossSkillType, SkillState), int> parameterHashDict = new()
		{
			{ (BossSkillType.BasicCast, SkillState.Started), Animator.StringToHash("BasicSkillStarted") },
			{ (BossSkillType.BasicCast, SkillState.Performed), Animator.StringToHash("BasicSkillPerformed") },
			{ (BossSkillType.BasicCast, SkillState.Ended), Animator.StringToHash("BasicSkillEnded") },
			{ (BossSkillType.HorizontalLaser, SkillState.Started), Animator.StringToHash("LaserSkillStarted") },
			{ (BossSkillType.HorizontalLaser, SkillState.Performed), Animator.StringToHash("LaserSkillPerformed") },
			{ (BossSkillType.HorizontalLaser, SkillState.Ended), Animator.StringToHash("LaserSkillEnded") },
			{ (BossSkillType.VerticalLaser, SkillState.Started), Animator.StringToHash("LaserSkillStarted") },
			{ (BossSkillType.VerticalLaser, SkillState.Performed), Animator.StringToHash("LaserSkillPerformed") },
			{ (BossSkillType.VerticalLaser, SkillState.Ended), Animator.StringToHash("LaserSkillEnded") }
		};

		private Dictionary<(BossSkillType, SkillState), Action<Animator>> skillActionDict = new()
		{
			{ (BossSkillType.HorizontalLaser, SkillState.Started), a => { a.SetBool(LaserDirection, true); } },
			{ (BossSkillType.VerticalLaser, SkillState.Started), a => { a.SetBool(LaserDirection, false); } }
		};

		private Animator animator;

		private void Awake()
		{
			animator = GetComponent<Animator>();
		}

		public void OnAttackStarted(BossSkillType skillType) => SetAnimation(skillType, SkillState.Started);
		public void OnAttackPerformed(BossSkillType skillType) => SetAnimation(skillType, SkillState.Performed);
		public void OnAttackEnded(BossSkillType skillType) => SetAnimation(skillType, SkillState.Ended);

		public void OnBossDied() => animator.SetTrigger("Die");

		private void SetAnimation(BossSkillType skillType, SkillState skillState)
		{
			if (skillActionDict.TryGetValue((skillType, skillState), out Action<Animator> action)) action(animator);
			if (parameterHashDict.TryGetValue((skillType, skillState), out var hash)) animator.SetTrigger(hash);
		}
	}
}