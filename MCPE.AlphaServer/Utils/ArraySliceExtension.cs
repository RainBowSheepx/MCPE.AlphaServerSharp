using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCPE.AlphaServer.Utils
{
    public static class ArraySliceExtension
    {
        public static T[] ArraySlice<T>(this T[] array, int offset, int size)
        {
            var result = new T[size];
            Array.Copy(array, offset, result, 0, size);

            return result;
        }
    }
}
