using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChutesAndLadders.Entities
{
    public class Spinner
    {
        byte _maxValue = 6;
        byte _alwaysSpin = 0;
        Random _random = new Random();

        public Spinner() { }

        public Spinner(byte maxValue) : this(maxValue, 0) { }

        public Spinner(byte maxValue, byte alwaysSpin)
        {
            _maxValue = maxValue;
            _alwaysSpin = alwaysSpin;
        }

        public byte Spin()
        {
            byte result = _alwaysSpin;
            if (_alwaysSpin == 0)
                result = Convert.ToByte(_random.Next(_maxValue) + 1);
            return result;
        }
    }
}
