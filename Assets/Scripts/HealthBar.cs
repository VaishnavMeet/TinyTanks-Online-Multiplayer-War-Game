using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar; // Assign the Image component (not the GameObject)

    private System.Reflection.FieldInfo healthField;
    private System.Reflection.FieldInfo maxHealthField;

    private MonoBehaviour targetScript;
    private float healthValue;
    private float maxValue;

    private void Start()
    {
        // Get the script with the 'health' and 'maxHealth' fields (e.g., TankController2D)
        targetScript = GetComponent<MonoBehaviour>();
        if (targetScript == null) return;

        // Use reflection to access 'health' and 'maxHealth'
        healthField = targetScript.GetType().GetField("health");
        maxHealthField = targetScript.GetType().GetField("maxHealth");

        if (healthField != null && maxHealthField != null)
        {
            maxValue = (float)maxHealthField.GetValue(targetScript);
        }
    }

    private void Update()
    {
        if (targetScript == null || healthField == null || maxHealthField == null) return;

        healthValue = (float)healthField.GetValue(targetScript);
        float fill = Mathf.Clamp01(healthValue / maxValue);

        if (healthBar != null)
        {
            healthBar.fillAmount = fill;
        }
    }
}
