using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xstrat.Ui;

namespace xstrat.Core
{
    public class RoutineStepButtonClicked : EventArgs
    {
        private int type;
        public int Type
        {
            get { return type; }
        }
        private RoutineStep instance;
        public RoutineStep Instance
        {
            get { return instance; }
        }
        /// <summary>
        /// Routine Button Clicked event 
        /// 1- move down
        /// 1 - move up
        /// 2 - remove
        /// 3 - add
        /// </summary>
        /// <param name="type"></param>
        /// <param name="instance"></param>
        public RoutineStepButtonClicked(int type, RoutineStep instance)
        {
            this.type = type;
            this.instance = instance;
        }
    }
}
