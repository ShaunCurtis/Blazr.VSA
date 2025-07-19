# Diode

**Diode** is a low ambition *No Frills* library for building *CQS* based data pipelines.

It's:
 - A set of interfaces, definitions and base implementations,
 - A *Mediator Pattern* dispatcher.
 - Implements a functional paradigm Result monad.

It consists of three channels or paths:

1. List Queries returning a collection of `TRcords`.
2. Record Queries returning a single `TRecord`.
3. Commands issuing *Create/Update/Delete* instructions.

## `Result<T>` and `Result`

Result is one of the foundations of *Diode*.  The data pipelines return a *Result*.

There's a separate article on *Result*.


