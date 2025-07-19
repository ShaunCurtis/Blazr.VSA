# Vertical Slice Architecture

The solution is structured around the *Vertical Slice Architecture*.

There are two application projects:

1. Blazr.App - Library containing all of the main code.
1. Blazr.App.Server - Blazor Server deployment project.

The App project is split into two feature slices, *Customers* and *Invoices* and a *Common* slice.

*Common* contains all the shared code between the two festure slices.

Each **Feature Slice** is split into a set of functional folders:

1. *Entity Objects* - the domain entity and value objects, along with the infrastructure data store mapped and data transfer objects.

1. *Requests* - the Mediator CQS request objects.

1. *Handlers* - the Mediator CQS request handlers.

1. *Mutators* - the edit objects and *FluentValidation* validators.

1. *Providers* - objects that provide various services and information about the slice required for the generic presenters and UI components.

1. *Presenters* - objects that provide data to the UI components.

1. *Services* - definitions of services required by the feature slice.
 
1. *UI* - the Blazor UI components and routes.

