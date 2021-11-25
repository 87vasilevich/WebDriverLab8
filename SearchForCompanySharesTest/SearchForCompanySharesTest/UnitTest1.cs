using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace SearchForCompanySharesTest
{
    public class Tests
    {
        private WebDriverWait wait;

        private IWebDriver driver;        
        private List<IWebElement> CocaColaSharesSearchResultByFullName;
        private List<IWebElement> CocaColaSharesSearchResultByPartialName;
        private List<IWebElement> CocaColaSharesSearchResultByPartialWrongNameSearch;

        private string userLogin = "87vasilevich@gmail.com";
        private string userPassword = "n3S*%27ep3s5R-R";

        private string SearchByFullName { get; set; }
        private string SearchByPartialName { get; set; }
        private string PartialWrongNameSearch { get; set; }

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://exante.eu/live-trade/auth");

            // Открытие браузера в полноэкранном режиме.
            driver.Manage().Window.Maximize();

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            // Ожидание полной загрузки веб-страницы "Авторизация".
            wait.Until(driver => driver.FindElement(By.Id("email")));

            // Выбор демо режима торговли.
            driver.FindElement(By.CssSelector("div[data-test-id='header__div--modeSwitcherDemo']")).Click();

            driver.FindElement(By.XPath("//input[@id='email']")).SendKeys(userLogin);
            driver.FindElement(By.XPath("//input[@id='password']")).SendKeys(userPassword);
            driver.FindElement(By.CssSelector("button[class='_3oZWc JMpAk _3hhP_']")).Click();
        }

        [Test]
        public void SearchForCompanySharesTest()
        {
            // Проблема поиска: база данных, скорее всего, не успевает подгрузиться.
            // Данный костыль помогает базе данных прогрузиться.
            Thread.Sleep(40000);

            SearchByFullName = "ko: coca-cola company";
            SearchByPartialName = "cola";
            PartialWrongNameSearch = "cica-cola";

            driver.FindElement(By.CssSelector("input[class='_2p0av _3E6J3']")).SendKeys(SearchByFullName);
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[@class='_8cZBM'][text()='KO: Coca-Cola Company']")));
            CocaColaSharesSearchResultByFullName = driver.FindElements(By.XPath("//div[@class='_8cZBM'][text()='KO: Coca-Cola Company']")).ToList();

            driver.FindElement(By.CssSelector("input[class='_2p0av _3E6J3']")).Clear();

            // Вероятность того, что предыдущий результат поиска не успеет исчезнуть, высока.
            // Из-за этого использую костыль.
            Thread.Sleep(2000);

            driver.FindElement(By.CssSelector("input[class='_2p0av _3E6J3']")).SendKeys(SearchByPartialName);
            wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//div[@class='_8cZBM'][text()='KO: Coca-Cola Company']")));
            CocaColaSharesSearchResultByPartialName = driver.FindElements(By.XPath("//div[@class='_8cZBM'][text()='KO: Coca-Cola Company']")).ToList();

            driver.FindElement(By.CssSelector("input[class='_2p0av _3E6J3']")).Clear();
            Thread.Sleep(2000);

            driver.FindElement(By.CssSelector("input[class='_2p0av _3E6J3']")).SendKeys(PartialWrongNameSearch);

            //Использую костыль, так как в резльтате данного поиска ничего не выводится.
            Thread.Sleep(2000);

            CocaColaSharesSearchResultByPartialWrongNameSearch = driver.FindElements(By.XPath("//div[@class='_8cZBM'][text()='KO: Coca-Cola Company']")).ToList();

            Assert.IsTrue(CocaColaSharesSearchResultByFullName.Count == 1 && CocaColaSharesSearchResultByPartialName.Count == 1
                && CocaColaSharesSearchResultByPartialWrongNameSearch.Count == 0);
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}