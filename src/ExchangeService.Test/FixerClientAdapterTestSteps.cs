using ExchangeService.Interfaces;
using ExchangeService.Models;
using ExchangeService.Models.Database;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using TechTalk.SpecFlow;
using System.Configuration;
using System.Threading.Tasks;
using System.Net.Http;
using FluentAssertions;

namespace ExchangeService.Test
{
  [Binding]
  public class FixerClientAdapterTestSteps
  {
    public FixerClientAdapter _fixerClientAdapter;
    private Mock<IHTTPClientAdapter> _httpClientAdapterMock;
    private List<ExchangeRate> _result;
    readonly string[] currencies = ConfigurationManager.AppSettings["Currencies"].Split(',');
    readonly ExchangeRate _rate = new ExchangeRate() { baseCurrency = "USD", targetCurrency = "AUD", exchangeRate = 2, timestamp = new DateTime(2000, 1, 1) };
    readonly ExchangeRate _rate2 = new ExchangeRate() { baseCurrency = "AUD", targetCurrency = "USD", exchangeRate = 0.5m, timestamp = new DateTime(2000, 1, 1) };
    readonly List<ExchangeRate> _rates = new List<ExchangeRate>();

    // Arrange
    [Given(@"there is an error when connecting to the API")]
    public void GivenThereIsAnErrorWhenConnectingToTheAPI()
    {
      _httpClientAdapterMock = new Mock<IHTTPClientAdapter>();
      _httpClientAdapterMock.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws(new IOException());
      _fixerClientAdapter = new FixerClientAdapter(_httpClientAdapterMock.Object);
    }

    // Act
    [Then(@"there is an error when running the adapter")]
    [ExpectedException(typeof(IOException))]
    public async void ThenThereIsAnErrorWhenRunningTheAdapter()
    {
      _result = await _fixerClientAdapter.GetAllData();
    }

    [Given(@"The data given is correct")]
    public void GivenTheDataGivenIsCorrect()
    {
      var resp1 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
      resp1.Content = new StringContent(File.ReadAllText(Directory.GetCurrentDirectory() + @"\CurrencyMocks\USD.json"));

      var resp2 = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
      resp2.Content = new StringContent(File.ReadAllText(Directory.GetCurrentDirectory() + @"\CurrencyMocks\AUD.json"));

      _httpClientAdapterMock = new Mock<IHTTPClientAdapter>();
      _httpClientAdapterMock.Setup(x => x.GetAsync(It.IsAny<string>(), currencies[0], currencies[1])).Returns(Task.FromResult<HttpResponseMessage>(resp1));
      _httpClientAdapterMock.Setup(x => x.GetAsync(It.IsAny<string>(), currencies[1], currencies[0])).Returns(Task.FromResult<HttpResponseMessage>(resp2));
      _fixerClientAdapter = new FixerClientAdapter(_httpClientAdapterMock.Object);
    }

    [When(@"the API is called")]
    public async void WhenTheAPIIsCalled()
    {
      _result = await _fixerClientAdapter.GetAllData();
    }

    // Assert 
    [Then(@"the values returned by the adapter are correct")]
    public void ThenTheValuesReturnedByTheAdapterAreCorrect()
    {
      _rates.Add(_rate);
      _rates.Add(_rate2);
      _result.Should().BeEquivalentTo(_rates, config => config.Excluding(x => x.timestamp));
    }
  }
}
