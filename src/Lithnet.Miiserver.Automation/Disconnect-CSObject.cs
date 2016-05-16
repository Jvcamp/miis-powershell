﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;
using System.IO;

namespace Lithnet.Miiserver.Automation
{
    [Cmdlet(VerbsCommunications.Disconnect, "CSObject")]
    public class DisconnectCSObject : Cmdlet
    {
        [Parameter(Mandatory = true, Position = 1, ValueFromPipeline =true), ValidateNotNullOrEmpty]
        public CSObject CSObject { get; set; }

        [Parameter(Mandatory = false, Position = 2)]
        public SwitchParameter Explicit {get;set;}

        private bool yesToAll;

        private bool noToAll;

        private bool prompted;

        protected override void ProcessRecord()
        {
            if (this.CSObject.WillDeleteMVObjectOnDisconnect())
            {
                if (noToAll && prompted)
                {
                    return;
                }

                if (yesToAll || this.ShouldContinue("This action will result in the metaverse object being deleted. Continue", "Confirm disconnection", ref yesToAll, ref noToAll))
                {
                    prompted = true;
                    this.CSObject.Disconnect(this.Explicit.IsPresent);
                }
            }
            else
            {
                this.CSObject.Disconnect(this.Explicit.IsPresent);
            }
        }
    }
}
