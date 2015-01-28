using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Giventocode.AzureSearchEntityManager
{
    public class AzureSearchManager
    {
        private string _apiVersion;
        private string _apiKey;
        private string _serviceName;
        private string _indexName;
        private Uri _indexUri;
        private Uri _indexPostUri;
        private Uri _docsOpsUri;
        private Uri _searchOpsUri;
                
        private Uri IndexUri
        {
            get
            {
                if (_indexUri == null)
                {
                    _indexUri = new Uri(string.Format("https://{0}.search.windows.net/indexes/{1}?api-version={2}",_serviceName,_indexName,_apiVersion));
                }
                return _indexUri;
            }
        }        
        private Uri IndexPostUri
        {
            get
            {
                if (_indexPostUri == null)
                {
                    _indexPostUri = new Uri(string.Format("https://{0}.search.windows.net/indexes?api-version={1}", _serviceName,_apiVersion));
                }
                return _indexPostUri;
            }
        }        
        private Uri DocsOpsUri
        {
            get
            {
                if (_docsOpsUri == null)
                {
                    _docsOpsUri = new Uri(string.Format("https://{0}.search.windows.net/indexes/{1}/docs/index?api-version={2}", _serviceName,_indexName,_apiVersion));
                }
                return _docsOpsUri;
            }
        }
        private Uri SearchOpsUri
        {
            get
            {
                if (_searchOpsUri == null)
                {
                    _searchOpsUri = new Uri(string.Format("https://{0}.search.windows.net/indexes/{1}/docs?api-version={2}", _serviceName, _indexName, _apiVersion));
                }
                return _searchOpsUri;
            }
        }

        public AzureSearchManager()
        {            
            _serviceName = Utils.GetRequiredConfigurationValue("ASEM_SearchServiceName");
            _indexName = Utils.GetRequiredConfigurationValue("ASEM_IndexName");
            _apiVersion = Utils.GetRequiredConfigurationValue("ASEM_ApiVersion");
            _apiKey = Utils.GetRequiredConfigurationValue("ASEM_ApiKey");
        }

        public AzureSearchManager(string serviceName, string indexName, string apiVersion, string apiKey)
        {
            
            if (serviceName == null)
            {
                throw new ArgumentNullException("serviceName");
            }
            if (indexName == null)
            {
                throw new ArgumentNullException("indexName");
            }
            if (apiVersion == null)
            {
                throw new ArgumentNullException("apiVersion");
            }
            if (apiKey == null)
            {
                throw new ArgumentNullException("apiKey");
            }

            _serviceName = serviceName;
            _indexName = indexName;
            _apiVersion = apiVersion;
            _apiKey = apiKey;
            
        }

        public async Task<JObject> SearchAsync(string criteria)
        {
            using (var http = new HttpClient())
            {
                var uri = new Uri(this.SearchOpsUri + "&search=" + criteria);
             
                http.DefaultRequestHeaders.Add("api-key", _apiKey);

                return  JsonConvert.DeserializeObject<JObject>(await http.GetStringAsync(uri));                
            }
        }

        public Task IndexDocumentAsync(string bodyMsg)
        {
            return IndexDocumentAsync(JsonConvert.DeserializeObject<EntityIndexInfo>(bodyMsg));
        }        

        public async Task IndexDocumentAsync(EntityIndexInfo indexInfo)
        {            
            await CreateIndexIfNotExistsAsync(indexInfo);

            var doc = GetIndexDocument(indexInfo);
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", _apiKey);

                var m = new HttpRequestMessage(HttpMethod.Post, this.DocsOpsUri);
                m.Content = new StringContent(JsonConvert.SerializeObject(doc));
                m.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                var response = await client.SendAsync(m);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(string.Format("Failed to perform the index update for the document. Response reason phrase {0}", response.ReasonPhrase));
                }

            }

        }

        private IndexDocument GetIndexDocument(EntityIndexInfo indexInfo)
        {
            var indexDoc = new IndexDocument();
            var dic = new Dictionary<string, object>();

            dic.Add("@search.action", indexInfo.Action);

            var keyPropName = indexInfo.GetKeyPropertyName();

            foreach (var prop in indexInfo.Properties)
            {
                if (indexInfo.Action != "delete" || prop.Name == keyPropName)
                {
                    dic.Add(prop.Name, prop.Value);
                }
            };

            indexDoc.Values.Add(dic);

            return indexDoc;
        }

        private async Task CreateIndexIfNotExistsAsync(EntityIndexInfo indexSchema)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("api-key", _apiKey);

                var response = await client.GetAsync(this.IndexUri);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    var indexModel = new IndexModel()
                    {                        
                        Name = _indexName,
                        Fields = indexSchema.Properties
                                .Select<EntityPropertyIndexInfo, Dictionary<string, object>>(p => p.IndexAttributes)
                                .ToList<Dictionary<string, object>>()
                    };
               

                    var m = new HttpRequestMessage(HttpMethod.Post, this.IndexPostUri);
                    m.Content = new StringContent(JsonConvert.SerializeObject(indexModel));
                    m.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    response = await client.SendAsync(m);

                    if (!response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        throw new Exception(string.Format("Failed to created the index. ResponsePhrase: {0} /n Content: {1}", response.ReasonPhrase,responseContent));
                    }

                }
            }

        }    
    }
}