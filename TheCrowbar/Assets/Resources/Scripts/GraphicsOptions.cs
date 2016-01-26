using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Resources.Scripts
{
    public static class GraphicsOptions
    {
        //color options
        public enum ColorTypes
        {
            Animated,
            Static,
            Random
        }
        public ColorTypes ColorType;        
        public static int StaticColor;

        //bottle options
        public enum BottleTypes
        { 
            Regular,
            Stubby,
            Wine,
            Whiskey,
            Tallboy,
            Random            
        }
        public BottleTypes BottleType;

        public static int GetRandomColor()
        { 
            Random r = new Random();
            return Int32.Parse(((255 * r.NextDouble()) + (255 * r.NextDouble()) + (255 * r.NextDouble())).ToString());
        }
    }
}
