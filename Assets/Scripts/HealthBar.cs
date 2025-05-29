using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public GameObject healthBarPrefab; // Assign the prefab in Inspector

    private Image healthBarImage;      // The instantiated Image component
    private GameObject healthBarInstance;

    private System.Reflection.FieldInfo healthField;
    private System.Reflection.FieldInfo maxHealthField;

    private MonoBehaviour targetScript;
    private float healthValue;
    private float maxValue;

    private void Start()
    {
        // Instantiate health bar and set it as a child of this object
        healthBarInstance = Instantiate(healthBarPrefab);
        healthBarInstance.transform.SetParent(transform);               // Parent to the object
        healthBarInstance.transform.localPosition = new Vector3(0, 0.5f); 
        healthBarImage = healthBarInstance.GetComponentInChildren<Image>();

        // Get the script containing 'health' and 'maxHealth'
        targetScript = GetComponent<MonoBehaviour>();
        if (targetScript == null) return;

        healthField = targetScript.GetType().GetField("health");
        maxHealthField = targetScript.GetType().GetField("maxHealth");

        if (healthField != null && maxHealthField != null)
        {
            maxValue = (float)maxHealthField.GetValue(targetScript);
        }
    }

    private void Update()
    {
        if (targetScript == null || healthField == null || maxHealthField == null || healthBarImage == null) return;

        healthValue = (float)healthField.GetValue(targetScript);
        float fill = Mathf.Clamp01(healthValue / maxValue);
        healthBarImage.fillAmount = fill;
    }
}
