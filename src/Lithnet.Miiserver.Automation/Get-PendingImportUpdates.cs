using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;
using System.IO;

namespace Lithnet.Miiserver.Automation
{
    [Cmdlet(VerbsCommon.Get, "PendingImportUpdates")]
    public class GetPendingImportUpdates : MATargetCmdlet
    {
        [Parameter]
        public SwitchParameter Delta { get; set; }

        [Parameter]
        public SwitchParameter Hologram { get; set; }

        protected override void ProcessRecord()
        {
            using(CSObjectEnumerator pendingImportUpdates =this.MAInstance.GetPendingImports(false, true, false) )
            foreach (var item in pendingImportUpdates )
            {
                if (this.Delta.IsPresent)
                {
                    this.WriteObject(item.PendingImportDelta);
                }
                else if (this.Hologram.IsPresent)
                {
                    this.WriteObject(item.PendingImportHologram);
                }
                else
                {
                    this.WriteObject(item);
                }
            }
        }
    }
}
