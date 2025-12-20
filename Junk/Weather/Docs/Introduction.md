# Introduction

*Blazr.Weather* is a simple demonstration solution for my Blazor Application Framework.  It uses concepts and patterns from various sources, but is principally a hybrid of Vertical Slice Architecture and Clean Design.  The goal is to create a Blazor application that is easy to maintain, test, and extend.

## Libraries and Applications

While you can keep everything in one project, I like to separate out the application code into one or more application libraries, and the deployment code into one or more delpoyment projects.  This is important if you wish to deploy the application with multiple front-ends.

In the Weather application we have three libraries:

1. *Blazr.App.Weather* contains all the main application code.

2. *Blazr.App.Weather.EntityFramework* contains the server side code to access the database via Entity Framework.

3. *Blazr.App.Weather.Api* contains the client side code to access the data via an API.
