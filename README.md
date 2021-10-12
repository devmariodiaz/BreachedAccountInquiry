# A Simple project with Azure Functions

**In this sample we create a set of Azure Functions that receive HTTP requests from a client, interact with a third party REST API and communicate through Queues and write to Database.**

### Description:
We create Azure Functions to verify if user account has been involved in data breaches, according to the following requirements:

1. Azure Function 1: Receives a HTTP POST request containing a JSON payload as below:
```
{ account: "<ANY_ACCOUNT_OR_EMAIL_ADDRESS>" }
```
Then, the function add the received payload onto an Azure Queue.

2. Azure Function 2: Reads the item that Azure Function 1 put on the queue and verify the account against a Mock API which is built based on https://haveibeenpwned.com/API/v3. To learn about the signature of the Mock API endpoint see the section #3.
Once a response from the API has been received, the Function should store the result (API response) together with the account as a JSON object in a COSMOSDB instance (Ideally MongoDB).


3. Mock API endpoint: 
This enables us to get all breaches for an account which has a signature as below:  
```
GET /api/v3/breachedaccount/{account}   
hibp-api-key: [your key]
```
The API takes a single parameter which is the account to be searched for. The account is not case sensitive and will be trimmed of leading or trailing white spaces. The account should always be URL encoded. This is an authenticated API and an HIBP API key must be passed with the request.   
For more information see https://haveibeenpwned.com/API/v3#BreachesForAccount   
The API responds with a list of all breaches for the account:
```
[
 {
  "Name":"Adobe",
  "Title":"Adobe",
  "Domain":"adobe.com",
  "BreachDate":"2013-10-04",
  "AddedDate":"2013-12-04T00:00Z",
  "ModifiedDate":"2013-12-04T00:00Z"
 },
 ...
]
```

  
