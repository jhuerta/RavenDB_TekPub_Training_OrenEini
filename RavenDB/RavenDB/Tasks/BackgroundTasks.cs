using System;
using System.IO;
using System.Net;
using NLog;
using Newtonsoft.Json;
using Raven.Client;

namespace RavenDB.Tasks
{
    public abstract class BackgroundTasks
    {

        protected IDocumentSession documentSession;
        private Logger log = LogManager.GetCurrentClassLogger();

        public abstract void Execute();
        public virtual void Initialize()
        {
            documentSession = MvcApplication.DocumentStore.OpenSession();
        }

        public void Run()
        {
            Initialize();
            try
            {
                Execute();
                documentSession.SaveChanges();
            }
            catch (Exception ex)
            {
                
                log.ErrorException("Error processing task" + ToString(), ex);
            }
        }

        public override abstract string ToString();
    }

    class ProcessNewOrderTask : BackgroundTasks
    {
        public string OrderId { get; set; }

        public override void Execute()
        {
            var request = WebRequest.Create("https://www.google.com?q=" + OrderId);

            WebResponse webresponse = null;
            try
            {
                webresponse = request.GetResponse();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            using (var response = webresponse)
            {
                using (var stream = response.GetResponseStream())
                {
                    using(var reader = new StreamReader(stream))
                    {
                        var jsonSerializer = new JsonSerializer();
                        var orderMessage = jsonSerializer.Deserialize <OrderMessage> (new JsonTextReader(reader));
                    }
                }
            }
        }

        public override string ToString()
        {
            return string.Format("OrderId: {0}", OrderId);
        }
    }

    internal class OrderMessage
    {
        private string Id { get; set; }
        private string Name { get; set; }
        private string Quantity { get; set; }
        private string Notes { get; set; }

        public override string ToString()
        {
            return string.Format("Id: {0}, Name: {1}, Quantity: {2}, Notes: {3}", Id, Name, Quantity, Notes);
        }
    }
}