# Blazor Application Design

Nothing intrigues me more that application design.

As a retiree, I have the time to indulge that itch.  I maintain a couple of live applications and a bunch of demo projects, so have a rich source of code to experiment on.

I've come to appreciate the doctrines of *Clean Design*, but used it on most of my more modern demo projects.  However, when I came to rebuild one of my live [rather more complex] applications, I started to feel the constraints.

My *layers* are defined as projects to enforce dependancies, and split out non compatible [normally WASM] functionality.

Related entity code split across too many projects.  I was forever opening and closing projects, searching for stuff.  My structure screamed *architecture* not *features*.  The development environments don't help.  They provide no way to group together related code/directory structures from different projects - a *Feature View* window in the right hand pane.

## MVC, MVP, MVVM

The important thing to notice here is they all contain *MV*.  

The **Model** is the Domain/Business Logic layer of any *Clean Design* organised project.

The ***View* is the UI or Presentation layer of any *Clean Design* organised project.

It's only the glue - the **Controller*, *Presenter*, *View Model* - that's different.












