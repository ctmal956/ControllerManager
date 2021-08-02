using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ControllerManager.ExtensionMethods
{
    public static class ObservableCollectionExtensions
    {
        public static void SwapItems<T>(this ObservableCollection<T> collection, int sourceIndex, int destinationIndex)
        {
            T tmp = collection[sourceIndex];
            collection[sourceIndex] = collection[destinationIndex];
            collection[destinationIndex] = tmp;
        }
    }
}
