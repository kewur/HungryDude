using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Assets.Scripts.GameLogic
{
    public class FoodObjective : INotifyPropertyChanged
    {
        private readonly FoodTypes _FoodType = FoodTypes.Meat;
        public FoodTypes FoodType { get { return _FoodType; } }

        public const string AchievedPropertyName = "Achieved";
        private bool _Achieved = false;
        public bool Achieved
        {
            get { return _Achieved; }

            set
            {
                if (_Achieved == value)
                    return;

                _Achieved = value;
                RaisePropertyChanged(AchievedPropertyName);
            }
        }

        public FoodObjective(FoodTypes foodType)
        {
            _FoodType = foodType;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged(string propertyName)
        {
            var propChanged = PropertyChanged;
            if (propChanged != null)
                propChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
