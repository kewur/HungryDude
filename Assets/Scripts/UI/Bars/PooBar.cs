using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI.Bars
{
    public class PooBar : BarBase
    {
        protected override void OnMaximum()
        {
            //spew shit from the bar.
        }

        protected override void Awake()
        {
            base.Awake();

            if (!GameController.Instance.PooEnabled)
            {
                Hide();
                gameObject.transform.localScale = Vector3.zero;
            }

            GameController.Instance.PropertyChanged += GameController_PropertyChanged;
        }

        public void keke()
        {
            Show();
        }

        private void GameController_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == GameController.PooEnabledPropertyName)
            {
                if (GameController.Instance.PooEnabled)
                    Show();
                else
                    Hide();
            }
        }
    }
}
