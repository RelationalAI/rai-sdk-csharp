## main
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