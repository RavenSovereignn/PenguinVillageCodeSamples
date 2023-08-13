using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpeningLore : MonoBehaviour
{
    [Header("Lore Text")]
    [TextArea(2,10)]
    public string lore;

    [Header("UI References")]
    public TMPro.TextMeshProUGUI LoreText;
    public TMPro.TextMeshProUGUI ContinueText;
    public Image PenguinVillageLogo;
    public Image fade;

    [Header("References")]
    public OpeningCinematic openingCinematic;

    private bool canContinue = false;

    private IEnumerator coroutine;


    void Start()
    {
        coroutine = TypeSentence(lore);
        StartCoroutine(coroutine);

        Time.timeScale = 1.0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        Debug.Log("started");

        if (canContinue && Input.GetKeyDown(KeyCode.E)) {
            StartCoroutine(FadeOut());
            openingCinematic.startMoving = true;

            canContinue = false;
        }

    }

    IEnumerator TypeSentence(string sentence)
    {
        LoreText.text = "";
        foreach (char letter in sentence.ToCharArray()) {
            LoreText.text += letter;
            yield return new WaitForSeconds(0.035f);
        }

        yield return new WaitForSeconds(0.2f);

        PenguinVillageLogo.gameObject.SetActive(true);

        PenguinVillageLogo.color = new Color(1, 1, 1, 0);

        WaitForSeconds delay = new WaitForSeconds(0.02f);
        for (int i = 0; i < 100; i++) {
            PenguinVillageLogo.color = new Color(1, 1, 1, 0.01f * i);
            yield return delay;
        }

        ContinueText.gameObject.SetActive(true);
        canContinue = true;
    }

    IEnumerator FadeOut()
    {
        LoreText.gameObject.SetActive(false);
        ContinueText.gameObject.SetActive(false);

        for (int i = 0; i < 50; i++) {
            Color colour = fade.color;
            colour.a -= 0.02f;
            fade.color = colour;
            yield return new WaitForSeconds(0.01f);
        }
    }



}
