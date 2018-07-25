Feature: FixerClientAdapterTest
  We test the different scenarios when using the fixer.io adapter

Scenario: An error when connecting to the API raises an Exception
	Given there is an error when connecting to the API
	Then there is an error when running the adapter

Scenario: The adapter parses the data correctly
	Given The data given is correct
	When the API is called
	Then the values returned by the adapter are correct