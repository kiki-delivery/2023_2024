using UnityEngine;

public class Tongs : MonoBehaviour
{    
    [SerializeField] RectTransform _back;    
    [SerializeField] RectTransform _pickArea;
    [SerializeField] RectTransform _front;
    [SerializeField] RectTransform _spoon;

    public RectTransform RectTransform { get; private set; }
    public CanvasGroup CanvasGroup { get; private set; }
    public Vector3 StartScale { get; private set; }
    public RectTransform PickArea => _pickArea;

    Vector2 _readyPosition = new Vector2(324, 102);

    void Awake()
    {
        RectTransform = GetComponent<RectTransform>();
        CanvasGroup = GetComponent<CanvasGroup>();
        StartScale = RectTransform.localScale;
    }    

    public void Ready()
    {
        RectTransform.anchoredPosition = _readyPosition;
        RectTransform.rotation = Quaternion.Euler(0, 0, 0);
        _back.rotation = Quaternion.Euler(0, 0, 0);
        _front.rotation = Quaternion.Euler(0, 0, 0);
        _spoon.rotation = Quaternion.Euler(0, 0, 3);
        CanvasGroup.alpha = 1;
    }

    public void SetTool(bool useSpoon)
    {
        _back.gameObject.SetActive(!useSpoon);
        _front.gameObject.SetActive(!useSpoon);
        _spoon.gameObject.SetActive(useSpoon);
    }

    public void SetAngle(float value)
    {
        _back.rotation = Quaternion.Euler(0, 0, value);
        _front.rotation = Quaternion.Euler(0, 0, -value);
        _spoon.rotation = Quaternion.Euler(0, 0, 3 - value);
    }
}
