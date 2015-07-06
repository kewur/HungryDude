using Assets.Scripts.Entities;
using Assets.Scripts.GameLogic;
using Assets.Scripts.GameLogic.Bars;
using Assets.Scripts.Utilities;
using Assets.Scripts.Utilities.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour, INotifyPropertyChanged
    {

        public const float BASE_SCORE_EFFECTOR = 20f;
        public const float PRIMARY_MULTIPLIER = 2f;

        private static GameController _Instance = null;
        public static GameController Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<GameController>();
                    if (_Instance == null)
                        _Instance = new GameObject("Game Controller").AddComponent<GameController>();
                }

                return _Instance;
            }
        }

        private PooBar _PooBarSlider = null;
        public PooBar PooBarSlider
        {
            get
            {
                if (_PooBarSlider == null)
                    _PooBarSlider = FindObjectOfType<PooBar>();

                return _PooBarSlider;
            }
        }

        private PowerBar _PowerBarSlider = null;
        public PowerBar PowerBarSlider
        {
            get
            {
                if (_PowerBarSlider == null)
                    _PowerBarSlider = FindObjectOfType<PowerBar>();

                return _PowerBarSlider;
            }
        }

        public const string GameLogicPropertyName = "GameLogic";
        private GameLogicBase _GameLogic = null;
        public GameLogicBase GameLogic
        {
            get { return _GameLogic; }
            set
            {
                if (_GameLogic == value)
                    return;

                _GameLogic = value;
                RaisePropertyChanged(GameLogicPropertyName);
            }
        }

        public const string GamePausedPropertyName = "GamePaused";
        private bool _GamePaused = false;
        public bool GamePaused
        {
            get { return _GamePaused; }
            set
            {
                if (_GamePaused == value)
                    return;

                _GamePaused = value;

                if (_GamePaused)
                    Time.timeScale = 0f;
                else
                    Time.timeScale = 1f;

                RaisePropertyChanged(GamePausedPropertyName);
            }
        }

        public const string HighScorePropertyName = "HighScore";
        private int _HighScore = 100;
        public int HighScore
        {
            get { return _HighScore; }
            private set
            {
                if (_HighScore == value)
                    return;

                _HighScore = value;
                RaisePropertyChanged(HighScorePropertyName);
            }
        }

        public const string ChefMoveIntervalPropertyName = "ChefMoveInterval";
        private float _ChefMoveInterval = 1.5f;
        public float ChefMoveInterval
        {
            get { return _ChefMoveInterval; }
            set
            {
                if (_ChefMoveInterval == value)
                    return;

                _ChefMoveInterval = value;
                RaisePropertyChanged(ChefMoveIntervalPropertyName);
            }
        }

        public void Awake()
        {
            Invoke("keke", 2f);
        }

        private void  keke()
        {
            HighScore = 500;
        }

        public void AddScore(int score)
        {
            HighScore += score;
        }

        public void LoseScore(int score)
        {
            HighScore -= score;
        }

        public static int PooChance = 15;
        public static bool PooEnabled = true;

        public const string CurrentObjectivePropertyName = "CurrentObjective";
        private ObservableCollection<Tuple<FoodTypes, int>> _CurrentObjective = new ObservableCollection<Tuple<FoodTypes, int>>();
        public ObservableCollection<Tuple<FoodTypes, int>> CurrentObjective
        {
            get { return _CurrentObjective; }
            set
            {
                if (_CurrentObjective == value)
                    return;

                _CurrentObjective = value;

                if(_CurrentObjective != null)
                    _CurrentObjective.CollectionChanged += _CurrentObjective_CollectionChanged;

                RaisePropertyChanged(CurrentObjectivePropertyName);
            }
        }

        private void _CurrentObjective_CollectionChanged(ICollection source, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(CurrentObjectivePropertyName);
        }

        /// <summary>
        /// Primary is the food that has been clicked. secondary is the mirror chef's food.
        /// </summary>
        /// <param name="food"></param>
        /// <param name="effectiveness"></param>
        /// <param name="primary"></param>
        public void FoodEaten(Food food, float effectiveness, bool primary)
        {
            foreach (Tuple<FoodTypes, int> t in CurrentObjective)
                if (food.FoodType == t.Item1)
                {
                    ObjectiveFoodEaten(food, effectiveness, primary);
                    if (food.FoodType == FoodTypes.Poo)
                        PenaltyFoodEaten(food, effectiveness, primary);

                    return;
                }

            PenaltyFoodEaten(food, effectiveness, primary);
        }

        private void PenaltyFoodEaten(Food food, float effectiveness, bool primary)
        {
            if(food.FoodType == FoodTypes.Poo)
            {
               //do poo stuff
            }
            else
                HighScore -= (int)(effectiveness * BASE_SCORE_EFFECTOR * (primary ? 1f : PRIMARY_MULTIPLIER));
        }

        private void ObjectiveFoodEaten(Food food, float effectiveness, bool primary)
        {

        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
