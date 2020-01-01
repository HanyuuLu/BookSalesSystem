using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using BookStore.Models;
using Newtonsoft.Json;

namespace BookStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            Console.WriteLine("/login");
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            if (username == null || password == null)
            {
                ViewData["msg"] = "please input username and password.";
                return View();
            }
            else
            {
                Models.DatabaseConn conn = (Models.DatabaseConn)HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
                User res = conn.Login(username);
                if (res.uuid>0)
                {
                    // User exists
                    if (password==res.password)
                    {
                        //success, login.
                        Response.Cookies.Append("uuid", res.uuid.ToString());
                        Response.Cookies.Append("username", res.username);
                        return LocalRedirect("/home/ShoppingBasket");
                    }
                    else
                    {
                        ViewData["msg"] = "Wrong password.";
                        return View();
                    }
                }
                else
                {
                    ViewData["msg"] = "no such user";
                    return View();
                }
            }
        }
        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }
        [HttpPost]
        public IActionResult Register(string username, string password, string confirmPassword)
        {
            if (username == null || password == null || confirmPassword == null)
            {
                ViewData["msg"] = "inconplete information.";
                return View();
            }
            else if (password != confirmPassword)
            {
                ViewData["msg"] = "password is not same as confirm password.";
                return View();
            }
            //else if (username == "Hanyuu"/*TODO 重名*/)
            //{
            //    ViewData["msg"] = "username already token.";
            //    return View();
            //}
            else
            {
                Models.DatabaseConn conn = (Models.DatabaseConn)HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
                User res = conn.Register(username, password);
                if (res.uuid==0)
                {
                    //username already token
                    ViewData["msg"] = "username already token.";
                    return View();
                }
                else
                {
                    IActionResult loginRes= Login(username, password);
                    return loginRes;
                }
            }
        }

        public IActionResult Search(string isbn, string title, string author)
        {
            if (isbn==null && title==null && author==null)
            {
                return View();
            }
            Models.DatabaseConn conn=(Models.DatabaseConn) HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
            List<Models.Book> res = conn.GetBooks(isbn, title, author);
            ViewData["res"] = res;
            return View();
        }
        [HttpGet]
        public IActionResult BookInformation(string isbn)
        {
            Models.DatabaseConn conn = (Models.DatabaseConn)HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
            Book res = conn.GetBookInformation(isbn);
            if (res==null)
            {
                ViewData["msg"] = "no such book, please check your requets or ask worker for help.";
            }
            else
            { 
                ViewData["title"] = res.title;
                ViewData["author"] = res.author;
                ViewData["isbn"] = res.isbn;
                string available;
                if (res.reserve<0)
                {
                    available = "in for weeks.";
                }
                else if(res.reserve<=4)
                {
                    available = "in one week.";
                }
                else if (res.reserve<=19)
                {
                    available = "about 2 to 3 days.";
                }
                else
                {
                    available = "about a day or more.";
                }
                ViewData["available"] = available;

            }

            return View();
        }

        [HttpPost]
        public IActionResult BookInformation(string isbn,int meanless)
        {
            Models.DatabaseConn conn = (Models.DatabaseConn)HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
            Book res = conn.GetBookInformation(isbn);
            if (res==null)
            {
                // bad request
                return StatusCode(404);
            }
            else
            {
                // okey
                if (Request.Cookies["basket"]==null)
                {
                    Dictionary<string, int> basket = new Dictionary<string, int>();
                    basket.Add(isbn, 1);
                    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
                }
                else
                {
                    Dictionary<string, int> basket = JsonConvert.DeserializeObject<Dictionary<string, int>>(Request.Cookies["basket"]);
                    if (basket.ContainsKey(isbn))
                    {
                        basket[isbn] += 1;
                    }
                    else
                    {
                        basket[isbn] = 1;
                    }
                    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
                }
                return StatusCode(200);
            }
        }
        [HttpGet]
        public IActionResult ShoppingBasket(int? status)
        {
            if(Request.Cookies["username"]==null)
            {
                return LocalRedirect("/home/login");
            }
            else if (Request.Cookies["basket"]==null)
            {
                ViewData["msg"] = "The shopping basket is empty.";
                return View();
            }
            else
            {
                bool submit = false;
                Models.DatabaseConn conn = (Models.DatabaseConn)HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
                Dictionary<string, int> basket = JsonConvert.DeserializeObject<Dictionary<string, int>>(Request.Cookies["basket"]);
                List<Book> order = new List<Book>();
                if (basket.Count()>0)
                {
                    submit = true;
                }
                foreach(KeyValuePair<string,int> item in basket)
                {
                    Book book = conn.GetBookInformation(item.Key);
                    book.quantity = item.Value;
                    order.Add(book);
                }
                ViewData["res"] = order;
                if (submit)
                {
                    ViewData["submitEnabled"] = true;
                }
                return View();
            }
        }
        [HttpPost]
        public IActionResult ShoppingBasket(string isbn, int modify)
        {
            Models.DatabaseConn conn = (Models.DatabaseConn)HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
            Book res = conn.GetBookInformation(isbn);
            if (res == null)
            {
                // bad request
                return StatusCode(404);
            }
            else
            {
                // okey
                if (Request.Cookies["basket"] == null)
                {
                    Dictionary<string, int> basket = new Dictionary<string, int>();
                    basket.Add(isbn, 1);
                    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
                }
                else
                {
                    Dictionary<string, int> basket = JsonConvert.DeserializeObject<Dictionary<string, int>>(Request.Cookies["basket"]);
                    if (basket.ContainsKey(isbn))
                    {
                        basket[isbn] += modify;
                        if (basket[isbn] == 0)
                        {
                            basket.Remove(isbn);
                        }
                    }
                    else
                    {
                        basket[isbn] = 1;
                    }
                    Response.Cookies.Append("basket", JsonConvert.SerializeObject(basket));
                }
                return StatusCode(200);
            }
        }
        public IActionResult Help()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Checkout()
        {
            if (Request.Cookies["username"] == null)
            {
                return LocalRedirect("/home/login");
            }
            else if (Request.Cookies["basket"] == null)
            {
                ViewData["msg"] = "The shopping basket is empty.";
                return View();
            }
            else
            {
                bool submit = false;
                Models.DatabaseConn conn = (Models.DatabaseConn)HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
                Dictionary<string, int> basket = JsonConvert.DeserializeObject<Dictionary<string, int>>(Request.Cookies["basket"]);
                List<Book> order = new List<Book>();
                if (basket.Count() > 0)
                {
                    submit = true;
                }
                foreach (KeyValuePair<string, int> item in basket)
                {
                    Book book = conn.GetBookInformation(item.Key);
                    book.quantity = item.Value;
                    order.Add(book);
                }
                ViewData["res"] = order;
                if (submit)
                {
                    ViewData["submitEnabled"] = true;
                }
                return View();
            }

        }
        [HttpPost]
        public IActionResult Checkout(string cardNumber,string address)
        {
            if (cardNumber == null || address == null)
            {
                if (Request.Cookies["username"] == null)
                {
                    return LocalRedirect("/home/login");
                }
                else if (Request.Cookies["basket"] == null)
                {
                    ViewData["msg"] = "The shopping basket is empty.";
                    return View();
                }
                else
                {
                    bool submit = false;
                    Models.DatabaseConn conn = (Models.DatabaseConn)HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
                    Dictionary<string, int> basket = JsonConvert.DeserializeObject<Dictionary<string, int>>(Request.Cookies["basket"]);
                    List<Book> order = new List<Book>();
                    if (basket.Count() > 0)
                    {
                        submit = true;
                    }
                    foreach (KeyValuePair<string, int> item in basket)
                    {
                        Book book = conn.GetBookInformation(item.Key);
                        book.quantity = item.Value;
                        order.Add(book);
                    }
                    ViewData["res"] = order;
                    if (submit)
                    {
                        ViewData["submitEnabled"] = true;
                        ViewData["status"] = "confirm";
                    }
                    return View();
                }
            }
            else
            {
                string uuid = Request.Cookies["uuid"];
                ViewData["msg"] = "order commited.";
                ViewData["status"] = "finished";
                Models.DatabaseConn conn = (Models.DatabaseConn)HttpContext.RequestServices.GetService(typeof(Models.DatabaseConn));
                Dictionary<string, int> basket = JsonConvert.DeserializeObject<Dictionary<string, int>>(Request.Cookies["basket"]);
                foreach (KeyValuePair<string, int> item in basket)
                {
                    conn.Order(uuid, item.Key, item.Value, cardNumber, address);
                }
                return View();
            }

        }
        // public IActionResult Privacy() => View();

        // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
