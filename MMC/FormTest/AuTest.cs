using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace FormTest
{
    class AuTest
    {
        private AutomationElement window;
        public void Test(int hwd)
        {
            var h = FindTradeWindow.Find(new IntPtr(hwd));
            if (h < 0) // 说明之前没有启动过
            {
                Process pro = Process.Start(@"D:\tc_pazq\tc.exe", "");
            }

            while (window == null)
            {
                Thread.Sleep(2000);
                try
                {
                    if (h > 0)
                    {
                        window = AutomationElement.FromHandle(new IntPtr(h));
                        break;
                    }
                    else 
                    {
                        h = FindTradeWindow.Find(new IntPtr(hwd));
                    }
                }
                catch (Exception e)
                {
                }
            }
            
            /*var desktop = AutomationElement.RootElement;
            var condition = new PropertyCondition(AutomationElement.NameProperty, "通达信网上交易V6.51 芜湖营业部 周嘉洁"); // 定义我们的查找条件，名字是test
            var window = desktop.FindFirst(TreeScope.Children, condition);*/
        }

        public void GetControls() 
        {
            //window = AutomationElement.FromHandle(new IntPtr(window.Current.NativeWindowHandle));
            var autoIds = new string[] {
                "12005",
                "12006",
                "12007", 
                "2010"};
            var types = new TradeContorlTypes[] {
                TradeContorlTypes.BuyCode, 
                TradeContorlTypes.BuyMoney, 
                TradeContorlTypes.BuyAmount, 
                TradeContorlTypes.BuyButton };

            PropertyCondition propertyCondition = null;
            for (var i = 0; i < autoIds.Length; i++)
            {
                propertyCondition = new PropertyCondition(AutomationElement.AutomationIdProperty, autoIds[i]);
                var control = window.FindFirst(TreeScope.Subtree, propertyCondition);
                if (control != null)
                {
                    TradeControls.Instance().AddControl(types[i], control);
                }
            }
        }

        public void Test3() 
        {
            var a = TreeWalker.ControlViewWalker.GetFirstChild(window);
            var b = TreeWalker.RawViewWalker.GetFirstChild(window);
        }

        public void TestBuy()
        {
            var buyCodeConsoltr = TradeControls.Instance().GetControl(TradeContorlTypes.BuyCode);
            ValuePattern textPattern = buyCodeConsoltr.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            textPattern.SetValue("600012");

            var buyMoneyConsoltr = TradeControls.Instance().GetControl(TradeContorlTypes.BuyMoney);
            textPattern = buyMoneyConsoltr.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            textPattern.SetValue("12.10");

            var buyAmountConsoltr = TradeControls.Instance().GetControl(TradeContorlTypes.BuyAmount);
            textPattern = buyAmountConsoltr.GetCurrentPattern(ValuePattern.Pattern) as ValuePattern;
            textPattern.SetValue("200");

            var buyButtonControl = TradeControls.Instance().GetControl(TradeContorlTypes.BuyButton);
            var invokePattern = buyButtonControl.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
            invokePattern.Invoke();
        }
    }
}
