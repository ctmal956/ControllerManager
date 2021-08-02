using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WPFCommon
{
    //custom event args class
    public class MessageValueChangedEventArgs : EventArgs
    {

        public string PropertyName { get; set; }

        public object OldValue { get; set; }

        public object NewValue { get; set; }
    }
}
