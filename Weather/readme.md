# Blazr.Weather

Blazr.Weather is a repository that demonstrates some key design principles and patterns that can be applied to Blazor applications.

The repository will always be a work in progress.  We never stop learning and acquiring knoweldge.

The principle technologies, concepts and patterns you will see applied here are:

1. Clean Design - Domains are represented as projects, which apply dependency relationships strictly.
1. CQS - Command/Query separation is applied to the data pipeline.
1. Mediator - The Mediator pattern is used to decouple the CQS pipeline from the Core and UI.
1. Message Bus - A simple message bus implementation [Blazr.Gallium] provides event notification.
1. Immutability - Everything is immutable by default.  Entities and data objects are normally records. Everything that can be is a `readonly record struct`.
1. A simple implementation of the `Flux` pattern [Blazr.FluxGate] is used to manage object mutation where appropriate.


The Server version works, but the  WebAssembly/API Version is **Under Construction**.
