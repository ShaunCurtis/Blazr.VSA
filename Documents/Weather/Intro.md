# Introduction

> **This set of documentation is in-the-making.  Some are rough-and-ready complete, others are still being written when I get time.**

Blazr.Weather is a demonstration project that showcases how to build a data centric application.

It applies and uses various technologies and concepts such as:

1. Clean Design - The code is divided into proejcts responsible for each of the Clean Design domains.  The projects apply the dependency relationships.
1. CQS - Command/Query separation is applied to the data pipeline.
1. Mediator - The Mediator pattern is used to decouple CQS pipeline from the Core and UI.
1. Readonly - The data pipeline is readonly.  All data objects are records and immutable.
1. Simple Message Bus - A simple message bus implementation [Blazr.Gallium] provides event notification.
1. Immutability - Everything is immutable by default.
