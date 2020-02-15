# derin programming language
Interpreter for DERIN programming language


## Documentation

Every line begins with a line number.
Example:

10 PRINT "Hi"



Declaring a variable:

20 a = 2



Joining strings:

30 PRINT "Hello ";"World"



Strings should be in quotes.



> < and = works as expected including ==, <= and >=




Math: + - * / %



AND and OR 



10 a = True
20 b = False
30 c = a AND b
40 d = a OR b




COMMANDS:


PRINT         prints out to the console

PRINT:LN   prints and adds a \n

ABS              returns the absolute value of the number

ASC              returns ASCII value of the first char

ATN              returns Atangent

CHR$            returns the char value of the ASCII value

COS              Cosine

IF <> THEN <>     does the thing if the first thing is true

GOTO            goes to the line number specified

CRSR            sends the cursor to the specified coordinates

CLS               clears screenBGCOLOR    sets the background color

FGCOLOR    sets the text color

GOSUB         goes to the line number and comes back when "RETURN" is seen

RETURN        ^^

END               ends the program

CSSIZE          sets the console size

INPUT             gets input from user
