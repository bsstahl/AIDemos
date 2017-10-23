using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ChutesAndLadders.Tools.ReplaceGuids
{
    public class IdCache
    {
        Dictionary<Guid, int> _data = new Dictionary<Guid, int>();
        int _maxValue = 0;

        Guid _lastId = Guid.Empty;
        int _lastValue = 0;

        public int Locate(Guid id)
        {
            int result;
            if (id == _lastId)
                result = _lastValue;
            else if (_data.ContainsKey(id))
            {
                result = _data[id];
                _lastId = id;
                _lastValue = result;
            }
            else
            {
                _maxValue++;
                _data.Add(id, _maxValue);
                result = _maxValue;
            }

            return result;
        }

    }
}
