using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

// 음식 테마에 맞게 음식 생성
public class FoodController : MonoBehaviour
{
    public UnityEvent<Food> foodSpawned;

    [SerializeField] AnimationCurve[] _foodMovingCurves;
    [SerializeField] AnimationCurve _foodFadeMoveCurve;

    [SerializeField] Plate[] _plates;
    [SerializeField] RectTransform _tongsParent;
    [SerializeField] Tongs _tongs;
    [SerializeField] RectTransform _foodMovingArea;

    int[] _orderList;
    int _currentOrder;
    Plate _usePlate;
    Food _spawnedFood;

    public void SetOrderList()
    {
        _currentOrder = 0;
        _orderList = Utility.GetRandomNumberArray(_plates.Length, 0);
    }

    public void SpawnFood()
    {
        AppearTongs();                
                
        ////food.RectTransform.DOLocalMoveY(food.RectTransform.anchoredPosition.y + 150, 0.2f).OnComplete(SetFoodMoving);        
    }    

    void AppearTongs()
    {
        int pickPlateNumber = _orderList[_currentOrder];
        _usePlate = _plates[pickPlateNumber];
        _currentOrder++;
        
        _spawnedFood = _usePlate.FoodPool.Get();
        _spawnedFood.Collider2D.enabled = false;
        _spawnedFood.gameObject.SetActive(false);
        
        _tongsParent.anchoredPosition = new Vector2(1293, -140);
        // 음식으로 집게 갈 때 따라가는 용도
        _tongs.transform.SetParent(_tongsParent);
        _tongs.Ready();        
        _tongs.SetTool(_spawnedFood._useSpoon);
        _tongsParent.transform.DOMove(_usePlate.transform.position, 2).OnComplete(GrabFood);
    }    

    void GrabFood()
    {
        _spawnedFood.gameObject.SetActive(true);
        _spawnedFood.RectTransform.SetParent(_tongs.PickArea);
        _spawnedFood.RectTransform.anchoredPosition = Vector2.zero;
        _spawnedFood.meetMouth.AddListener(DissolveFood);
        _spawnedFood.meetMouth.AddListener(MovingFoodInMouth);        
        foodSpawned.Invoke(_spawnedFood);
        _spawnedFood.CanvasGroup.DOFade(1, 0.2f ).OnUpdate(() => _tongs.SetAngle(_spawnedFood.CanvasGroup.alpha * 3)).OnComplete(MoveTongs);                
    }

    void MoveTongs()
    {
        // 집게 움직일 때 영역지정 편하게 하는 용도
        _tongs.transform.SetParent(_foodMovingArea);
        _spawnedFood.Collider2D.enabled = true;
        SetFoodMoving();
    }

    
    void SetFoodMoving()
    {
        // 도착 지점만 고르고 시간, 반복, 회전, 속도는 랜덤으로..        
        Vector2 startPosition = _tongs.RectTransform.anchoredPosition;
        Vector2 targetPostion = GetTargetPosition();
        float movingTime = Random.Range(7, 9) * 0.1f;

        while (Vector2.Distance(startPosition, targetPostion) < 100)
        {
            targetPostion = GetTargetPosition();
        }

        int loopCount = Random.Range(1, 2);
        _tongs.RectTransform.DOKill();
        _tongs.RectTransform.DOLocalMove(targetPostion, movingTime).SetLoops(loopCount, LoopType.Yoyo)
                                    .OnStart(SetMovingOption).OnComplete(SetFoodMoving);
    }

    void SetMovingOption()
    {
        bool isRotation = Random.Range(0, 2) == 1 ? true : false;
        bool isScaleChange = Random.Range(0, 2) == 1 ? true : false;

        if(_spawnedFood._useSpoon)
        {
            isRotation = false;
        }

        if (isRotation)
        {
            float rotationTime = Random.Range(5, 11) * 1f;
            _tongs.RectTransform.DORotate(new Vector3(0, 0, 360), rotationTime, RotateMode.LocalAxisAdd);
        }        

        float scaleTime = Random.Range(9, 21) * 0.1f;

        if (isScaleChange)
        {
            float targetScale = Random.Range(8, 13) * 0.1f;            
            _tongs.RectTransform.DOScale(_tongs.StartScale * targetScale, scaleTime);
        }
        else
        {                        
            _tongs.RectTransform.DOScale(_tongs.StartScale, scaleTime);
        }
    }

    Vector2 GetTargetPosition()
    {        
        int x = Random.Range(-500, 500);
        int y = Random.Range(100, 250);

        if (_spawnedFood._useSpoon)
        {
            x = Random.Range(-500, 800);
            y = Random.Range(100, 350);
        }
        return new Vector2(x, y);
    }

    void DissolveFood(Food food)
    {
        _tongs.RectTransform.DOKill();
        _tongs.CanvasGroup.DOFade(0f, 1).SetEase(_foodFadeMoveCurve).OnComplete(() => OnDissolved(food));
    }    

    void OnDissolved(Food food)
    {
        food.Collider2D.enabled = false;
        _usePlate.FoodPool.Release(food);        
        food.EatMouth.OnFoodEated();
    }

    void MovingFoodInMouth(Food food)
    {
        food.transform.DOMove(food.EatMouth.transform.position, 1).SetEase(_foodFadeMoveCurve);
    }    

    public void ClearSpawnFood()
    {
        _spawnedFood?.RectTransform.DOKill();
        _spawnedFood?.CanvasGroup.DOFade(0, 0.3f).OnComplete(() => _usePlate.FoodPool.Release(_spawnedFood));
        _tongs.CanvasGroup.DOFade(0, 0.3f);
    }
}
