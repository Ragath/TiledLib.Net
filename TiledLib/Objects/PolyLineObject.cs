using System.Collections.Generic;

namespace TiledLib.Objects
{
    public class PolyLineObject : BaseObject
    {
        internal PolyLineObject(Dictionary<string, string> properties) : base(properties) { }
        public PolyLineObject() : base(new Dictionary<string, string>()) { }

        public Position[] polyline { get; set; }
    }
}
