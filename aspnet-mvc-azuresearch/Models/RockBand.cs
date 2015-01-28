using Giventocode.AzureSearchEntityManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aspnet_mvc_azuresearch.Models
{
    public class RockBand
    {
        [Indexable("Edm.String", false, Key = true, Name = "id")]
        public string Id { get; set; }

        [Indexable("Edm.String", true, Name = "name", Retrievable=true)]        
        public string Name { get; set; }

        [Indexable("Edm.String", true, Name = "genre", Retrievable = true)]
        public string Genre { get; set; }

        [Indexable("Edm.String", true, Name = "description", Retrievable = true)]
        public string Description { get; set;}
    }
}