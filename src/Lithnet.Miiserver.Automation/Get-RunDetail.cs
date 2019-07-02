using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;

namespace Lithnet.Miiserver.Automation
{
    [Cmdlet(VerbsCommon.Get, "RunDetail")]
    public class GetRunDetail : MATargetCmdlet
    {
        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "FromRunNumber")]
        public int RunNumber { get; set; }

        [Parameter(Mandatory = true, Position = 1, ParameterSetName = "FromSummary", ValueFromPipeline = true)]
        public RunSummary Item { get; set; }

        protected override void ProcessRecord()
        {
            if (this.Item != null)
            {
                using (RunDetails syncrundetail = SyncServer.GetRunDetail(this.Item))
                {
                    this.WriteObject(syncrundetail);
                }
            }
            else
            {
                using (RunDetails marundetail = this.MAInstance.GetRunDetail(this.RunNumber))
                {
                    this.WriteObject(marundetail);
                }
            }
        }
    }
}
