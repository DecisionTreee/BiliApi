using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Xml.Linq;

namespace BiliApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DynamicController : ControllerBase
    {
        private readonly ILogger<DynamicController> _logger;

        public DynamicController(ILogger<DynamicController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetLatestDynamic")]
        public Dynamic Get(string uid)
        {
            string time = string.Empty;
            string guid = string.Empty;
            string snapshot = string.Empty;

            var service = EdgeDriverService.CreateDefaultService();
            var options = new EdgeOptions();
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--no-sandbox");
            options.PageLoadStrategy = PageLoadStrategy.Normal;

        restart:
            using (IWebDriver driver = new EdgeDriver(service, options)) 
            {
                //driver.Manage().Window.Maximize();
                driver.Navigate().GoToUrl($"https://space.bilibili.com/{uid}/dynamic/");
                driver.Manage().Cookies.DeleteAllCookies();
                driver.Manage().Cookies.AddCookie(new Cookie("SESSDATA", "", ".bilibili.com", "/", null));
                driver.Navigate().Refresh();

                Task.Delay(1200).Wait();
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(1000));
                ReadOnlyCollection<IWebElement> elements = wait.Until((d) =>
                {
                    return d.FindElements(By.ClassName("bili-dyn-list__item"));
                });
                if (elements.Count == 0)
                {
                    driver.Dispose();
                    goto restart;
                }

                ReadOnlyCollection<IWebElement> firstDynamic = elements[0].FindElements(By.ClassName("bili-dyn-item__tag"));
                if (firstDynamic.Count == 0)
                {
                    IWebElement latestDynamicTime = elements[0].FindElement(By.ClassName("bili-dyn-time"));
                    time = latestDynamicTime.Text;
                    Screenshot screenshot = ((ITakesScreenshot)elements[0]).GetScreenshot();
                    guid = Guid.NewGuid().ToString();
                    screenshot.SaveAsFile($"resource/dynamic/{guid}.png");
                    snapshot = Convert.ToBase64String(screenshot.AsByteArray);
                }
                else
                {
                    IWebElement latestDynamicTime = elements[1].FindElement(By.ClassName("bili-dyn-time"));
                    time = latestDynamicTime.Text;
                    Screenshot screenshot = ((ITakesScreenshot)elements[1]).GetScreenshot();
                    guid = Guid.NewGuid().ToString();
                    screenshot.SaveAsFile($"resource/dynamic/{guid}.png");
                    snapshot = Convert.ToBase64String(screenshot.AsByteArray);
                }
            }

            return new Dynamic()
            {
                time = time,
                guid = guid,
                snapshot = snapshot
            };
        }
    }
}
