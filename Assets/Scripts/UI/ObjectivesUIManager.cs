using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ObjectivesUIManager : UIBase
    {
        public RawImageUI[] DoubleObjectives;

        private bool _HideDoubleObjectives = false;
        public bool HideDoubleObjectives
        {
            get { return _HideDoubleObjectives; }
            set
            {
                if (_HideDoubleObjectives == value)
                    return;

                _HideDoubleObjectives = value;

                if (!_HideDoubleObjectives) //don't let the other objectives get in the way. Hide if this is becoming visible.
                    HideSingleObjective = true;

                foreach (RawImageUI img in DoubleObjectives)
                    img.Hide = value;
            }
        }

        private bool _HideSingleObjective = false;
        public bool HideSingleObjective
        {
            get { return _HideSingleObjective; }
            set
            {
                if (_HideSingleObjective == value)
                    return;

                _HideSingleObjective = value;

                if (!_HideSingleObjective) //don't let the other objectives get in the way. Hide if this is becoming visible.
                    HideDoubleObjectives = true;

                SingleObjective.Hide = value;
            }
        }

        public RawImageUI SingleObjective;

        protected override void Awake()
        {
            GameController.Instance.PropertyChanged += GameController_PropertyChanged;
        }

        private void GameController_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == GameController.CurrentObjectivesPropertyName)
            {
                if(GameController.Instance.CurrentObjectives == null)
                {

                }
            }
        }
    }
}
