using Assets.Scripts.Entities;
using Assets.Scripts.GameLogic;
using Assets.Scripts.UI.Bars;
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

        public const float BASE_SCORE_EFFECTOR = 5f;
        public const float PRIMARY_MULTIPLIER = 2f;
        public const float POO_MULTIPLIER = 16f;

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

        public const string EatMirroredFoodsPropertyName = "EatMirroredFoods";
        private bool _EatMirroredFoods = true;
        public bool EatMirroredFoods
        {
            get
            {
                return _EatMirroredFoods;
            }

            set
            {
                if (_EatMirroredFoods == value)
                    return;

                _EatMirroredFoods = value;
                RaisePropertyChanged(EatMirroredFoodsPropertyName);
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

        public const string LevelAdvanceHighScorePropertyName = "LevelAdvanceHighScore";
        private int _LevelAdvanceHighScore = 1500;
        public int LevelAdvanceHighScore
        {
            get { return _LevelAdvanceHighScore; }
            set
            {
                if (_LevelAdvanceHighScore == value)
                    return;

                _LevelAdvanceHighScore = value;
                RaisePropertyChanged(LevelAdvanceHighScorePropertyName);
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

        public static int PooChance = 15;
        public int PooEatenCount = 0;
        public const string PooEnabledPropertyName = "PooEnabled";
        private bool _PooEnabled = true;
        public bool PooEnabled
        {
            get { return _PooEnabled; }
            set
            {
                if (_PooEnabled == value)
                    return;

                _PooEnabled = value;
                RaisePropertyChanged(PooEnabledPropertyName);
                if (PooEnabled)
                    StartCoroutine(PooDecayRoutine());
                else
                    StopCoroutine(PooDecayRoutine());
            }
        }

        public const string PooDecayRatePropertyName = "PooDecayRate";
        private float _PooDecayRate = 1f;
        public float PooDecay
        {
            get { return _PooDecayRate; }
            set
            {
                if (_PooDecayRate == value)
                    return;

                _PooDecayRate = value;
                RaisePropertyChanged(PooDecayRatePropertyName);
            }
        }

        public IEnumerator PooDecayRoutine()
        {
            
            yield return new WaitForEndOfFrame();

        }

        public const string CurrentObjectivesPropertyName = "CurrentObjectives";
        private ObservableCollection<FoodObjective> _CurrentObjectives = new ObservableCollection<FoodObjective>();
        public ObservableCollection<FoodObjective> CurrentObjectives
        {
            get { return _CurrentObjectives; }
            set
            {
                if (_CurrentObjectives == value)
                    return;

                _CurrentObjectives = value;

                if(_CurrentObjectives != null)
                    _CurrentObjectives.CollectionChanged += _CurrentObjective_CollectionChanged;

                RaisePropertyChanged(CurrentObjectivesPropertyName);
            }
        }

        private void _CurrentObjective_CollectionChanged(ICollection source, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(CurrentObjectivesPropertyName);
        }

        public void Start()
        {
            CreateObjectives();
        }

        public void AddScore(int score)
        {
            HighScore += score;
        }

        public void LoseScore(int score)
        {
            HighScore -= score;
        }

        private void PenaltyFoodEaten(Food food, float effectiveness, bool primary)
        {
            HighScore -= (int)(effectiveness * BASE_SCORE_EFFECTOR * (primary && EatMirroredFoods ? PRIMARY_MULTIPLIER : 1f));
        }

        private void ObjectiveFoodEaten(Food food, float effectiveness, bool primary)
        {
            CurrentObjectives.Where(f => f.FoodType == food.FoodType).ToList().ForEach(f => f.Achieved = true);
            HighScore += (int)(effectiveness * BASE_SCORE_EFFECTOR * (primary && EatMirroredFoods ? PRIMARY_MULTIPLIER : 1f));
        }

        /// <summary>
        /// Primary is the food that has been clicked. secondary is the mirror chef's food.
        /// </summary>
        /// <param name="food"></param>
        /// <param name="effectiveness"></param>
        /// <param name="primary"></param>
        public void FoodEaten(Chef sender, Food food, bool primary)
        {
            float effectiveness = BASE_SCORE_EFFECTOR * sender.CurrentPosition;

            if (CurrentObjectives.Any(t =>  t.FoodType == food.FoodType))
                ObjectiveFoodEaten(food, effectiveness, primary);
            else
                PenaltyFoodEaten(food, effectiveness, primary);
        }

        public void CreateObjectives()
        {
            CurrentObjectives.Clear();

            List<FoodTypes> possibleObjectives = Enum.GetValues(typeof(FoodTypes)).Cast<FoodTypes>().ToList();
            possibleObjectives.Remove(FoodTypes.Poo);

            FoodObjective foodObj = new FoodObjective(possibleObjectives.RandomElement());
            CurrentObjectives.Add(foodObj);
        }
       

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
