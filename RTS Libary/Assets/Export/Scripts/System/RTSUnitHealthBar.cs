using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RTSUnitHealthBar : MonoBehaviour
{
    [SerializeField] Material lowHp, midHp, highHp, overflowHp;
    [SerializeField] Transform forground;
    [SerializeField] Transform background;
    [SerializeField] float updateSpeedSeconds = 0.5f;

    public void Start()
    {
        forground.GetComponent<Image>().fillAmount = 1f;
        UpdateMaterial();
    }

    private void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.parent.forward);
    }

    public void SetHPPercentage(float percentage)
    {
        if (gameObject.activeSelf) {
            StartCoroutine(ChangeHp(percentage));
            UpdateMaterial();
        }
    }

    private IEnumerator ChangeHp(float value)
    {
        float preChzangePct = forground.GetComponent<Image>().fillAmount;
        float elapsed = 0f;

        while (elapsed < updateSpeedSeconds)
        {
            elapsed += Time.deltaTime;
            forground.GetComponent<Image>().fillAmount = Mathf.Lerp(preChzangePct, value, elapsed / updateSpeedSeconds);
            yield return null;
        }

        forground.GetComponent<Image>().fillAmount = value;
    }

    private void UpdateMaterial()
    {
        if (forground.GetComponent<Image>().fillAmount > 1)
            forground.GetComponent<Image>().material = overflowHp;
        else if (forground.GetComponent<Image>().fillAmount >= .75f)
            forground.GetComponent<Image>().material = highHp;
        else if (forground.GetComponent<Image>().fillAmount >= .5f)
            forground.GetComponent<Image>().material = midHp;
        else
            forground.GetComponent<Image>().material = lowHp;
    }
}