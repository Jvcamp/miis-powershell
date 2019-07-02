using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;
using System.IO;

namespace Lithnet.Miiserver.Automation
{
    [Cmdlet(VerbsCommon.New, "MVObject")]
    public class NewMVObject : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline = false), ValidateNotNullOrEmpty]
        public string MVObjectType { get; set; }

        [Parameter(Mandatory = true, Position = 2, ValueFromPipeline = true), ValidateNotNullOrEmpty]
        public CSObject CSObject { get; set; }

        protected override void ProcessRecord()
        {
            if (MiisController.Schema.ObjectClasses.ContainsKey(this.MVObjectType))
            {
                using (MVObject mvobject = this.CSObject.Project(this.MVObjectType))
                {
                    this.WriteObject(mvobject);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException("MVObjectType", "No such object type");
            }
        }
    }
}
