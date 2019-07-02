using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;
using System.IO;

namespace Lithnet.Miiserver.Automation
{
    [Cmdlet(VerbsCommon.Get, "Connectors")]
    public class GetConnectors : MATargetCmdlet
    {
        [Parameter(Mandatory = false, Position = 2)]
        public ConnectorState? Type { get; set; }

        protected override void ProcessRecord()
        {
            IEnumerable<CSObject> results;

            if (this.Type.HasValue)
            {
                using (CSObjectEnumerator connectors = this.MAInstance.GetConnectors(this.Type.Value))
                {
                    results = connectors;
                }
            }
            else
            {
                using (CSObjectEnumerator connectors = this.MAInstance.GetConnectors())
                {
                    results = connectors;
                }
            }

            foreach (var item in results)
            {
                this.WriteObject(item);
            }
        }
    }
}
