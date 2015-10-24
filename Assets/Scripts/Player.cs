using UnityEngine;
using System.Collections;
using System.ComponentModel;
using System;
using Assets.Scripts.Entities;
using Assets.Scripts.Utilities;
using Assets.Scripts.Controllers;


public enum ScreenSide
{
    Left,
    Right
}

public class Player : MonoBehaviour, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    private Transform _EatingFoodPosition;
    public Transform EatingFoodPosition
    {
        get
        {
            if (_EatingFoodPosition == null)
                _EatingFoodPosition = GameObject.FindGameObjectWithTag(Tags.FoodPosition).transform;

            return _EatingFoodPosition;
        }
    }

    public const string FoodToMouthTimePropertyName = "FoodToMouthTime";
    private float _FoodToMouthTime = 0.8f;
    public float FoodToMouthTime
    {
        get { return _FoodToMouthTime; }
        set
        {
            if (_FoodToMouthTime == value)
                return;

            _FoodToMouthTime = value;
            RaisePropertyChanged(FoodToMouthTimePropertyName);
        }
    }

    public void RaisePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    private static Player _Instance = null;
    public static Player Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = FindObjectOfType<Player>();

            return _Instance;
        }
    }

    public const string EatingSpeedPropertyName = "EatingSpeed";
    private float _EatingSpeed = 0.1f;
    public float EatingSpeed
    {
        get { return _EatingSpeed; }
        set
        {
            if (_EatingSpeed == value)
                return;

            _EatingSpeed = value;
            RaisePropertyChanged(EatingSpeedPropertyName);
        }
    }

    public const string EatingPropertyName = "Eating";
    private bool _Eating = false;
    public bool Eating
    {
        get { return _Eating; }
        set
        {
            if (_Eating == value)
                return;

            _Eating = value;
            RaisePropertyChanged(EatingPropertyName);
        }
    }

    // Use this for initialization
    void Start()
    {
        GameController.Instance.PropertyChanged += GameController_PropertyChanged;
    }

    private void GameController_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == GameController.HighScorePropertyName)
        {

        }
    }

    public void EatingDone()
    {
        Eating = false;
        foreach (Transform t in EatingFoodPosition)
            Destroy(t.gameObject);

        //play dissapearing food
    }

    public void EatFood(Chef sender, Food carryingFood)
    {
        MoveFoodToMouth(carryingFood,
            () =>
            {
                Eating = true;
                GameController.Instance.FoodEaten(sender, carryingFood, true);
            }
            );

        if (GameController.Instance.EatMirroredFoods)
        {
            ////also eat the food on the opposite side.
            //Chef mirrorChef = sender.GetMirrorChef();
            //if (mirrorChef.CarryingFood != null)
            //    mirrorChef.CarryingFood.Interact();
        }
    }

    public void MoveFoodToMouth(Food food, Action OnComplete = null)
    {

        food.transform.parent = null;

        Vector3 startPosition = food.transform.position;

        Vector3 endPosition = EatingFoodPosition.position;

        Vector3 midPosition = new Vector3((startPosition.x + endPosition.x) / 2, endPosition.y * (3 / 2), endPosition.z + 0.5f);

        Action completeCallback = () =>
        {
            if (OnComplete != null)
                OnComplete();

            food.transform.parent = EatingFoodPosition;
        };

        iTween.MoveTo(food.gameObject, iTween.Hash("path", new Vector3[] { startPosition, midPosition, endPosition }, "time", FoodToMouthTime, "oncomplete", completeCallback));
    }

}
