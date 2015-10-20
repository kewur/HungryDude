using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    [RequireComponent(typeof(RawImage))]
    public class RawImageUI : UIBase
    {
        private RawImage _RawImage;
        public Texture Texture
        {
            get
            {
                if(_RawImage == null)
                    _RawImage = GetComponent<RawImage>();

                return _RawImage.texture;
            }
            set
            {
                if (_RawImage == null)
                    _RawImage = GetComponent<RawImage>();

                if (_RawImage.texture == value)
                    return;

                _RawImage.texture = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }
    }
}
