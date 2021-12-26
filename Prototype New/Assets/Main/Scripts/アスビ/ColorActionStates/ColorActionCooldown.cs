using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public class ColorActionCooldown : MonoBehaviour
{
    public bool cooldown;

    public enum ColorState
    {
        red,
        blue,
        yellow,
        darkred,
        darkblue,
        darkyellow,
        purple,
        orange,
        green
    };

    [SerializeField] Color colorRed;
    [SerializeField] Gradient gradientRed;
    [SerializeField] Color colorBlue;
    [SerializeField] Gradient gradientBlue;
    [SerializeField] Color colorYellow;
    [SerializeField] Gradient gradientYellow;
    [SerializeField] Color colorDarkRed;
    [SerializeField] Gradient gradientDarkRed;
    [SerializeField] Color colorDarkBlue;
    [SerializeField] Gradient gradientDarkBlue;
    [SerializeField] Color colorDarkYellow;
    [SerializeField] Gradient gradientDarkYellow;
    [SerializeField] Color colorPurple;
    [SerializeField] Gradient gradientPurple;
    [SerializeField] Color colorOrange;
    [SerializeField] Gradient gradientOrange;
    [SerializeField] Color colorGreen;
    [SerializeField] Gradient gradientGreen;

    public void ResetCooldown()
    {
        cooldown = false;
        transform.Find("Bar").GetComponent<Image>().fillAmount = 0;
        this.gameObject.SetActive(false);
    }

    public void StartCooldown(float duration, ColorState colorState)
    {
        cooldown = true;

        PickColor(colorState);
        
        if (Cooldown != null)
        {
            StopCoroutine(Cooldown);
        }
        Cooldown = StartCoroutine(OnCooldown(duration));
    }
    Coroutine Cooldown;
    IEnumerator OnCooldown(float duration)
    {
        float timer = 0f;

        while (timer < duration)
        {
            transform.Find("Bar").GetComponent<Image>().fillAmount = timer / duration;

            timer += Time.deltaTime;   
            yield return null;
        }

        ResetCooldown();
    }

    void PickColor(ColorState colorState)
    {
        Color color = Color.black;
        Gradient gradient = null;

        if (colorState == ColorState.red)
        {
            color = colorRed;
            gradient = gradientRed;
        }
        else if (colorState == ColorState.blue)
        {
            color = colorBlue;
            gradient = gradientBlue;
        }
        else if (colorState == ColorState.yellow)
        {
            color = colorYellow;
            gradient = gradientYellow;
        }
        else if (colorState == ColorState.darkred)
        {
            color = colorDarkRed;
            gradient = gradientDarkRed;
        }
        else if (colorState == ColorState.darkblue)
        {
            color = colorDarkBlue;
            gradient = gradientDarkBlue;
        }
        else if (colorState == ColorState.darkyellow)
        {
            color = colorDarkYellow;
            gradient = gradientDarkYellow;
        }
        else if (colorState == ColorState.purple)
        {
            color = colorPurple;
            gradient = gradientPurple;
        }
        else if (colorState == ColorState.orange)
        {
            color = colorOrange;
            gradient = gradientOrange;
        }
        else if (colorState == ColorState.green)
        {
            color = colorGreen;
            gradient = gradientGreen;
        }

        transform.Find("Bar").GetComponent<Image>().color = color;
        transform.Find("SpiralCooldown").GetComponent<VisualEffect>().SetGradient("Strip Gradient", gradient);
    }
}
