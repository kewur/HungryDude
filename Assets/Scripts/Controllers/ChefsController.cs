using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Utilities;
using Assets.Scripts.Entities;
using System.Collections;
using Assets.Scripts.Entities.Events;
using Assets.Scripts.Utilities.Invoker;

namespace Assets.Scripts.Controllers
{
    public class ChefsController : MonoBehaviour
    {
        private Transform[] _Locations;

        private ChefGroup _ChefGroup;
        private List<Chef> Chefs;

        public int BackOfTheLineIndex
        {
            get
            {
                return _Locations.Length - 1;
            }
        }

        private bool MovementStopped = true;

        private void Awake()
        {
            Chefs = GetComponentsInChildren<Chef>().ToList();

            foreach (Chef c in Chefs)
                c.OnInteraction += ChefInteracted;

            _ChefGroup = GetComponentInParent<ChefGroup>();

            _Locations = transform.GetChildrenWithTag(Tags.ChefLocation);
            _Locations = _Locations.OrderBy(item => int.Parse(item.name)).ToArray();
        }

        private void ChefInteracted(ChefInteractedEventArgs e)
        {
            Chef c = e.Source as Chef;
            MoveAllChefs();
            _ChefGroup.GetMirrorController(c.ChefSide).MoveAllChefs();
        }

        public Chef GetMirrorChef(Chef chef)
        {
           return _ChefGroup.GetMirrorController(chef.ChefSide).GetChefAtLocation(chef.CurrentPosition);
        }

        public Transform GetLocation(int index)
        {
            if (index > _Locations.Length - 1)
                return null;

            return _Locations[index];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="location"></param>
        /// <returns>null if not present or the Chef at Chef.CurrentPosition == location</returns>
        public Chef GetChefAtLocation(int location)
        {
            foreach (Chef c in Chefs)
                if (c.CurrentPosition == location)
                    return c;

            Debug.LogError("No chef at this location");
            return null;
        }

        public void MoveAllChefs()
        {
            MoveChefs(BackOfTheLineIndex, 0);
        }

        public void MoveChefsBehindThis(Chef c)
        {
            if(c.CurrentPosition != BackOfTheLineIndex)
                MoveChefs(BackOfTheLineIndex, c.CurrentPosition + 1);
        }

        private void MoveChefs(int startIndex, int endIndex)
        {
            if (!MovementStopped)
                return;

            MovementStopped = false;
            Invoker.WaitThanCallback(() => { MovementStopped = true; }, Chef.ChefMovespeed);

            if (!this.MovementStopped)
            {
                    for (int i = startIndex; i >= endIndex; i--)
                        if (!MovementStopped) GetChefAtLocation(i).Move();
            }
        }
    }
}
