using System.Collections;

public interface IBossSkill
{
	public IEnumerator StartAttack();
	public IEnumerator PerformAttack();
	public IEnumerator EndAttack();
}