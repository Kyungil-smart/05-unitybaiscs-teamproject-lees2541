using UnityChan.Combat;
using UnityEngine;

namespace UnityChan
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Rigidbody))]
    public class Side_PlayerController : MonoBehaviour
    {
        bool jDown;
        public bool isJump;
        private bool canAttack = true;
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
        public float jumpPower = 70.0f;
        public LayerMask groundLayers;

        [Header("Animation")] public float animSpeed = 1.5f;
        public float lookSmoother = 3.0f;
        public bool useCurves = true;
        public float useCurvesHeight = 0.5f;

        private CapsuleCollider col;
        private Rigidbody rb;
        private Animator anim;

        private Vector3 velocity;

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
            jDown = Input.GetButtonDown("Jump");
            Move();
        }

        void FixedUpdate()
        {
            ProcessInput();
            Jump();
        }
        public void SetCameraTransform(Transform tr)
        {
            MainCameraTransform = tr;
        }

        private void ProcessInput()
        {
            if (!canAttack) return;
            if (Input.GetMouseButtonDown(0) && CurrentViewMode != ViewMode.FPS)
            {
                canAttack = false;
                anim.SetTrigger("Attack");
                Invoke(nameof(DoAttackHit), 0.15f);
                Invoke(nameof(ResetAttack), 0.5f);
            }
        }

        private void ResetAttack()
        {
            canAttack = true;
        }

        private void DoAttackHit()
        {
            if (attackSystem != null)
            {
                attackSystem.OnAttackHit();
            }
        }

        void Jump()
        {
            if (jDown && !isJump)
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                anim.SetBool("Jump", true);
                isJump = true;
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
                    /*if (useCurves)
                    {
                        float jumpHeight = anim.GetFloat("JumpHeight");
                        float gravityControl = anim.GetFloat("GravityControl");
                        /*if (gravityControl > 0)
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
                    }*/

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
            //rb.useGravity = true;

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

        void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                isJump = false;
            }
        }
    }
}