using ExchangeService.Controllers;
using ExchangeService.Interfaces;
using ExchangeService.Models.Database;
using ExchangeService.Models.DTOs;
using ExchangeService.Models.RedisCache;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using TechTalk.SpecFlow;
using FluentAssertions;
using System.Collections.Generic;
using ExchangeService.ExtensionMethods;

namespace ExchangeService.Test
{
  [Binding]
  public class HomeControllerTestSteps
  {
    private HomeController _appController;
    private Mock<IRepository<IHTTPClientAdapter>> _apiClientMock;
    private Mock<IRepository<DataModel>> _dbRepositoryMock;
    private Mock<IRepository<AzureRedisControllerCache>> _cacheRepositoryMock;
    private string _baseCurrency;
    private string _targetCurrency;
    private IHttpActionResult _result;
    readonly ExchangeRate _rate = new ExchangeRate() { baseCurrency = "USD", targetCurrency = "AUD", exchangeRate = 2, timestamp = new DateTime(2000, 1, 1) };
    readonly List<ExchangeRate> _rates = new List<ExchangeRate>();

    // Arrange
    [Given(@"there is a Home Controller")]
    
    public void GivenThereIsAHomeController()
    {
      _apiClientMock = new Mock<IRepository<IHTTPClientAdapter>>();
      _dbRepositoryMock = new Mock<IRepository<DataModel>>();
      _cacheRepositoryMock = new Mock<IRepository<AzureRedisControllerCache>>();

      _appController = new HomeController(_apiClientMock.Object, _dbRepositoryMock.Object, _cacheRepositoryMock.Object);
    }

    [Given(@"the cache is null or outdated")]
    public void GivenTheCacheIsNullOrOutdated()
    {
      _cacheRepositoryMock.Setup(x => x.GetData(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<ExchangeRate>(null));
    }

    [Given(@"the request is for the currencies ""(.*)"" and ""(.*)""")]
    public void GivenTheRequestIsForTheCurrenciesAnd(string baseCurrency, string targetCurrency)
    {
      _baseCurrency = baseCurrency; _targetCurrency = targetCurrency;
    }

    [Given(@"the cache is not null or outdated")]
    public void GivenTheCacheIsNotNullOrOutdated()
    {
      _cacheRepositoryMock.Setup(x => x.GetData(_baseCurrency, _targetCurrency)).Returns(Task.FromResult<ExchangeRate>(_rate));
    }

    [Given(@"the API is available")]
    public void GivenTheAPIIsAvailable()
    {
      _rates.Add(_rate);
      _apiClientMock.Setup(x => x.GetAllData()).Returns(Task.FromResult(_rates));
    }

    [Given(@"connecting to the cache triggers an exception")]
    public void GivenConnectingToTheCacheTriggersAnException()
    {
      _cacheRepositoryMock.Setup(x => x.GetData(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
    }

    [Given(@"the database is available")]
    public void GivenTheDatabaseIsAvailable()
    {
      _dbRepositoryMock.Setup(x => x.GetData(_baseCurrency, _targetCurrency)).Returns(Task.FromResult<ExchangeRate>(_rate));
    }

    // Act
    [When(@"the Controller is called")]
    public async void WhenTheControllerIsCalled()
    {
      _result = await _appController.GetExchangeRate(_baseCurrency, _targetCurrency);
    }

    // Assert
    [Then(@"the cache is called")]
    public void ThenTheCacheIsCalled()
    {
      _cacheRepositoryMock.Verify(x => x.GetData(_baseCurrency, _targetCurrency));
    }

    [Then(@"the returned value is correct")]
    public void ThenTheReturnedValueIsCorrect()
    {
      Assert.IsTrue(_result is OkNegotiatedContentResult<ExchangeRateViewModel>);
      var result = (OkNegotiatedContentResult<ExchangeRateViewModel>)_result;
      result.Content.Should().BeEquivalentTo(_rate.ToViewModel());
    }
    
    [Then(@"the API is called")]
    public void ThenTheAPIIsCalled()
    {
      _apiClientMock.Verify(x => x.GetAllData());
    }

    [Then(@"the values are saved in the database")]
    public void ThenTheValuesAreSavedInTheDatabase()
    {
      _dbRepositoryMock.Setup(x => x.GetAllData()).Returns(Task.FromResult(_rates));
      _dbRepositoryMock.Verify(x => x.InsertData(_rates));
    }

    [Then(@"the values are saved in the cache")]
    public void ThenTheValuesAreSavedInTheCache()
    {
      _cacheRepositoryMock.Setup(x => x.GetAllData()).Returns(Task.FromResult(_rates));
      _cacheRepositoryMock.Verify(x => x.InsertData(_rates));
    }

    [Then(@"the values are taken from the database")]
    public void ThenTheValuesAreTakenFromTheDatabase()
    {
      _dbRepositoryMock.Verify(x => x.GetData(_baseCurrency, _targetCurrency));
    }
  }
}
