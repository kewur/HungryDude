﻿using Assets.Scripts.Controllers;
using Assets.Scripts.Entities.Events;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public delegate void ChefInteractedDelegate(ChefInteractedEventArgs e);

    [RequireComponent(typeof(SpriteRenderer))]
    public class Chef : InteractableEntity
    {
        public event ChefInteractedDelegate OnInteraction;

        public static readonly float[] PositionMultipliers = new float[] { 0.5f, 1f, 2f, 3f };

        public static float ChefMovespeed = 0.8f;
        public int CurrentPosition;
        public ChefAlignment ChefSide = ChefAlignment.Right;
        private ChefsController _ChefLocations;


        private SpriteRenderer sprite;

        private Transform FoodSlotTransform;

        private Food _CarryingFood;
        public Food CarryingFood
        {
            get { return _CarryingFood; }
            set
            {
                if (_CarryingFood != null) //if it already has a fod drop it.
                {
                    _CarryingFood.DropFood();
                    _CarryingFood.OnInteraction -= FoodEaten;
                }

                _CarryingFood = value;

                if (_CarryingFood != null)
                {
                    _CarryingFood.OnInteraction += FoodEaten;
                    _CarryingFood.transform.parent = FoodSlotTransform;
                    _CarryingFood.transform.localPosition = Vector3.zero;
                }
            }
        }

        protected void Awake()
        {
            _ChefLocations = GetComponentInParent<ChefsController>();

            sprite = GetComponent<SpriteRenderer>();
            InitializeFoodSlot();

            CarryingFood = Chef.CreateRandomFood();
        }

        private void InitializeFoodSlot()
        {
            FoodSlotTransform = transform.FindChildbyTag(Tags.FoodSlot);
            if (FoodSlotTransform == null)
            {
                FoodSlotTransform = transform.FindChild("FoodSlot");
                FoodSlotTransform.tag = Tags.FoodSlot;
            }
        }

        protected override void Update()
        {
            base.Update();
        }

        public override void Interact()
        {
            if (OnInteraction != null)
                OnInteraction(new ChefInteractedEventArgs(this));
        }

        public void MoveToTheBack()
        {

        }

        public void MoveToPosition(int newPosition)
        {
            if (newPosition < 0)
                newPosition = _ChefLocations.BackOfTheLineIndex;

            if (newPosition > CurrentPosition)
            {
                
                //starts from one :p
                sprite.sortingOrder = 0;
                if(CarryingFood != null)
                    CarryingFood.SpriteRenderer.sortingOrder = 1;

                ChefAlignment goDirection = ChefSide.GetOppositeDirection();

                //if (DefaultFacingDirection == ChefAlignment.Left) //needs to go the opposite direction where it's facing. I did this very weirdly. Should have done this with bools.
                //    goDirection = ChefAlignment.Right;
                //else
                //    goDirection = ChefAlignment.Left;

                FaceOppositeDirection();
                // gameObject.MoveTo(iTweenPath.GetPath(goDirection.ToString() + "GoBackPath"), ChefMovespeed, 0f, EaseType.easeInSine);

                Vector3 startLocation = transform.position;
                Vector3 nextLocation = _ChefLocations.GetLocation(newPosition).position;
                Vector3 midLocation = nextLocation;
                midLocation.z = startLocation.z + 1;
                midLocation.x = (startLocation.x + nextLocation.x) / 2;

                gameObject.MoveTo(new Vector3[] { startLocation, midLocation, nextLocation }, ChefMovespeed, 0f, EaseType.easeInSine);
                Invoke("MoveFinishedCallback", ChefMovespeed);
                Invoke("BackOfTheLineCallback", ChefMovespeed);
            }
            else
            {
                gameObject.MoveTo(_ChefLocations.GetLocation(newPosition).position, ChefMovespeed, 0f, EaseType.easeInSine);
            }

                CurrentPosition = newPosition;
        }


        public void MoveRight()
        {
            int newPosition = CurrentPosition - 1;
            MoveToPosition(newPosition);
        }

        public static Food CreateRandomFood()
        {
            return new GameObject("Food").AddComponent<Food>();
        }

        private void FoodEaten(FoodInteractedEventArgs e)
        {
            MoveToTheBack();
            if (Player.Instance.Eating)
                DropFood();
            else
            {
                Player.Instance.EatFood(this ,CarryingFood);
                _CarryingFood = null;
            }
        }

        private void DropFood()
        {
            
        }

        public Chef GetMirrorChef()
        {
            return _ChefLocations.GetMirrorChef(this);
        }

        /// <summary>
        /// Sets the chef's position to the index.
        /// </summary>
        /// <param name="chef"></param>
        /// <param name="location"></param>
        private void _SetLocation(int location)
        {
            gameObject.MoveTo(_ChefLocations.GetLocation(location).position, ChefMovespeed, 0f, EaseType.easeInSine);

            if (CurrentPosition < location)
            {
                FaceOppositeDirection();
                Invoke("MoveFinishedCallback", ChefMovespeed);
            }
        }

        private void MoveFinishedCallback()
        {
            sprite.sortingOrder = 2;
            
            FaceOppositeDirection();

            if(CarryingFood != null)
                CarryingFood.SpriteRenderer.sortingOrder = 3;
        }

        private void BackOfTheLineCallback()
        {
            if (CarryingFood != null)
                CarryingFood.DropFood();

            CarryingFood = CreateRandomFood();
        }
    }
}
