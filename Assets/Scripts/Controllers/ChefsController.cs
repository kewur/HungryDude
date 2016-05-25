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
        private Transform _ClosestSlot;
        private List<Transform> _ChefLocations = new List<Transform>(); //0 is closestSlot, last item is the farthest.
        private GameObject _ChefLocationsGroup;
        private GameObject _ChefPrefab;
        private ChefGroup _Group;

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

        private void Start()
        {
            InitializeClosestTransfrorm();
            UpdateChefLocations();

            CreateChefs();

            if(Side ==  ScreenSide.Left)
                Chefs = Chefs.OrderByDescending((c) => (c.transform.position.x)).ToList();
            else
                Chefs = Chefs.OrderBy((c) => (c.transform.position.x)).ToList();

            if (Chefs.Count != _ChefLocations.Count)
                Debug.LogError("Chef count and location count is different");

            for (int i = 0; i < _ChefLocations.Count; i++)
                Chefs[i].TargetPosition = _ChefLocations[i];

            //Invoker.CallWithIntervals(()=> 
            //{ MoveAll(); }, int.MaxValue, 3f);
        }

        private void CreateChefs()
        {
            if(_ChefPrefab == null)
                _ChefPrefab = Resources.Load<GameObject>("ChefPrefab");

            foreach (Transform location in _ChefLocations)
            {
                Chef newChef = Instantiate(_ChefPrefab).GetComponent<Chef>();
                newChef.transform.parent = transform;

                if (Side == ScreenSide.Left)
                    newChef.transform.localScale = new Vector3(-1, 1, 1);

                newChef.transform.position = location.position;
                newChef.TargetPosition = location;

                Chefs.Add(newChef);
            }
        }

        private void InitializeChefGroup()
        {
            _Group = GetComponentInParent<ChefGroup>();

            if (_Group == null)
                Debug.LogError("ChefController has to have a ChefGroup parent");

            if (!_Group.Controllers.ContainsKey(Side))
                _Group.Controllers.Add(Side, new List<ChefsController>() { this });
            else
                _Group.Controllers[Side].Add(this);
        }

        private void InitializeClosestTransfrorm()
        {
            if (_ClosestSlot == null)
            {
                _ClosestSlot = new GameObject("ClosestSlot").transform;
                _ClosestSlot.tag = Tags.ChefLocation;

                if (Side == ScreenSide.Left)
                    _ClosestSlot.transform.position = new Vector3(-2, Player.Instance.transform.position.y, 0);
                else
                    _ClosestSlot.transform.position = new Vector3(2, Player.Instance.transform.position.y, 0);

                _ClosestSlot.parent = transform;
            }
        }

        public void CoverAllFood()
        {
            foreach (Chef c in Chefs)
                c.CoverFood(true);
        }

        public void OpenFoodsAtRandom(int amount)
        {
            List<Chef> randomChefs = Chefs.ToList();

            for (int i = 0; i < amount; i++)
            {
                Chef c = randomChefs.RandomElement();
                randomChefs.Remove(c);

                c.CoverFood(false);
            }
        }

        public void MoveAll()
        {
            MoveChefs(0, Chefs.Count);
        }

        public void MoveChefs(int startIndex, int endIndex)
        {
            _ChefLocations.Shift(startIndex, endIndex, true);
            for (int currentChef = startIndex; currentChef < endIndex; currentChef++)
                Chefs[currentChef].TargetPosition = _ChefLocations[currentChef];
        }

        private void UpdateChefLocations()
        {
            DestroyAllChefLocations();

            float VerticalScale = Camera.main.orthographicSize * 2;
            float HorizontalScale = VerticalScale * Camera.main.aspect;

            Vector2 otherEnd = Vector2.zero;

            if (Side == ScreenSide.Right)
                otherEnd = new Vector2(HorizontalScale / 2, _ClosestSlot.position.y);
            else
                otherEnd = new Vector2(-HorizontalScale / 2, _ClosestSlot.position.y);

            _ChefLocations = new List<Transform>();

            _ChefLocationsGroup = new GameObject("Chef Locations Group");
            _ChefLocationsGroup.transform.parent = transform;
            _ChefLocationsGroup.transform.localPosition = Vector3.zero;

            Vector2 currentLocation = _ClosestSlot.position;
            int index = 0;
            if (Side == ScreenSide.Right)
            {
                while (currentLocation.x <= otherEnd.x + 2f)
                {
                    Transform newLocation = new GameObject(index++.ToString()).transform;
                    newLocation.transform.parent = _ChefLocationsGroup.transform;
                    newLocation.transform.position = currentLocation;
                    _ChefLocations.Add(newLocation);
                    currentLocation.x += 2;
                }
            }
            else
            {
                while (currentLocation.x >= otherEnd.x - 2f)
                {
                    Transform newLocation = new GameObject(index++.ToString()).transform;
                    newLocation.transform.parent = _ChefLocationsGroup.transform;
                    newLocation.transform.position = currentLocation;
                    _ChefLocations.Add(newLocation);
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
            if (_ChefLocations == null || _ChefLocations.Count == 0)
            {
                _ChefLocations = new List<Transform>();
                return;
            }

            foreach (Transform t in _ChefLocations)
                Destroy(t.gameObject);

           if(_ChefLocationsGroup != null)
                Destroy(_ChefLocationsGroup);

            _ChefLocations.Clear();
            _ChefLocations = null;
            _ChefLocations = new List<Transform>();
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
