using System.Collections;

namespace Boss.Skills
{
	public interface IBossSkill
	{
		public BossSkillType Type { get; }
		public IEnumerator StartAttack();
		public IEnumerator PerformAttack();
		public IEnumerator EndAttack();
	}
}