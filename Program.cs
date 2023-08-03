// See https://aka.ms/new-console-template for more information

// Main method
class Program{
    public static void Main(String[] args){
        // Read in puzzle as input
        Console.WriteLine("Enter the digits of the sodoku puzzle from left to right then up and down, replacing the empty squares with zeros:");
        String input = Console.ReadLine();
        if(input == null){
            return;
        }

        // Initializes sudoku puzzle solver
        SudokuBoard puzzle = new SudokuBoard();
        puzzle.inputString(input);
        puzzle.setValidValuesForEachSquare();

        // Fills out each square one by one using cross hatching  and naked pair method
        int filledSquares = puzzle.getSolvedSquares();
        while(puzzle.getSolvedSquares() < 81){
            puzzle.checkNakedPairs();
            puzzle.scanBoardForSingleValue();

            // If the sudoku puzzle can't be solved
            if(filledSquares == puzzle.getSolvedSquares()){
                Console.WriteLine();
                Console.WriteLine("This Sudoku Puzzle can't be solved using the cross hatching or naked pair method");
                Console.WriteLine();
                puzzle.printBoard();
                return;
            }

            filledSquares = puzzle.getSolvedSquares();
        }

        // Outputs board after done
        Console.WriteLine();
        Console.WriteLine("The sudoku puzzle has been solved");
        Console.WriteLine();
        puzzle.printBoard();
    }
}

// Sudoku Square (81 in a puzzle)
public class SudokuSquare{
    private int value;
    private List<int> validValues;

    // Initializes square
    public SudokuSquare(int value){
        this.value = value;

        validValues = new List<int>();
        if(value == 0){
            for(int i = 1; i < 10; i ++){
                validValues.Add(i);
            }
        }
    }

    public int getValue(){
        return value;
    }

    // Sets value of square (Succ: 1, Unsucc: 0)
    public int setValue(int newValue){
        // Unsuccessful if square already has value
        if(value != 0){
            return 0;
        }

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
        if(input.Length < 81){
            return 0;
        }

        for(int i = 0; i < 9; i ++){
            for(int j = 0; j < 9; j ++){
                board[i, j] = new SudokuSquare(Int32.Parse(input.Substring((9 * i) + j, 1)));
            }
        }

        return 1;
    }

    // Eliminates all impossible values for each empty square
    public void setValidValuesForEachSquare(){
        for(int i = 0; i < 9; i ++){
            for(int j = 0; j < 9; j ++){
                if(board[i,j].getValue() != 0){
                    solvedSquares ++;
                    elimRow(board[i,j].getValue(), i);
                    elimColumn(board[i,j].getValue(), j);
                    elimBox(board[i,j].getValue(), (i/3) * 3 + (j/3));
                }
            }       
        }
    }

    // Removes 1 specific valid value for a row
    public void elimRow(int invalid, int row){
        for(int i = 0; i < 9; i ++){
            board[row,i].removeValidValue(invalid);
        }
    }

    // Removes 1 specific valid value for a column
    public void elimColumn(int invalid, int col){
        for(int i = 0; i < 9; i ++){
            board[i, col].removeValidValue(invalid);
        }
    }

    // Removes 1 specific valid value for a box
    public void elimBox(int invalid, int box){
        for(int i = 0; i < 3; i ++){
            for(int j = 0; j < 3; j ++){
                board[(3*(box/3)) + i, (3*(box%3)) + j].removeValidValue(invalid);
            }
        }
    }

    // Searches the board for an empty square that can only have 1 possible value and fills it in (Cross hatching method)
    public void scanBoardForSingleValue(){
        for(int i = 0; i < 9; i ++){
            for(int j = 0; j < 9; j ++){
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
                if(board[i,j].getValue() != 0){
                    Console.WriteLine(board[i,j].getValue());
                }
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
        for(int i = 0; i < 3; i ++){
            for(int j = 0; j < 3; j ++){
                // If valid values list doesn't have exactly 2 values, skip
                if(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValuesCount() != 2){
                    continue;
                }
                
                for(int k = 0; k < 3 && k != i; k ++){
                    for(int l = 0; l < 3 && l != j; l ++){
                        // If 2nd valid values list doesn't have exactly 2 values, skip
                        if(board[(3*(box/3)) + k, (3*(box%3)) + l].getValidValuesCount() != 2){
                            continue;
                        }

                        // If the 2 valid values lists don't match, skip
                        if(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValues().Except(board[(3*(box/3)) + k, (3*(box%3)) + l].getValidValues()).ToList().Count != 0 || board[(3*(box/3)) + k, (3*(box%3)) + l].getValidValues().Except(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValues()).ToList().Count != 0){
                            continue;
                        }

                        // Removes naked pair from other squares in box
                        for(int m = 0; m < 3; m ++){
                            for(int n = 0; n < 3; n ++){
                                if((m == i && n == j) || (m == k && n == l))
                                    continue;

                                board[(3*(box/3)) + m, (3*(box%3)) + n].removeValidValue(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValues()[0]);
                                board[(3*(box/3)) + m, (3*(box%3)) + n].removeValidValue(board[(3*(box/3)) + i, (3*(box%3)) + j].getValidValues()[1]);
                            }
                        }
                    }
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
}