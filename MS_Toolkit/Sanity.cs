using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MS_Toolkit
{
    static class Sanity
    {
        public static void Requires(bool valid, string message)
        {
            if (!valid)
                throw new Exception(message);
        }

        public static void Requires(bool valid)
        {
            Requires(valid, "Unexpected result.");
        }

        public static void BeBetter(bool valid, string message)
        {
            try
            {
                Requires(valid, message);
            }
            catch
            {

            }
        }

        public static void BeBetter(bool valid)
        {
            try
            {
                Requires(valid);
            }
            catch
            {

            }
        }
    }

    class MException : Exception
    {
        public MException() : base() { }
        public MException(string message) : base(message) { }
    }
}
