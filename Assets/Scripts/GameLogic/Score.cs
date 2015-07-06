using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic
{
    [RequireComponent(typeof(Text))]
    public class Score : MonoBehaviour
    {
        private Text _HighScoreText = null;
        public Text HighScoreText
        {
            get
            {
                if (_HighScoreText == null)
                    _HighScoreText = GetComponent<Text>();

                return _HighScoreText;
            }
        }

        private int _Value = 0;
        public int Value
        {
            get
            {
                return _Value;
            }

            set
            {
                if (_Value == value)
                    return;

                _Value = value;
                HighScoreText.text = _Value.ToString();
            }
        }

        void Awake()
        {
            GameController.Instance.PropertyChanged += Instance_PropertyChanged;
            Value = GameController.Instance.HighScore;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GameController.HighScorePropertyName)
                Value = GameController.Instance.HighScore;
        }
    }
}
