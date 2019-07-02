using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;
using System.IO;

namespace Lithnet.Miiserver.Automation
{
    [Cmdlet(VerbsCommon.Get, "PendingExportDeletes")]
    public class GetPendingExportDeletes : MATargetCmdlet
    {
        [Parameter]
        public SwitchParameter Delta { get; set; }

        [Parameter]
        public SwitchParameter Hologram { get; set; }

        protected override void ProcessRecord()
        {
            using (CSObjectEnumerator pendingExportDeletes = this.MAInstance.GetPendingExports(false, false, true))
            {
                foreach (var item in pendingExportDeletes)
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
}
