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

    public delegate void FoodInteractDelegate(FoodInteractedEventArgs e); 

    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Food : InteractableEntity
    {
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

                SpriteRenderer.sprite = Resources.Load<Sprite>(_FoodType.ToString());
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

            FoodCollider.size = new Vector3(1.5f, 1, 0.1f);

            transform.localPosition = new Vector3(0f, 0f, ZPosition);
            
            gameObject.name = FoodType.ToString();
            gameObject.tag = Tags.Food;
        }

        public override void Interact()
        {
            if (OnInteraction != null)
                OnInteraction(new FoodInteractedEventArgs(this));
        }

        public void DropFood()
        {
            transform.parent = null;
            gameObject.AddComponent<Rigidbody>();
            gameObject.layer = Layers.NoCollision;

            FoodCollider.enabled = false;
            Destroy(gameObject, DestroyTime);
        }

        void OnDestroy()
        {
            print("Destroy");
        }
    }
}
