using Assets.Scripts.Controllers;
using Assets.Scripts.Entities.Events;
using Assets.Scripts.Shop;
using Assets.Scripts.Utilities;
using Assets.Scripts.Utilities.Invoker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public delegate void ChefInteractedDelegate(ChefInteractedEventArgs e);

    public enum ChefType
    {
        Default,
        Mega
    };

    [RequireComponent(typeof(SpriteRenderer))]
    public class Chef : InteractableEntity
    {
        private static GameObject _FoodCoverPrefab = Resources.Load<GameObject>("FoodCover");

        private ChefsController _Controller;
        protected ChefsController Controller
        {
            get
            {
                if (_Controller == null)
                    _Controller = GetComponentInParent<ChefsController>();

                return _Controller;
            }
        }

        private SpriteRenderer _Sprite;
        private Transform _FoodSlot;
        private GameObject _FoodCover;

        private ChefType _Type;
        public ChefType Type
        {
            get
            {
                return _Type;
            }

            private set
            {
                if (_Type == value)
                    return;

                _Type = value;
                _Sprite.sprite = Resources.Load<Sprite>(string.Format("ChefSkins/{0}", _Type,ToString()));
            }
        }

        Food _CarryingFood;
        public Food CarryingFood
        {
            get
            {
                return _CarryingFood;
            }

            set
            {
                if (_CarryingFood == value)
                    return;

                if (_CarryingFood != null)
                {
                    _CarryingFood.OnInteraction -= FoodEatenCallBack;
                }

                _CarryingFood = value;

                if (_CarryingFood != null)
                {
                    _CarryingFood.OnInteraction += FoodEatenCallBack;
                    _CarryingFood.transform.parent = _FoodSlot;
                    _CarryingFood.transform.position = _FoodSlot.transform.position;
                }
            }
        }

        private Transform _TargetPosition;
        public Transform TargetPosition
        {
            get
            {
                return _TargetPosition;
            }

            set
            {
                if (_TargetPosition == value)
                    return;

                _TargetPosition = value;
            }
        }

        public static float SmoothMovement = 3.8f;

        private void Awake()
        {
            _Sprite = GetComponent<SpriteRenderer>();

            InitializeFoodSlot();
            InitialzieFoodCover();

            CarryingFood = Food.CreateRandomFood();

            ResetChefType();
        }

        private void FoodEatenCallBack(FoodInteractedEventArgs e)
        {
            Food f = e.Source as Food;
            Controller.EatFood(this, f);

            CarryingFood = Food.CreateRandomFood(); // create a new one.
        }

        private void FixedUpdate()
        {
            if (TargetPosition != null)
            {
                transform.position = Vector3.Lerp(transform.position, TargetPosition.position, SmoothMovement * Time.deltaTime);

                AdjustFacingDirection();
            }
        }

        private void AdjustFacingDirection()
        {
            if (Controller.Side == ScreenSide.Left)
            {
                if (TargetPosition.position.x + 0.1f < transform.position.x)
                {
                    transform.localScale = Vector3.one;
                    _Sprite.sortingOrder = -1;
                }
                else
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    _Sprite.sortingOrder = 0;
                }
            }

            else
            {
                if (TargetPosition.position.x - 0.1f > transform.position.x)
                {
                    transform.localScale = new Vector3(-1, 1, 1);
                    _Sprite.sortingOrder = -1;
                }
                else
                {
                    transform.localScale = Vector3.one;
                    _Sprite.sortingOrder = 0;
                }

            }
        }

        private void InitializeFoodSlot()
        {
            _FoodSlot = transform.FindChildbyTag(Tags.FoodSlot);
            if (_FoodSlot == null)
            {
                _FoodSlot = transform.FindChild("FoodSlot");
                _FoodSlot.tag = Tags.FoodSlot;
            }
        }

        private void InitialzieFoodCover()
        {
            if (_FoodSlot == null)
                InitializeFoodSlot();

            _FoodCover = Instantiate(_FoodCoverPrefab);
            _FoodCover.tag = Tags.FoodCover;
            _FoodCover.transform.parent = _FoodSlot;
            _FoodCover.transform.localPosition = Vector3.zero;
        }

        public void CoverFood(bool cover)
        {
            if (_CarryingFood != null)
                _CarryingFood.IsLocked = cover;

            _FoodCover.SetActive(cover);
        }

        public override void Interact()
        {
            Controller.SendToBack(this);
        }

        public void ResetChefType()
        {
            Type = (ChefType)CryptoRandom.DefaultRandom.Next(0, PurchasedItems.Chefskins.Count);
        }

        public void DropFood()
        {

        }

    }
}
