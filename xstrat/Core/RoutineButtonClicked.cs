using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xstrat.Ui;

namespace xstrat.Core
{
    public class RoutineButtonClicked : EventArgs
    {
        private int type;
        public int Type
        {
            get { return type; }
        }
        private Routine instance;
        public Routine Instance
        {
            get { return instance; }
        }
        /// <summary>
        /// Routine Button Clicked event 
        /// 0 - open
        /// 1 - add
        /// -1 - remove
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        public RoutineButtonClicked(int type, Routine instance)
        {
            this.type = type;
            this.instance = instance;
        }
    }
}
