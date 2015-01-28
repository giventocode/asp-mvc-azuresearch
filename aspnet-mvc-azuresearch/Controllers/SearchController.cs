using aspnet_mvc_azuresearch.Models;
using Giventocode.AzureSearchEntityManager;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace aspnet_mvc_azuresearch.Controllers
{
    public class SearchController : Controller
    {
        private AzureSearchManager searchMan = new AzureSearchManager();
        
        [HttpGet]
        public async Task<ActionResult> Index(string criteria) 
        {

            if (string.IsNullOrEmpty(criteria))
            {
                return View(new SearchViewModel() { Results = new SearchResultViewModel[0] });
            }

            var results = await searchMan.SearchAsync(criteria);
            var resultsVM = ((JArray)results["value"])
                            .Select<JToken, SearchResultViewModel>(t => new SearchResultViewModel() { Id = (string)t["id"], Name = (string)t["name"], Genre = (string)t["genre"], Description = (string)t["description"] })
                            .ToArray<SearchResultViewModel>();

            return View(new SearchViewModel() { Results = resultsVM });
        }
    }
}