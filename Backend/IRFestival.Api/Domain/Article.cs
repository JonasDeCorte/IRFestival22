using Newtonsoft.Json;

namespace IRFestival.Api.Domain
{
    public class Article
    {
        [JsonProperty(propertyName: "id")]
        public string Id { get; set; }

        [JsonProperty(propertyName: "tag")]
        public string Tag { get; set; }

        [JsonProperty(propertyName: "title")]
        public string Title { get; set; }

        [JsonProperty(propertyName: "subTitle")]
        public string SubTitle { get; set; }

        [JsonProperty(propertyName: "message")]
        public string Message { get; set; }

        [JsonProperty(propertyName: "imagepath")]
        public string ImagePath { get; set; }

        [JsonProperty(propertyName: "status")]
        public string Status { get; set; }

        [JsonProperty(propertyName: "date")]
        public DateTime Date { get; set; }
    }

    public enum Status
    {
        Published,
        Unpublished
    }
}