using System;

namespace DataStructures
{
    public class HeapifyEmptyArrayException : Exception
    {
        public HeapifyEmptyArrayException()
        {
        }
        public HeapifyEmptyArrayException(string message) : base(message)
        {
        }
    }

    public class HeapFullException : Exception
    {
        public HeapFullException()
        {
        }
        public HeapFullException(string message) : base(message)
        {
        }
    }

    public class HeapEmptyException : Exception
    {
        public HeapEmptyException()
        {
        }
        public HeapEmptyException(string message) : base(message)
        {
        }
    }

}