using System.Collections.Generic;
using Clay.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Clay.Controllers
{
    [Route("api/admin")]
    [ApiController]
    public class AdminController : Controller
    {
        [HttpGet]
        public List<Lock> GetLocks()
        {
            return null;
        }

        [HttpPost]
        public void CreateLock()
        {

        }

        [HttpPost]
        public void DeleteLock()
        {

        }


        [HttpPost]
        public void AssignUserToLock()
        {

        }

        [HttpPost]
        public void UnAssignUserFromLock()
        {

        }

        [HttpGet]
        public void GetAllAttempts()
        {

        }
    }
}