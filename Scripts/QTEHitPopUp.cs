using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;

public class QTEHitPopUp : MonoBehaviour
{
    public Color hitColour;
    public Color missColour;

    private RectTransform rect;
    private TMPro.TextMeshProUGUI text;

    void Start()
    {
        rect = GetComponent<RectTransform>();
        text = GetComponent<TMPro.TextMeshProUGUI>();
        StartCoroutine(FadeAway());
    }

    public void Setup(bool hit)
    {
        GetComponent<TMPro.TextMeshProUGUI>().text = (hit) ? "Hit!" : "Miss";
        GetComponent<TMPro.TextMeshProUGUI>().color = (hit) ? hitColour : missColour;
    }

    private IEnumerator FadeAway()
    {
        WaitForEndOfFrame tillEndOfFrame = new WaitForEndOfFrame();
        Color fadeColour = new Color(0, 0, 0, 0.02f);

        Vector3 startingPos = rect.localPosition;
        Vector3 startingScale = rect.localScale;
        Color startingColour = text.color;

        float progress = 0.0f;
        while(progress < 1.0f) {
            progress += Time.unscaledDeltaTime;

            rect.localPosition = startingPos  - ((Vector3.up * 0.02f) * progress * 100);
            rect.localScale = startingScale - ((Vector3.one * 0.005f) * progress * 100);
            text.color = startingColour - (fadeColour * progress * 100);

            yield return tillEndOfFrame;
        }

        Destroy(gameObject);
    }

    

}
