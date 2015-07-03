using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Location
{
    public class ImitateTarget : MonoBehaviour
    {

        public Vector3 PositionOffset = Vector3.zero;
        public Transform ImitationTransform;


        private void Update()
        {
            transform.position = ImitationTransform.transform.position + PositionOffset;
            transform.rotation = ImitationTransform.transform.rotation;
        }

    }
}
