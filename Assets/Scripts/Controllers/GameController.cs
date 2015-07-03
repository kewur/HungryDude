using Assets.Scripts.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class GameController : MonoBehaviour, INotifyPropertyChanged
    {
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
        private int _HighScore = 0;
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

      
        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
