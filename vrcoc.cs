using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace vrco
{
    public class vrcoc
    {
        public static Dictionary<string, string> WordReplace = new Dictionary<string, string>()
        {
            {"Console", "Console"},
            {"Print", "Print" }
        };
        public static string Cut = "";
        public static string Compiler(string Code)
        {
            Code.Replace("\n", "");
            foreach (var word in WordReplace)
            {
                Code = Code.Replace(word.Value, word.Key);
            }
            string pattern = @"[^{};]+(?=;)|\{[^{}]+\}|[^{};]+";
            MatchCollection Lines = Regex.Matches(Code, pattern);
            Code = String.Empty;
            foreach(var LineM in Lines)
            {
                string Line = LineM.ToString();
                while (Line.StartsWith(" ")) Line = Line.Remove(0, 1);
                while (Line.EndsWith(" ")) Line = Line.Remove(Line.Length - 1);
                foreach (var Mode in Modes)
                {
                    Mode.Invoke(Line);
                    if (!String.IsNullOrEmpty(Cut))
                    {
                        Code += "\n" + Cut;
                        Cut = "";
                        break;
                    }
                }
            }
            string Final = string.Empty;
            foreach(string s in Code.Split("\n"))
            {
                if (s != "")Final+="\n"+s;
            }
            try { Final = Final.Remove(0, 1); } catch { }
            return Final;
        }
        public static string VAR = "";
        public static string VarRead(string var)
        {
            string v = "";
            foreach (Action<string> vo in Varout)
            {
                vo.Invoke(var);
                if (VAR != "")
                {
                    v = VAR;
                    VAR = "";
                    break;
                }
                if (VAR == " ")
                {
                    v = VAR;
                    VAR = "";
                    break;
                }
            }
            return v;
        }
        public static List<Action<string>> Varout = new List<Action<string>>();
        public static List<Action<string>> Modes = new List<Action<string>>(); 
    }
    public class OebCodeFinder
    {

    }
    public class Error
    {
        public int TaskNumber;
        public string ScriptPath;
        public string ErrorCode;
        public string ErrorMessage;
        public static Dictionary<string, Error> Errors = new Dictionary<string, Error>();
        public override string ToString()
        {
            return $"Err:{ErrorCode};\n{ErrorMessage}\n\nin \"{ScriptPath}\" in {TaskNumber}";
        }
        public Error(Error @base, int taskNumber, string Path = "unknown")
        {
            ErrorMessage = @base.ErrorMessage;
            ErrorCode = @base.ErrorCode;
            TaskNumber = taskNumber;
            ScriptPath = Path;
        }
        public Error(string ErrorMessage = "unknown", string ErrorCode = "unknown")
        {
            this.ErrorMessage = ErrorMessage ;
            this.ErrorCode = ErrorCode ;
            Errors.Add(ErrorCode, this);
        }
    }
}
