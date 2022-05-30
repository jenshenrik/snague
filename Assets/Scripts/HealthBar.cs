using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    
    public void SetMaxHealth(int maxHealth)
    {
        slider.maxValue = maxHealth;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}
