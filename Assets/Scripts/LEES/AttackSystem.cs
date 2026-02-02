using System.Collections.Generic;
using UnityEngine;

namespace UnityChan.Combat
{
    /// <summary>
    /// 공격 판정 컴포넌트. Animation Event에서 OnAttackHit() 호출.
    /// </summary>
    public class AttackSystem : MonoBehaviour
    {
        [Header("Attack Settings")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private float attackRadius = 1f;
        [SerializeField] private Vector3 attackOffset = new Vector3(0, 0.5f, 0.8f);
        [SerializeField] private LayerMask targetLayers = ~0;
        
        [Header("Debug")]
        [SerializeField] private bool showGizmos = true;
        
        private HashSet<GameObject> hitTargets = new HashSet<GameObject>();
        private Collider[] hitBuffer = new Collider[8];

        /// <summary>
        /// Animation Event에서 호출. 공격 판정 실행.
        /// </summary>
        public void OnAttackHit()
        {
            hitTargets.Clear();
            
            Vector3 attackPos = transform.TransformPoint(attackOffset);
            int hitCount = Physics.OverlapSphereNonAlloc(attackPos, attackRadius, hitBuffer, targetLayers);
            
            for (int i = 0; i < hitCount; i++)
            {
                ProcessHit(hitBuffer[i]);
            }
        }

        private void ProcessHit(Collider col)
        {
            // 자기 자신 무시
            if (col.transform.IsChildOf(transform) || col.gameObject == gameObject)
                return;
            
            // 중복 히트 방지
            GameObject root = col.attachedRigidbody != null ? col.attachedRigidbody.gameObject : col.gameObject;
            if (hitTargets.Contains(root))
                return;
            
            // ★ TryGetComponent로 IDamageable 검사
            IDamageable damageable = null;
            
            if (!col.TryGetComponent(out damageable))
            {
                if (col.attachedRigidbody != null)
                    col.attachedRigidbody.TryGetComponent(out damageable);
            }
            
            if (damageable == null)
                damageable = col.GetComponentInParent<IDamageable>();
            
            // IDamageable 없으면 무시
            if (damageable == null)
                return;
            
            hitTargets.Add(root);
            damageable.TakeDamage(damage, gameObject);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!showGizmos) return;
            
            Vector3 pos = transform.TransformPoint(attackOffset);
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireSphere(pos, attackRadius);
        }
#endif
    }
}
