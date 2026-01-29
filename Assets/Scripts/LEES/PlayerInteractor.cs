using UnityEngine;

namespace UnityChan
{
    [DisallowMultipleComponent]
    public class PlayerInteractor : MonoBehaviour
    {
        [Header("First Person (Look Ray)")]
        [SerializeField] private Transform rayOrigin;     // 비워두면 Camera.main 사용
        [SerializeField] private float rayDistance = 3f;

        [Header("Third Person / Quarter View (Proximity)")]
        [SerializeField] private float proximityRadius = 2f;
        [SerializeField] private float proximityScanInterval = 0.05f; // 0.05~0.1 권장
        [SerializeField] private bool includeTriggers = true;

        public bool CanInteract => _currentInteract != null;
        public IInteract Current => _currentInteract;
        public Component CurrentComponent => _currentComponent; // (대부분 MonoBehaviour)

        private PlayerController _controller;
        private Collider[] _selfColliders;
        private readonly Collider[] _overlapBuffer = new Collider[32];

        private float _scanTimer;
        private IInteract _currentInteract;
        private Component _currentComponent;

        private void Awake()
        {
            _controller = GetComponent<PlayerController>();
            _selfColliders = GetComponentsInChildren<Collider>();
        }

        private void Update()
        {
            UpdateTarget();
        }

        /// <summary>PlayerController에서 F키 눌렀을 때 호출</summary>
        public bool TryInteract()
        {
            if (_currentInteract == null) return false;
            _currentInteract.Interact(); // IInteract.cs의 기본 구현(혹은 구현체의 오버라이드) 호출 :contentReference[oaicite:1]{index=1}
            return true;
        }

        private void UpdateTarget()
        {
            // 1인칭: 바라보는 대상 레이캐스트
            if (_controller != null && _controller.PerspectiveState == PlayerController.STATE.FirstP)
            {
                UpdateByRaycast();
                return;
            }

            // 쿼터뷰/사이드뷰: 근접(반경) 검사
            _scanTimer -= Time.deltaTime;
            if (_scanTimer <= 0f)
            {
                _scanTimer = proximityScanInterval;
                UpdateByProximity();
            }
        }

        private Transform GetRayOrigin()
        {
            if (rayOrigin != null) return rayOrigin;
            var cam = Camera.main;
            return cam != null ? cam.transform : transform;
        }

        private void UpdateByRaycast()
        {
            Transform origin = GetRayOrigin();
            Ray ray = new Ray(origin.position, origin.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, ~0,
                    includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore))
            {
                if (TryGetInteract(hit.collider, out var interact, out var comp))
                {
                    SetCurrent(interact, comp);
                    return;
                }
            }

            ClearCurrent();
        }

        private void UpdateByProximity()
        {
            var qti = includeTriggers ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;

            int count = Physics.OverlapSphereNonAlloc(transform.position, proximityRadius, _overlapBuffer, ~0, qti);

            IInteract best = null;
            Component bestComp = null;
            float bestD2 = float.PositiveInfinity;

            for (int i = 0; i < count; i++)
            {
                var c = _overlapBuffer[i];
                if (c == null) continue;
                if (IsSelfCollider(c)) continue;

                if (!TryGetInteract(c, out var interact, out var comp)) continue;

                Vector3 closest = c.ClosestPoint(transform.position);
                float d2 = (closest - transform.position).sqrMagnitude;

                if (d2 < bestD2)
                {
                    bestD2 = d2;
                    best = interact;
                    bestComp = comp;
                }
            }

            if (best != null) SetCurrent(best, bestComp);
            else ClearCurrent();
        }

        private bool TryGetInteract(Collider col, out IInteract interact, out Component comp)
        {
            interact = null;
            comp = null;
            if (col == null) return false;

            // 인터페이스는 TryGetComponent<T>로 바로 못 받는 경우가 많아서(GetComponent가 안전),
            // 콜라이더(또는 부모)에 붙은 IInteract 구현 MonoBehaviour를 찾습니다.
            interact = col.GetComponentInParent<IInteract>();
            if (interact == null) return false;

            comp = interact as Component; // 구현체가 MonoBehaviour일 때만 유효
            return comp != null;
        }

        private bool IsSelfCollider(Collider other)
        {
            for (int i = 0; i < _selfColliders.Length; i++)
                if (_selfColliders[i] == other) return true;
            return false;
        }

        private void SetCurrent(IInteract interact, Component comp)
        {
            if (_currentComponent == comp && _currentInteract == interact) return;

            // (선택) 오브젝트 UI 쪽에서 쓰고 싶으면 아래 메시지 이름으로 구현해두면 됨
            if (_currentComponent != null)
                _currentComponent.gameObject.SendMessage("OnInteractTargetExit", this, SendMessageOptions.DontRequireReceiver);

            _currentInteract = interact;
            _currentComponent = comp;

            if (_currentComponent != null)
                _currentComponent.gameObject.SendMessage("OnInteractTargetEnter", this, SendMessageOptions.DontRequireReceiver);
        }

        private void ClearCurrent()
        {
            if (_currentComponent != null)
                _currentComponent.gameObject.SendMessage("OnInteractTargetExit", this, SendMessageOptions.DontRequireReceiver);

            _currentInteract = null;
            _currentComponent = null;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, proximityRadius);
        }
#endif
    }
}
