using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;
using System.IO;

namespace Lithnet.Miiserver.Automation
{
    [Cmdlet(VerbsCommon.Get, "PendingExportUpdates")]
    public class GetPendingExportUpdates : MATargetCmdlet
    {
        [Parameter]
        public SwitchParameter Delta { get; set; }

        [Parameter]
        public SwitchParameter Hologram { get; set; }

        protected override void ProcessRecord()
        {
            using(CSObjectEnumerator pendingExportUpdates = this.MAInstance.GetPendingExports(false, true, false) )
            foreach (var item in pendingExportUpdates )
            {
                if (this.Delta.IsPresent)
                {
                    this.WriteObject(item.UnappliedExportDelta);
                }
                else if (this.Hologram.IsPresent)
                {
                    this.WriteObject(item.UnappliedExportHologram);
                }
                else
                {
                    this.WriteObject(item);
                }
            }
        }
    }
}
