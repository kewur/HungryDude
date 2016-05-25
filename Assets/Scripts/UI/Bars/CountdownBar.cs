using Assets.Scripts.Controllers;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI.Bars
{
    public class CountdownBar : BarBase
    {
        private static CountdownBar _Instance = null;
        public static CountdownBar Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<CountdownBar>();
                    if (_Instance == null)
                    {
                        GameObject mainPanel = GameObject.FindGameObjectWithTag(Tags.MainUIPanel);
                        if (mainPanel == null)
                            throw new NullReferenceException("Main panel is not tagged, or does not exists");

                        _Instance = Instantiate(Resources.Load<GameObject>("CountDownSlider")).GetComponent<CountdownBar>();
                    }
                }

                return _Instance;
            }
        }

        public List<ChefGroup> _Groups = new List<ChefGroup>();
        public List<ChefGroup> Groups
        {
            get
            {
                return _Groups;
            }
        }

        public override void FillUp(float duration, Action onComplete = null)
        {
            foreach(ChefGroup group in Groups)
                group.ResetFoodCovers();

            base.FillUp(duration, onComplete);
        }

        protected override void OnMaximum()
        {
            CurrentValue = 0;
            //FillUp(GameController.Instance.OpenFoodCoverTime);
        }
    }
}
