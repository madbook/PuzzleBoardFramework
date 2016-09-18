# PuzzleBoard Framework

A collection of C# scripts to build 2D grid-based puzzle games with. As
someone new to both Unity _and_ `C#`, this is primarily a learning
project for me.  Feedback is welcome.

## Getting started

Although there are several classes available that provide more granular
control, the easiest way to get going is to extend the `BoardController<T>`
class, where `T` is the data type that makes up the state of your board.
`BoardController` provides a quick way to set up a game.  There are [a few
examples available](Examples) to give you an idea of how it
works.
