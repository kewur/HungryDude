using System;
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
        private bool _Hide = false;
        public bool Hide
        {
            get { return _Hide; }
            set
            {
                if (_Hide == value)
                    return;

                _Hide = value;

                if (_Hide)
                    gameObject.ScaleTo(Vector3.zero, 0.8f, 0f);
                else
                    gameObject.ScaleTo(VisibleScale, 0.8f, 0f);
            }
        }

        protected virtual void Awake()
        {
            VisibleScale = transform.localScale;

            if (StartHidden)
            {
                transform.localScale = Vector3.zero;
                _Hide = StartHidden;
            }
        }

    }
}
