# Wallet API

## Project Structure

```
Wallet.Api --> Main Controller With Endpoints
|
|
Wallet.Service --> Main Service classes containing business logic and Caching services
|
|
Wallet.Database --> Contains Database Entities and DB Context
|
|
Wallet.Models --> Model classes for controller and config
|
|
Wallet.Scheduler --> Quartz Job and Configuration classes 
|
|
Wallet.Client --> Client Class integrating the ECB Api Endpoint
```


## How To Run

1. Open project in Rider or Visual Studio
2. Under the Wallets.Api Project, there is a file called ` docker-compose.yaml `. Run this docker file so that the MSSql database container is instantiated.
3. Once the docker container is up, the application can be started. This will create the database and tables with relationships to be used within the app.
4. You can then go on Swagger UI on this url ` http://localhost:5108/index.html ` to view the Wallet Api Endpoints.


## Wallet Endpoints

There are 3 Wallet Endpoints:
Create Wallet:
` POST /api/Wallets `
Parameters: 
 - Currency {string} (Required)

Get Wallet Balance:
` GET /api/Wallets/{id} `
Parameters: 
 - Wallet Id {GUID} (Required)
 - Wallet Currency {string} (Optional: Will default the wallet current currency)

Adjust Wallet Balance:
` POST /api/Wallets/{id}/adjustBalance `
Parameters:
 - Wallet Id {GUID} (Required)
 - Amount {decimal} (Required)
 - Currency {string} (Required: Must be a currency present in the currency table)
 - Strategy {string} (Requred: Muust be one of these options: ` ForceSubtractFundsStrategy || SubtractFundsStrategy || AddFundsStrategy `


## Database Structure

The database consists of three main tables:

| Table           | Description                                                             |
| --------------- | ----------------------------------------------------------------------- |
| `Wallets`       | Represents user wallets containing balances in specific currencies.     |
| `CurrencyRates` | Stores daily exchange rates for each supported currency.                |
| `CurrencyCodes` | Contains master data for all supported currency codes (e.g., USD, EUR). |


#### CurrencyCodes Table

| Column         | Type               | Constraints | Description                                        |
| -------------- | ------------------ | ----------- | -------------------------------------------------- |
| `Id`           | `uniqueidentifier` | Primary Key | Unique identifier for the currency.                |
| `CurrencyCode` | `nvarchar(450)`    | Unique      | The ISO code of the currency (e.g., “USD”, “EUR”). |

Relationships:
1 → Many with WalletEntity
1 → Many with CurrencyRatesEntity


#### Wallets Table

| Column         | Type               | Constraints | Description                              |
| -------------- | ------------------ | ----------- | ---------------------------------------- |
| `Id`           | `uniqueidentifier` | Primary Key | Unique identifier for the wallet.        |
| `Balance`      | `decimal(18,2)`    | Required    | Represents the wallet’s balance.         |
| `CurrencyCode` | `nvarchar(450)`    | Foreign Key | References `CurrencyCodes.CurrencyCode`. |

Relationships:
Many → 1 with CurrencyEntity via CurrencyCode
On delete → Restrict (cannot delete a currency that is referenced by a wallet)


#### CurrencyRates Table

| Column           | Type               | Constraints | Description                                       |
| ---------------- | ------------------ | ----------- | ------------------------------------------------- |
| `Id`             | `uniqueidentifier` | Primary Key | Unique identifier for the rate record.            |
| `CurrencyCode`   | `nvarchar(450)`    | Foreign Key | References `CurrencyCodes.CurrencyCode`.          |
| `ConversionDate` | `datetime2`        | Required    | The date the rate applies to.                     |
| `Rate`           | `decimal(18,4)`    | Required    | Exchange rate for the currency on the given date. |

Relationships:
Many → 1 with CurrencyEntity via CurrencyCode

