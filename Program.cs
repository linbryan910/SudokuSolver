﻿// See https://aka.ms/new-console-template for more information

// Main method
class Program{
    public static void Main(String[] args){
        if(args.Length == 0){
            Console.WriteLine("Please enter a command line argument for the puzzle input method (f for file input and c for console input)");
            return;
        }

        SudokuBoard puzzle = new SudokuBoard();

        // Reads in file name with puzzle
        if(args[0] == "f"){
            Console.WriteLine("Enter the file with the sudoku puzzle");
            String inputFile = Console.ReadLine();
            if(inputFile == null)
                return;

            puzzle.inputFile(inputFile);
        }
        // Read in puzzle as input
        else if(args[0] == "c"){
            Console.WriteLine("Enter the digits of the sodoku puzzle from left to right then up and down, replacing the empty squares with zeros:");
            String input = Console.ReadLine();
            if(input == null)
                return;

            puzzle.inputString(input);
        }
        else{
            Console.WriteLine("Please enter a valid command line argument for the puzzle input method (f for file input and c for console input)");
            return;
        }

        // Finished initialization of sudoku puzzlbe board
        puzzle.setValidValuesForEachSquare();

        // Fills out each square
        int filledSquares = puzzle.getSolvedSquares();
        while(puzzle.getSolvedSquares() < 81){
            puzzle.checkNakedPairs();
            puzzle.checkAssertions();
            puzzle.scanBoardForSingleValue();

            // If the sudoku puzzle can't be solved
            if(filledSquares == puzzle.getSolvedSquares()){
                Console.WriteLine("\nThis Sudoku Puzzle can't be solved\n");
                puzzle.printBoard();
                return;
            }

            filledSquares = puzzle.getSolvedSquares();
        }

        // Outputs board after done
        Console.WriteLine("\nThe sudoku puzzle has been solved\n");
        puzzle.printBoard();
    }
}

// Sudoku Square (81 in a puzzle)
public class SudokuSquare{
    private int value; // Value of this square
    private List<int> validValues; // List of possible values for this square

    // Initializes square
    public SudokuSquare(int value){
        this.value = value;

        validValues = new List<int>();
        if(value == 0)
            for(int i = 1; i < 10; i ++)
                validValues.Add(i);
    }

    public int getValue(){
        return value;
    }

    // Sets value of square (Succ: 1, Unsucc: 0)
    public int setValue(int newValue){
        // Unsuccessful if square already has value
        if(value != 0)
            return 0;

        value = newValue;
        validValues.Clear();
        return 1;
    }

    // Gets all valid values for this square
    public List<int> getValidValues(){
        return validValues;
    }

    // Crosses off a possible value for this square
    public void removeValidValue(int invalid){
        validValues.Remove(invalid);
    }

    public int getValidValuesCount(){
        return validValues.Count;
    }
}

// Sudoku Puzzle
public class SudokuBoard{
    private int solvedSquares;
    private SudokuSquare[,] board;

    public SudokuBoard(){
        solvedSquares = 0;
        board = new SudokuSquare[9,9];
    }

    // Reads in sudoku board and sets values (Succ: 1, Unsucc: 0)
    public int inputString(String input){
        if(input.Length < 81)
            return 0;

        for(int i = 0; i < 9; i ++)
            for(int j = 0; j < 9; j ++)
                board[i, j] = new SudokuSquare(Int32.Parse(input.Substring((9 * i) + j, 1)));

        return 1;
    }

    // Reads in sudoku board and sets values
    public void inputFile(String filename){
        String[] input = File.ReadAllLines(filename);
        
        for(int i = 0; i < 9; i ++){
            String currRow = input[i];
            for(int j = 0; j < 9; j ++)
                board[i, j] = new SudokuSquare(Int32.Parse(currRow.Substring(j, 1)));
        }
    }

    // Eliminates all impossible values for each empty square
    public void setValidValuesForEachSquare(){
        for(int i = 0; i < 9; i ++)
            for(int j = 0; j < 9; j ++)
                if(board[i,j].getValue() != 0){
                    solvedSquares ++;
                    elimRow(board[i,j].getValue(), i);
                    elimColumn(board[i,j].getValue(), j);
                    elimBox(board[i,j].getValue(), (i/3) * 3 + (j/3));
                }
    }

    // Removes 1 specific valid value for a row
    public void elimRow(int invalid, int row){
        for(int i = 0; i < 9; i ++)
            board[row,i].removeValidValue(invalid);
    }

    // Removes 1 specific valid value for a column
    public void elimColumn(int invalid, int col){
        for(int i = 0; i < 9; i ++)
            board[i, col].removeValidValue(invalid);
    }

    // Removes 1 specific valid value for a box
    public void elimBox(int invalid, int box){
        for(int i = 0; i < 3; i ++)
            for(int j = 0; j < 3; j ++)
                board[(3*(box/3)) + i, (3*(box%3)) + j].removeValidValue(invalid);
    }

    // Searches the board for an empty square that can only have 1 possible value and fills it in (Cross hatching method)
    public void scanBoardForSingleValue(){
        for(int i = 0; i < 9; i ++)
            for(int j = 0; j < 9; j ++)
                if(board[i,j].getValue() == 0 && board[i,j].getValidValuesCount() == 1){
                    int newValue = board[i,j].getValidValues()[0];
                    board[i,j].setValue(newValue);
                    solvedSquares ++;

                    int elimValue = newValue;

                    elimRow(elimValue, i);
                    elimColumn(elimValue, j);
                    elimBox(elimValue, (i/3) * 3 + (j/3));
                    return;
                }
    }

    // Gets the number of filled squares
    public int getSolvedSquares(){
        return solvedSquares;
    }

    // Outputs the board
    public void printBoard(){
        for(int i = 0; i < 9; i ++){
            for(int j = 0; j < 9; j ++){
                if(board[i,j].getValue() == 0)
                    Console.Write(". ");
                else
                    Console.Write(board[i,j].getValue() + " ");

                if(j == 2 || j == 5)
                    Console.Write("| ");
            }
            Console.WriteLine();

            if(i == 2 || i == 5)
                Console.WriteLine("----------------------");
        }
        Console.WriteLine();
    }

    // Outputs board and all possible values for each empty square
    // (Only for programming/debugging purposes)
    public void printFullBoard(){
        for(int i = 0; i < 9; i ++){
            for(int j = 0; j < 9; j ++){
                if(board[i,j].getValue() != 0)
                    Console.WriteLine(board[i,j].getValue());
                else{
                    Console.Write(board[i,j].getValue() + " - ");
                    Console.WriteLine(String.Join(" ", board[i,j].getValidValues()));
                }
            }
            Console.WriteLine();
        }
    }

    // Checks for a naked pair in a row
    public void checkNakedPair_Row(int row){
        // Goes through row
        for(int i = 0; i < 9; i ++){
            
            // If valid values list doesn't have exactly 2 values, skip
            if(board[row,i].getValidValuesCount() != 2)
                continue;

            for(int j = i + 1; j < 9; j ++){
                // If 2nd valid values list doesn't have exactly 2 values, skip
                if(board[row,j].getValidValuesCount() != 2)
                    continue;
                    
                // If the 2 valid values lists don't match, skip
                if(board[row,i].getValidValues().Except(board[row,j].getValidValues()).ToList().Count != 0 || board[row,j].getValidValues().Except(board[row,i].getValidValues()).ToList().Count != 0)
                    continue;
                
                // Removes naked pair from other squares in row
                for(int k = 0; k < 9; k ++){
                    if(k == i || k == j)
                        continue;

                    board[row,k].removeValidValue(board[row,i].getValidValues()[0]);
                    board[row,k].removeValidValue(board[row,i].getValidValues()[1]);
                }
            }
        }
    }

    // Checks for a naked pair in a column
    public void checkNakedPair_Column(int column){
        // Goes through column
        for(int i = 0; i < 9; i ++){
            
            // If valid values list doesn't have exactly 2 values, skip
            if(board[i,column].getValidValuesCount() != 2)
                continue;

            for(int j = i + 1; j < 9; j ++){
                // If 2nd valid values list doesn't have exactly 2 values, skip
                if(board[j,column].getValidValuesCount() != 2)
                    continue;
                    
                // If the 2 valid values lists don't match, skip
                if(board[i,column].getValidValues().Except(board[j,column].getValidValues()).ToList().Count != 0 || board[j,column].getValidValues().Except(board[i,column].getValidValues()).ToList().Count != 0)
                    continue;
                
                // Removes naked pair from other squares in column
                for(int k = 0; k < 9; k ++){
                    if(k == i || k == j)
                        continue;

                    board[k,column].removeValidValue(board[i,column].getValidValues()[0]);
                    board[k,column].removeValidValue(board[i,column].getValidValues()[1]);
                }
            }
        }
    }

    // Checks for a naked pair in a box
    public void checkNakedPair_Box(int box){
        for(int i = 0; i < 3; i ++)
            for(int j = 0; j < 3; j ++){
                // If valid values list doesn't have exactly 2 values, skip
                if(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValuesCount() != 2)
                    continue;
                
                for(int k = 0; k < 3 && k != i; k ++)
                    for(int l = 0; l < 3 && l != j; l ++){
                        // If 2nd valid values list doesn't have exactly 2 values, skip
                        if(board[(3*(box/3)) + k, (3*(box%3)) + l].getValidValuesCount() != 2)
                            continue;

                        // If the 2 valid values lists don't match, skip
                        if(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValues().Except(board[(3*(box/3)) + k, (3*(box%3)) + l].getValidValues()).ToList().Count != 0 || board[(3*(box/3)) + k, (3*(box%3)) + l].getValidValues().Except(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValues()).ToList().Count != 0)
                            continue;

                        // Removes naked pair from other squares in box
                        for(int m = 0; m < 3; m ++)
                            for(int n = 0; n < 3; n ++){
                                if((m == i && n == j) || (m == k && n == l))
                                    continue;

                                board[(3*(box/3)) + m, (3*(box%3)) + n].removeValidValue(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValues()[0]);
                                board[(3*(box/3)) + m, (3*(box%3)) + n].removeValidValue(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValues()[1]);
                            }
                    }
            }
    }

    // Checks for naked pairs in all rows, columns, and boxes
    public void checkNakedPairs(){
        for(int i = 0; i < 9; i ++){
            checkNakedPair_Row(i);
            checkNakedPair_Column(i);
            checkNakedPair_Box(i);
        }
    }

    // Checks assertion in a row in the sudoku puzzle
    public void checkAssertion_Row(int row){
        // Initializes the dictionary to hold valid values
        Dictionary<int, int> dict = new Dictionary<int, int>();
        for(int i = 1; i < 10; i ++)
            dict[i] = 0;

        // Adds all valid values in the row to the dictionary
        for(int i = 0; i < 9; i ++)
            foreach (int j in board[row,i].getValidValues())
                dict[j] = dict[j] + 1;

        // If there is only 1 copy of a valid value in the row, set that square to that value
        // Then removes that value from all valid value lists in its column and box
        foreach (int i in dict.Keys)
            if(dict[i] == 1)
                for(int j = 0; j < 9; j ++)
                    if(board[row,j].getValidValues().Contains(i)){
                        board[row,j].setValue(i);
                        solvedSquares ++;
                        elimColumn(i, j);
                        elimBox(i, (row/3) * 3 + (j/3));
                    }
    }

    // Checks assertion in a column in the sudoku puzzle
    public void checkAssertion_Column(int column){
        // Initializes the dictionary to hold valid values
        Dictionary<int, int> dict = new Dictionary<int, int>();
        for(int i = 1; i < 10; i ++)
            dict[i] = 0;

        // Adds all valid values in the column to the dictionary
        for(int i = 0; i < 9; i ++)
            foreach (int j in board[i,column].getValidValues())
                dict[j] = dict[j] + 1;

        // If there is only 1 copy of a valid value in the column, set that square to that value
        // Then removes that value from all valid value lists in its row and box
        foreach (int i in dict.Keys)
            if(dict[i] == 1)
                for(int j = 0; j < 9; j ++)
                    if(board[j,column].getValidValues().Contains(i)){
                        board[j,column].setValue(i);
                        solvedSquares ++;
                        elimRow(i, j);
                        elimBox(i, (j/3) * 3 + (column/3));
                    }
    }

    // Checks assertion in a box in the sudoku puzzle
    public void checkAssertion_Box(int box){
        // Initializes the dictionary to hold valid values
        Dictionary<int, int> dict = new Dictionary<int, int>();
        for(int i = 1; i < 10; i ++)
            dict[i] = 0;

        // Adds all valid values in the box to the dictionary
        for(int i = 0; i < 3; i ++)
            for(int j = 0; j < 3; j ++)
                foreach(int k in board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValues())
                    dict[k] = dict[k] + 1;

        // If there is only 1 copy of a valid value in the box, set that square to that value
        // Then removes that value from all valid value lists in its row and column
        foreach (int i in dict.Keys)
            if(dict[i] == 1)
                for(int j = 0; j < 3; j ++)
                    for(int k = 0; k < 3; k ++)
                        if(board[(3*(box/3)) + j, (3*(box%3)) + k].getValidValues().Contains(i)){
                            board[(3*(box/3)) + j, (3*(box%3)) + k].setValue(i);
                            solvedSquares ++;
                            elimRow(i, (3*(box/3)) + j);
                            elimColumn(i, (3*(box%3)) + k);
                        }
    }

    public void checkAssertions(){
        for(int i = 0; i < 9; i ++){
            checkAssertion_Row(i);
            checkAssertion_Column(i);
            checkAssertion_Box(i);
        }
    }
}