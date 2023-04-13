using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace StaffServiceTests_4;

public class StaffServiceTests
{
    private ChromeDriver driver;
    private WebDriverWait wait;



    [SetUp]
    public void Setup()
    {
        var options = new ChromeOptions();
        options.AddArguments("--no-sandbox", "--start-maximized", "--disable-extensions");

        driver = new ChromeDriver(options);
        
        Authorization();
    }

    public void Authorization()
    {
        driver.Navigate().GoToUrl("https://staff-testing.testkontur.ru/");
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        var login = driver.FindElement(By.Id("Username"));
        login.SendKeys("user");
        var password = driver.FindElement(By.Id("Password"));
        password.SendKeys("1q2w3e4r%T");
        var enter = driver.FindElement(By.Name("button"));
        enter.Click();

        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(3));
        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/news"));
    }

    [Test]
    public void AuthorizationTest()
    {
        var titlePageElement = driver.FindElement(By.CssSelector("[data-tid='Title']"));
        var titleInBrowser = driver.Title;
        Assert.Multiple(() =>
        {
            Assert.AreEqual("Новости", titlePageElement.Text, "Новости не загрузились");
            Assert.AreEqual("Новости", titleInBrowser);
        });
    }


    [Test]
    public void NavigationMenuElementTest()
    {
        var sidebarMenuButton = driver.FindElement(By.CssSelector("[data-tid='SidebarMenuButton']"));
        sidebarMenuButton.Click();

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='SidePage__root']")));

        var community = driver.FindElements(By.CssSelector("[data-tid='Community']"))
            .First(element => element.Displayed);
        community.Click();

        wait.Until(ExpectedConditions.UrlToBe("https://staff-testing.testkontur.ru/communities"));
        var titlePageElement = driver.FindElement(By.CssSelector("[data-tid='Title']"));

        Assert.AreEqual("Сообщества", titlePageElement.Text, "На странице 'сообщества' нет заголовка 'Сообщества'");
    }

    [Test]
    public void SearchTest()
    {
        var search = driver.FindElement(By.CssSelector("[data-tid='SearchBar']"));
        search.Click();
        var searchInput =
            driver.FindElement(
                By.CssSelector("[placeholder='Поиск сотрудника, подразделения, сообщества, мероприятия']"));
        searchInput.SendKeys("агапова алиса алексеевна ");

        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='ScrollContainer__inner']")));
        var scrollElement = driver.FindElement(By.CssSelector("[data-tid='ComboBoxMenu__item']"));

        scrollElement.Click();
        wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector("[data-tid='EmployeeName']")));

        var profileId = driver.Url;
        Assert.AreEqual("https://staff-testing.testkontur.ru/profile/f23f7980-6b93-4959-9fc0-dbc3359c0dbb", profileId,
            "Не открылся профиль сотрудника");
    }

    [Test]
    public void Test()
    {
        var profileIcon = driver.FindElement(By.CssSelector("[data-tid='Avatar']"));
        profileIcon.Click();

        var profile = driver.FindElement(By.CssSelector("[data-tid='Profile']"));
        profile.Click();
        wait.Until(ExpectedConditions.UrlContains("https://staff-testing.testkontur.ru/profile/"));

        var profileLabel = driver.FindElement(By.CssSelector("[data-tid='EmployeeName']"));
        Assert.AreEqual("User", profileLabel.Text, "Не открылся профиль пользователя");
    }


    [TearDown]
    public void TearDown()
    {
        driver.Quit();
    }
}