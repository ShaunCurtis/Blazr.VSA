# DTO's

A Data Transfer Object (DTO) is a design pattern used to transfer data between systems and layers within an application.

The key characteristic of DTOs is **Simplicity**:  DTOs are simple objects that contain no business logic. They typically consist of fields and properties to hold data.

In modern C# a DTO should be implemented as a  `record`  type to take advantage of immutability and value-based equality.  There's no valid reason to use a class for a DTO in C# anymore.

