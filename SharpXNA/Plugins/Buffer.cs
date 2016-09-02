namespace SharpXNA.Plugins
{
    public class Buffer
    {
        private int index;
        private bool recorded;
        public double[] Values;

        public Buffer(int max) { Values = new double[max]; }

        public void Record(double value) { Values[index] = value; index++; if (index >= Values.Length) { index = 0; recorded = true; } }

        public double Lowest { get { if (!recorded && (index == 0)) return 0; double value = double.MaxValue; int count = (recorded ? Values.Length : (index + 1)); for (int i = 0; i < count; i++) value = System.Math.Min(value, Values[i]); return value; } }
        public double Highest { get { if (!recorded && (index == 0)) return 0; double value = double.MinValue; int count = (recorded ? Values.Length : (index + 1)); for (int i = 0; i < count; i++) value = System.Math.Max(value, Values[i]); return value; } }
        public double Average { get { if (!recorded && (index == 0)) return 0; double value = 0; int count = (recorded ? Values.Length : (index + 1)); for (int i = 0; i < count; i++) value += Values[i]; return ((value == 0) ? 0 : (value / count)); } }
        public double Current { get { return Values[index]; } }
    }
}