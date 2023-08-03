# SudokuSolver

This program solves a sudoku puzzle using:
 - the cross hatching method
 - the naked pairs method

Cross hatching method: 
    When you find that a value fits in a square because it hasn't been used in the box and it is used in the other rows and columns that intersect with the box
Naked pairs method:
    When you find two squares in the same row, column, or box that have the same 2 possible values, so you can eliminate those values from the squares in the other rows, columns, or boxes respectively

With the cross hatching and naked pairs methods implemented, this sudoku solver is able to solve easy to intermediate puzzles. 

This program was written in C# using .NET. 

More features will be added to this program in the future (which will allow it to solve more difficult puzzles and make it more convenient/efficient).

To-Do: