using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Entities.Events
{
    public class FoodInteractedEventArgs : EventArgs
    {
        private readonly object _Source;
        public object Source
        {
            get { return _Source; }
        }

        public FoodInteractedEventArgs(object source)
        {
            _Source = source;
        }
    }
}
