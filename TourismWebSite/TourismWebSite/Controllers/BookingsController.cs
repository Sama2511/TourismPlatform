﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TourismWebSite.Controllers
{
    public class BookingsController : Controller
    {
        // GET: Bookings
        public ActionResult Index()
        {
            return View();
        }
    }
}