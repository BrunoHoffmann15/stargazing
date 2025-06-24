using TMPro;
using UnityEngine;

public class ConfigurationManager : MonoBehaviour
{
    public TMP_Text velocityInfo;

    public GameObject panel;

    public void Start()
    {
        velocityInfo.text = "Normal";
        panel.SetActive(false);
    }

    public void ShowConfigurationMenu()
    {
        panel.SetActive(!panel.activeSelf);
    }

    public void IncreaseClick() 
    {
        VelocityManager.Increase();
        UpdateVelocityInfo();
    }

    public void DecreaseClick()
    {
        VelocityManager.Decrease();
        UpdateVelocityInfo();
    }

    public void UpdateVelocityInfo()
    {
        float velocity = VelocityManager.GetVelocity();

        if (velocity == 0)
        {
            velocityInfo.text = "Paused";
        } 
        else
        {
            velocityInfo.text = velocity.ToString() + "x";
        }
    }
}

public static class VelocityManager
{
    private static float velocityFactor = 1f;

    public static void Increase()
    {
        if (velocityFactor >= 3f)
        {
            return;
        }

        velocityFactor += 0.25f;
    }

    public static void Decrease()
    {
        if (velocityFactor <= 0f)
        {
            return;
        }

        velocityFactor -= 0.25f;
    }

    public static float GetVelocity()
    {
        return velocityFactor;
    }

}