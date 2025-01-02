using Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CardManager : MonoBehaviour
{    
    public int cardCount = 6;

    [SerializeField] CardObj _cardObj;
    [SerializeField] Sprite _backImage;
    [SerializeField] Sprite[] _frontBGImages;
    [SerializeField] Sprite[] _frontObjImages;
    [SerializeField] Transform _gameField;

    CardObj _pickedCardObj1;
    CardObj _pickedCardObj2;

    GameManager _gameManager;    
    GridLayoutGroup _gridLayout;
    List<CardObj> _allCards = new List<CardObj>();
    Queue<CardObj> _pickedCardObjQueue = new Queue<CardObj>();

    bool _isCorrect;
    bool _isClear;

    int _currentCorrectCount = 0;
    int _pickedCount = 0;
    int _targetCorrectCount = 6;

    [SerializeField] private AudioCtrl audioCtrl;

    public bool IsClear => _isClear;

    public int CurrentSuccessCount
    {
        get => _currentCorrectCount;
        set
        {
            _currentCorrectCount = value;

            if (_currentCorrectCount == _targetCorrectCount)
            {
                _isClear = true;
                _gameManager.StopTimer();
            }
        }
    }

    public void Setting(GameManager gameManager)
    {
        _gameManager = gameManager;

        _gridLayout = _gameField.GetComponent<GridLayoutGroup>();
        _gridLayout.cellSize = _gridLayout.cellSize * (1 - (cardCount - 6) * 0.1f);


        _targetCorrectCount = cardCount;

        for (int cardNumber = 0; cardNumber < cardCount; cardNumber++)
        {
            InstantiateCardObj(cardNumber);
            InstantiateCardObj(cardNumber);
        }

        void InstantiateCardObj(int cardCount)
        {
            Card card = new Card(_frontBGImages[cardCount], _frontObjImages[cardCount], _backImage, cardCount);
            CardObj cardObj = Instantiate(_cardObj, _gameField);
            cardObj.Setting(card, this);
            _allCards.Add(cardObj);
        }
    }

    public void SetClickBlock(bool isBlock)
    {
        _gameManager.ClickBlock.SetActive(isBlock);
    }

    public void Init()
    {
        _pickedCardObjQueue.Clear();
        _currentCorrectCount = 0;        
        _pickedCount = 0;
        _isClear = false;        
        StopAllCoroutines();

        SetRandomImage();
        SetRandomPosition();

        foreach (CardObj cardObj in _allCards)
        {
            cardObj.SetState(CardState.Idle);
        }

        void SetRandomImage()
        {
            int[] randomNumberArray = Utility.GetRandomNumberArray(_frontBGImages.Length, 0);

            foreach (CardObj cardObj in _allCards)
            {
                int cardImageNumber = randomNumberArray[cardObj.Number];
                cardObj.SetImage(_frontBGImages[cardImageNumber], _frontObjImages[cardImageNumber]);
            }
        }

        void SetRandomPosition()
        {
            int[] randomNumberArray = Utility.GetRandomNumberArray(cardCount * 2, 0);
            int idx = 0;

            foreach (CardObj cardObj in _allCards)
            {
                int newPosition = randomNumberArray[idx];
                cardObj.transform.SetSiblingIndex(newPosition);
                idx++;
            }
        }
    }    

    public void PreviewStart()
    {        
        foreach(CardObj cardObj in _allCards)
        {
            cardObj.IsPreviewMode(true);
            cardObj.SetState(CardState.Turn);
        }
    }

    public void PreviewEnd()
    {        
        foreach (CardObj cardObj in _allCards)
        {
            cardObj.SetState(CardState.Wrong);
        }
    }

    public void SetPlayMode()
    {        
        _gameManager.StartGame();

        foreach (CardObj cardObj in _allCards)
        {
            cardObj.SetState(CardState.Idle);
        }
    }


    public void PickCard(CardObj cardObj)
    {
        _pickedCount++;
        _pickedCardObjQueue.Enqueue(cardObj);

        if(_pickedCount == 2)
        {
            CheckSameCards();
            SetClickBlock(true);            
        }

        void CheckSameCards()
        {
            _pickedCardObj1 = _pickedCardObjQueue.Dequeue();
            _pickedCardObj2 = _pickedCardObjQueue.Dequeue();

            _isCorrect = _pickedCardObj1.Number == _pickedCardObj2.Number;

            if(_isCorrect)
            {
                CurrentSuccessCount++;
            }            
        }
    }    

    public void ActionOfResult()
    {
        if(_pickedCount != 2)
        {
            return;
        }

        if(_pickedCardObj2.IsTurnning)
        {
            return;
        }


        if(_isCorrect)
        {
            audioCtrl.CorrectAudioPlay();
            _pickedCardObj1.SetState(CardState.Correct);
            _pickedCardObj2.SetState(CardState.Correct);
            SetClickBlock(false);
        }
        else
        {
            audioCtrl.WrongAudioPlay();
            _pickedCardObj1.SetState(CardState.Wrong);
            _pickedCardObj2.SetState(CardState.Wrong);
        }

        if(_isClear)
        {            
            StartCoroutine(SuccessAnimationCoroutine());
        }

        _pickedCount = 0;

        IEnumerator SuccessAnimationCoroutine()
        {
            yield return new WaitForSeconds(1f);
            _gameManager.Success();
        }
    }    
}
