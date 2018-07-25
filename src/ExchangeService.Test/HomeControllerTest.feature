Feature: HomeControllerTest
    We test that the database, cache and api repositories are called, depending on the situtation

Scenario: The cache exists so it is called
	Given there is a Home Controller
	And the request is for the currencies "USD" and "AUD"
	And the cache is not null or outdated
  When the Controller is called
	Then the cache is called
  Then the returned value is correct

Scenario: The cache is empty so we call the API and store the result in the cache and the database
	Given there is a Home Controller
	And the request is for the currencies "USD" and "AUD"
	And the cache is null or outdated
	And the API is available
	When the Controller is called
	Then the API is called
	Then the values are saved in the database
	Then the values are saved in the cache
  Then the returned value is correct

Scenario: Connecting to the cache triggers an exception, so we take the latest values from the database
	Given there is a Home Controller
	And the request is for the currencies "USD" and "AUD"
	And connecting to the cache triggers an exception
	And the database is available
	When the Controller is called
	Then the values are taken from the database
  Then the returned value is correct