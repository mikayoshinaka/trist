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

        transform.Find("Bar").GetComponent<Image>().color = PickColor(colorState);
        transform.Find("SpiralCooldown").GetComponent<VisualEffect>().SetGradient("Strip Gradient", PickGradient(colorState));

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

    public Color PickColor(ColorState colorState)
    {
        Color color = Color.black;

        if (colorState == ColorState.red)
        {
            color = colorRed;
        }
        else if (colorState == ColorState.blue)
        {
            color = colorBlue;
        }
        else if (colorState == ColorState.yellow)
        {
            color = colorYellow;
        }
        else if (colorState == ColorState.darkred)
        {
            color = colorDarkRed;
        }
        else if (colorState == ColorState.darkblue)
        {
            color = colorDarkBlue;
        }
        else if (colorState == ColorState.darkyellow)
        {
            color = colorDarkYellow;
        }
        else if (colorState == ColorState.purple)
        {
            color = colorPurple;
        }
        else if (colorState == ColorState.orange)
        {
            color = colorOrange;
        }
        else if (colorState == ColorState.green)
        {
            color = colorGreen;
        }

        return color;
    }

    public Gradient PickGradient(ColorState colorState)
    {
        Gradient gradient = null;

        if (colorState == ColorState.red)
        {
            gradient = gradientRed;
        }
        else if (colorState == ColorState.blue)
        {
            gradient = gradientBlue;
        }
        else if (colorState == ColorState.yellow)
        {
            gradient = gradientYellow;
        }
        else if (colorState == ColorState.darkred)
        {
            gradient = gradientDarkRed;
        }
        else if (colorState == ColorState.darkblue)
        {
            gradient = gradientDarkBlue;
        }
        else if (colorState == ColorState.darkyellow)
        {
            gradient = gradientDarkYellow;
        }
        else if (colorState == ColorState.purple)
        {
            gradient = gradientPurple;
        }
        else if (colorState == ColorState.orange)
        {
            gradient = gradientOrange;
        }
        else if (colorState == ColorState.green)
        {
            gradient = gradientGreen;
        }

        return gradient;
    }
}
