﻿using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utilities
{
    public static class Extensions
    {
        public static Transform[] GetChildrenWithTag(this Transform obj, string tag)
        {
            List<Transform> children = new List<Transform>();
            foreach(Transform c in obj)
            {
                if (c.tag == tag)
                    children.Add(c);
            }

            return children.ToArray();
        }

        public static Transform FindChildbyTag(this Transform obj, string tag)
        {
            foreach(Transform t in obj.transform)
            {
                if (t.tag == tag)
                    return t;
            }

            return null;
        }

        public static ChefAlignment GetOppositeDirection(this ChefAlignment align)
        {
            if (align == ChefAlignment.Left)
                return ChefAlignment.Right;
            else
                return ChefAlignment.Left;
        }

    }

    

}
