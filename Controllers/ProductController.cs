using System.Collections.Generic;
using identity.Models;
using Microsoft.AspNetCore.Mvc;

namespace identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        [HttpGet]
        [Route("List")]
        public List<Products> GetList()
        {
            var chair = new Products {Name = "Chair", Price = 100};
            var desk = new Products {Name = "desk", Price = 50 };
            return new List<Products>(){chair, desk};
        } 
    }
}