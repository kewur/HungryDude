using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI.Bars
{
    public class EatingBar : BarBase
    {
        protected override void Awake()
        {
            base.Awake();
            Player.Instance.PropertyChanged += Player_PropertyChanged;
        }

        private void Player_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == Player.EatingPropertyName)
            {
                IsHidden = !Player.Instance.Eating;

                if(Player.Instance.Eating)
                    FillUp(Player.Instance.EatingSpeed, ()=> { IsHidden = true; });
            }
        }

        protected override void OnMaximum()
        {
            Player.Instance.EatingDone();
            CurrentValue = 0f;
        }
    }
}
