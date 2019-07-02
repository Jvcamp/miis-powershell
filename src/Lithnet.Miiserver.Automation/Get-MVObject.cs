using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Management.Automation;
using Lithnet.Miiserver.Client;

namespace Lithnet.Miiserver.Automation
{
    [Cmdlet(VerbsCommon.Get, "MVObject", DefaultParameterSetName = "SearchByObjectType")]
    public class GetMVObject : PSCmdlet
    {
        [ValidateNotNullOrEmpty]
        [Parameter(ValueFromPipeline = true, ParameterSetName = "Guid", Mandatory = true, Position = 1)]
        public Guid ID { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(ValueFromPipeline = false, ParameterSetName = "SearchByObjectType", Mandatory = true, Position = 1)]
        [Parameter(ValueFromPipeline = false, ParameterSetName = "SearchByQuery", Mandatory = false, Position = 1)]
        [Parameter(ValueFromPipeline = false, ParameterSetName = "SearchByKey", Mandatory = true, Position = 1)]
        public string ObjectType { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(ValueFromPipeline = false, ParameterSetName = "SearchByKey", Mandatory = true, Position = 2)]
        public string Attribute { get; set; }

        [ValidateNotNullOrEmpty]
        [Parameter(ValueFromPipeline = false, ParameterSetName = "SearchByKey", Mandatory = true, Position = 3)]
        public object Value { get; set; }

        [Parameter(ValueFromPipeline = true, ParameterSetName = "SearchByQuery", Mandatory = false, Position = 2)]
        public MVAttributeQuery[] Queries { get; set; }

        [Parameter(ValueFromPipeline = false, ParameterSetName = "SearchByQuery", Mandatory = false, Position = 3)]
        public string Collation { get; set; }

        private List<MVAttributeQuery> collectedQueries = new List<MVAttributeQuery>();

        protected override void EndProcessing()
        {
            if (this.ParameterSetName == "SearchByKey")
            {
                this.GetByKey();
            }
            else if (this.ParameterSetName == "SearchByQuery")
            {
                this.GetByPipeLineQuery();
            }
            else if (this.ParameterSetName == "SearchByObjectType")
            {
                this.GetByObjectType();
            }
        }

        protected override void ProcessRecord()
        {
            if (this.ParameterSetName == "Guid")
            {
                this.GetByGuid();
            }
            else if (this.ParameterSetName == "SearchByQuery")
            {
                if (this.Queries != null)
                {
                    foreach(MVAttributeQuery query in this.Queries)
                    {
                        this.collectedQueries.Add(query);
                    }
                }
            }
        }

        private void GetByPipeLineQuery()
        {
            MVQuery q = new MVQuery();
            using (DsmlObjectClass objecttype = this.GetObjectType()) {
                q.ObjectType = objecttype;
            }

            if (this.collectedQueries.Count > 0)
            {
                foreach (var item in this.collectedQueries)
                {
                    q.QueryItems.Add(item);
                }
            }

            q.CollationOrder = this.Collation ?? q.CollationOrder;
            using (MVObjectCollection mvobjects = SyncServer.GetMVObjects(q)) {
                this.WriteObject(mvobjects, true);
            }
        }

        private void GetByGuid()
        {
            using (MVObject mvobject = SyncServer.GetMVObject(this.ID))
            {
                this.WriteObject(mvobject);
            }
        }

        private void GetByKey()
        {
            MVAttributeQuery a = new MVAttributeQuery();
            using (DsmlAttribute attr = this.GetAttribute()) {
                a.Attribute = attr;
                a.Operator = MVSearchFilterOperator.Equals;
                a.Value = this.Value;
            }

            MVQuery q = new MVQuery();
            using (DsmlObjectClass objecttype = this.GetObjectType()) {
                q.ObjectType = objecttype;
                q.QueryItems.Add(a);
            }

            using (MVObjectCollection mvobjects = SyncServer.GetMVObjects(q))
            {
                this.WriteObject(mvobjects, true);
            }
        }

        private void GetByObjectType()
        {
            MVQuery q = new MVQuery();
            using (DsmlObjectClass objecttype = this.GetObjectType())
            {
                q.ObjectType = objecttype;
            }

            using (MVObjectCollection mvobjects = SyncServer.GetMVObjects(q)) {
                this.WriteObject(mvobjects, true);
            }
        }

        private DsmlObjectClass GetObjectType()
        {
            if (this.ObjectType == null)
            {
                return null;
            }

            DsmlObjectClass objectClass;

            if (MiisController.Schema.ObjectClasses.TryGetValue(this.ObjectType, out objectClass))
            {
                return objectClass;
            }
            else
            {
                throw new ItemNotFoundException(string.Format("Object type {0} does not exist in the schema", this.ObjectType));
            }
        }

        private DsmlAttribute GetAttribute()
        {
            using (DsmlObjectClass objectClass = this.GetObjectType())
            {
                IReadOnlyDictionary<string, DsmlAttribute> attributesToSearch;

                if (objectClass != null)
                {
                    attributesToSearch = objectClass.Attributes;
                }
                else
                {
                    attributesToSearch = MiisController.Schema.Attributes;
                }

                DsmlAttribute attribute;

                if (attributesToSearch.TryGetValue(this.Attribute, out attribute))
                {
                    return attribute;
                }
                else
                {
                    throw new ItemNotFoundException(string.Format("Attribute {0} does not exist{1}{2}", this.Attribute, this.ObjectType == null ? string.Empty : " on object type ", this.ObjectType ?? string.Empty));
                }
            }
        }
    }
}