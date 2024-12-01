using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Health Bar References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthSmoothSlider; // 用于渐变效果
    [SerializeField] private Image fillImage; // 血条填充图像
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Visual Effects Settings")]
    [SerializeField] private float smoothSpeed = 5f; // 血条平滑变化速度
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 30f; // 低血量阈值
    [SerializeField] private float damageFlashDuration = 0.2f; // 受伤闪烁持续时间
    [SerializeField] private Color damageFlashColor = new Color(1, 0, 0, 0.5f); // 受伤闪烁颜色

    private PlayerManager playerManager;
    private float currentDisplayHealth;
    private Image damageFlashImage; // 受伤闪烁效果图像

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        currentDisplayHealth = playerManager.health.Value;

        // 初始化血条
        InitializeHealthBars();

        // 创建受伤闪烁效果图像
        CreateDamageFlashImage();
    }

    private void InitializeHealthBars()
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = 100;
            healthSlider.value = playerManager.health.Value;
        }

        if (healthSmoothSlider != null)
        {
            healthSmoothSlider.maxValue = 100;
            healthSmoothSlider.value = playerManager.health.Value;
        }

        UpdateHealthUI(playerManager.health.Value);
    }

    private void CreateDamageFlashImage()
    {
        // 创建全屏红色闪烁效果
        GameObject flashObj = new GameObject("DamageFlash");
        flashObj.transform.SetParent(transform.parent, false);
        damageFlashImage = flashObj.AddComponent<Image>();
        damageFlashImage.color = Color.clear;
        damageFlashImage.rectTransform.anchorMin = Vector2.zero;
        damageFlashImage.rectTransform.anchorMax = Vector2.one;
        damageFlashImage.rectTransform.sizeDelta = Vector2.zero;
    }

    private void Update()
    {
        if (playerManager != null)
        {
            // 检测血量变化
            if (currentDisplayHealth != playerManager.health.Value)
            {
                // 如果血量减少，触发受伤效果
                if (currentDisplayHealth > playerManager.health.Value)
                {
                    StartCoroutine(DamageFlashEffect());
                }
                currentDisplayHealth = playerManager.health.Value;
            }

            // 平滑更新血条
            UpdateHealthUI(playerManager.health.Value);

            // 更新血条颜色
            UpdateHealthColor();
        }
    }

    private void UpdateHealthUI(int targetHealth)
    {
        // 实时更新主血条
        if (healthSlider != null)
        {
            healthSlider.value = targetHealth;
        }

        // 平滑更新渐变血条
        if (healthSmoothSlider != null)
        {
            healthSmoothSlider.value = Mathf.Lerp(healthSmoothSlider.value, targetHealth, Time.deltaTime * smoothSpeed);
        }

        // 更新血量文本
        if (healthText != null)
        {
            healthText.text = $"{targetHealth}/100";
        }
    }

    private void UpdateHealthColor()
    {
        if (fillImage != null)
        {
            // 根据血量百分比计算颜色
            float healthPercentage = healthSlider.value / healthSlider.maxValue;
            if (healthPercentage <= lowHealthThreshold / 100f)
            {
                fillImage.color = lowHealthColor;
            }
            else
            {
                fillImage.color = Color.Lerp(lowHealthColor, normalColor, (healthPercentage - lowHealthThreshold / 100f) / (1f - lowHealthThreshold / 100f));
            }
        }
    }

    private IEnumerator DamageFlashEffect()
    {
        // 闪烁效果
        damageFlashImage.color = damageFlashColor;

        // 渐隐效果
        float elapsed = 0f;
        while (elapsed < damageFlashDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(damageFlashColor.a, 0f, elapsed / damageFlashDuration);
            damageFlashImage.color = new Color(damageFlashColor.r, damageFlashColor.g, damageFlashColor.b, alpha);
            yield return null;
        }

        // 确保完全透明
        damageFlashImage.color = Color.clear;
    }
}