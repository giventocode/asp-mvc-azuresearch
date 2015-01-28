using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Giventocode.AzureSearchEntityManager
{
    public class EntityPropertyIndexInfo
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public IndexableAttribute IndexDefinition { get; set; }
        private Dictionary<string, object> _indexAttributes;

        public Dictionary<string, object> IndexAttributes
        {
            get {

                if (_indexAttributes == null)
                {
                    _indexAttributes = new Dictionary<string, object>();
                }
                return _indexAttributes; }
            set { _indexAttributes = value; }
        }
        
    }


    public class EntityIndexInfo
    {

        public string Action { get; set; }

        private List<EntityPropertyIndexInfo> _properties;
        public List<EntityPropertyIndexInfo> Properties
        {
            get
            {
                if (_properties == null)
                {
                    _properties = new List<EntityPropertyIndexInfo>();
                } return _properties;
            }
            set { _properties = value; }
        }

        internal string GetKeyPropertyName()
        {
            var keyProp = Properties.FirstOrDefault<EntityPropertyIndexInfo>(p =>
                              {
                                  return p.IndexAttributes.ContainsKey("key");                                  
                              });

            if (keyProp == null)
                return string.Empty;

            return keyProp.Name;
        }
    }

    public class IndexDocument
    {
        private List<Dictionary<string, object>> _values { get; set; }

        [JsonProperty("value")]
        public List<Dictionary<string, object>> Values
        {
            get
            {
                if (_values == null)
                {
                    _values = new List<Dictionary<string, object>>();
                } return _values;
            }
            set { _values = value; }
        }
    }


    public class IndexModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        private List<Dictionary<string, object>> _fields { get; set; }

        [JsonProperty("fields")]
        public List<Dictionary<string, object>> Fields
        {
            get
            {
                if (_fields == null)
                {
                    _fields = new List<Dictionary<string, object>>();
                } return _fields;
            }
            set { _fields = value; }
        }


       
    }
}