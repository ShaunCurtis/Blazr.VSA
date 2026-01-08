# Introduction

> **This set of documentation is in-the-making.  Some are rough-and-ready complete, others are still being written when I get time.**

Blazr.Invoice is a demonstration project that showcases a set of patterns and methodologies you can use to build data centric applications.

1. *Functional Core/Imperative I/O*
1. *Clean Design* - The code is divided into proejcts responsible for each of the Clean Design domains.  The projects apply the dependency relationships.
1. *CQS* - Command/Query separation is applied to the data pipeline.
1. *Mediator* - The Mediator pattern is used to decouple CQS pipeline from the Core and UI.
1. *Simple Message Bus* - A simple message bus implementation [Blazr.Gallium] provides event notification.
1. *Immutability* - Everything is immutable by default.

It's built on two fundimental coding principles:

## Functional Core/Imperative I/O  

Clean Design boils down to two fundimental layers: 

1. The core domain containing the domain models and application logic, and
2. The I/O layer that deals with Databases, User interaction though a UI, API,  mailing services,...  

The core is built on the Functional Programming paradigm, te I/O on a mixture of Imperative and Functional programming.

Functional programming in C# comes down to two core principles:

1. Immutable data
2. Pure Functions

I'll explore this in the docmentation articles

