using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Giventocode.AzureSearchEntityManager
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IndexableAttribute : Attribute
    {

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public readonly string Type;
        public readonly bool Searchable;
        
        private bool _filterable = true;

        public bool Filterable
        {
            get { return _filterable; }
            set { _filterable = value; }
        }

        private bool _sortable = true;

        public bool Sortable
        {
            get { return _sortable; }
            set { _sortable = value; }
        }


        private bool _facetable = true;

        public bool Facetable
        {
            get { return _facetable; }
            set { _facetable = value; }
        }


        private bool _suggestions = false;

        public bool Suggestions
        {
            get { return _suggestions; }
            set { _suggestions = value; }
        }

        private bool _key = false;

        public bool Key
        {
            get { return _key; }
            set { _key = value; }
        }


        private bool _retrievable = false;

        public bool Retrievable
        {
            get { return _retrievable; }
            set { _retrievable = value; }
        }

        public IndexableAttribute(string type, bool searchable )
        {            
            this.Type = type;
            this.Searchable = searchable;
        }
    }
}