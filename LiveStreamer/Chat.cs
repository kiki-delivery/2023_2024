using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    [SerializeField] TMP_Text _idTmp;
    [SerializeField] TMP_Text _textTmp;

    public RectTransform RectTransform { get; private set; }
    public CanvasGroup CanvasGroup { get; private set; }

    public float YPosition => RectTransform.anchoredPosition.y;
    public float YScale => RectTransform.sizeDelta.y;

    HorizontalLayoutGroup _horizontalLayoutGroup;

    void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();
        _horizontalLayoutGroup = GetComponent<HorizontalLayoutGroup>();
    }    

    public void SetChatText(string id, string text)
    {
        _idTmp.text = id;
        _textTmp.text = text;        
        // ������Ʈ�� enable ���¿��� �۵���
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);

        // �� ���� ���� �� �Ʒ� ������ �ٸ�
        int paddingValue = RectTransform.sizeDelta.y > 34 ? 5 : 0;
        _horizontalLayoutGroup.padding.top = paddingValue;
        _horizontalLayoutGroup.padding.bottom = paddingValue;
        LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
    }        
}
