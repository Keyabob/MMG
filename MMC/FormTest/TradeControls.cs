using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace FormTest
{
    class TradeControls
    {
        private Dictionary<TradeContorlTypes, AutomationElement> handles;

        private static TradeControls _instance;

        private TradeControls() 
        {
            handles = new Dictionary<TradeContorlTypes, AutomationElement>();
        }

        public static TradeControls Instance() 
        {
            if (_instance == null)
            {
                _instance = new TradeControls();
            }

            return _instance;
        }

        public void AddControl(TradeContorlTypes type, AutomationElement element) 
        {
            Debug.Assert(type != null, "type为空");
            Debug.Assert(element != null, "界面元素为空");
            this.handles.Add(type, element);
        }

        public AutomationElement GetControl(TradeContorlTypes type)
        {
            return this.handles[type];
        }
    }
}
