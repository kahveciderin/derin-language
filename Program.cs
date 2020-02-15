using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CommodoreBASIC
{
    class Program
    {
        #region variables
        static string[] forarg = { "" };
        static int i;
        static int forargf = -1;
        static int l;
        static List<int> errors = new List<int>();
        static List<int> warnings = new List<int>();
        static int error = 0;
        static int warning = 0;
        static string output;
        static List<string> varnames = new List<string>();
        static List<string> vars = new List<string>();
        static int i_old = 0;
        static bool debug = false;
        static int returnline = 0;
        static List<string> funcnames = new List<string>();
        static List<int> funcs = new List<int>();
        static string x = "";
        static string[] words;
        static string[] matharg = { "" };
        static double math1 = 0;
        static double math2 = 0;
        static double mathequal = 0;
        static bool boolequal = false;
        static bool logic1 = false;
        static bool logic2 = false;
        static int nextline = 0;
        static int elseline = 0;
        static Random rand = new Random();
        static string currentPath;
        static string name;
        static bool OutShow = false;
        static int lineno;
        static int returnto;
        static bool isthisaline;








        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();
        #endregion
        [STAThread]
        static void Main(string[] args)
        {
            IntPtr handle = GetConsoleWindow();
            IntPtr sysMenu = GetSystemMenu(handle, false);
            
            DeleteMenu(sysMenu, SC_MINIMIZE, MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(sysMenu, SC_SIZE, MF_BYCOMMAND);
            #region open file
            currentPath = Environment.CurrentDirectory;
            if (!File.Exists("program.direc"))
            {

                OpenFileDialog file = new OpenFileDialog();
                file.Filter = "DERIN Files |*.derin";

                if (file.ShowDialog() == DialogResult.OK)
                {
                    x = file.FileName;
                    name = file.SafeFileName;
                }
            }
            else
            {
                x = currentPath + "\\" + File.ReadAllText(currentPath + "\\program.direc");
            }

            #endregion



            #region read file and print
            string code = File.ReadAllText(x);

            string[] words = code.Split('\n');
            l = words.Length;
            i = 0;

                Console.WriteLine("BASIC CODE:");
                Console.WriteLine(code);
                Console.WriteLine("PRESS ENTER TO COMPILE");
                Console.ReadLine();
                Console.WriteLine("LENGTH OF CODE: {0}", l);
                Console.WriteLine("START");
            
            #endregion
                Console.Clear();
            #region compile line by line
            while (i < l)
            {
                i++;
                compile(line(code, i));
            }
            #endregion

            Exit();
            

        }

        static void varop(string varname, string value)
        {
            string add = "";
            add = value;

            if (varnames.Contains(varname))
            {
                int varindex = varnames.IndexOf(varname);
                vars[varindex] = value;
                if (debug) Console.WriteLine("CHANGE VARIABLE {0} TO {1}.", varname, add);
            }
            else
            {
                varnames.Add(varname);
                vars.Add(add);
                if (add == string.Empty)
                {
                    if (debug) Console.WriteLine("SET NEW VARIABLE {0} TO NULL.", varname);
                }
                else
                {
                    if (debug) Console.WriteLine("SET NEW VARIABLE {0} TO {1}.", varname, add);
                }

            }

        }

        static string line(string code, int line)
        {

            words = code.Split('\n');
            string word = words[line - 1];
            string[] asd = word.Split(' ');
            var foo = new List<string>(asd);
            lineno = Int32.Parse(foo[0]);
            foo.RemoveAt(0);
            asd = foo.ToArray();
            return String.Join(" ", asd);
            
        }

        static string compile(string line,[CallerMemberName] string callerName = "")
        {
            if (callerName == "compile")
                isthisaline = false;
            else
                isthisaline = true;
            #region setup
            //get rid of undefined characters
            line = line.Replace("(", " ");
            line = line.Replace(";", " ; ");
            line = line.Replace(")", " ");
            line = line.Replace("\r", string.Empty);
            line = line.Replace("\n", string.Empty);


            string[] arg = line.Split(' ');
            var foo = new List<string>(arg);
            foo.Remove("");
            arg = foo.ToArray();
            string cmd = arg[0];
            string ret = "";

            //remove the command from arguments
            var foos = new List<string>(arg);
            foos.RemoveAt(0);
            foos.Remove("");
            arg = foos.ToArray();
            string a = string.Join(" ", arg);

            #endregion

            #region notacommand
            if (arg.Length > 0 && arg[0] == "=")
            {

                varop(cmd, compile(a.Replace("=", string.Empty)));
            }
            else if (arg.Length > 0 && a.Contains(";") && !isthisaline)
            {
                a = String.Join(" ", cmd, a);
                var temp1 = a.Split(';');
                string temp3 = "";
                for (int temp2 = 0; temp2 < temp1.Length; temp2++)
                {
                    temp1[temp2] = temp1[temp2].Replace(" ", "");
                    temp3 = String.Join("", temp3, compile(temp1[temp2]));
                }
                ret = temp3;
            }
            else
                if (cmd[0] == '\"' && cmd[cmd.Length - 1] == '\"')
            {
                cmd = cmd.Replace("-", " ");
                return cmd.Replace("\"", string.Empty);
            }
            else if (arg.Length > 0 && (Regex.IsMatch(cmd, @"[a-zA-Z]") || Regex.IsMatch(arg[1], @"[a-zA-Z]")) && (arg[0] == "<" || arg[0] == ">" || arg[0] == "==" || arg[0] == "<=" || arg[0] == ">="))
            {
                var temp1 = compile(cmd);
                var temp2 = compile(arg[1]);
                ret = compile(temp1 + " " + arg[0] + " " + temp2);
            }
            else if (arg.Length > 0 && (Regex.IsMatch(cmd, @"[a-zA-Z]") || Regex.IsMatch(arg[1], @"[a-zA-Z]")) && (arg[0] == "+" || arg[0] == "-" || arg[0] == "/" || arg[0] == "*" || arg[0] == "%"))
            {
                var temp1 = compile(cmd);
                var temp2 = compile(arg[1]);
                ret = compile(temp1 + " " + arg[0] + " " + temp2);
            }
            else
                if (Regex.IsMatch(cmd, @"\d"))
            {
                a = a.Replace("==", "=");
                ret = new DataTable().Compute(cmd + a, null).ToString();

            }
            else if (arg.Length > 0 && a.Contains("AND") && !isthisaline)
            {
                a = String.Join(" ", cmd, a);
                var temp1 = a.Split(new string[] { "AND" }, StringSplitOptions.RemoveEmptyEntries);
                string temp3 = "";
                for (int temp2 = 0; temp2 < temp1.Length; temp2++)
                {
                    if (compile(temp1[temp2]) == "True")
                        temp3 = "True";
                    else
                        temp3 = "False";
                }
                ret = temp3;
            }
            else if (arg.Length > 0 && a.Contains("OR") && !isthisaline)
            {
                a = String.Join(" ", cmd, a);
                var temp1 = a.Split(new string[] { "OR" }, StringSplitOptions.RemoveEmptyEntries);
                string temp3 = "False";
                for (int temp2 = 0; temp2 < temp1.Length; temp2++)
                {
                    if (compile(temp1[temp2]) == "True")
                        temp3 = "True";
                }
                ret = temp3;
            }

            else if (cmd == "True" || cmd == "False")
            {
                ret = cmd;
            }

            else
            {


                #endregion
                switch (cmd)
                {
                    default:
                        int varindex = varnames.IndexOf(cmd);
                        ret = vars[varindex];
                        break;
                    case "PRINT":
                        string test = compile(a);
                        Console.Write(compile(a));
                        break;
                    case "PRINT:LN":
                        Console.WriteLine(compile(a));
                        break;
                    case "ABS":
                        ret = Math.Abs(Int32.Parse(compile(a))).ToString();
                        break;
                    case "ASC":
                        ret = compile(a);
                        var r = (int)ret[0];
                        ret = r.ToString();
                        break;
                    case "ATN":
                        ret = Math.Atan(Double.Parse(compile(arg[0]))).ToString();
                        break;
                    case "CHR$":
                        ret = Char.ToString((char)Int32.Parse(compile(a)));
                        break;
                    case "COS":
                        ret = Math.Cos(Int32.Parse(compile(a))).ToString();
                        break;
                    case "IF":
                        var compareto = new List<string>(a.Split(new string[] { "THEN" }, StringSplitOptions.RemoveEmptyEntries));
                        //var compareto = Regex.Split(string, "THEN");
                        var acompareto = compile(compareto[0]);
                        if ("True" == acompareto)
                        {

                            compareto.RemoveAt(0);
                            string temp2 = String.Join("", compareto);
                            compile(temp2);

                        }
                        break;
                    case "GOTO":
                        if (lineno < Int32.Parse(a))
                        {
                            int gotot1 = -999;
                            int gotot2 = i;
                            while (Int32.Parse(a) != gotot1)
                            {
                                string word = words[gotot2 - 1];
                                string[] asd = word.Split(' ');
                                var fooga = new List<string>(asd);
                                gotot1 = Int32.Parse(fooga[0]);
                                gotot2++;
                                i = gotot2 - 2;
                            }


                        }
                        else if (lineno > Int32.Parse(a))
                        {
                            int gotot1 = -999;
                            int gotot2 = i;
                            while (Int32.Parse(a) != gotot1)
                            {
                                string word = words[gotot2 - 1];
                                string[] asd = word.Split(' ');
                                var fooga = new List<string>(asd);
                                gotot1 = Int32.Parse(fooga[0]);
                                gotot2--;
                                i = gotot2;
                            }
                        }
                        else
                        {
                            i = i - 1;
                        }
                        break;
                    case "CRSR":
                        Console.SetCursorPosition(Int32.Parse(compile(arg[0])), Int32.Parse(compile(arg[1])));
                        break;
                    case "CLS":
                        Console.Clear();
                        break;
                    #region colors
                    case "BGCOLOR":
                        switch (Int32.Parse(compile(a)))
                        {
                            case 0:
                                Console.BackgroundColor = ConsoleColor.Black;
                                break;
                            case 1:
                                Console.BackgroundColor = ConsoleColor.White;
                                break;
                            case 2:
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                break;
                            case 3:
                                Console.BackgroundColor = ConsoleColor.Cyan;
                                break;
                            case 4:
                                Console.BackgroundColor = ConsoleColor.Magenta;
                                break;
                            case 5:
                                Console.BackgroundColor = ConsoleColor.DarkGreen;
                                break;
                            case 6:
                                Console.BackgroundColor = ConsoleColor.DarkBlue;
                                break;
                            case 7:
                                Console.BackgroundColor = ConsoleColor.Yellow;
                                break;
                            case 8:
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                                break;
                            case 9:
                                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                                break;
                            case 10:
                                Console.BackgroundColor = ConsoleColor.Red;
                                break;
                            case 11:
                                Console.BackgroundColor = ConsoleColor.DarkGray;
                                break;
                            case 12:
                                Console.BackgroundColor = ConsoleColor.Gray;
                                break;
                            case 13:
                                Console.BackgroundColor = ConsoleColor.Green;
                                break;
                            case 14:
                                Console.BackgroundColor = ConsoleColor.Blue;
                                break;
                            case 15:
                                Console.BackgroundColor = ConsoleColor.Gray;
                                break;
                        }
                        break;
                    case "FGCOLOR":
                        switch (Int32.Parse(compile(a)))
                        {
                            case 0:
                                Console.ForegroundColor = ConsoleColor.Black;
                                break;
                            case 1:
                                Console.BackgroundColor = ConsoleColor.White;
                                break;
                            case 2:
                                Console.BackgroundColor = ConsoleColor.DarkRed;
                                break;
                            case 3:
                                Console.BackgroundColor = ConsoleColor.Cyan;
                                break;
                            case 4:
                                Console.BackgroundColor = ConsoleColor.Magenta;
                                break;
                            case 5:
                                Console.BackgroundColor = ConsoleColor.DarkGreen;
                                break;
                            case 6:
                                Console.BackgroundColor = ConsoleColor.DarkBlue;
                                break;
                            case 7:
                                Console.BackgroundColor = ConsoleColor.Yellow;
                                break;
                            case 8:
                                Console.BackgroundColor = ConsoleColor.DarkYellow;
                                break;
                            case 9:
                                Console.BackgroundColor = ConsoleColor.DarkMagenta;
                                break;
                            case 10:
                                Console.BackgroundColor = ConsoleColor.Red;
                                break;
                            case 11:
                                Console.BackgroundColor = ConsoleColor.DarkGray;
                                break;
                            case 12:
                                Console.BackgroundColor = ConsoleColor.Gray;
                                break;
                            case 13:
                                Console.BackgroundColor = ConsoleColor.Green;
                                break;
                            case 14:
                                Console.BackgroundColor = ConsoleColor.Blue;
                                break;
                            case 15:
                                Console.BackgroundColor = ConsoleColor.Gray;
                                break;
                        }
                        break;
                    #endregion
                    case "GOSUB":
                        returnto = i;
                        if (lineno < Int32.Parse(a))
                        {
                            int gotot1 = -999;
                            int gotot2 = i;
                            while (Int32.Parse(a) != gotot1)
                            {
                                string word = words[gotot2 - 1];
                                string[] asd = word.Split(' ');
                                var fooga = new List<string>(asd);
                                gotot1 = Int32.Parse(fooga[0]);
                                gotot2++;
                                i = gotot2 - 2;
                            }


                        }
                        else if (lineno > Int32.Parse(a))
                        {
                            int gotot1 = -999;
                            int gotot2 = i;
                            while (Int32.Parse(a) != gotot1)
                            {
                                string word = words[gotot2 - 1];
                                string[] asd = word.Split(' ');
                                var fooga = new List<string>(asd);
                                gotot1 = Int32.Parse(fooga[0]);
                                gotot2--;
                                i = gotot2;
                            }
                        }
                        else
                        {
                            i = i - 1;
                        }
                        break;
                    case "RETURN":
                        i = returnto;
                        break;
                    case "END":
                        Exit();
                        break;
                    case "CSSIZE":

                        Console.SetWindowSize(Int32.Parse(compile(arg[0])), Int32.Parse(compile(arg[1])));
                        break;

                    case "INPUT":
                        ret = Console.ReadLine();
                        break;
                }
            }

            return ret;
                    }
        static void Exit()
        {
            Console.ReadKey();

            Environment.Exit(0);
        }

            
        }
    }

