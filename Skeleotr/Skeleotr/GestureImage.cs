using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GestureRecognizer;

namespace Skeleotr
{
   public class GestureImage
    {

        public GestureType GestTypeImage { get; set; }
        


        public GestureImage (GestureType gestType)
        {
            GestTypeImage = gestType;
        }

        public string getGestType()
        {
            if (GestTypeImage.Equals(GestureType.Toilet))
            {
                return "toilet";
            } else
            {
                return null;
            }


        }
    }
}
