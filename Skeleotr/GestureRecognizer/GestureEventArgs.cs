using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestureRecognizer
{
    public class GestureEventArgs : EventArgs

    {
        public RecognitionResult Result { get; internal set; }


        public GestureEventArgs(RecognitionResult result)
        {
            this.Result = result;
        }
    }
}
