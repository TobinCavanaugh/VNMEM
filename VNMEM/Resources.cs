namespace VNMEM;

public static class Resources
{
    public static string defaultProgram = @"//Our z is our output value
LOD #1
STO Z

//If our exponent is 0, the result is one
LOD X 
JMZ #25


//z *= y
LOD Y
MUL Z
STO Z
                                   
//X -= 1, if x == 0 -> jump, else -> loop
LOD X
SUB #1
STO X
JMZ #22
JMP #10
                                   
//Regular program exit
HLT //22
                                   
//Exponent of 0 exit
HLT";
}