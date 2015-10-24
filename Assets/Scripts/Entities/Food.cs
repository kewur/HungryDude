using Assets.Scripts.Controllers;
using Assets.Scripts.Entities.Events;
using Assets.Scripts.Utilities;
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

    public enum FoodPowerUps
    {
        Explode,
        Double,
        Advertisiment
    }

    public delegate void FoodInteractDelegate(FoodInteractedEventArgs e); 

    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Food : InteractableEntity
    {
        private bool _Dropping = false;

        public event FoodInteractDelegate OnInteraction;

        public const float ZPosition = -0.1f;
        public const float DestroyTime = 0.4f;

        private FoodTypes _FoodType;
        public FoodTypes FoodType
        {
            get { return _FoodType; }
            private set
            {
                _FoodType = value;
                SpriteRenderer.sprite = GetFoodImage(_FoodType);
            }
        }

        private bool _IsLocked = false;
        public bool IsLocked
        {
            get
            {
                return _IsLocked;
            }

            set
            {
                if (_IsLocked == value)
                    return;

                _IsLocked = value;

                if(_SpriteRenderer != null)
                    _SpriteRenderer.enabled = !value;
            }
        }

        private SpriteRenderer _SpriteRenderer = null;
        public SpriteRenderer SpriteRenderer
        {
            get { return _SpriteRenderer; }
            private set { _SpriteRenderer = value; }
        }

        private BoxCollider _FoodCollider = null;
        public BoxCollider FoodCollider
        {
            get
            {
                if (_FoodCollider == null)
                    FoodCollider = GetComponent<BoxCollider>();

                return _FoodCollider;
            }
            private set { _FoodCollider = value; }
        }

        private FoodPowerUps _PowerUp;
        public FoodPowerUps PowerUp
        {
            get { return _PowerUp; }
            set { _PowerUp = value; }
        }
        
        void Awake()
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            SpriteRenderer.sortingOrder = 2;

            if (GameController.Instance.PooEnabled && CryptoRandom.DefaultRandom.Next(0, 100) < GameController.PooChance)
                FoodType = FoodTypes.Poo;
            else
                FoodType = (FoodTypes)CryptoRandom.DefaultRandom.Next(0, (int)Enum.GetValues(typeof(FoodTypes)).Cast<FoodTypes>().Last());

            FoodCollider.size = new Vector3(1.5f, 1, 0.1f);

            transform.localPosition = new Vector3(0f, 0f, ZPosition);
            
            gameObject.name = FoodType.ToString();
            gameObject.tag = Tags.Food;
        }

        public override void Interact()
        {
            if (IsLocked)
                return;

            if (OnInteraction != null)
                OnInteraction(new FoodInteractedEventArgs(this));

            DropFood();
        }

        public void DropFood()
        {
            if (_Dropping)
                return;

            _Dropping = true;
            transform.parent = null;
            gameObject.AddComponent<Rigidbody>();
            gameObject.layer = Layers.NoCollision;

            FoodCollider.enabled = false;
            Destroy(gameObject, DestroyTime);
        }

        void OnDestroy()
        {
            
        }

        public void Eat()
        {
            
            Destroy(gameObject);
        }

        public static Sprite GetFoodImage(FoodTypes foodType)
        {
            return Resources.Load<Sprite>(foodType.ToString());
        }

        public static Texture GetFoodImageUI(FoodTypes foodType)
        {
            return Resources.Load<Texture>(foodType.ToString() + "UI");
        }

        public static Food CreateRandomFood()
        {
            return new GameObject("Food").AddComponent<Food>();
        }
    }
}
