﻿//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
using AlexaSearch.Models;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
namespace AlexaSearch.Controllers
{
    public class AjaxSearchIndexController : Controller
    {
        // GET: AjaxSearchIndex
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DemoComponent()
        {
            var item = Sitecore.Context.Database.GetItem("{C76807A4-7D9D-4048-972A-B269CF31ECBF}");
            string str = item.Fields["Copyright"].Value;
            // Logic
            // Debugging - Being The Detective In a Crime Movie Where You Are Also The Murderer.
            return View("~/Views/Alexa/ControllerDemo.cshtml", item);
        }

        public ActionResult DoSearch(string searchText)
        {
            var myResults = new SearchResults
            {
                Results = new List<SearchResult>()
            };
            var searchIndex = ContentSearchManager.GetIndex("sitecore_web_index"); // Get the search index
            var searchPredicate = GetSearchPredicate(searchText); // Build the search predicate
            using (var searchContext = searchIndex.CreateSearchContext()) // Get a context of the search index
            {
                //Select * from Sitecore_web_index Where Author="searchText" OR Description="searchText" OR Title="searchText"
                //var searchResults = searchContext.GetQueryable<SearchModel>().Where(searchPredicate); // Search the index for items which match the predicate
                var searchResults = searchContext.GetQueryable<SearchModel>()
                    .Where(x => x.Author.Contains(searchText) || x.Title.Contains(searchText) || x.Description.Contains(searchText));   //LINQ query

                var fullResults = searchResults.GetResults();

                // This is better and will get paged results - page 1 with 10 results per page
                //var pagedResults = searchResults.Page(1, 10).GetResults();
                foreach (var hit in fullResults.Hits)
                {
                    myResults.Results.Add(new SearchResult
                    {
                        Description = hit.Document.Description,
                        Title = hit.Document.Title,
                        ItemName = hit.Document.ItemName,
                        Author = hit.Document.Author
                    });
                }
                return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = myResults };
            }
        }

        /// <summary>
        /// Search logic
        /// </summary>
        /// <param name="searchText">Search term</param>
        /// <returns>Search predicate object</returns>
        public static Expression<Func<SearchModel, bool>> GetSearchPredicate(string searchText)
        {
            var predicate = PredicateBuilder.True<SearchModel>(); // Items which meet the predicate
                                                                  // Search the whole phrase - LIKE
                                                                  // predicate = predicate.Or(x => x.DispalyName.Like(searchText)).Boost(1.2f);
                                                                  // predicate = predicate.Or(x => x.Description.Like(searchText)).Boost(1.2f);
                                                                  // predicate = predicate.Or(x => x.Title.Like(searchText)).Boost(1.2f);
                                                                  // Search the whole phrase - CONTAINS
            predicate = predicate.Or(x => x.Author.Contains(searchText)); // .Boost(2.0f);
            predicate = predicate.Or(x => x.Description.Contains(searchText)); // .Boost(2.0f);
            predicate = predicate.Or(x => x.Title.Contains(searchText)); // .Boost(2.0f);
            //Where Author="searchText" OR Description="searchText" OR Title="searchText"
            return predicate;
        }

    }
}

    
