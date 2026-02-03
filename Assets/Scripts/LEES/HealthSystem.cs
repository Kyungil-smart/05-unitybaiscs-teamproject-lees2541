using System;
using UnityEngine;

namespace UnityChan.Combat
{
    /// <summary>
    /// 체력 관리 컴포넌트. 플레이어와 몬스터 모두 사용.
    /// </summary>
    public class HealthSystem : MonoBehaviour, IDamageable
    {
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float currentHealth;

        [Header("Death Settings")]
        [SerializeField] private bool destroyOnDeath = true;
        [SerializeField] private float destroyDelay = 0f;

        public float CurrentHealth => currentHealth;
        public float MaxHealth => maxHealth;
        public bool IsAlive => currentHealth > 0;

        public event Action<float> OnDamaged;      // 데미지량
        public event Action OnDeath;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(float damage, GameObject attacker)
        {
            if (!IsAlive) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            Debug.Log($"[{gameObject.name}] {damage} 데미지! 남은 체력: {currentHealth}/{maxHealth}");

            OnDamaged?.Invoke(damage);

            if (currentHealth <= 0)
            {
                Debug.Log($"[{gameObject.name}] 사망!");
                OnDeath?.Invoke();

                if (destroyOnDeath)
                {
                    Destroy(gameObject, destroyDelay);
                }
            }
        }

        public void Heal(float amount)
        {
            if (!IsAlive) return;
            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        }
    }
}
