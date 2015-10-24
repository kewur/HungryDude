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
        private Transform ClosestSlot;
        private List<Transform> ChefLocations = new List<Transform>(); //0 is closestSlot, last item is the farthest.
        private GameObject ChefLocationsGroup;

        List<Chef> _Chefs = new List<Chef>();
        public List<Chef> Chefs
        {
            get { return _Chefs; }
            set
            {
                if (_Chefs == value)
                    return;

                _Chefs = value;
            }
        }

        public ScreenSide Side
        {
            get
            {
                if (transform.position.x < Player.Instance.transform.position.x)
                    return ScreenSide.Left;
                else
                    return ScreenSide.Right;
            }
        }

        private void Awake()
        {
            InitializeClosestTransfrorm();
            UpdateChefLocations();

            Chefs = GetComponentsInChildren<Chef>().ToList();

            if(Side ==  ScreenSide.Left)
                Chefs = Chefs.OrderByDescending((c) => (c.transform.position.x)).ToList();
            else
                Chefs = Chefs.OrderBy((c) => (c.transform.position.x)).ToList();


            if (Chefs.Count != ChefLocations.Count)
                Debug.LogError("Chef count and location count is different");

            for (int i = 0; i < ChefLocations.Count; i++)
                Chefs[i].TargetPosition = ChefLocations[i];

            Invoker.CallWithIntervals(()=> 
            { MoveAll(); }, int.MaxValue, 3f);
        }

        private void InitializeClosestTransfrorm()
        {
            if (ClosestSlot == null)
            {
                ClosestSlot = new GameObject("ClosestSlot").transform;
                ClosestSlot.tag = Tags.ChefLocation;

                if (Side == ScreenSide.Left)
                    ClosestSlot.transform.position = new Vector3(-2, Player.Instance.transform.position.y, 0);
                else
                    ClosestSlot.transform.position = new Vector3(2, Player.Instance.transform.position.y, 0);

                ClosestSlot.parent = transform;
            }
        }

        public void MoveAll()
        {
            ChefLocations.Shift(false);
            for (int currentChef = 0; currentChef < Chefs.Count; currentChef++)
                Chefs[currentChef].TargetPosition = ChefLocations[currentChef];
        }

        private void UpdateChefLocations()
        {
            DestroyAllChefLocations();

            float VerticalScale = Camera.main.orthographicSize * 2;
            float HorizontalScale = VerticalScale * Camera.main.aspect;

            Vector2 otherEnd = Vector2.zero;

            if (Side == ScreenSide.Right)
                otherEnd = new Vector2(HorizontalScale / 2, ClosestSlot.position.y);
            else
                otherEnd = new Vector2(-HorizontalScale / 2, ClosestSlot.position.y);

            ChefLocations = new List<Transform>();

            ChefLocationsGroup = new GameObject("Chef Locations Group");
            ChefLocationsGroup.transform.parent = transform;
            ChefLocationsGroup.transform.localPosition = Vector3.zero;

            Vector2 currentLocation = ClosestSlot.position;
            int index = 0;
            if (Side == ScreenSide.Right)
            {
                while (currentLocation.x <= otherEnd.x + 2f)
                {
                    Transform newLocation = new GameObject(index++.ToString()).transform;
                    newLocation.transform.parent = ChefLocationsGroup.transform;
                    newLocation.transform.position = currentLocation;
                    ChefLocations.Add(newLocation);
                    currentLocation.x += 2;
                }
            }
            else
            {
                while (currentLocation.x >= otherEnd.x - 2f)
                {
                    Transform newLocation = new GameObject(index++.ToString()).transform;
                    newLocation.transform.parent = ChefLocationsGroup.transform;
                    newLocation.transform.position = currentLocation;
                    ChefLocations.Add(newLocation);
                    currentLocation.x -= 2;
                }
            }

            Camera.main.transform.hasChanged = false; //if the camera position has changed, the screen can hold more chefs, this is checked on the FixedUpdate function.
        }

        void FixedUpdate()
        {
            if (Camera.main.transform.hasChanged)
                UpdateChefLocations();
        }

        private void DestroyAllChefLocations()
        {
            if (ChefLocations == null || ChefLocations.Count == 0)
            {
                ChefLocations = new List<Transform>();
                return;
            }

            foreach (Transform t in ChefLocations)
                Destroy(t.gameObject);

           if(ChefLocationsGroup != null)
                Destroy(ChefLocationsGroup);

            ChefLocations.Clear();
            ChefLocations = null;
            ChefLocations = new List<Transform>();
        }

        public void EatFood(Chef c, Food food)
        {
            GameController.Instance.FoodEaten(c, food, true);
        }

        public void SendToBack(Chef c)
        {

        }
    }
}
