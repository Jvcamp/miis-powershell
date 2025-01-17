﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;

namespace Lithnet.Miiserver.Automation
{
    public class MATargetCmdlet : Cmdlet
    {
        private ManagementAgent ma;

        [Parameter(Mandatory = true, Position = 1), ValidateNotNullOrEmpty]
        public string MA { get; set; }

        protected ManagementAgent MAInstance
        {
            get
            {
                if (this.ma == null)
                {
                    using(ManagementAgent managementagent = MiisController.GetManagementAgent(this.MA, false))
                    this.ma = managementagent;
                }

                return this.ma;
            }
        }
    }
}
