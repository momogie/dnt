using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DNT
{
    internal class UIString
    {
    }

    public class BButton : System.Windows.Forms.Button
    {
        public override string Text { get => base.Text; set => base.Text = value; }
        public BButton()
        {
            //base.Font.FontFamily = System.Drawing.FontFamily.GenericSansSerif;
        }
    }
}
