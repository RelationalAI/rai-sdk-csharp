# Changelog

## v0.5.0-alpha
* In this update we ensure the SDK follows the standard async/await approach for the methods. The following methods have been renamed to include the "Async" postfix, made async and now return a `Task<T>` to the user:
  - `CreateDatabase`, `GetDatabase`, `ListDatabases`, `DeleteDatabase`, `CloneDatabase`.
  - `CreateEngine`, `CreateEngineWait`, `GetEngine`, `ListEngines`, `DeleteEngine`.
  - `CreateOAuthClient`, `FindOAuthClient`, `GetOAuthClient`, `ListOAuthClients`, `DeleteOAuthClient`.
  - `CreateUser`, `UpdateUser`, `FindUser`, `GetUser`, `ListUsers`, `DeleteUser`, `DisableUser`, `EnableUser`.
  - `GetTransactions`, `GetTransaction`, `GetTransactionResults`, `GetTransactionMetadata`, `GetTransactionProblems`, `CancelTransaction`, `DeleteTransaction`.
  - `ListEdbs`, `LoadModel`, `LoadModels`, `ListModels`, `ListModelNames`, `GetModel`, `DeleteModel`.
  - `LoadJson`, `LoadCsv`.
  - `ExecuteV1`.
  - `Execute` renamed to `ExecuteWaitAsync` to match the naming of the sync operations.

## v0.4.0-alpha
* Renamed:
  - `Execute` to `ExecuteV1`.
  - `ExecuteAsyncWait` to `Execute`.

## v0.3.0-alpha
* Added `CancelTransaction` feature.

## v0.2.2-alpha
* Added `FinishedAt` field to `TransactionAsyncResponse`.

## v0.2.1-alpha
* Fixed `executeAsync` inputs issue.

## v0.2.0-alpha
* Added v2 predefined results formats:

    - `GetTransactions` returns `TransactionsAsyncMultipleResponses`.
    - `GetTransaction` returns `TransactionAsyncSingleResponse`.
    - `GetTransactionResults` returns `List<ArrowRelation>`.
    - `GetTransactionMetadata` returns `List<TransactionAsyncMetadataResponse>`.
    - `GetTransactionProblems` return `List<ClientProblem|IntegrityConstraintViolation>`.
    - `ExecuteAsync` returns `TransactionAsyncResult`.

## v0.1.0-alpha
* Added support to the asynchronous protocol including:
    - `executeAsync`: runs an asynchronous request.
    - `executeAsyncWait`: runs an asynchronous request and wait of its completion.
    - `getTransaction`: gets information about transaction.
    - `getTransactions`: gets the list of transactions.
    - `getTransactionResults`: gets transaction execution results.
    - `getTransactionMetadata`: gets transaction metadata.
    - `getTransactionProblems`: gets transaction execution problems.
* Added more api functionalities:
    - `ListEdbs`: lists the current edbs.
    - `LoadModel`: loads a model.
    - `LoadModels`: loads a dictionary of models.
    - `ListModels`: lists the current models.
    - `ListModelNames`: lists the current model names.
    - `GetModel`: gets a specific model.
    - `DeleteModel`: deletes a specific model.
    - `LoadJson`: loads json data.
    - `LoadCsv`: loads csv data.
    - `CloneDatabase`: clone a database.
