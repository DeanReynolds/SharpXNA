namespace SharpXNA.Plugins
{
    public class Buffer
    {
        private int _index;
        private bool _recorded;
        private readonly double[] _values;
        private double _lowest = double.MaxValue, _highest;

        public Buffer(int max) { _values = new double[max]; }

        public double this[int index] => _values[index];

        public void Record(double value)
        {
            _values[_index] = value;
            if (value < _lowest) _lowest = value;
            if (value > _highest) _highest = value;
            _index++;
            if (_index < _values.Length) return;
            _index = 0;
            _recorded = true;
        }

        public double Lowest => _lowest;
        public double Highest => _highest;
        public double Average
        {
            get
            {
                var value = 0d;
                var count = (_recorded ? _values.Length : (_index + 1));
                for (var i = 0; i < count; i++) value += _values[i];
                return (value/count);
            }
        }
        public double Current => _values[_index];
    }
}