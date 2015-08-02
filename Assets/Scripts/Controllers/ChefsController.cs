using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Utilities;
using Assets.Scripts.Entities;
using System.Collections;

namespace Assets.Scripts.Controllers
{
    public class ChefsController : MonoBehaviour
    {
        private Transform[] _Locations;

        private ChefGroup _ChefGroup;

        private List<Chef> Chefs;

        public int LocationCount
        {
            get
            {
                if (_Locations == null)
                    return 0;

                return _Locations.Length;
            }
        }

        public int BackOfTheLineIndex
        {
            get
            {
                return _Locations.Length - 1;
            }
        }

        public Transform BackOfTheLine
        {
            get { return GetLocation(_Locations.Length - 1); }
        }

        private bool MovementStopped;

        private void Awake()
        {
            Chefs = GetComponentsInChildren<Chef>().ToList();
            _ChefGroup = GetComponentInParent<ChefGroup>();

            _Locations = transform.GetChildrenWithTag(Tags.ChefLocation);
            _Locations = _Locations.OrderBy(item => int.Parse(item.name)).ToArray();
        }

        private void OnEnable()
        {
            StartCoroutine(MoveChefsCoroutine());
        }

        private void OnDisable()
        {
            StopCoroutine(MoveChefsCoroutine());
        }

        public Chef GetMirrorChef(Chef chef)
        {
           return _ChefGroup.GetMirrorController(chef.ChefSide).GetChefAtLocation(chef.CurrentPosition);
        }

        public Transform GetLocation(int index)
        {
            if (index > _Locations.Length - 1)
            {
                Debug.LogError("Out of range");
                return null;
            }

            return _Locations[index];
        }

        public void StopChefs()
        {
            MovementStopped = true;
            StopCoroutine(MoveChefsCoroutine());
        }

        public void MoveChefs()
        {
            MovementStopped = false;
            StopCoroutine(MoveChefsCoroutine());
            StartCoroutine(MoveChefsCoroutine());
        }

        private IEnumerator MoveChefsCoroutine()
        {
            while (!GameController.Instance.GamePaused)
            {
                yield return new WaitForSeconds(GameController.Instance.ChefMoveInterval);
                foreach (Chef chef in Chefs)
                    if(!MovementStopped) chef.MoveRight();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="alignment"></param>
        /// <param name="location"></param>
        /// <returns>null if not present or the Chef at Chef.CurrentPosition == location</returns>
        public Chef GetChefAtLocation(int location)
        {
            //if the dictionary doesn't have the alignment or the list is null or out of bounds return null
            if (Chefs == null || Chefs.Count - 1 < location)
                return null;

            foreach (Chef chef in Chefs)
            {
                if (chef.CurrentPosition == location)
                    return chef;
            }

            return null;
        }


    }
}
