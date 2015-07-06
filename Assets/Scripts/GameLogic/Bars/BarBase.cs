using Assets.Scripts.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.GameLogic.Bars
{
    [RequireComponent(typeof(Slider))]
    public abstract class BarBase : MonoBehaviour
    {
        public event Action MaximumReached;

        private Slider _Slider = null;
        public Slider Slider
        {
            get
            {
                if (_Slider == null)
                {
                    _Slider = GetComponent<Slider>();
                    _Slider.minValue = 0;
                    _Slider.maxValue = 100;
                }

                return _Slider;
            }
        }

        const float SliderSpeed = 2f;

        private float _CurrentValue = 0f;
        protected float CurrentValue
        {
            get { return _CurrentValue; }
            set
            {
                _CurrentValue = value;
                Slider.value = _CurrentValue;
            }
        }

        [Range(0.0f, 100.0f)]
        public float TargetValue = 0f;

        protected virtual void Update()
        {
            if (CurrentValue < TargetValue || CurrentValue > TargetValue)
            {
                CurrentValue = Mathf.Lerp(CurrentValue, TargetValue, Time.fixedDeltaTime * SliderSpeed);
            }
        }

        protected abstract void OnMaximum();
    }
}
