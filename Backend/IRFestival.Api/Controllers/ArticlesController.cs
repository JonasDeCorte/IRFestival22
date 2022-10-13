using IRFestival.Api.Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private CosmosClient _cosmosClient { get; set; }
        public Container _websiteArticlesContainer { get; set; }

        public ArticlesController(IConfiguration configuration)
        {
            _cosmosClient = new CosmosClient(configuration.GetConnectionString("CosmosConnection"));
            _cosmosClient.CreateDatabaseIfNotExistsAsync("IRFestivalArticles");
            var database = _cosmosClient.GetDatabase("IRFestivalArticles");
            database.CreateContainerIfNotExistsAsync("WebsiteArticles", "/tag");
            _websiteArticlesContainer = _cosmosClient.GetContainer("IRFestivalArticles", "WebsiteArticles");

        }

        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(Article))]
        public async Task<ActionResult> GetAsync()
        {
            var result = new List<Article>();

            var queryDefinition = _websiteArticlesContainer.GetItemLinqQueryable<Article>().Where(p => p.Status == nameof(Status.Published))
                .OrderBy(p => p.Date);
            var iterator = queryDefinition.ToFeedIterator();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                result = response.ToList();
            }
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult> PostArticle()
        {
            Article dummyArticle = new Article
            {
                Id = "lksdjflsqdkj",
                Date = DateTime.Now,
                Message = "test",
                Status = "Published",
                SubTitle = "Subtitle",
                Tag = "oh a new tag",
                Title = "title oh new",
                ImagePath = "stillemptyfornow"
            };

            await _websiteArticlesContainer.CreateItemAsync(dummyArticle);

            return Ok(dummyArticle);
        }
    }
}