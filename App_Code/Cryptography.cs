using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Techwork.App_Code
{
    public class Cryptography
    {
        public string EncryptMyData(string PlainText)
        {
               string EncryptedText = "";
               int ASCIIValue, NewASCIIValue;
               char ch;
               for(int x=0;x<PlainText.Length;x++)
              { 
                 ch=PlainText[x];
                 ASCIIValue=ch;
                if(ASCIIValue >=65 && ASCIIValue<=90)
                {
                    //UPPERCASE
                    NewASCIIValue=122 -ASCIIValue+65;
                }else if(ASCIIValue>=97 && ASCIIValue<=122)
                {
                    //LOWERCASE
                    NewASCIIValue =  122-ASCIIValue+97;

                }
                else if(ASCIIValue>=48 && ASCIIValue<=57)
                {   //DIGITS
                    NewASCIIValue = 57 - ASCIIValue + 48;

                }
                else
                {//Special Symbol
                    NewASCIIValue =ASCIIValue;
                }
                ch = (char)NewASCIIValue;
                EncryptedText=EncryptedText + ch;
            } 
            return EncryptedText;
        }
    }
}
/*
 What are ww doing in upper class to encrypt the user password to make it more save
A to Z =>65 to90
here we are converting
A=>z                  
B=>y
C=>x
Z=>a
Y=>b
this can be solved by
NewASCIIvalue=122-OldAsciivalue+65
========================================
here we are converting 
a=>z
b=>y
c=>x
z=>a
y=>b
this can be solved by
NewASCIIValue =  122-ASCIIValue+97;
======================================
here we are converting
0=>9
1=>8
2=>7
9=>1
NewASCIIValue = 57 - ASCIIValue + 48;
and not converting to special characters
 */