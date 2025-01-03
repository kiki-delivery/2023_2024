using DG.Tweening;
using EnumTypes;
using UnityEngine;

public class TableController : MonoBehaviour
{
    [SerializeField] AnimationCurve _animationCurve;
    [SerializeField, Range(0.2f, 0.5f)] float _plateSetTime;

    [SerializeField] RectTransform _table;
    [SerializeField] Table _koreaFoodTable;
    [SerializeField] Table _schoolFoodTable;
    [SerializeField] Table _chickenTable;
    [SerializeField] Table _holidayTable;
    [SerializeField] Table _dessertTable;
    [SerializeField] Plate[] _plates;
    [SerializeField] Food[] _koreaFoods;
    [SerializeField] Food[] _schoolFood;
    [SerializeField] Food[] _chickens;
    [SerializeField] Food[] _holidayFoods;
    [SerializeField] Food[] _dessertFoods;

    Food[] _useFoods;
    Table _currentTable;

    float _tableStartY;

    void Awake()
    {
        _tableStartY = _table.anchoredPosition.y;
    }

    // ���ø��� �׸��� �´� ���� �������� ����
    public void ReadyPlate(FoodTheme foodTheme)
    {        
        SetTable(foodTheme);
        SetUseFood(foodTheme);
        SetPlate();
        AppearTable();
        // ���� �����ϴ� ������
    }

    void SetTable(FoodTheme foodTheme)
    {
        _koreaFoodTable.gameObject.SetActive(FoodTheme.KoreaFood == foodTheme);
        _schoolFoodTable.gameObject.SetActive(FoodTheme.SchoolFood == foodTheme);
        _chickenTable.gameObject.SetActive(FoodTheme.Chicken == foodTheme);
        _holidayTable.gameObject.SetActive(FoodTheme.Holiday == foodTheme);
        _dessertTable.gameObject.SetActive(FoodTheme.Dessert == foodTheme);

        switch (foodTheme)
        {
            case FoodTheme.KoreaFood:
                _currentTable = _koreaFoodTable;
                break;
            case FoodTheme.SchoolFood:
                _currentTable = _schoolFoodTable;
                break;
            case FoodTheme.Chicken:
                _currentTable = _chickenTable;
                break;
            case FoodTheme.Holiday:
                _currentTable = _holidayTable;
                break;
            case FoodTheme.Dessert:
                _currentTable = _dessertTable;
                break;
        }
    }

    void SetUseFood(FoodTheme foodTheme)
    {
        switch (foodTheme)
        {
            case FoodTheme.KoreaFood:
                _useFoods = _koreaFoods;
                break;
            case FoodTheme.SchoolFood:
                _useFoods = _schoolFood;
                break;
            case FoodTheme.Chicken:
                _useFoods = _chickens;
                break;
            case FoodTheme.Holiday:
                _useFoods = _holidayFoods;
                break;
            case FoodTheme.Dessert:
                _useFoods = _dessertFoods;
                break;
        }
    }

    void SetPlate()
    {
        for (int i = 0; i < _plates.Length; i++)
        {                        
            _plates[i].SetFood(_useFoods[i]);
            _plates[i].RectTransform.position = _currentTable.FoodZones[i].position;
        }
    }

    void AppearTable()
    {
        _table.DOAnchorPosY(0, _plateSetTime).SetEase(_animationCurve);
    }

    public void ClearAllPlate()
    {
        // ���̺� �������� �ִϸ��̼�
        _table.DOAnchorPosY(_tableStartY, _plateSetTime).SetEase(_animationCurve);        
    }
}
