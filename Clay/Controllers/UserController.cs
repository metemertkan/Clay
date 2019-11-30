using System;
using System.Collections.Generic;
using Clay.Models.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clay.Controllers
{
    //[Authorize(Roles = "User,Administrator")]
    public class UserController : Controller
    {
        [HttpGet]
        public List<Lock> GetMyLocks()
        {
            return null;
        }

        [HttpGet]
        public List<Attempt> GetMyHistory()
        {
            return null;
        }

        [HttpGet]
        public IActionResult GetLockHistory(string lockId)
        {
            return Ok(new List<Lock>() { new Lock { Id = Guid.NewGuid(), Name = lockId } });
        }

        [HttpPost]
        public void Lock(string lockId)
        {

        }

        [HttpPost]
        public void UnLock(string lockId)
        {

        }

    }
}