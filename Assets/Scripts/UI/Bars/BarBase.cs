using Assets.Scripts.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Bars
{
    [RequireComponent(typeof(Slider))]
    public abstract class BarBase : UIBase
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

                if (_CurrentValue == Slider.maxValue)
                {
                    OnMaximum();
                    if (MaximumReached != null)
                        MaximumReached();
                }
            }
        }

        private float _TargetValue;
        public float TargetValue
        {
            get
            {
                return _TargetValue;
            }

            set
            {
                if (_TargetValue == value)
                    return;

                _TargetValue = value;
                iTween.ValueTo(gameObject, iTween.Hash("from", CurrentValue, "to", TargetValue, "time", SliderSpeed, "onupdate", "FillingUp", "name", "GrowText"));
            }
        }

        protected virtual void Update()
        {
        }

        public virtual void FillUp(float duration, Action onComplete = null)
        {
            Hashtable hash = iTween.Hash("from", CurrentValue, "to", Slider.maxValue, "time", duration, "onupdate", "FillingUp", "name", "GrowText");

            if (onComplete != null)
                hash.Add("oncomplete", onComplete);

            iTween.ValueTo(gameObject, hash);

            _TargetValue = Slider.maxValue;
        }

        private void FillingUp(float val)
        {
            CurrentValue = val;
        }

        protected abstract void OnMaximum();
    }
}
