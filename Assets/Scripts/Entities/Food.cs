using Assets.Scripts.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Entities
{
    public enum FoodTypes
    {
        Fruit,
        Desert,
        Veggies,
        Meat,
        Poo
    }

    [RequireComponent(typeof(SpriteRenderer))]
    public class Food : InteractableEntity
    {

        private FoodTypes _FoodType;
        public FoodTypes FoodType
        {
            get { return _FoodType; }
            private set
            {
                _FoodType = value;

                SpriteRenderer.sprite = Resources.Load<Sprite>(_FoodType.ToString());
            }
        }

        private SpriteRenderer _SpriteRenderer = null;
        public SpriteRenderer SpriteRenderer
        {
            get { return _SpriteRenderer; }
            private set { _SpriteRenderer = value; }
        }

        private bool _GoldFood;
        public bool GoldFood
        {
            get { return _GoldFood; }
            set
            {
                if (_GoldFood == value)
                    return;

                _GoldFood = value;
                //TODO set material detail to gold.
            }
        }

        void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer.sortingOrder = 2;

            if (GameController.PooEnabled && UnityEngine.Random.Range(0, 100) < GameController.PooChance)
                FoodType = FoodTypes.Poo;
            else
                FoodType = (FoodTypes)UnityEngine.Random.Range(0, (int)Enum.GetValues(typeof(FoodTypes)).Cast<FoodTypes>().Last());


        }

        protected override void OnInteraction()
        {


        }

        public void DropFood()
        {

        }

    }
}
