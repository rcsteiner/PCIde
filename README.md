# PCIde
This is a full IDE for writing and running the pseudo code defined in **"Programming Logic and Design"** book by **Tony Gaddis**. I wrote the IDE and compiler for my students so they can try all the examples in the book and do the homework without having to setup complicated IDE environments.

## Features
*  Full syntax of book
*  Compiler
*  Editor            (uses Avalon Edit)
*  Multiple windows  (uses Avalon Dock)
*  Run and debug with breakpoints.
*  Adds file headers (template based)

## Screenshots


![](Screenshots/clock.png)

## Comments about implementation
I included a copy of the *Avalon* edit and dock repositories here to simplify my development and debugging. The workplace must be created first, see **How To ...**  documents in the *Document* folder.

## Compiling on Visual Studio 2022  (2019)

There should not be anything special to download.  Just Compile as normal (Avalon Editor/Dock is in the project). 
Previous image was missing the Lift library that simulates an elevator that can be controlled by simple code.

Note: the *Ide* project is the actual project to run.  (*pc* project is a library/command line version of the compiler).
