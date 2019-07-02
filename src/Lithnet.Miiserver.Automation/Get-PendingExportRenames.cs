﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;
using System.IO;

namespace Lithnet.Miiserver.Automation
{
    [Cmdlet(VerbsCommon.Get, "PendingExportRenames")]
    public class GetPendingExportRenames : MATargetCmdlet
    {
        [Parameter]
        public SwitchParameter Delta { get; set; }

        [Parameter]
        public SwitchParameter Hologram { get; set; }

        protected override void ProcessRecord()
        {
            using (CSObjectEnumerator pendingExportRenames = this.MAInstance.GetPendingExports(false, true, false))
            {
                foreach (var item in pendingExportRenames)
                {
                    if (!string.Equals(item.DN, item.UnappliedExportHologram.DN, StringComparison.OrdinalIgnoreCase))
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
}