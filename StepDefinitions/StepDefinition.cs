using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System;
using System.Threading;
using TechTalk.SpecFlow;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager;

namespace CorreiosAutomation
{
    [Binding]
    public class CorreiosSteps
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [BeforeScenario]
        public void Setup()
        {
            // Configuração do WebDriver
            new DriverManager().SetUpDriver(new ChromeConfig());
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [AfterScenario]
        public void Teardown()
        {
            // Fechar o navegador após o cenário
            driver.Quit();
        }

        [Given(@"que eu estou no site de Busca CEP dos Correios")]
        public void DadoQueEuEstouNoSiteDosCorreios()
        {
            driver.Navigate().GoToUrl("https://buscacepinter.correios.com.br/app/endereco/index.php");
            DetectarECaptura();
        }

        [When(@"eu procuro pelo CEP ""(.*)""")]
        public void QuandoEuProcuroPeloCEP(string cep)
        {
            DetectarECaptura();
            var searchBox = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("endereco")));
            searchBox.Clear();
            searchBox.SendKeys(cep);
            searchBox.SendKeys(Keys.Enter);
        }

        [Then(@"o CEP deve ser inexistente")]
        public void EntaoOCepDeveSerInexistente()
        {
            DetectarECaptura();
            var errorMessage = wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//div[contains(text(),'não encontrado')]")));
            Assert.IsTrue(errorMessage.Displayed, "A mensagem de 'não encontrado' não foi exibida.");
        }

        [Then(@"o resultado deve ser ""(.*)""")]
        public void EntaoOResultadoDeveSer(string endereco)
        {
            DetectarECaptura();
            var result = driver.FindElement(By.CssSelector(".resultado > strong"));
            Assert.AreEqual(endereco, result.Text);
        }

        [When(@"eu procuro no rastreamento pelo código ""(.*)""")]
        public void QuandoEuProcuroNoRastreamentoPeloCodigo(string codigo)
        {
            driver.Navigate().GoToUrl("https://www.correios.com.br/rastreamento");
            DetectarECaptura();
            var searchBox = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("objetos")));
            searchBox.Clear();
            searchBox.SendKeys(codigo);
            driver.FindElement(By.XPath("//button[contains(text(),'Buscar')]")).Click();
        }

        [Then(@"o código deve estar incorreto")]
        public void EntaoOCodigoDeveEstarIncorreto()
        {
            DetectarECaptura();
            var result = wait.Until(ExpectedConditions.ElementIsVisible(By.CssSelector(".alert-warning")));
            Assert.IsTrue(result.Displayed, "A mensagem de 'código incorreto' não foi exibida.");
        }

        // Método para detectar e lidar com CAPTCHA
        private void DetectarECaptura()
        {
            try
            {
                // Verifica se o CAPTCHA está presente
                if (driver.FindElements(By.CssSelector("div.g-recaptcha")).Count > 0)
                {
                    Console.WriteLine("CAPTCHA detectado. Resolva o CAPTCHA manualmente.");
                    Thread.Sleep(TimeSpan.FromSeconds(30)); // Pausa por 30 segundos para o CAPTCHA ser resolvido manualmente
                }
            }
            catch (NoSuchElementException)
            {
                // CAPTCHA não encontrado, seguir em frente
            }
        }
    }
}
