using UnityEngine;

namespace UnityChan.Combat
{
    /// <summary>
    /// 피격 가능한 오브젝트가 구현하는 인터페이스.
    /// TryGetComponent로 이 인터페이스 유무를 검사하여 공격 대상 여부를 판별합니다.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(float damage, GameObject attacker);
    }
}
