
namespace PNetJson
{
    public struct JsonPos
    {
        public int Start { get { return _Start; } private set { _Start = value; } }
        private int _Start;
        public int End { get { return _End; } private set { _End = value; } }
        private int _End;

        public JsonPos(int start, int end)
        {
            _Start = start;
            _End = end;
        }

        public override string ToString()
        {
            return string.Concat('|', Start, '|', End, '|');
        }
    }
}
