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
        public IHttpActionResult GetClient(int id)
        {
            Client client = db.Client.Find(id);
            if (client == null)
            {
                return NotFound();
            }

            return Ok(client);
        }

        // GET: api/clients/{id}/orders
        [Route("api/clients/{id}/orders")]
        public IHttpActionResult GetClientOrders(int id)
        {
            var orders = db.Order.Where(p => p.ClientId == id);
            return Ok(orders);
        }

        // GET: api/clients/{id}/orders/1
        [Route("api/clients/{id}/orders/{orderId}")]
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
        [Route("api/clients/{id}/orders/pending")]
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
        [Route("api/clients/{id}/orders/{*date}")]
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
        public IHttpActionResult PostClient(Client client)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Client.Add(client);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = client.ClientId }, client);
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