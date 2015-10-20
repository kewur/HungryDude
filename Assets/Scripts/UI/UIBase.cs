﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public abstract class UIBase : MonoBehaviour
    {
        public bool StartHidden = false;

        private Vector3 VisibleScale = Vector3.one;
        private bool _IsHidden = false;
        public bool IsHidden
        {
            get { return _IsHidden; }
            protected set
            {
                if (_IsHidden == value)
                    return;

                _IsHidden = value;

                if (_IsHidden)
                    gameObject.ScaleTo(Vector3.zero, 0.8f, 0f);
                else
                    gameObject.ScaleTo(VisibleScale, 0.8f, 0f);
            }
        }

        public void Show()
        {
            IsHidden = false;
        }

        public void Hide()
        {
            IsHidden = true;
        }

        protected virtual void Awake()
        {
            VisibleScale = transform.localScale;

            if (StartHidden)
            {
                transform.localScale = Vector3.zero;
                _IsHidden = StartHidden;
            }
        }

    }
}
