# GumYung (金庸) Rule Tokenizer

Definition of rules derived from the Gum Yung novels and the tokenizing / parsing of those rules

A project I did in 2016.

## Background

This is the by-product of my project of estimating the "power levels" of characters from the universe of 金庸 novels.

I have developed a way to determine power difference formulae from the texts (mostly training my own mind in dealing with the scenarios described), and these formulae are considered when I manually placed the characters in different levels.

Later on, I thought maybe I could parse the formulae and a programme could deduce the estimated power levels instead of me manually doing so. This is why I have this completed tokenizer/parser. But the AI part I gave up (not a data scientist)

## Sample Formulae

~~~
霍青桐(三分)~ 霍阿伊 ~ 閻世章+戴永明 2+-

駱冰<吳國棟+馮輝+蔣天壽+韓春霖2-3 ~ 3

吳國棟>馮輝 蔣天壽 韓春霖2+ --不高

孫老三0-1 ~ 1

文泰來>童兆和3++ --錢不高強

陸菲青(容讓)>霍青桐(三分)2？

霍青桐(三分)>李沅芷2+ --陸說

張召重>李沅芷~3

余魚同 > 吳國棟+蔣天壽+韓春霖0-2 ~ 1 再~四公差1+-~ 0 --（馮彈子up?）
~~~

## Grammar For the Formulae

~~~
File = Row {("\r\n" | "," | "，") [Row]};

Row = Party [Operator] RangeWithPrefer | Party Challenger {["再"] Challenger } ["?" | "？"];

Challenger = Operator Party [RangeWithPrefer];

Party = Fighter {"+" Fighter} | Fighter {Fighter};

RangeWithPrefer = ["~"] Range [["~"] Range];

Range = Number ["-" Number] | PositiveNumber ("++" | "+" | "-" | "+-");

Operator = ">" | "<" | "=" | "~";

Number = ["-"] PositiveNumber;

PositiveNumber = Digit{Digit};

Digit = "0" | "1" | "2" | "3" | "4" | "5" | "6" | "7" | "8" | "9" ;
~~~
