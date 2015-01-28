using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace aspnet_mvc_azuresearch.Models
{

    public class SearchViewModel
    {
        public string Criteria { get; set; }
        public SearchResultViewModel[] Results { get; set; }
    }

    public class SearchResultViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Genre { get; set; }
    }
}