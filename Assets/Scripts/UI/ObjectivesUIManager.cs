using Assets.Scripts.Controllers;
using Assets.Scripts.Entities;
using Assets.Scripts.GameLogic;
using Assets.Scripts.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class ObjectivesUIManager : UIBase
    {
        private RawImageUI[] _ObjectiveImages;

        public FoodTypes objectif;

        protected override void Awake()
        {
            _ObjectiveImages = GetComponentsInChildren<RawImageUI>().OrderBy(obj => int.Parse(obj.name)).ToArray();

            GameController.Instance.CurrentObjectives.CollectionChanged += CurrentObjectives_CollectionChanged;
        }

        private void CurrentObjectives_CollectionChanged(System.Collections.ICollection source, Utilities.Collections.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                for (int i = 0; i < e.NewItems.Count; i++)
                {
                    if (_ObjectiveImages.Length < i)
                        return;

                    _ObjectiveImages[i].Texture = Food.GetFoodImage(((FoodObjective)e.NewItems[i]).FoodType).texture;
                     objectif = ((FoodObjective)e.NewItems[i]).FoodType;
                    _ObjectiveImages[i].Show();
                    ((FoodObjective)e.NewItems[i]).PropertyChanged += ObjectivesUIManager_PropertyChanged;
                }
            }

            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    ((FoodObjective)e.NewItems[i]).PropertyChanged -= ObjectivesUIManager_PropertyChanged;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                for (int i = 0; i < e.OldItems.Count; i++)
                {
                    ((FoodObjective)e.NewItems[i]).PropertyChanged -= ObjectivesUIManager_PropertyChanged;
                }
            }
        }

        private void ObjectivesUIManager_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            
        }
    }
}
