using UnityEngine;
using UnityEngine.UI;

namespace UnityChan.Combat
{
    /// <summary>
    /// 오브젝트 위에 체력바를 표시하는 컴포넌트.
    /// HealthSystem과 함께 사용하며, 자동으로 Canvas와 UI를 생성합니다.
    /// </summary>
    [RequireComponent(typeof(HealthSystem))]
    public class HealthBarUI : MonoBehaviour
    {
        [Header("Position")]
        [SerializeField] private Vector3 offset = new Vector3(0, 2f, 0);

        [Header("Size")]
        [SerializeField] private float barWidth = 100f;
        [SerializeField] private float barHeight = 10f;

        [Header("Colors")]
        [SerializeField] private Color healthColor = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color damageColor = new Color(0.8f, 0.2f, 0.2f);
        [SerializeField] private Color backgroundColor = new Color(0.3f, 0.3f, 0.3f, 0.9f);
        [SerializeField] private Color borderColor = Color.black;

        [Header("Options")]
        [SerializeField] private bool hideWhenFull = true;
        [SerializeField] private bool faceCamera = true;
        [SerializeField] private float smoothSpeed = 5f;

        private HealthSystem healthSystem;
        private Canvas canvas;
        private RectTransform fillRect;
        private RectTransform damageRect;
        private float displayedHealth = 1f;
        private Transform cameraTransform;
        private float innerWidth;

        private const float BORDER_SIZE = 2f;
        private const float WORLD_SCALE = 0.01f;

        private void Awake()
        {
            healthSystem = GetComponent<HealthSystem>();
            innerWidth = barWidth - (BORDER_SIZE * 2);
            CreateHealthBarUI();
        }

        private void Start()
        {
            cameraTransform = Camera.main?.transform;
            displayedHealth = 1f;

            healthSystem.OnDamaged += HandleDamage;
            healthSystem.OnDeath += HandleDeath;

            UpdateVisibility();
        }

        private void OnDestroy()
        {
            if (healthSystem != null)
            {
                healthSystem.OnDamaged -= HandleDamage;
                healthSystem.OnDeath -= HandleDeath;
            }
        }

        private void LateUpdate()
        {
            if (canvas == null) return;

            // 위치 업데이트
            canvas.transform.position = transform.position + offset;

            // 빌보드
            if (faceCamera && cameraTransform != null)
            {
                canvas.transform.forward = cameraTransform.forward;
            }

            // 체력 비율 계산
            float targetHealth = healthSystem.CurrentHealth / healthSystem.MaxHealth;

            // 회복 시 즉시, 데미지 시 부드럽게
            if (targetHealth > displayedHealth)
            {
                displayedHealth = targetHealth;
            }
            else
            {
                displayedHealth = Mathf.Lerp(displayedHealth, targetHealth, Time.deltaTime * smoothSpeed);
            }

            // 바 너비 업데이트
            UpdateBarWidth(fillRect, targetHealth);
            UpdateBarWidth(damageRect, displayedHealth);

            UpdateVisibility();
        }

        private void UpdateBarWidth(RectTransform rect, float ratio)
        {
            if (rect == null) return;
            Vector2 size = rect.sizeDelta;
            size.x = innerWidth * Mathf.Clamp01(ratio);
            rect.sizeDelta = size;
        }

        private void CreateHealthBarUI()
        {
            // Canvas 생성
            GameObject canvasObj = new GameObject("HealthBar_Canvas");
            canvasObj.transform.SetParent(transform);
            canvasObj.transform.localPosition = offset;
            canvasObj.transform.localRotation = Quaternion.identity;
            canvasObj.transform.localScale = Vector3.one * WORLD_SCALE;

            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100;

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(barWidth, barHeight);
            canvasRect.pivot = new Vector2(0.5f, 0.5f);

            // 테두리 (검은색 배경)
            GameObject borderObj = CreateUIElement("Border", canvasObj.transform);
            Image borderImage = borderObj.AddComponent<Image>();
            borderImage.color = borderColor;
            RectTransform borderRect = borderObj.GetComponent<RectTransform>();
            borderRect.sizeDelta = new Vector2(barWidth, barHeight);
            borderRect.anchoredPosition = Vector2.zero;

            // 배경 (회색)
            GameObject bgObj = CreateUIElement("Background", canvasObj.transform);
            Image bgImage = bgObj.AddComponent<Image>();
            bgImage.color = backgroundColor;
            RectTransform bgRect = bgObj.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(innerWidth, barHeight - (BORDER_SIZE * 2));
            bgRect.anchoredPosition = Vector2.zero;

            // 데미지 바 (빨간색) - 왼쪽 정렬
            GameObject damageObj = CreateUIElement("DamageBar", canvasObj.transform);
            Image damageImage = damageObj.AddComponent<Image>();
            damageImage.color = damageColor;
            damageRect = damageObj.GetComponent<RectTransform>();
            damageRect.pivot = new Vector2(0f, 0.5f); // 왼쪽 피벗
            damageRect.sizeDelta = new Vector2(innerWidth, barHeight - (BORDER_SIZE * 2));
            damageRect.anchoredPosition = new Vector2(-innerWidth / 2f, 0f);

            // 체력 바 (초록색) - 왼쪽 정렬
            GameObject fillObj = CreateUIElement("HealthFill", canvasObj.transform);
            Image fillImage = fillObj.AddComponent<Image>();
            fillImage.color = healthColor;
            fillRect = fillObj.GetComponent<RectTransform>();
            fillRect.pivot = new Vector2(0f, 0.5f); // 왼쪽 피벗
            fillRect.sizeDelta = new Vector2(innerWidth, barHeight - (BORDER_SIZE * 2));
            fillRect.anchoredPosition = new Vector2(-innerWidth / 2f, 0f);
        }

        private GameObject CreateUIElement(string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            return obj;
        }

        private void HandleDamage(float damage)
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(true);
            }
        }

        private void HandleDeath()
        {
            if (canvas != null)
            {
                canvas.gameObject.SetActive(false);
            }
        }

        private void UpdateVisibility()
        {
            if (canvas == null) return;

            bool shouldShow = !hideWhenFull ||
                              healthSystem.CurrentHealth < healthSystem.MaxHealth;

            if (canvas.gameObject.activeSelf != shouldShow)
            {
                canvas.gameObject.SetActive(shouldShow);
            }
        }
    }
}