using UnityEngine;
using UnityEngine.Events;

namespace UnityChan
{
    // 상호작용 대상(문, 아이템, 버튼 등)이 구현해야 하는 인터페이스
    // (MonoBehaviour가 이 인터페이스를 구현하면 아래 TryGetComponent(Type)로 잡힙니다)
    public interface IInteract
    {
        void Interact();
    }

    public class PlayerInteractor : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private PlayerController playerController;   // 같은 오브젝트의 PlayerController
        [SerializeField] private Transform firstPersonRayOrigin;      // 1인칭 레이 시작점(보통 FPS 카메라)

        [Header("Layer / Query")]
        [SerializeField] private LayerMask interactableLayers = ~0;   // 상호작용 오브젝트 레이어만 선택 권장
        [SerializeField] private QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.Collide;

        [Header("First Person (ray)")]
        [SerializeField] private float firstPersonRange = 3.0f;

        [Header("Third Person (proximity)")]
        [SerializeField] private float thirdPersonRadius = 1.5f;
        [SerializeField] private Vector3 thirdPersonCenterOffset = new Vector3(0f, 1.0f, 0f); // 가슴 높이쯤
        [SerializeField] private bool requireLineOfSightInThirdP = false;

        [Header("UI Hook (optional)")]
        public UnityEvent<bool> OnCanInteractChanged; // F키 프롬프트(활성/비활성) 연결용

        private IInteract _current;
        private Component _currentComponent;
        private bool _lastCanInteract;

        public bool CanInteract => _current != null;
        public Component CurrentComponent => _currentComponent;

        private void Awake()
        {
            if (!playerController)
                playerController = GetComponent<PlayerController>();

            if (!firstPersonRayOrigin)
            {
                // 기본값: MainCamera
                var cam = Camera.main;
                if (cam) firstPersonRayOrigin = cam.transform;
            }

            _lastCanInteract = false;
        }

        private void Update()
        {
            UpdateTarget();
        }

        public bool TryInteract()
        {
            if (_current == null) return false;

            _current.Interact();

            // 상호작용 후 오브젝트가 사라지거나 상태가 바뀔 수 있으니 재탐색
            UpdateTarget();
            return true;
        }

        private void UpdateTarget()
        {
            IInteract found = null;
            Component foundComp = null;

            if (playerController != null && playerController.PerspectiveState == PlayerController.STATE.FirstP)
            {
                found = FindByRaycast(out foundComp);
            }
            else if (playerController != null && playerController.PerspectiveState == PlayerController.STATE.ThirdP)
            {
                found = FindByProximity(out foundComp);
            }
            else
            {
                // SideView 등: 기본은 근접으로 처리(원하면 레이캐스트로 바꿔도 됨)
                found = FindByProximity(out foundComp);
            }

            SetCurrent(found, foundComp);
        }

        private void SetCurrent(IInteract found, Component foundComp)
        {
            _current = found;
            _currentComponent = foundComp;

            bool nowCan = (_current != null);
            if (nowCan != _lastCanInteract)
            {
                _lastCanInteract = nowCan;
                OnCanInteractChanged?.Invoke(nowCan);
            }
        }

        private IInteract FindByRaycast(out Component foundComp)
        {
            foundComp = null;
            if (!firstPersonRayOrigin) return null;

            Ray ray = new Ray(firstPersonRayOrigin.position, firstPersonRayOrigin.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, firstPersonRange, interactableLayers, triggerInteraction))
            {
                if (TryGetInteract(hit.collider, out IInteract interact, out Component comp))
                {
                    foundComp = comp;
                    return interact;
                }
            }

            return null;
        }

        private IInteract FindByProximity(out Component foundComp)
        {
            foundComp = null;

            Vector3 center = transform.position + thirdPersonCenterOffset;
            Collider[] cols = Physics.OverlapSphere(center, thirdPersonRadius, interactableLayers, triggerInteraction);

            float bestSqr = float.MaxValue;
            IInteract best = null;
            Component bestComp = null;

            foreach (var col in cols)
            {
                if (!TryGetInteract(col, out IInteract interact, out Component comp))
                    continue;

                if (requireLineOfSightInThirdP)
                {
                    Vector3 targetPos = col.bounds.center;
                    Vector3 dir = (targetPos - center);
                    float dist = dir.magnitude;
                    if (dist > 0.0001f)
                    {
                        dir /= dist;
                        if (Physics.Raycast(center, dir, out RaycastHit blockHit, dist, ~0, triggerInteraction))
                        {
                            // 중간에 다른 게 막으면 스킵(자기 자신 맞으면 OK)
                            if (blockHit.collider != col) continue;
                        }
                    }
                }

                float sqr = (col.bounds.center - transform.position).sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    best = interact;
                    bestComp = comp;
                }
            }

            foundComp = bestComp;
            return best;
        }

        // 지시사항: Raycast/Overlap 후 TryGetComponent로 IInteract 가능한 컴포넌트 있는지 확인
        private static bool TryGetInteract(Collider col, out IInteract interact, out Component foundComp)
        {
            interact = null;
            foundComp = null;
            if (!col) return false;

            // 1) collider 자신에서 찾기
            if (col.TryGetComponent(typeof(IInteract), out Component comp) && comp is IInteract i1)
            {
                foundComp = comp;
                interact = i1;
                return true;
            }

            // 2) 부모(루트)에 IInteract가 달린 경우까지 커버
            comp = col.GetComponentInParent(typeof(IInteract));
            if (comp != null && comp is IInteract i2)
            {
                foundComp = comp;
                interact = i2;
                return true;
            }

            return false;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Vector3 center = transform.position + thirdPersonCenterOffset;
            Gizmos.DrawWireSphere(center, thirdPersonRadius);

            if (firstPersonRayOrigin)
            {
                Gizmos.color = Color.cyan;
                Gizmos.DrawRay(firstPersonRayOrigin.position, firstPersonRayOrigin.forward * firstPersonRange);
            }
        }
#endif
    }
}
