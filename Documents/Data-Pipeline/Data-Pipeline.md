#  Data Pipeline

The application data pipeline is a composite design incorporating patterns from several frameworks.

- Entity Framework is used as the **Object Request Mapper**, implementing the basic repository and unit of work patterns.  

- A **Command/Query Separation** implementation provides a thin layer over EF to separate commands from queries.  

- Mediatr is used to implement the CQS pattern and decouple the front and back end.


All data within the data pipeline is *READONLY*: declared as either `record` or `readonly struct`.  Data retrieved from a data source is a **copy** of the data within the data source.  You don't mutate the source data by changing the copy: you pass a mutated copy to the data store to replace the existing data.

The data pipeline performs two basic activities:

1. Querying for single items or lists of items - retrieve a *record* or a *list*.
2. Submitting conmands to change data. - *add*, *update* or *delete* a record.
