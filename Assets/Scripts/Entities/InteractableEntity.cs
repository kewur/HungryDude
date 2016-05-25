﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Assets.Scripts.Entities.Interfaces;

namespace Assets.Scripts.Entities
{
    public abstract class InteractableEntity : MonoBehaviour, IInteractable
    {
        protected virtual void Update()
        {

        }

		public abstract void Interact();

        public void FaceOppositeDirection()
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }

    }
}
