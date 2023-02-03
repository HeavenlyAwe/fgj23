using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;


namespace SplitterTools
{
    public class Splitter
    {
        public SplitterValue CountSplitValues(int value, int divider)
        {
            int rounded = value / divider;
            int remainder = value - (rounded * divider);
            int count = divider - remainder;

            return new SplitterValue(count, rounded, remainder, rounded + 1);
        }
    }

    public class SplitterValue
    {
        private int count1;
        public int Count1
        {
            get { return count1; }
            set { count1 = value; }
        }
        private int count2;
        public int Count2
        {
            get { return count2; }
            set { count2 = value; }
        }

        private int value1;
        public int Value1
        {
            get { return value1; }
            set { value1 = value; }
        }
        private int value2;
        public int Value2
        {
            get { return value2; }
            set { value2 = value; }
        }

        public SplitterValue(int count1, int value1, int count2, int value2)
        {
            Count1 = count1;
            Count2 = count2;
            Value1 = value1;
            Value2 = value2;
        }
    }
}