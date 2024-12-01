using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Health Bar References")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Slider healthSmoothSlider; // ���ڽ���Ч��
    [SerializeField] private Image fillImage; // Ѫ�����ͼ��
    [SerializeField] private TextMeshProUGUI healthText;

    [Header("Visual Effects Settings")]
    [SerializeField] private float smoothSpeed = 5f; // Ѫ��ƽ���仯�ٶ�
    [SerializeField] private Color normalColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private float lowHealthThreshold = 30f; // ��Ѫ����ֵ
    [SerializeField] private float damageFlashDuration = 0.2f; // ������˸����ʱ��
    [SerializeField] private Color damageFlashColor = new Color(1, 0, 0, 0.5f); // ������˸��ɫ

    private PlayerManager playerManager;
    private float currentDisplayHealth;
    private Image damageFlashImage; // ������˸Ч��ͼ��

    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        currentDisplayHealth = playerManager.health.Value;

        // ��ʼ��Ѫ��
        InitializeHealthBars();

        // ����������˸Ч��ͼ��
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
        // ����ȫ����ɫ��˸Ч��
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
            // ���Ѫ���仯
            if (currentDisplayHealth != playerManager.health.Value)
            {
                // ���Ѫ�����٣���������Ч��
                if (currentDisplayHealth > playerManager.health.Value)
                {
                    StartCoroutine(DamageFlashEffect());
                }
                currentDisplayHealth = playerManager.health.Value;
            }

            // ƽ������Ѫ��
            UpdateHealthUI(playerManager.health.Value);

            // ����Ѫ����ɫ
            UpdateHealthColor();
        }
    }

    private void UpdateHealthUI(int targetHealth)
    {
        // ʵʱ������Ѫ��
        if (healthSlider != null)
        {
            healthSlider.value = targetHealth;
        }

        // ƽ�����½���Ѫ��
        if (healthSmoothSlider != null)
        {
            healthSmoothSlider.value = Mathf.Lerp(healthSmoothSlider.value, targetHealth, Time.deltaTime * smoothSpeed);
        }

        // ����Ѫ���ı�
        if (healthText != null)
        {
            healthText.text = $"{targetHealth}/100";
        }
    }

    private void UpdateHealthColor()
    {
        if (fillImage != null)
        {
            // ����Ѫ���ٷֱȼ�����ɫ
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
        // ��˸Ч��
        damageFlashImage.color = damageFlashColor;

        // ����Ч��
        float elapsed = 0f;
        while (elapsed < damageFlashDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(damageFlashColor.a, 0f, elapsed / damageFlashDuration);
            damageFlashImage.color = new Color(damageFlashColor.r, damageFlashColor.g, damageFlashColor.b, alpha);
            yield return null;
        }

        // ȷ����ȫ͸��
        damageFlashImage.color = Color.clear;
    }
}