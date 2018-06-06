using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MetaphoneBr {
    public class MetaphoneBr : IPhonetic {
        public string Translate (string STRING) {
            int LENGTH = 50;
            /*
             *    inicializa a chave metafônica
             */
            var META_KEY = "";

            /*
             *    configura o tamanho máximo da chave metafônica
             */
            var KEY_LENGTH = (int) LENGTH;

            /*
             *    coloca a posição no começo
             */
            var CURRENT_POS = (int) 0;

            /*
             *    recupera o tamanho máximo da string
             */
            var STRING_LENGTH = (int) STRING.Length;

            /*
             *    configura o final da string
             */
            var END_OF_STRING_POS = STRING_LENGTH - 1;
            var ORIGINAL_STRING = STRING + "    ";

            /*
             *    vamos repor alguns caracteres portugueses facilmente
             *     confundidos, substituíndo os números não confundir com
             *    os encontros consonantais (RR), dígrafos (LH, NH) e o
             *    C-cedilha:
             *
             *    'LH' to '1'
             *    'RR' to '2'
             *    'NH' to '3'
             *    'Ç'  to 'SS'
             *    'CH' to 'X'
             */
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "[1|2|3|4|5|6|7|8|9|0]", " ");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "[ã|á|â]", "A");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "[é|ê]", "E");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "[í|y]", "I");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "[õ|ó|ô]", "O");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "[ú|ü]", "U");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "ç", "SS");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "Ç", "SS");
            /*
             *    Converte a string para caixa alta
             */
            ORIGINAL_STRING = ORIGINAL_STRING.ToUpper ();

            /*
             *    faz substituições
             *    -> "olho", "ninho", "carro", "exceção", "cabaça"
             */
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "LH", "1");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "NH", "3");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "RR", "2");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "XC", "SS");
            //Console.WriteLine (ORIGINAL_STRING);
            /*
             *    a correção do SCH e do TH por conta dos nomes próprios:
             *    -> "schiffer", "theodora", "ophelia", etc..
             *
            ORIGINAL_STRING = Regex.Replace('SCH','X',ORIGINAL_STRING);*/
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "TH", "T");
            ORIGINAL_STRING = Regex.Replace (ORIGINAL_STRING, "PH", "F");

            /*
             *    remove espaços extras
             */
            ORIGINAL_STRING = ORIGINAL_STRING.Trim ();

            string CURRENT_CHAR;

            /*
             *    loop principal
             */
            while (META_KEY.Length < KEY_LENGTH) {
                /*
                 *    sai do loop se maior que o tamanho da string
                 */
                if (CURRENT_POS >= STRING_LENGTH) {
                    break;
                }

                /*
                 *    pega um caracter da string
                 */
                CURRENT_CHAR = Substr (ORIGINAL_STRING, CURRENT_POS, 1);

                /*
                 *    se é uma vogal e faz parte do começo da string,
                 *    coloque-a como parte da metachave
                 */
                if ((IsVowel (ORIGINAL_STRING, CURRENT_POS)) &&
                    ((CURRENT_POS == 0) ||
                        (StringAt (ORIGINAL_STRING, CURRENT_POS - 1, 1, " "))
                    )
                ) {
                    META_KEY += CURRENT_CHAR;
                    CURRENT_POS += 1;
                }
                /*
                 *    procurar por consoantes que tem um único som, ou que
                 *    que já foram substituídas ou soam parecido, como
                 *     'Ç' para 'SS' e 'NH' para '1'
                 */
                else if (StringAt (ORIGINAL_STRING, CURRENT_POS, 1,
                        "1", "2", "3", "B", "D", "F", "J", "K", "L", "M", "P", "T", "V")) {
                    META_KEY += CURRENT_CHAR;

                    /*
                     *    incrementar por 2 se uma letra repetida for encontrada
                     */
                    if (Substr (ORIGINAL_STRING, CURRENT_POS + 1, 1) == CURRENT_CHAR) {
                        CURRENT_POS += 2;
                    }

                    /*
                     *    senão incrementa em 1
                     */
                    CURRENT_POS += 1;
                } else {
                    /*
                     *    checar consoantes com som confuso e similar
                     */
                    switch (CURRENT_CHAR) {

                        case "G":
                            switch (Substr (ORIGINAL_STRING, (CURRENT_POS + 1), 1)) {
                                case "E":
                                case "I":
                                    META_KEY += "J";
                                    CURRENT_POS += 2;
                                    break;

                                case "U":
                                    META_KEY += "G";
                                    CURRENT_POS += 2;

                                    break;

                                case "R":
                                    META_KEY += "GR";
                                    CURRENT_POS += 2;
                                    break;

                                default:
                                    META_KEY += "G";
                                    CURRENT_POS += 2;
                                    break;
                            }
                            break;

                        case "U":
                            if (IsVowel (ORIGINAL_STRING, CURRENT_POS - 1)) {
                                CURRENT_POS += 1;
                                META_KEY += "L";
                                break;
                            }
                            /*
                             *    senão...
                             */
                            CURRENT_POS += 1;
                            break;

                        case "R":
                            if ((CURRENT_POS == 0) || (Substr (ORIGINAL_STRING, (CURRENT_POS - 1), 1) == " ")) {
                                CURRENT_POS += 1;
                                META_KEY += "2";
                                break;
                            } else if ((CURRENT_POS == END_OF_STRING_POS) || (Substr (ORIGINAL_STRING, (CURRENT_POS + 1), 1) == " ")) {
                                CURRENT_POS += 1;
                                META_KEY += "2";
                                break;
                            } else if (IsVowel (ORIGINAL_STRING, CURRENT_POS - 1) && IsVowel (ORIGINAL_STRING, CURRENT_POS + 1)) {
                                CURRENT_POS += 1;
                                META_KEY += "R";
                                break;
                            }
                            /*
                             *    senão...
                             */
                            CURRENT_POS += 1;
                            META_KEY += "R";
                            break;

                        case "Z":
                            if (CURRENT_POS >= (ORIGINAL_STRING.Length - 1)) {
                                CURRENT_POS += 1;
                                META_KEY += "S";
                                break;
                            } else if (Substr (ORIGINAL_STRING, (CURRENT_POS + 1), 1) == "Z") {
                                META_KEY += "Z";
                                CURRENT_POS += 2;
                                break;
                            }
                            /*
                             *    senão...
                             */
                            CURRENT_POS += 1;
                            META_KEY += "Z";
                            break;

                        case "N":
                            if ((CURRENT_POS >= (ORIGINAL_STRING.Length - 1))) {
                                META_KEY += "M";
                                CURRENT_POS += 1;
                                break;
                            } else if (Substr (ORIGINAL_STRING, (CURRENT_POS + 1), 1) == "N") {
                                META_KEY += "N";
                                CURRENT_POS += 2;
                                break;
                            }
                            /*
                             *    senão...
                             */
                            META_KEY += "N";
                            CURRENT_POS += 1;
                            break;

                        case "S":
                            /*
                             *    caso especial 'assado', 'posse', 'sapato', 'sorteio'
                             */
                            if ((Substr (ORIGINAL_STRING, (CURRENT_POS + 1), 1) == "S") ||
                                (CURRENT_POS == END_OF_STRING_POS) ||
                                (Substr (ORIGINAL_STRING, (CURRENT_POS + 1), 1) == " ")
                            ) {
                                META_KEY += "S";
                                CURRENT_POS += 2;
                            } else if ((CURRENT_POS == 0) || (Substr (ORIGINAL_STRING, (CURRENT_POS - 1), 1) == " ")) {
                                META_KEY += "S";
                                CURRENT_POS += 1;
                            } else if ((IsVowel (ORIGINAL_STRING, CURRENT_POS - 1)) &&
                                (IsVowel (ORIGINAL_STRING, CURRENT_POS + 1))) {
                                META_KEY += "Z";
                                CURRENT_POS += 1;
                            }
                            /*
                             *  Ex.: Ascender, Lascivia
                             */
                            else if (
                                (Substr (ORIGINAL_STRING, (CURRENT_POS + 1), 1) == "C") &&
                                (
                                    (Substr (ORIGINAL_STRING, (CURRENT_POS + 2), 1) == "E") ||
                                    (Substr (ORIGINAL_STRING, (CURRENT_POS + 2), 1) == "I")
                                )
                            )

                            {
                                META_KEY += "S";
                                CURRENT_POS += 3;
                            }
                            /*
                             * Ex.: Asco, Auscutar, Mascavo
                             */
                            else if (
                                (Substr (ORIGINAL_STRING, (CURRENT_POS + 1), 1) == "C") &&
                                (
                                    (Substr (ORIGINAL_STRING, (CURRENT_POS + 2), 1) == "A") ||
                                    (Substr (ORIGINAL_STRING, (CURRENT_POS + 2), 1) == "O") ||
                                    (Substr (ORIGINAL_STRING, (CURRENT_POS + 2), 1) == "U")
                                )
                            )

                            {
                                META_KEY += "SC";
                                CURRENT_POS += 3;
                            } else {
                                META_KEY += "S";
                                CURRENT_POS += 1;
                            }
                            break;

                        case "X":
                            /*
                             *    caso especial 'táxi', 'axioma', 'axila', 'tóxico'
                             */
                            if ((Substr (ORIGINAL_STRING, (CURRENT_POS - 1), 1) == "E") && (CURRENT_POS == 1)) {
                                META_KEY += "Z";
                                CURRENT_POS += 1;
                            } else if ((Substr (ORIGINAL_STRING, (CURRENT_POS - 1), 1) == "I") && (CURRENT_POS == 1)) {
                                META_KEY += "X";
                                CURRENT_POS += 1;
                            } else if ((IsVowel (ORIGINAL_STRING, CURRENT_POS - 1)) && (CURRENT_POS == 1)) {
                                META_KEY += "KS";
                                CURRENT_POS += 1;
                            } else {
                                META_KEY += "X";
                                CURRENT_POS += 1;
                            }
                            break;

                        case "C":
                            /*
                             *    caso especial 'cinema', 'cereja'
                             */
                            if (StringAt (ORIGINAL_STRING, CURRENT_POS, 2, "CE", "CI")) {
                                META_KEY += "S";
                                CURRENT_POS += 2;
                            } else if ((Substr (ORIGINAL_STRING, (CURRENT_POS + 1), 1) == "H")) {
                                META_KEY += "X";
                                CURRENT_POS += 2;
                            } else {
                                META_KEY += "K";
                                CURRENT_POS += 1;
                            }
                            break;

                            /*
                             *    como a letra 'h' é silenciosa no português, vamos colocar
                             *    a chave meta como a vogal logo após a letra 'h'
                             */
                        case "H":
                            if (IsVowel (ORIGINAL_STRING, CURRENT_POS + 1)) {
                                META_KEY += ORIGINAL_STRING[CURRENT_POS + 1];
                                CURRENT_POS += 2;
                            } else {
                                CURRENT_POS += 1;
                            }
                            break;

                        case "Q":
                            if (Substr (ORIGINAL_STRING, CURRENT_POS + 1, 1) == "U") {
                                CURRENT_POS += 2;
                            } else {
                                CURRENT_POS += 1;
                            }

                            META_KEY += "K";
                            break;

                        case "W":
                            if (IsVowel (ORIGINAL_STRING, CURRENT_POS + 1)) {
                                META_KEY += "V";
                                CURRENT_POS += 2;
                            } else {
                                META_KEY += "U";
                                CURRENT_POS += 2;
                            }
                            break;

                        default:
                            CURRENT_POS += 1;
                            break;
                    }
                }
            }

            /*
             *    corta os caracteres em branco
             */
            META_KEY = META_KEY.Trim ();

            /*
             *    retorna a chave matafônica
             */
            return META_KEY;
        }

        private bool StringAt (string STRING, int START, int STRING_LENGTH, params string[] LIST) {
            if ((START < 0) || (START >= STRING.Length)) {
                return false;
            }
            for (var I = 0; I < LIST.Length; I++) {
                if (LIST[I] == Substr (STRING, START, STRING_LENGTH)) {
                    return true;
                }
            }
            return false;
        }

        private bool IsVowel (string s, int pos) {
            if (pos >= s.Length) return false;

            return Regex.IsMatch ("[AEIOU]", Substr (s, pos, 1));
        }

        public string Substr (string str, int start, int length) {
            if (start >= str.Length) return "";

            return str.Substring (start, length);
        }
    }
}