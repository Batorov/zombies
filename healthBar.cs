using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class healthBar : MonoBehaviour
{
    public Slider slider;
    public float Max = 100;
    public void SetHealth(float health)
    {
        slider.value = health / Max;
    }
    public void SetMax()
    {
        slider.value = 1;
    }
}
