using Assets.Scripts.Controllers;
using Assets.Scripts.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class Chef : InteractableEntity
    {
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
                    _CarryingFood.DropFood();

                _CarryingFood = value;
                _CarryingFood.transform.parent = FoodSlotTransform;
                _CarryingFood.transform.localPosition = Vector3.zero;
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

        protected override void OnInteraction()
        {
            DropFood();
        }

        public void DropFood()
        {
            if (CarryingFood != null)
                CarryingFood.DropFood();
        }

        public void MoveToTheBack()
        {
            int currentPosition = CurrentPosition;

        }

        public void MoveToPosition(int newPosition)
        {
            int currentPosition = CurrentPosition;
            Chef otherChef = _ChefLocations.GetChefAtLocation(ChefSide, newPosition);
            _SetLocation(newPosition);
            otherChef._SetLocation(currentPosition);
        }


        public static Food CreateRandomFood()
        {
            return new GameObject("Food").AddComponent<Food>();
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

        public void Move()
        {
            CurrentPosition--;
            if (CurrentPosition < 0)
            {
                CurrentPosition = _ChefLocations.LocationCount - 1; //starts from one :p
                sprite.sortingOrder = 0;
                CarryingFood.SpriteRenderer.sortingOrder = 1;
                ChefAlignment goDirection = ChefSide.GetOppositeDirection();

                //if (DefaultFacingDirection == ChefAlignment.Left) //needs to go the opposite direction where it's facing. I did this very weirdly. Should have done this with bools.
                //    goDirection = ChefAlignment.Right;
                //else
                //    goDirection = ChefAlignment.Left;

                FaceOppositeDirection();
                // gameObject.MoveTo(iTweenPath.GetPath(goDirection.ToString() + "GoBackPath"), ChefMovespeed, 0f, EaseType.easeInSine);

                Vector3 startLocation = transform.position;
                Vector3 nextLocation = _ChefLocations.GetLocation(CurrentPosition).position;
                Vector3 midLocation = nextLocation;
                midLocation.z = 1;
                midLocation.x = (startLocation.x + nextLocation.x) / 2;

                gameObject.MoveTo(new Vector3[] { startLocation, midLocation, nextLocation }, ChefMovespeed, 0f, EaseType.easeInSine);
                Invoke("MoveFinishedCallback", ChefMovespeed);
            }
            else
                gameObject.MoveTo(_ChefLocations.GetLocation(CurrentPosition).position, ChefMovespeed, 0f, EaseType.easeInSine);
        }

        private void MoveFinishedCallback()
        {
            sprite.sortingOrder = 2;
            CarryingFood.SpriteRenderer.sortingOrder = 3;
            FaceOppositeDirection();
        }
    }
}
