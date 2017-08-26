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
using TrabalhoFinal.Models;
using System.Security.Principal;

namespace TrabalhoFinal.Controllers
{
    [Authorize]
    [RoutePrefix("api/orders")]
    public class OrdersController : ApiController
    {
        private TrabalhoFinalContext db = new TrabalhoFinalContext();

        [ResponseType(typeof(Product))]
        [HttpGet]
        [Route("byemail")]
        public IHttpActionResult GetOrderByEmail(string email)
        {
            var order = db.Orders.Where(p => p.userName == email);
            if (order == null)
            {
                return BadRequest("Sorry, não temos esse pedido cadastrado.");
            }
            return Ok(order);
        }
        // POST: api/Orders
        [ResponseType(typeof(Order))]
        [HttpPost]
        [Route("closeOrder")]
        public IHttpActionResult CloseOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (checkUserFromOrder(User, order))
            {
                if (order == null)
                {
                    return BadRequest("Sorry, não temos esse pedido cadastrado.");
                }
                else
                {
                    if (order.status != "fechado")
                    {
                        if (order.precoFrete == 0)
                        {
                            return BadRequest("Sorry, preço do frete ainda não foi calculado.");
                        }
                        else
                        {
                            order.status = "fechado";
                            db.SaveChanges();
                            return BadRequest("Seu pedido foi fechado com sucesso.");
                        }
                    }
                    return BadRequest("Sorry, pedido foi fechado anteriormente.");
                }
            }
            else
            {
                return BadRequest("Sorry, você não é ADMIN e nem esse pedido é seu!");
            }
        }


        // GET: api/Orders
        [Authorize(Roles = "ADMIN")]
        public IQueryable<Order> GetOrders()
        {
            return db.Orders;
        }

        // GET: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult GetOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (checkUserFromOrder(User, order)) {
                if (order == null)
                {
                    return BadRequest("Sorry, não temos esse pedido cadastrado.");
                }
                return Ok(order);
            }
            else
            {
                return BadRequest("Sorry, você não é ADMIN e nem esse pedido é seu!");
            }

        }

        // PUT: api/Orders/5
        [Authorize]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutOrder(int id, Order order)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != order.Id)
            {
                return BadRequest();
            }

            db.Entry(order).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
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

        // POST: api/Orders
        [ResponseType(typeof(Order))]
        public IHttpActionResult PostOrder(Order order)
        {
            order.status = "novo";
            order.userName = User.Identity.Name;
            order.pesoTotal = 0;
            order.precoTotal = 0;
            order.precoFrete = 0;
            order.data = DateTime.Now;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Orders.Add(order);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = order.Id}, order);
        }

        // DELETE: api/Orders/5
        [ResponseType(typeof(Order))]
        public IHttpActionResult DeleteOrder(int id)
        {
            Order order = db.Orders.Find(id);
            if (checkUserFromOrder(User, order))
            {
                if (order == null)
                {
                    return BadRequest("Sorry, não temos esse pedido cadastrado.");
                }
                db.Orders.Remove(order);
                db.SaveChanges();

                return Ok(order);
            }
            else
            {
                return BadRequest("Sorry, você não é ADMIN e nem esse pedido é seu!");
            }          
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OrderExists(int id)
        {
            return db.Orders.Count(e => e.Id == id) > 0;
        }

        private bool checkUserFromOrder(IPrincipal user, Order order)
        {
            return ((user.Identity.Name.Equals(order.userName)) || (user.IsInRole("ADMIN")));
        }

    }
}