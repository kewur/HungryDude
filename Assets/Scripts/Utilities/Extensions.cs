using Assets.Scripts.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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

        public static bool Equal(this float f, float other, float sensitivity = 0.01f)
        {
            if (other < f + sensitivity && other > f - sensitivity)
                return true;

            return false;
        }

        public static T RandomElement<T>(this IEnumerable<T> enumerable)
        {
            return enumerable.RandomElementUsing(new System.Random());
        }

        public static T RandomElementUsing<T>(this IEnumerable<T> enumerable, System.Random rand)
        {
            int index = rand.Next(0, enumerable.Count());
            return enumerable.ElementAt(index);
        }

        public static void Shift<T>(this IList<T> list, int start, int end, bool toRight)
        {
            // a b c d e
            // b c d e a

            //a b c d e
            //e a b c d

            //b c d e a

            if (list == null || list.Count < 2)
                return;

            if (toRight)
                for (int i = end; i >= start; i--)
                    list.Switch(i, i - 1);

            else
                for (int i = start; i < end; i++)
                    list.Switch(i, i + 1);

        }

        public static void Shift<T>(this IList<T> list, bool toRight)
        {
            if (list == null || list.Count < 2)
                return;

            if (toRight)
                for (int i = 0; i < list.Count; i++)
                    list.Switch(i, i + 1);

            else
                for (int i = list.Count - 1; i >= 0; i--)
                    list.Switch(i, i - 1);

        }

        public static void Switch<T>(this IList<T> list, int index1, int index2)
        {
            if (Math.Min(index1, index2) < 0 || list.Count <= Math.Max(index1, index2))
                return;

            T tmp = list[index1];
            list[index1] = list[index2];
            list[index2] = tmp;
        }

    }

    

}
