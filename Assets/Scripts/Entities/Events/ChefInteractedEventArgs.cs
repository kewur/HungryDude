using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.Entities.Events
{
    public class ChefInteractedEventArgs: EventArgs
    {
        private readonly object _Source = null;
        public object Source
        {
            get { return _Source; }
        }

        public ChefInteractedEventArgs(object source)
        {
            _Source = source;
        }
    }
}
