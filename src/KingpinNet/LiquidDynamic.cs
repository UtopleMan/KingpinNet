using DotLiquid;

namespace KingpinNet
{
    internal class LiquidDynamic : Drop
    {
        private dynamic content;

        public LiquidDynamic(dynamic content)
        {
            this.content = content;
        }

        public override object BeforeMethod(string method)
        {
            foreach (var item in content)
            {
                if (item.Key == method)
                    return content[method];
            }
            return null;
        }
    }
}
