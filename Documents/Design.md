# Blazor Application Design

Nothing intrigues me more that application design.

As a retiree, I have the time to indulge that itch.  I maintain a couple of live applications and a bunch of demo projects, so have a rich source of code to experiment on.

I've come to appreciate the doctrines of *Clean Design*, and have used it on most of my more modern demo projects.  However, when I came to rebuild one of my live [rather more complex] applications, I started to feel the constraints.

My *layers* are defined as projects to enforce dependancies.  With the need to split out non compatible [normally WASM] functionality I end up with a lot of projects.

Related entity code is split across too many projects.  I'm forever opening and closing projects, searching for stuff.  My structure screams *architecture* not *features*.  The development environments don't help.  They provide no way to group together related code/directory structures from different projects - a *Feature View* window in the right hand pane would be great.

So, what other options do I have?

## MVC, MVP, MVVM?

Notice they all contain *MV*.  

The **Model** is the Domain/Business Logic layer of any *Clean Design* organised project.

The **View** is the UI or Presentation layer of any *Clean Design* organised project.

It's only the glue - the **Controller**, **Presenter** or **View Model** - that's different.

An interesting take on the **MVx**, is that they are products of their time.  The functionality and role of the glue was dictated by the functionality of the UI structure.

The problems with trying to apply them to Blazor are:

1. The Blazor UI, components. have a very different design and functionality to what came before.
2. We now have built-in immutability.  We can build read-only data pipeines, and apply much stricter control over object mutation.

## Mediator and Flux

When I first met the Mediator pattern and Mediatr, I wasn't sure.  I was into building generic repositories, so didn't see it's application in the data pipeline.

Flux was a little different, I saw the advantages in controling mutation, but didn't really like the implementations, so wrote my own.

After I dumped repositories and moved to CQS, Mediator jumped out as the obvious pattern to couple with it.  And once yo have a compelling reason to use it, it starts to creep out into other areas of your code.

With Flux I moved in the opposite direction.  My main applications were in component state, specifically preserving grid state in views and in managing state in *Aggregate* entities.  I now have a state service for UI state, and build the Flux concept into my aggregate objects.

## MVB and Vertical Slice Architecture

**Model, View, Broker** is my name for the different glue. It doesn't matter what its called.  The **Broker** provides the Blazor centric connectivity between the application Core - the 
Domain and Business logic code - and the UI - components.

## Vertical Slice Architecture

I solved most of my project architecture problems by adopting the *Vertical Slice Architecture*.  It isn't a panacea.  It provides cohesion, but doesn't force decoupling.  


