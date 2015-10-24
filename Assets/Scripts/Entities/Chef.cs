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
        private ChefsController _Controller;
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

        public static float SmoothMovement = 8f;

        private void FoodEatenCallBack(FoodInteractedEventArgs e)
        {
            Food f = e.Source as Food;
            _Controller.EatFood(this, f);

            CarryingFood = Food.CreateRandomFood(); // create a new one.
        }

        private void Awake()
        {
            _Controller = GetComponentInParent<ChefsController>();
            _Sprite = GetComponent<SpriteRenderer>();

            InitializeFoodSlot();
            CarryingFood = Food.CreateRandomFood();

            ResetChefType();
        }

        private void FixedUpdate()
        {
            if(TargetPosition != null)
                transform.position = Vector3.Lerp(transform.position, TargetPosition.position, SmoothMovement * Time.deltaTime);
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

        public void CoverFood(bool cover)
        {
            if (_CarryingFood != null)
                _CarryingFood.IsLocked = cover;

            _FoodCover.SetActive(cover);
        }

        public override void Interact()
        {
            _Controller.SendToBack(this);
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
