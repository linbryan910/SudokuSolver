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

        // Fills out each square one by one using cross hatching method
        int filledSquares = puzzle.getSolvedSquares();
        while(puzzle.getSolvedSquares() < 81){
            puzzle.scanBoardForSingleValue();

            if(filledSquares == puzzle.getSolvedSquares()){
                Console.WriteLine("This Sudoku Puzzle can't be solved using the cross hatching method");
                puzzle.printFullBoard();
                return;
            }
        }

        // Outputs board after done
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

    // Searches the board for an empty square that can only have 1 possible value and fills it in
    public void scanBoardForSingleValue(){
        for(int i = 0; i < 9; i ++){
            for(int j = 0; j < 9; j ++){
                if(board[i,j].getValue() == 0 && board[i,j].getValidValues().Count == 1){
                    board[i,j].setValue(board[i,j].getValidValues()[0]);
                    solvedSquares ++;

                    int elimValue = board[i,j].getValidValues()[0];

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
                Console.Write(board[i,j].getValue());
            }
            Console.WriteLine();
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
}