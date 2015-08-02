using Assets.Scripts.Controllers;
using System;
using System.Collections;
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

        private static int SmallFontSize = 40;
        private static int BigFontSize = 70;
        private static float SizeChangeTime = 0.1f;


        private Color NormalColor = new Color(1f, 1f, 0f);
        private Color IncreaseColor = Color.green;
        private Color DecreaseColor = Color.red;

        public float ScaleTime = 0.7f;

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

                if (_Value < value)
                    HighScoreText.color = IncreaseColor;
                else
                    HighScoreText.color = DecreaseColor;

              //  iTween.StopByName(gameObject, "GrowText");
                iTween.Stop(gameObject);

                if (IsInvoking("ShrinkText"))
                    CancelInvoke("ShrinkText");

                if (IsInvoking("BackToNormalColor"))
                    CancelInvoke("BackToNormalColor");
                    
                HighScoreText.fontSize = SmallFontSize;

                _Value = value;
                HighScoreText.text = _Value.ToString();
               
                iTween.ValueTo(gameObject, iTween.Hash("from", SmallFontSize, "to", BigFontSize, "time", SizeChangeTime, "onupdate", "TextChange", "name", "GrowText"));
                Invoke("ShrinkText", SizeChangeTime);
            }
        }

        void Awake()
        {
            GameController.Instance.PropertyChanged += Instance_PropertyChanged;
            HighScoreText.text = GameController.Instance.HighScore.ToString();
            HighScoreText.color = NormalColor;
        }

        private void Instance_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GameController.HighScorePropertyName)
                Value = GameController.Instance.HighScore;
        }

        void TextChange(int size)
        {
            HighScoreText.fontSize = size;
        }

        void ShrinkText()
        {
            iTween.ValueTo(gameObject, iTween.Hash("from", BigFontSize, "to", SmallFontSize, "time", SizeChangeTime, "onupdate", "TextChange", "name", "ShrinkText"));
            BackToNormalColor();
        }

        void BackToNormalColor()
        {
            iTween.ValueTo(gameObject, iTween.Hash("from", HighScoreText.color, "to", NormalColor, "time", 0.3f, "onupdate", "ColorChange", "name", "ColorChange"));
        }

        void ColorChange(Color color)
        {
            HighScoreText.color = color;
        }
    }
}
