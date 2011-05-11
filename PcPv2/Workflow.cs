using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcPv2
{
    class Workflow : IComparable
    {
        public string uuid;
        public string name;
        public string description;

        public Workflow(string name, string uuid, string description)
        {
            this.name = name;
            this.uuid = uuid;
            this.description = description;
        }

        public override string ToString()
        {
            return this.name;
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            Workflow otherFlow = obj as Workflow;
            if (otherFlow != null)
                return this.name.CompareTo(otherFlow.name);
            else
                throw new ArgumentException("Object is not a Workflow");

            
        }

        #endregion
    }
}
