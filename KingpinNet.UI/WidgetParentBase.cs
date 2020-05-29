using System.Collections.Generic;

namespace KingpinNet.UI
{
    public class WidgetParentBase : WidgetBase
    {
        public List<WidgetBase> Children { get; private set; }
        public WidgetParentBase(IConsole console) : base(console)
        {
            Children = new List<WidgetBase>();
        }
        public void Add(WidgetBase child)
        {
            Children.Add(child);
        }
        protected override void Draw()
        {
            foreach (var child in Children)
            {
                child.Render();
            }
        }
    }
}
