using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WebAPI0911.Models;

namespace WebAPI0911.Controllers
{
    [RoutePrefix("clients")]
    public class ClientsController : ApiController
    {
        private FabricsEntities db = new FabricsEntities();

        public ClientsController()
        {
            db.Configuration.LazyLoadingEnabled = false;
        }

        // GET: api/Clients
        public IQueryable<Client> GetClient()
        {
            return db.Client;
        }

        // GET: api/Clients/5
        [ResponseType(typeof(Client))]
        [Route("{id}",Name = "GetOrderById")]
        public IHttpActionResult GetClient(int id)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        // GET: api/Clients/5
        [ResponseType(typeof(Client))]
        [Route("~/clients/type2/{id}")]
        public Client GetClientType2(int id)
        {
            return db.Client.Find(id);
        }

        // GET: api/Clients/5
        [ResponseType(typeof(Client))]
        [Route("~/clients/type3/{id}")]
        public IHttpActionResult GetClientType3(int id)
        {
            return Json(db.Client.Find(id));
        }

        [Route("~/clients/type4/{id:int}")]
        public HttpResponseMessage GetClientType4(int id)
        {
            var data = db.Client.Find(id);

            return Request.CreateResponse(HttpStatusCode.OK, data);
        }

        [Route("~/clients/type5/{id:int}")]
        public HttpResponseMessage GetClientType5(int id)
        {
            var data = db.Client.Find(id);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ObjectContent<Client>(data,
                    GlobalConfiguration.Configuration.Formatters.JsonFormatter)
            };
        }

        [Route("~/clients/type6/{id:int}")]
        public HttpResponseMessage GetClientType6(int id)
        {
            var data = db.Client.Find(id);

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                ReasonPhrase = "Hello World",
                Content = new ObjectContent<Client>(data,
                    GlobalConfiguration.Configuration.Formatters.JsonFormatter)
            };
        }

        [Route("~/clients/type7/{id:int}")]
        public HttpResponseMessage GetClientType7(int id)
        {
            var data = db.Client.Find(id);

            var res = new HttpResponseMessage(HttpStatusCode.OK)
            {
                ReasonPhrase = "Hello World",
                Content = new ObjectContent<Client>(data,
                    GlobalConfiguration.Configuration.Formatters.JsonFormatter)
            };

            res.Headers.Add("X-JobId", "1");

            return res;
        }

        // GET: api/clients/{id}/orders
        [Route("{id}/orders")]
        public IHttpActionResult GetClientOrders(int id)
        {
            var orders = db.Order.Where(p => p.ClientId == id);
            return Ok(orders);
        }

        // GET: api/clients/{id}/orders/1
        [Route("{id}/orders/{orderId}")]
        public IHttpActionResult GetClientOrders(int id,int orderId)
        {
            var order = db.Order.Where(p => p.ClientId == id && p.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // GET: api/clients/{id}/orders/pending
        [Route("{id}/orders/pending")]
        public IHttpActionResult GetClientOrdersPending(int id)
        {
            var order = db.Order.Where(p => p.ClientId == id && p.OrderStatus == "P");

            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        // GET: api/clients/1/orders/2001/05/27
        [Route("{id}/orders/{*date}")]
        public IHttpActionResult GetClientOrdersbyDate(int id,DateTime date)
        {
            var order = db.Order.Where(p => p.ClientId == id 
            && p.OrderDate.Value.Year == date.Year
            && p.OrderDate.Value.Month == date.Month
            && p.OrderDate.Value.Day == date.Day
            );

            if (order == null || order.Count() ==0 )
            {
                return NotFound();
            }

            return Ok(order);
        }

        // PUT: api/Clients/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutClient(int id, Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != client.ClientId)
            {
                return BadRequest();
            }

            db.Entry(client).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Clients
        [ResponseType(typeof(Client))]
        [Route("")]
        public IHttpActionResult PostClient(Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Client.Add(client);
            db.SaveChanges();

            return CreatedAtRoute("GetOrderById", new { id = client.ClientId }, client);
        }

        // DELETE: api/Clients/5
        [ResponseType(typeof(Client))]
        public IHttpActionResult DeleteClient(int id)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            db.Client.Remove(client);
            db.SaveChanges();

            return Ok(client);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ClientExists(int id)
        {
            return db.Client.Count(e => e.ClientId == id) > 0;
        }
    }
}