using UnityChan.Combat;
using UnityEngine;

namespace UnityChan
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        public enum MovementSpace
        {
            World,
            Camera
        }

        public enum ViewMode
        {
            Default,
            SideView,   // 2D 횡스크롤 스타일 (W/S 비활성화)
            FPS         // 1인칭 모드 (마우스 공격 비활성화)
        }

        [Header("Movement")] public MovementSpace PerspectiveState = MovementSpace.World;
        public ViewMode CurrentViewMode = ViewMode.Default;
        public Transform MainCameraTransform;

        [Header("Control")] public float moveSpeed = 7.0f;
        public float rotateSpeed = 7.0f;
        public float jumpPower = 1.0f;
        public LayerMask groundLayers;

        [Header("Animation")] public float animSpeed = 1.5f;
        public float lookSmoother = 3.0f;
        public bool useCurves = true;
        public float useCurvesHeight = 0.5f;

        private CapsuleCollider col;
        private Rigidbody rb;
        private Animator anim;

        private Vector3 velocity;

        // --- Jump input buffer + grounded ---
        private bool jumpRequest;
        private bool jumpConsumed;
        private float jumpRequestTimer;
        private const float groundedVelYThreshold = 0.05f;
        private const float jumpBufferTime = 0.15f;

        private float coyoteTimer;
        private const float coyoteTime = 0.10f;
        private const float groundEpsilon = 0.06f;

        // CapsuleCollider 초기값
        private float colliderOriginHeight;
        private Vector3 colliderOriginCenter;

        private AnimatorStateInfo currentBaseState;

        static int locoState = Animator.StringToHash("Base Layer.Locomotion");
        static int jumpState = Animator.StringToHash("Base Layer.Jump");
        static int idleState = Animator.StringToHash("Base Layer.Idle");
        static int restState = Animator.StringToHash("Base Layer.Rest");
        static int attackState = Animator.StringToHash("Base Layer.punch");

        private AttackSystem attackSystem;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            col = GetComponent<CapsuleCollider>();
            rb = GetComponent<Rigidbody>();
            attackSystem = GetComponent<AttackSystem>();
        }

        void Start()
        {
            if (MainCameraTransform == null)
                MainCameraTransform = Camera.main.transform;

            colliderOriginHeight = col.height;
            colliderOriginCenter = col.center;
        }

        void Update()
        {
            ProcessInput();

            if (jumpRequest)
            {
                jumpRequestTimer -= Time.deltaTime;
                if (jumpRequestTimer <= 0f)
                {
                    jumpRequest = false;
                    jumpRequestTimer = 0f;
                }
            }
        }

        void FixedUpdate()
        {
            Move();
            Jump();
        }

        public void SetCameraTransform(Transform tr)
        {
            MainCameraTransform = tr;
        }

        private void ProcessInput()
        {
            if (Input.GetButtonDown("Jump"))
            {
                jumpRequest = true;
                jumpRequestTimer = jumpBufferTime;
            }

            if (Input.GetKey(KeyCode.Q)) transform.Rotate(0, -1 * rotateSpeed, 0);
            if (Input.GetKey(KeyCode.E)) transform.Rotate(0, 1 * rotateSpeed, 0);

            if (Input.GetMouseButtonDown(0) && CurrentViewMode != ViewMode.FPS)
            {
                if (currentBaseState.fullPathHash != jumpState &&
                    currentBaseState.fullPathHash != attackState)
                {
                    anim.SetTrigger("Attack");
                    Invoke(nameof(DoAttackHit), 0.15f);
                }
            }
        }

        private void DoAttackHit()
        {
            if (attackSystem != null)
            {
                attackSystem.OnAttackHit();
            }
        }

        private void Jump()
        {
            // 접지 판정: 올라가는 중이면 접지로 보지 않음
            bool grounded = IsGrounded() && rb.velocity.y <= groundedVelYThreshold;

            // 착지했으면 점프 락 해제
            if (grounded)
            {
                jumpConsumed = false;
                coyoteTimer = coyoteTime;
            }
            else
            {
                coyoteTimer = Mathf.Max(0f, coyoteTimer - Time.fixedDeltaTime);
            }

            // 점프 실행: 착지 전까지 1번만
            if (jumpRequest && !jumpConsumed && coyoteTimer > 0f &&
                currentBaseState.fullPathHash != restState &&
                currentBaseState.fullPathHash != attackState)
            {
                // 하강 중이면 하강 속도 제거
                Vector3 vel = rb.velocity;
                if (vel.y < 0f) vel.y = 0f;
                rb.velocity = vel;

                // 애니메이션 + 즉시 점프 힘 적용
                anim.SetBool("Jump", true);
                rb.AddForce(Vector3.up * jumpPower, ForceMode.VelocityChange);

                // 상태 업데이트
                jumpConsumed = true;
                jumpRequest = false;
                jumpRequestTimer = 0f;
                coyoteTimer = 0f;
            }

            // Animator State별 처리
            if (currentBaseState.fullPathHash == locoState)
            {
                if (useCurves)
                {
                    ResetCollider();
                }
            }
            else if (currentBaseState.fullPathHash == jumpState)
            {
                if (!anim.IsInTransition(0))
                {
                    if (useCurves)
                    {
                        float jumpHeight = anim.GetFloat("JumpHeight");
                        float gravityControl = anim.GetFloat("GravityControl");
                        if (gravityControl > 0)
                            rb.useGravity = false;

                        Ray ray = new Ray(transform.position + Vector3.up, -Vector3.up);
                        RaycastHit hitInfo = new RaycastHit();
                        if (Physics.Raycast(ray, out hitInfo))
                        {
                            if (hitInfo.distance > useCurvesHeight)
                            {
                                col.height = colliderOriginHeight - jumpHeight;
                                col.center = new Vector3(0, colliderOriginCenter.y + jumpHeight, 0);
                            }
                            else
                            {
                                ResetCollider();
                            }
                        }
                    }

                    anim.SetBool("Jump", false);
                }
            }
            else if (currentBaseState.fullPathHash == idleState)
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    anim.SetBool("Rest", true);
                }
            }
            else if (currentBaseState.fullPathHash == restState)
            {
                if (!anim.IsInTransition(0))
                {
                    anim.SetBool("Rest", false);
                }
            }
            else if (currentBaseState.fullPathHash == attackState)
            {
                // 공격 애니메이션 재생 중
            }
        }

        void Move()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            anim.SetFloat("Direction", h);
            anim.speed = animSpeed;
            currentBaseState = anim.GetCurrentAnimatorStateInfo(0);
            rb.useGravity = true;

            Vector3 moveDir = Vector3.zero;

            if (CurrentViewMode == ViewMode.SideView)
            {
                v = 0f;
            }

            if (PerspectiveState == MovementSpace.World)
            {
                moveDir = new Vector3(h, 0, v).normalized;
            }
            else if (PerspectiveState == MovementSpace.Camera)
            {
                moveDir = MainCameraTransform.transform.forward * v + MainCameraTransform.transform.right * h;
                moveDir.y = 0;
                moveDir.Normalize();
            }

            float speedParam = Mathf.Clamp01(moveDir.magnitude);
            anim.SetFloat("Speed", speedParam);
            anim.SetFloat("Direction", 0f);

            if (moveDir.magnitude > 0.01f)
            {
                rb.MovePosition(transform.position + moveDir * (moveSpeed * Time.fixedDeltaTime));

                Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
                float t = Mathf.Clamp01(rotateSpeed * Time.fixedDeltaTime);
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, t));
            }
        }

        // Animation Event용 (나중에 사용하려면 남겨둠)
        public void OnJumpTakeoff()
        {
            // 현재는 사용 안 함 - 즉시 점프 방식으로 변경됨
        }

        void ResetCollider()
        {
            col.height = colliderOriginHeight;
            col.center = colliderOriginCenter;
        }

        private bool IsGrounded()
        {
            var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
            return Physics.Raycast(ray, 0.2f, groundLayers);
        }
    }
}