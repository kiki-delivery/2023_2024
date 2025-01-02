using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class JellyMessageManager : MonoBehaviour
{
    [SerializeField] SettingData settingData;

    [Header("=============메시지=============")]
    [Header("참조 오브젝트")]
    [SerializeField] CanvasGroup answerTextGroup;
    [SerializeField] Text questionText;    
    [SerializeField] Text answerName;
    [SerializeField] Text answerMessage;    

    [Header("설정")]
    [SerializeField] Color[] fontColors;
    [SerializeField] Color[] camelFontColors;
    [SerializeField, TextArea] string[] names;
    [SerializeField, TextArea] string[] messages;    
    [SerializeField, Range(1, 100)] int alphaChangeSpeed = 6;

    Color questionColor;
    void Start()
    {
        Setting();
    }

    void Setting()
    {
        alphaChangeSpeed = settingData.alphaChangeSpeed;

        for (int i = 0; i < fontColors.Length; i++)
        {
            fontColors[i] = ConvertHexCode2Color(settingData.fontColorHexCodes[i]);
        }

        for (int i = 0; i < camelFontColors.Length; i++)
        {
            camelFontColors[i] = ConvertHexCode2Color(settingData.camelFontColorHexCodes[i]);
        }

        string[] camelRichTextColors = new string[4];
        for(int i = 0; i < camelFontColors.Length; i++)
        {
            camelRichTextColors[i] = "<color=\"" + settingData.camelFontColorHexCodes[i] + "\">";
        }

        names[6] = camelRichTextColors[0] + "카</color>" +
                    camelRichTextColors[1] + "멜</color>" +
                    camelRichTextColors[2] + "레</color>" +
                    camelRichTextColors[3] + "온</color>";

    }

    Color ConvertHexCode2Color(string hexCode)
    {
        Color color;
        if(ColorUtility.TryParseHtmlString(hexCode, out color))
        {
            return color;
        }

        return Color.white;
    }

    public void Question()
    {
        StartCoroutine(IQuestion());
    }

    IEnumerator IQuestion()
    {
        float answerAlpha = 1;
        ChangeMessageAlpha(answerAlpha);

        while (answerAlpha > 0)
        {
            answerAlpha = answerAlpha - 0.001f * alphaChangeSpeed;
            ChangeMessageAlpha(answerAlpha);            
            yield return null;
        }

        answerAlpha = 0;

        ChangeMessageAlpha(answerAlpha);
    }

    public void Answer(int jellyNum)
    {
        SetAnswerMessage(jellyNum);
        StartCoroutine(IAnswer());
    }

    void SetAnswerMessage(int jellyNum)
    {
        answerName.text = names[jellyNum];
        answerName.color = fontColors[jellyNum];
        answerMessage.text = messages[jellyNum];
    }

    IEnumerator IAnswer()
    {
        float answerAlpha = 0;
        ChangeMessageAlpha(answerAlpha);

        while (answerAlpha < 1)
        {
            answerAlpha = answerAlpha +  0.001f * alphaChangeSpeed;
            ChangeMessageAlpha(answerAlpha);
            yield return null;
        }

        answerAlpha = 1;

        ChangeMessageAlpha(answerAlpha);
    }

    public void ChangeMessageAlpha(float answerAlpha)
    {        
        questionColor = new Color(1, 1, 1, 1 - answerAlpha * 2);
        questionText.color = questionColor;
        answerTextGroup.alpha = answerAlpha;
    }
}