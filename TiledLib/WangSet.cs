using Newtonsoft.Json;
using System;
using System.ComponentModel;
using System.Xml.Serialization;

namespace TiledLib
{
    public class WangColor
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("color")]
        public string Color { get; set; }

        [XmlAttribute("tile")]
        public int Tile { get; set; }

        [XmlAttribute("probability")]
        public double Probability { get; set; }
    }

    public class WangTile
    {
        [DefaultValue(false)]
        [XmlAttribute("dflip")]
        public bool DFlip { get; set; }

        [DefaultValue(false)]
        [XmlAttribute("hflip")]
        public bool HFlip { get; set; }

        [DefaultValue(false)]
        [XmlAttribute("vflip")]
        public bool VFlip { get; set; }

        [XmlAttribute("tileid")]
        public int TileId { get; set; }

        [XmlIgnore]
        public int[] WangId { get; set; }

        [XmlAttribute("wangid")]
        [JsonIgnore]
        public string CompressedWangId
        {
            get
            {
                var c = 0;
                for(var i=7;i>=0;i--)
                {
                    c = (c << 4) + WangId[i];
                }
                return "0x" + Convert.ToString(c, 16);
            }
            set
            {
                var c = Convert.ToInt32(value, 16);
                var r = new[] { 0, 0, 0, 0, 0, 0, 0, 0 };
                for(var i=0;i<8;i++)
                {
                    r[i] = c & 0xF;
                    c = c >> 4;
                }
                WangId = r;
            }
        }
    }

    public class WangSet
    {
        [XmlElement("wangcornercolor")]
        public WangColor[] CornerColors { get; set; }

        [XmlElement("wangedgecolor")]
        public WangColor[] EdgeColors { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("tile")]
        public int Tile { get; set; }

        [XmlElement("wangtile")]
        public WangTile[] WangTiles { get; set; }
    }
}