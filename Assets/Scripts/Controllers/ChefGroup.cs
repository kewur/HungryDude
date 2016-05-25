using Assets.Scripts.Entities;
using Assets.Scripts.UI.Bars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class ChefGroup : MonoBehaviour
    {
        Dictionary<ScreenSide, List<ChefsController>> _Controllers = new Dictionary<ScreenSide, List<ChefsController>>();
        public Dictionary<ScreenSide, List<ChefsController>> Controllers
        {
            get
            {
                return _Controllers;
            }
        }

        void Start()
        {
            CountdownBar.Instance.Groups.Add(this);
        }

        public List<ChefsController> GetMirrorController(ScreenSide side)
        {
            if(!_Controllers.ContainsKey(side))
                return null;

            return _Controllers[side];
        }

        public void ResetFoodCovers()
        {
            foreach (KeyValuePair<ScreenSide, List<ChefsController>> keyValue in _Controllers)
            {
                foreach (ChefsController cont in keyValue.Value)
                {
                    cont.CoverAllFood();
                    cont.OpenFoodsAtRandom(GameController.Instance.OpenFoodCoverAmount);
                }
            }
        }

    }
}
