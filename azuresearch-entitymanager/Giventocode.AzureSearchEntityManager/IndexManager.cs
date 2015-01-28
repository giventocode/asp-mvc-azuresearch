using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Microsoft.WindowsAzure;
using Newtonsoft.Json;
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
    public class IndexManager
    {
        private QueueClient _qClient;        
        public  const string INDEXER_QUEUE = "indexerqueue";

        public IndexManager()
        {
            _qClient = GetQClient(Utils.GetRequiredConfigurationValue("ASEM_SbConnString"));
         
        }
        
        public IndexManager(string sbConnectionString)
        {
            if (sbConnectionString == null)
            {
                throw new ArgumentNullException("sbConnectionString");
            }
            
            _qClient = GetQClient(sbConnectionString);
            
        }
        
        public TData EnqueueEntity<TData>(TData entity, string action) where TData : class
        {                        
            var ixEntity = GetEntityIndexInfo<TData>(entity, action);
            
            var msg = new BrokeredMessage(JsonConvert.SerializeObject(ixEntity));
            msg.TimeToLive = new TimeSpan(7, 0, 0, 0);
            _qClient.Send(msg);

            return entity;
        }
         
        public async Task<Boolean> ReadFromQueueAndIndexAsync(AzureSearchManager searchMan)
        {
            if (searchMan == null)
            {
                throw new ArgumentNullException("searchMan");
            }

            try
            {
                var msg = _qClient.Receive();

                if (msg == null)
                {
                    return false;
                }
                
                await searchMan.IndexDocumentAsync(msg.GetBody<string>());

                _qClient.Complete(msg.LockToken);

                return true;
            }
            catch(Exception ex)
            {
                _qClient.Abort();

                throw ex;
            }

        }
        
        private IndexDocument GetIndexDocument(EntityIndexInfo indexInfo)
        {
            var indexDoc =new IndexDocument();
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

        private EntityIndexInfo GetEntityIndexInfo<TData>(TData entity, string action) where TData : class
        {
            var ixEntity = new EntityIndexInfo() { Action = action };
            var props = typeof(TData).GetProperties();

            foreach (var prop in props)
            {
                var att = prop.GetCustomAttribute<IndexableAttribute>();

                if (att != null)
                {
                    var name = att.Name;
                    if (string.IsNullOrEmpty(name))
                    {
                        name = prop.Name;
                    }

                    var ixAtt = new EntityPropertyIndexInfo() { Name = name.ToLower(), Value = prop.GetValue(entity), IndexDefinition = att };


                    if (att.Key)
                    {
                        ixAtt.IndexAttributes.Add("name", name.ToLower());
                        ixAtt.IndexAttributes.Add("type", "Edm.String");
                        ixAtt.IndexAttributes.Add("key", "true");
                    }
                    else
                    {
                        ixAtt.IndexAttributes.Add("name", name.ToLower());
                        ixAtt.IndexAttributes.Add("type", att.Type);
                        ixAtt.IndexAttributes.Add("facetable", att.Facetable);
                        ixAtt.IndexAttributes.Add("filterable", att.Filterable);
                        ixAtt.IndexAttributes.Add("searchable", att.Searchable);
                        ixAtt.IndexAttributes.Add("sortable", att.Sortable);
                        ixAtt.IndexAttributes.Add("suggestions", att.Suggestions);
                        ixAtt.IndexAttributes.Add("retrievable", att.Retrievable);
                    }

                    ixEntity.Properties.Add(ixAtt);
                }

            }

            return ixEntity;
        }
        
        private QueueClient GetQClient(string sbConnString)
        {
            if (sbConnString == null)
            {
                throw new ArgumentNullException("serviceBusConnString");
            }

            var mgr = NamespaceManager.CreateFromConnectionString(sbConnString);

            if (!mgr.QueueExists(INDEXER_QUEUE))
            {
                var q = mgr.CreateQueue(INDEXER_QUEUE);
            }

            return QueueClient.CreateFromConnectionString(sbConnString, INDEXER_QUEUE);

        }

    }
}