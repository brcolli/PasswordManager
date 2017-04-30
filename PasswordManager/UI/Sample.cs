using System;
using Xwt;

namespace Samples
{
    public class WidgetRendering : VBox
    {
        public WidgetRendering()
        {
            VBox box = new VBox();
            Button b = new Button("Click here to take a shot if this box");
            box.PackStart(b);
            box.PackStart(new CheckBox("Test checkbox"));
            PackStart(box);
            b.Clicked += delegate {
                var img = Toolkit.CurrentEngine.RenderWidget(box);
                PackStart(new ImageView(img));
            };
        }
    }
}