using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class ChefGroup : MonoBehaviour
    {
        public ChefAlignment Side1;
        public ChefsController Side1Controller;

        public ChefAlignment Side2;
        public ChefsController Side2Controller;

        void Awake()
        {
            
        }

        public ChefsController GetMirrorController(ChefAlignment side)
        {
            if (Side1 == side)
                return Side2Controller;

            if (Side2 == side)
                return Side1Controller;

            return null;
        } 

    }
}
