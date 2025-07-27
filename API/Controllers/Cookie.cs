//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class Cookie : ControllerBase
//    {
//        [HttpGet("GetCulture")]
//        public IActionResult GetCulture()
//        {
//            var cultureCookie = Request.Cookies[".AspNetCore.Culture"];
//            if (!string.IsNullOrEmpty(cultureCookie))
//            {
//                // Parse the culture from the cookie
//                var culture = ParseCultureFromCookie(cultureCookie);
//                return Ok(culture);
//            }
//            return Ok("en-US"); // Default culture
//        }

//        private string ParseCultureFromCookie(string cookieValue)
//        {
//            var parts = cookieValue.Split('|');
//            foreach (var part in parts)
//            {
//                if (part.StartsWith("c=")) // Look for "c=" (culture)
//                {
//                    return part.Substring(2);
//                }
//            }
//            return "en-US"; // Default culture
//        }
//    }
//}
