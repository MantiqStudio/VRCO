using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static opio.OpioSimply;

namespace opio
{
    public class OpioEngine
    {
        public static Dictionary<string, object> oc = new Dictionary<string, object>();
        public static List<object> oo = new List<object>();
        //old command runner (bad) \/
        public static int LineKey = 0;
        //public static bool Break = false;
        //public static void StartOpioCommand(string command)
        //{
        //    if (command == "") return;
        //    bool finded = false;
        //    object value = null;
        //    bool isThis = false;
        //    int fei = 0;
        //    foreach (OpioInfo info in OpioInfo.infos)
        //    {
        //        info.ApplyInfo(command);
        //        if (info.isInfo)
        //        {
        //            value = info.re;
        //            finded = true;
        //            if (info.Name == "admin")
        //            {
        //                return;
        //            }
        //            break;
        //        }
        //    }
        //    if (value != null) oo.Add(value);
        //    if (!finded)
        //    {
        //        bool act = command.Last() == ')';
        //        if (act)
        //        {
        //            string[] cpp = command.Split('(');
        //            if (cpp.Length > 2)
        //            {
        //                List<string> cpp2 = cpp.ToList();
        //                string newCPPat1 = cpp2[1];
        //                cpp2.RemoveAt(0);
        //                cpp2.RemoveAt(0);
        //                foreach (string c in cpp2)
        //                {
        //                    newCPPat1 += "(" + c;
        //                }
        //                cpp[1] = newCPPat1;
        //            }
        //            cpp[1] = cpp[1].Remove(cpp[1].Length - 1);
        //            if (cpp[1] != "")
        //                StartOpioCommands(cpp[1].Replace(",", ";"));
        //            command = cpp[0];

        //        }
        //        List<string> commandPoint = command.Split(".").ToList();
        //        OpioClass nowClass;
        //        try
        //        {
        //            nowClass = oc[commandPoint.First()];
        //            commandPoint.RemoveAt(0);
        //        }
        //        catch
        //        {
        //            try { nowClass = oc["this"]; }
        //            catch { oo.Add(new OpioError { error = "none class", code = "0004" }); return; }
        //        }
        //        if (commandPoint.Count == 0)
        //        {
        //            oo.Add(nowClass); 
        //            return;
        //        }

        //        int i = commandPoint.Count;
        //        int o = 0;
        //        while (o != commandPoint.Count)
        //        {
        //            string key = commandPoint[0];
        //            commandPoint.RemoveAt(0);
        //            if (!nowClass.objects.ContainsKey(key) && key.Replace("=", "") == key)
        //            {
        //                oo.Add(new OpioError { error = "object?", code = "0001" });
        //                return;
        //            }
        //            o++;
        //            fei++;
        //            i--;
        //            string okey = key;
        //            if (act)
        //            {

        //                if (nowClass.objects[key] is Action)
        //                {
        //                    ((Action)nowClass.objects[key]).Invoke();
        //                }
        //                else
        //                {
        //                    if (nowClass.objects[key] is OpioAction)
        //                    {
        //                        (nowClass.objects[key] as OpioAction).invoke();
        //                    }
        //                    else { oo.Add(new OpioError() { error = "Action?", code = "0007" }); }
        //                }
        //                break;
        //            }
        //            else if (key.Replace("=", "") != key)
        //            {
        //                string[] cpg = key.Split('=');
        //                if (cpg.Length < 2) { oo.Add(new OpioError { error = "none new", code = "0002" }); }
        //                StartOpioCommand(cpg[1]);
        //                try
        //                {
        //                    try { nowClass.objects[cpg[0]] = oo[oo.Count - 1]; }
        //                    catch { nowClass.objects.Add(cpg[0], oo[oo.Count - 1]); }
        //                }
        //                catch { oo.Add(new OpioError() { error = "object?", code = "0001" }); }
        //                break;
        //            }
        //            else
        //            {
        //                if (nowClass.objects[okey] is OpioClass)
        //                {
        //                    OpioClass @class = nowClass.objects[okey] as OpioClass;
        //                    if (@class != null)
        //                    {
        //                        nowClass = @class;
        //                    }
        //                    else
        //                    {
        //                        oo.Add(nowClass.objects[okey]);
        //                        break;
        //                    }
        //                }
        //                else
        //                {
        //                    oo.Add(nowClass.objects[okey]);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}
        //public static void StartOpioCommands(string commands)
        //{
        //    string[] coomandsArray = commands.Split(";");
        //    LineValue = coomandsArray.Length;
        //    LineKey = 0;
        //    while (LineKey < LineValue)
        //    {
        //        StartOpioCommand(coomandsArray[LineKey]);
        //        LineKey++;
        //    }
        //}
        public static int LineValue = 0;
        //old command runner (bad) /\
        public static void StartOpioCommands(string command)
        {
            BuildSimplys(opsCompiler(command));
        }
        public static object GiveOpioObject(int num = 1)
        {
            object obj = oo[oo.Count - num];
            oo.RemoveAt(oo.Count - num);
            return obj;
        }

        public static Dictionary<string, OpioMode> modes = new Dictionary<string, OpioMode>();
        public static void BuildSimply(string command)
        {
            string[] commandSpy = command.Split(new[] { '>' }, 2);

            string mode = commandSpy[0];
            string code = "";
            if (commandSpy.Length == 2) code = commandSpy[1];
            modes[mode].Run(code);
        }

        public static List<string> SmartSplit(string str, char FirstChar, char LastChar, char spiltWith)
        {
            List<string> r = new List<string>();
            r.Add("");
            int ZO = 0;
            char c2 = '.';
            foreach (char c in str)
            {
                if (c == FirstChar) ZO++;
                else if (c == LastChar) ZO--;
                else if (c == spiltWith && ZO == 0 && c2 != '\\') r.Add("");
                else
                {
                    c2 = c;
                    r[r.Count - 1] += c;
                }
            }
            return r;
        }
        public static void BuildSimplys(string commands)
        {
            foreach (string cspy in SmartSplit(commands, '{', '}', '<'))
            {
                if (cspy != "") BuildSimply(cspy);
            }
        }
        public static int Line = 0;
        public static List<string> Lines = new List<string>();
        public static void AddLines(string commands)
        {
            var scs = SmartSplit(commands, '{', '}', '<');
            foreach (string cspy in scs) Lines.Add(cspy);
        }
        public static bool Break = false;
        public static void RunLines()
        {
            Line = Lines.Count;
            while (Line > 0)
            {
                Line--;
                string cspy = Lines[Line];
                if (cspy != "") BuildSimply(cspy);
                if (Break)
                {
                    Break = false;
                    return;
                }
            }
        }
    }
    public class OpioMode
    {
        public virtual void Run(string code) { }
        public string Return = "";
        public virtual void Compiled(string code) { }
        public static void Apply(string name, OpioMode mode)
        {
            try { OpioEngine.modes.Add(name, mode); }
            catch { }
        }
    }
    public class OpioSimply
    {
        public static string opsCommandCompiler(string code)
        {
            foreach (OpioMode mode in OpioEngine.modes.Values)
            {
                try { mode.Compiled(code); }
                catch
                {
                }
                if (mode.Return != null && mode.Return != "")
                {
                    code = mode.Return;
                    mode.Return = null;
                    break;
                }
            }
            return code;
        }
        public static string opsCompiler(string code)
        {
            string pattern = ";(?![^{]*})";
            List<string> list = Regex.Split(code, pattern).ToList();
            int i = list.Count - 1;
            while (i > -1)
            {
                list[i] = opsCommandCompiler(list[i]);
                i--;
            }
            code = list[0];
            list.RemoveAt(0);
            foreach (string item in list)
            {
                code += "<" + item;
            }
            return code;
        }
        public static string ReaderCompiler(string command)
        {
            foreach (string modeKey in OpioEngine.modes.Keys)
            {
                OpioMode mode = OpioEngine.modes[modeKey];
                try { mode.Compiled(command); }
                catch
                {
                }
                if (mode.Return != null && mode.Return != "")
                {
                    command = modeKey;
                    mode.Return = null;
                    break;
                }
            }
            return command;
        }
        public static void ApplyOfficialModes()
        {
            OpioMode.Apply("IMPORT", new IMPORT());
            //OpioMode.Apply("MATH", new MathOM());
            OpioMode.Apply("Compiler", new CompilerFixer());
            OpioMode.Apply("so", new SetChild());
            OpioMode.Apply("IF", new IF());
            OpioMode.Apply("WHILE", new WHILE());
            OpioMode.Apply("raa", new RunAutoAction());
            OpioMode.Apply("ao", new AddChild());
            OpioMode.Apply("NEW", new NEW());
            OpioMode.Apply("go", new GetChild());
            OpioMode.Apply("ro", new RemoveChild());
            OpioMode.Apply("STRING", new STRING());
            OpioMode.Apply("INT", new INT());
            OpioMode.Apply("FLOAT", new FLOAT());
            OpioMode.Apply("TASK", new TASK());
            OpioMode.Apply("CHAR", new CHAR());
            OpioMode.Apply("TRUE", new TRUE());
            OpioMode.Apply("FULSE", new FULSE());
            OpioMode.Apply("CLASS", new CLASS());
            OpioMode.Apply("WC", new WHITECLASS());
            OpioMode.Apply("ACTION", new ACTION());
            OpioMode.Apply("rsa", new RunSystemAction());
            OpioMode.Apply("rosa", new RunOpioSimplyAction());
            OpioMode.Apply("rc", new ReClass());
            OpioMode.Apply("info", new info());
        }
        public class IMPORT : OpioMode
        {
            public override void Run(string code)
            {
                var oc = OpioEngine.oc["this"] as OpioClass;
                foreach (string key in (OpioEngine.GiveOpioObject() as OpioClass).Keys)
                {
                    object value = (OpioEngine.GiveOpioObject() as OpioClass)[key];
                    Console.WriteLine(key, value);
                    oc.Add(key, value);
                }
            }

            public override void Compiled(string code)
            {
                if (code.StartsWith("import "))
                {
                    code = code.Remove(0, 7);
                    Return = $"go>{code.Replace('.', '/')}<IMPORT>";
                }
            }
        }
        public class MathOM : OpioMode
        {
            public static DataTable db = new DataTable();
            public override void Run(string code)
            {
                OpioEngine.oo.Add(db.Compute(code, ""));
            }
            public override void Compiled(string code)
            {
                string co = code;
                string chars = "1234567890-+*/>()";
                foreach (char c in chars)
                {
                    co = co.Replace(c.ToString(), "");
                }
                if (co == "" && code != "")
                {
                    Return = $"MATH>{code}";
                }
            }
        }
        public class CompilerFixer : OpioMode
        {
            public override void Compiled(string code)
            {
                if (code.StartsWith('{') && code.EndsWith('}'))
                {
                    Return = opsCompiler(code.Remove(0, 1).Remove(code.Length - 2, 1));
                }
            }
        }
        public class IF : OpioMode
        {
            //demo : ACTION>STRING>HI|~|go>Console/Print|~|raa><TRUE><IF>
            public override void Run(string code)
            {
                bool boolen = (bool)OpioEngine.GiveOpioObject(2);
                OpioSimplyAction action = (OpioSimplyAction)OpioEngine.GiveOpioObject(1);
                if (boolen) action.invoke();
            }
            //demo : if(true){Console.Print("hi")}
            public override void Compiled(string code)
            {
                if (code.StartsWith("if("))
                {
                    string[] strings = RunAutoAction.FixSplit(code.Remove(0, 3), ')');
                    string boolen = strings[0];
                    string action = strings[1];
                    boolen = opsCompiler(boolen);
                    action = opsCompiler(action);
                    if (action.StartsWith('{') && action.EndsWith('}'))
                    {
                        Return = $"{boolen}<ACTION>{action}<IF>";
                    }
                    else
                    {
                        Return = $"{boolen}<ACTION>{{{action}}}<IF>";
                    }
                }
            }
        }
        public class WHILE : OpioMode
        {
            public override void Run(string code)
            {
                OpioSimplyAction boolen = (OpioSimplyAction)OpioEngine.GiveOpioObject(2);
                OpioSimplyAction action = (OpioSimplyAction)OpioEngine.GiveOpioObject(1);
                boolen.invoke();
                bool b = (bool)OpioEngine.GiveOpioObject();
                while (b)
                {
                    action.invoke();
                    boolen.invoke();
                    b = (bool)OpioEngine.GiveOpioObject();
                }
            }
            public override void Compiled(string code)
            {
                if (code.StartsWith("while("))
                {
                    string[] strings = RunAutoAction.FixSplit(code.Remove(0, 6), ')');
                    string boolen = strings[0];
                    string action = strings[1];
                    boolen = opsCompiler(boolen);
                    action = opsCompiler(action);
                    if (action.StartsWith('{') && action.EndsWith('}'))
                    {
                        Return = $"ACTION>{boolen}<ACTION>{action}<WHILE>";
                    }
                    else
                    {
                        Return = $"ACTION>{boolen}<ACTION>{{{action}}}<WHILE>";
                    }
                }
            }
        }
        public class NEW : OpioMode
        {
            public override void Run(string code)
            {
                OpioEngine.oo.Add(OpioClass.temp(code));
            }

            public override void Compiled(string code)
            {
                if (code.StartsWith("new "))
                {
                    code = code.Remove(0, 4);
                    Return = "NEW>" + code;
                }
            }
        }
        public class TASK : OpioMode
        {
            public TASK()
            {
                Tasks.Add("plus", plus);
                Tasks.Add("minus", minus);
                Tasks.Add("multiply", multiply);
                Tasks.Add("divide", divide);
                Tasks.Add("equal", equal);
                Tasks.Add("x", x);
            }
            public override void Run(string code)
            {
                Tasks[code].Invoke();
            }

            public override void Compiled(string code)
            {
                if (code == "+") Return = "TASK>plus";
                else if (code == "-") Return = "TASK>minus";
                else if (code == "/") Return = "TASK>divide";
                else if (code == "*") Return = "TASK>multiply";
                else if (code == "~") Return = "TASK>equal";
                else if (
                    code.Contains('*') ||
                    code.Contains('/') ||
                    code.Contains('+') ||
                    code.Contains('-') ||
                    code.Contains("~")
                    &&
                    !code.Contains('\"'))
                {

                    string[] numbers = Regex.Split(code, @"[+\-*~/]");
                    string[] operations = Regex.Split(code, @"\d+");
                    Return = opsCompiler(numbers[0]);
                    for (int i = 1; i < numbers.Length; i++)
                    {
                        Return += "<" + opsCompiler(numbers[i]);
                        if (i < operations.Length - 1)
                        {
                            Return += "<" + opsCompiler(operations[i]);
                        }
                    }
                }
            }

            public static Dictionary<string, Action> Tasks = new Dictionary<string, Action>();
            public static void x()
            {
                var v1 = OpioEngine.GiveOpioObject(1);
                OpioEngine.oo.Add(!(bool)v1);
            }
            public static void equal()
            {
                object v1 = OpioEngine.GiveOpioObject(2);
                object v2 = OpioEngine.GiveOpioObject(1);
                OpioEngine.oo.Add(v1.Equals(v2));
            }
            public static void plus()
            {
                var v1 = OpioEngine.GiveOpioObject(2);
                var v2 = OpioEngine.GiveOpioObject(1);
                if (v1 is float && v2 is float)
                {
                    OpioEngine.oo.Add((float)v1 + (float)v2);
                }
                else if (v1 is float && v2 is int)
                {
                    OpioEngine.oo.Add((float)v1 + (int)v2);
                }
                else if (v1 is int && v2 is float)
                {
                    OpioEngine.oo.Add((int)v1 + (float)v2);
                }
                else
                {
                    OpioEngine.oo.Add((int)v1 + (int)v2);
                }
            }
            public static void minus()
            {
                var v1 = OpioEngine.GiveOpioObject(2);
                var v2 = OpioEngine.GiveOpioObject(1);
                if (v1 is float && v2 is float)
                {
                    OpioEngine.oo.Add((float)v1 - (float)v2);
                }
                else if (v1 is float && v2 is int)
                {
                    OpioEngine.oo.Add((float)v1 - (int)v2);
                }
                else if (v1 is int && v2 is float)
                {
                    OpioEngine.oo.Add((int)v1 - (float)v2);
                }
                else
                {
                    OpioEngine.oo.Add((int)v1 - (int)v2);
                }
            }
            public static void multiply()
            {
                var v1 = OpioEngine.GiveOpioObject(2);
                var v2 = OpioEngine.GiveOpioObject(1);
                if (v1 is float && v2 is float)
                {
                    OpioEngine.oo.Add((float)v1 * (float)v2);
                }
                else if (v1 is float && v2 is int)
                {
                    OpioEngine.oo.Add((float)v1 * (int)v2);
                }
                else if (v1 is int && v2 is float)
                {
                    OpioEngine.oo.Add((int)v1 * (float)v2);
                }
                else
                {
                    OpioEngine.oo.Add((int)v1 * (int)v2);
                }
            }
            public static void divide()
            {
                var v1 = OpioEngine.GiveOpioObject(2);
                var v2 = OpioEngine.GiveOpioObject(1);
                if (v1 is float && v2 is float)
                {
                    OpioEngine.oo.Add((float)v1 / (float)v2);
                }
                else if (v1 is float && v2 is int)
                {
                    OpioEngine.oo.Add((float)v1 / (int)v2);
                }
                else if (v1 is int && v2 is float)
                {
                    OpioEngine.oo.Add((int)v1 / (float)v2);
                }
                else
                {
                    OpioEngine.oo.Add((int)v1 / (int)v2);
                }
            }

        }
        public class ReClass : OpioMode
        {
            public override void Run(string code)
            {
                OpioClass oc = OpioEngine.oc[code] as OpioClass;
                if (oc.CustomeReturn == null)
                {
                    OpioEngine.oo.Add(oc);
                }
                else
                {
                    OpioEngine.oo.Add(oc.CustomeReturn);
                }
            }
            public override void Compiled(string code)
            {
                if (code.Split('.', ' ', '!', '#', '@', '$', '%', '^', '&', '*', '(', ')', '\"').Length == 1 && OpioEngine.oc.ContainsKey(code))
                {
                    Return = "rc>" + code;
                }
            }
        }
        public class GetChild : OpioMode
        {
            public override void Run(string code)
            {
                if (!code.Contains("/"))
                {
                    OpioClass oc = (OpioClass)OpioEngine.oc[code];
                    if (oc.CustomeReturn == null)
                    {
                        OpioEngine.oo.Add(oc);
                    }
                    else
                    {
                        OpioEngine.oo.Add(oc.CustomeReturn);
                    }
                    return;
                }
                List<string> strings = code.Split("/").ToList();
                OpioClass main = OpioEngine.oc[strings[0]] as OpioClass;
                strings.RemoveAt(0);
                string key = strings[strings.Count - 1];
                strings.RemoveAt(strings.Count - 1);
                if (strings.Count > 0)
                {
                    foreach (var s in strings)
                    {
                        main = main[s] as OpioClass;
                    }
                }
                object value = main[key];
                OpioEngine.oo.Add(value);
            }
        }
        public class SetChild : OpioMode
        {
            public override void Run(string code)
            {
                List<string> strings = code.Split("/").ToList();
                OpioClass main = OpioEngine.oc[strings[0]] as OpioClass;
                strings.RemoveAt(0);
                string key = strings[strings.Count - 1];
                strings.RemoveAt(strings.Count - 1);
                if (strings.Count > 0)
                {
                    foreach (var s in strings)
                    {
                        main = main[s] as OpioClass;
                    }
                }
                main[key] = OpioEngine.GiveOpioObject();
            }

            public override void Compiled(string code)
            {
            }
        }
        public class AddChild : OpioMode
        {
            // demo: STRING>hi<ao>str<
            public override void Run(string code)
            {
                object obj = OpioEngine.GiveOpioObject();
                OpioClass cls = OpioEngine.oc["this"] as OpioClass;
                cls.Add(code, obj);
            }
            //demo: object str="hi";
            public override void Compiled(string code)
            {

            }
        }
        public class NULL : OpioMode
        {
            public override void Run(string code)
            {
                OpioEngine.oo.Add(null);
            }
            public override void Compiled(string code)
            {
                if (code == "null")
                {
                    Return = "NULL>";
                }
            }
        }
        public class RemoveChild : OpioMode
        {
            public override void Run(string code)
            {
                OpioClass cls = OpioEngine.GiveOpioObject() as OpioClass;
                cls.Remove(code);
            }
        }
        public class RunSystemAction : OpioMode
        {
            public override void Run(string code)
            {
                (OpioEngine.GiveOpioObject() as Action).Invoke();
            }
        }
        public class RunOpioSimplyAction : OpioMode
        {
            public override void Run(string code)
            {
                (OpioEngine.GiveOpioObject() as OpioSimplyAction).invoke();
            }
        }
        public class RunAutoAction : OpioMode
        {
            public override void Run(string code)
            {
                object action = OpioEngine.GiveOpioObject();
                if (action is OpioAction) ((OpioAction)action).Invoke(OpioEngine.oo.ToArray());
                else if (action is Action) (action as Action).Invoke();
                else if (action is OpioSimplyAction) (action as OpioSimplyAction).invoke();
            }
            public override void Compiled(string code)
            {
                if (code.Contains('(') && code.Last() == ')')
                {
                    string[] AV = FixSplit(code, '(');
                    if (AV[0].Split('(')[0].Contains(".")) Return = $"{opsCompiler(AV[1].Remove(AV[1].Length - 1))}<go>{AV[0].Replace('.', '/')}<raa>";
                    else Return = $"{opsCompiler(AV[1].Remove(AV[1].Length - 1))}<go>this/{AV[0].Replace('.', '/')}<raa>";
                }
            }
            public static string[] FixSplit(string str, char splitWith)
            {
                string str1 = "";
                string str2 = "";
                bool at = false;
                foreach (char c in str)
                {
                    if (!at)
                    {
                        if (c == splitWith)
                        {
                            at = true;
                        }
                        else
                        {
                            str1 += c;
                        }
                    }
                    else
                    {
                        str2 += c;
                    }
                }
                return new string[] { str1, str2 };
            }
        }
        public class STRING : OpioMode
        {
            public static Dictionary<char, char> switches = new Dictionary<char, char>();
            public override void Run(string code)
            {
                string[] cs = code.Split("\\\\");
                int i = cs.Length - 1;
                while (i >= 0)
                {
                    foreach (char c in switches.Keys)
                    {
                        cs[i] = cs[i].Replace("\\" + c, switches[c].ToString());
                    }
                    i--;
                }
                foreach (string s in cs)
                {
                    code = s + '\\';
                }
                code = code.Remove(code.Length - 1);

                OpioEngine.oo.Add(code);
            }
            public override void Compiled(string code)
            {
                if (code.First() == '\"' && code.Last() == '\"')
                {
                    Return = "STRING>{" + code.Substring(1, code.Length - 2) + "}";
                }
            }
        }
        public class INT : OpioMode
        {
            public override void Run(string code)
            {
                OpioEngine.oo.Add(int.Parse(code));
            }
            public override void Compiled(string code)
            {
                string c = code;
                c = c.Replace("0", "");
                c = c.Replace("1", "");
                c = c.Replace("2", "");
                c = c.Replace("3", "");
                c = c.Replace("4", "");
                c = c.Replace("5", "");
                c = c.Replace("6", "");
                c = c.Replace("7", "");
                c = c.Replace("8", "");
                c = c.Replace("9", "");
                if (c == "" && code != "")
                {
                    Return = "INT>" + code;
                }
            }
        }
        public class FLOAT : OpioMode
        {
            public override void Run(string code)
            {
                OpioEngine.oo.Add(float.Parse(code));
            }
            public override void Compiled(string code)
            {
                string c = code;
                c = c.Replace("0", "");
                c = c.Replace("1", "");
                c = c.Replace("2", "");
                c = c.Replace("3", "");
                c = c.Replace("4", "");
                c = c.Replace("5", "");
                c = c.Replace("6", "");
                c = c.Replace("7", "");
                c = c.Replace("8", "");
                c = c.Replace("9", "");
                c = c.Replace(".", "");
                if (c == "" && code != "")
                {
                    Return = "FLOAT>" + code;
                }
            }
        }
        public class CHAR : OpioMode
        {
            public override void Run(string code)
            {
                OpioEngine.oo.Add(code[0]);
            }
            public override void Compiled(string code)
            {
                if (code.First() == '\'' && code.Last() == '\'')
                {
                    Return = "CHAR>" + code.Substring(1, code.Length - 2);
                }
            }
        }
        public class TRUE : OpioMode
        {
            public override void Run(string code)
            {
                OpioEngine.oo.Add(true);
            }
            public override void Compiled(string code)
            {
                if (code == "true")
                {
                    Return = "TRUE>";
                }
            }
        }
        public class FULSE : OpioMode
        {
            public override void Run(string code)
            {
                OpioEngine.oo.Add(false);
            }
            public override void Compiled(string code)
            {
                if (code == "false")
                {
                    Return = "FULSE>";
                }
            }
        }
        public class ACTION : OpioMode
        {
            public override void Run(string code)
            {
                OpioEngine.oo.Add(new OpioSimplyAction { OpioCommands = code });
            }
            public override void Compiled(string code)
            {
                if (code.StartsWith('<') && code.EndsWith('}'))
                {
                    code = code.Remove(0, 1);
                    code = code.Remove(code.Length - 1, 1);

                    int index = code.IndexOf(">{");
                    string firstPart = code.Substring(0, index);
                    string secondPart = code.Substring(index + 1);

                    if (firstPart == "Act")
                    {
                        Return = $"ACTION>{{{opsCompiler(secondPart.Remove(0, 1))}}}";
                    }
                    else
                    {
                        Return = $"{firstPart}>{{{opsCompiler(secondPart.Remove(0, 1))}}}";
                    }
                }
            }
        }

        public class CLASS : OpioMode
        {
            public override void Run(string code)
            {
                OpioClass oc = new OpioClass { Name = code };
                OpioEngine.oo.Add(oc);
                if (OpioEngine.oc.ContainsKey("this"))
                {
                    OpioEngine.oc["this"] = oc;
                }
                else
                {
                    OpioEngine.oc.Add("this", oc);
                }
            }
            public override void Compiled(string code)
            {
                if (code.StartsWith("class "))
                {
                    code = code.Remove(0, 6);
                    Return = "CLASS>" + code;
                }
            }
        }
        public class WHITECLASS : OpioMode
        {
            public override void Run(string code)
            {
                OpioClass oc = OpioEngine.GiveOpioObject() as OpioClass;
                OpioEngine.oc.Add(oc.Name, oc);
            }

            public override void Compiled(string code)
            {
                if (code == ":static")
                {
                    code.Remove(0, 6);
                    Return = "WC>";
                }
            }
        }
        public class info : OpioMode
        {
            public override void Run(string code)
            {

            }
            public override void Compiled(string code)
            {
                if (code.StartsWith("object "))
                {
                    Return = "ao>";
                    code = code.Remove(0, 7);
                    int index = code.IndexOf('=');
                    string Key = code.Split("=")[0];
                    var S = code.Split("=").ToList();
                    S.RemoveAt(0);
                    string Value = "";
                    foreach (var ss in S)
                    {
                        Value += "=" + ss;
                    }
                    Value = Value.Remove(0, 1);

                    Return = $"{opsCompiler(Value)}<ao>{Key}";
                    return;
                }
                if (code[0] == '/' && code[1] == '/')
                {
                    Return = "info>" + code.Remove(0, 2);
                }
                else if (code.Contains('='))
                {
                    string[] s = RunAutoAction.FixSplit(code, '=');
                    Return = $"{opsCompiler(s[1])}<so>{s[0].Replace('.', '/')}";
                }
                else
                {
                    if (code.Contains(".")) Return = "go>" + code.Replace('.', '/').Replace(" ", "");
                    else Return = "go>this/" + code;
                }
            }
        }
    }

    
    public class OpioTask
    {
        public static Task<object> GetFPath(string path)
        {
            object obj = null;
            if (path.Contains('/'))
            {
                List<string> names = path.Split('/').ToList();
                object main = OpioEngine.oc[names[0]];
                names.RemoveAt(0);
                int i = names.Count;
                foreach (string name in names)
                {
                    if (i > 0) main = ((OpioClass)main)[name];
                    else
                    {
                        obj = OpioEngine.oc[path];
                        return Task.FromResult(obj);
                    }
                }
            }
            else
            {
                obj = OpioEngine.oc[path];
                return Task.FromResult(obj);
            }
            return Task.FromResult(obj);
        }
    }
    public class OpioSimplyAction
    {
        public string OpioCommands;
        public void invoke() { OpioEngine.BuildSimplys(OpioCommands); }
    }
    public class OpioClass : Dictionary<string, object>
    {
        public string Name { get; set; }
        public override string ToString()
        {
            return Name;
        }

        public object CustomeReturn = null;
        public static OpioClass from(string name, string temp)
        {
            OpioClass oc = new OpioClass() { Name = name };
            oc = ((OpioClass)OpioEngine.oc[temp]);
            return oc;
        }
        public static OpioClass temp(string temp)
        {
            OpioClass oc = new OpioClass() { Name = temp };
            oc = ((OpioClass)OpioEngine.oc[temp]);
            return oc;
        }
    }
    public class OpioAction
    {
        public List<string> Types;
        public virtual void Invoke(object[] objects)
        {

        }
    }
    public class OpioOpsAction : OpioAction
    {
        public string action;
        public override void Invoke(object[] objects)
        {
            OpioEngine.BuildSimplys(action);
        }
    }
    public class OpioSystemAction : OpioAction
    {
        public Action<object[]> action;
        public override void Invoke(object[] objects)
        {
            action.Invoke(objects);
            base.Invoke(objects);
        }
        public OpioSystemAction(Action<object[]> action)
        {
            this.action = action;
        }
    }
    //public class OpioInfo
    //{
    //    public static List<OpioInfo> infos = new List<OpioInfo>();
    //    public string Name { get; set; }
    //    public bool isInfo;
    //    public object re;
    //    public virtual void ApplyInfo(string info)
    //    {
    //    }
    //    public static OpioInfo MakeInfo(string infoName)
    //    {
    //        var info = new OpioInfo { Name = infoName };
    //        infos.Add(info);
    //        return info;
    //    }

    //}
    //public class OpioBasic
    //{
    //    public static void ApplyBasicInfo()
    //    {
    //        OpioInfo.infos.Add(new OpioNoteInfo { Name = "note" });
    //        OpioInfo.infos.Add(new OpioStringInfo { Name = "string" });
    //        OpioInfo.infos.Add(new OpioEqualsInfo { Name = "equals" });
    //        OpioInfo.infos.Add(new OpioHomeVarInfo { Name = "HomeVar" });
    //        OpioInfo.infos.Add(new OpioCharInfo { Name = "char" });
    //        OpioInfo.infos.Add(new OpioActionInfo { Name = "action" });
    //        OpioInfo.infos.Add(new OpioClassInfo { Name = "class" });
    //        OpioInfo.infos.Add(new OpioNullInfo { Name = "null" });
    //        OpioInfo.infos.Add(new OpioImportInfo { Name = "import" });
    //        OpioInfo.infos.Add(new OpioIfInfo { Name = "if" });
    //        OpioInfo.infos.Add(new OpioWhileInfo { Name = "while" });
    //        OpioInfo.infos.Add(new OpioStopInfo { Name = "stop" });
    //        new reeS("n", "\n");
    //        new reeS("e", ";");
    //        new reeS("l", "_");
    //        new reeS("s", "=");
    //        new reeS("c", ":");
    //        new reeS("d", ".");
    //        new reeS("r", ",");
    //        new reeS("t", "\"");
    //        new reeS("q", "~");
    //        new reeS("v", "\"");
    //    }

    //    public class OpioEqualsInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            bool tq = info.Replace("==", "") != info;
    //            bool fq = info.Replace("!=", "") != info;
    //            isInfo = tq || fq;
    //            if (isInfo)
    //            {
    //                if (tq)
    //                {
    //                    string[] strings = info.Split("==");
    //                    if (strings.Length > 2) isInfo = false;
    //                    OpioEngine.StartOpioCommand(strings[0]);
    //                    OpioEngine.StartOpioCommand(strings[1]);
    //                    OpioEngine.oo.Add(OpioEngine.GiveOpioObject() == OpioEngine.GiveOpioObject());
    //                }
    //                else
    //                {
    //                    string[] strings = info.Split("!=");
    //                    if (strings.Length > 2) isInfo = false;
    //                    OpioEngine.StartOpioCommand(strings[0]);
    //                    OpioEngine.StartOpioCommand(strings[1]);
    //                    OpioEngine.oo.Add(OpioEngine.GiveOpioObject() != OpioEngine.GiveOpioObject());
    //                }
    //            }
    //        }
    //    }
    //    public class OpioStopInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            isInfo = info == "stop";
    //            if (isInfo)
    //            {
    //                OpioEngine.LineKey = OpioEngine.LineValue;
    //            }
    //            isInfo = info.Split(" ")[0] == "key";
    //            if (isInfo)
    //            {
    //                OpioEngine.LineKey = int.Parse(info.Split(" ")[1]);
    //            }
    //        }
    //    }
    //    public class OpioIfInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            try
    //            {
    //                char[] chars = info.ToCharArray();
    //                isInfo = chars[0] == 'i' && chars[1] == 'f';
    //                if (isInfo)
    //                {
    //                    info = info.Remove(0, 3);
    //                    List<string> BAA = info.Split("){").ToList();
    //                    BAA[1] = BAA[1].Remove(BAA[1].Length - 1);
    //                    OpioEngine.StartOpioCommand(BAA[0]);
    //                    bool fif = (bool)OpioEngine.GiveOpioObject();
    //                    if (fif)
    //                    {
    //                        OpioEngine.StartOpioCommands(BAA[1]);
    //                    }
    //                }
    //            }
    //            catch { }
    //        }
    //    }
    //    public class OpioWhileInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            try
    //            {
    //                char[] chars = info.ToCharArray();
    //                isInfo = chars[0] == 'w' && chars[1] == 'h' && chars[2] == 'i' && chars[3] == 'l' && chars[4] == 'e';
    //                if (isInfo)
    //                {
    //                    info = info.Remove(0, 6);
    //                    List<string> BAA = info.Split("){").ToList();
    //                    BAA[1] = BAA[1].Remove(BAA[1].Length - 1);
    //                    while (r(BAA[0]))
    //                    {
    //                        OpioEngine.StartOpioCommands(BAA[1]);
    //                    }
    //                }
    //            }
    //            catch { }
    //        }
    //        public bool r(string o)
    //        {
    //            OpioEngine.StartOpioCommand(o);
    //            return (bool)OpioEngine.GiveOpioObject();
    //        }
    //    }
    //    public class OpioImportInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            string[] infoSpilt = info.Split(' ');
    //            isInfo = infoSpilt.First() == "import";
    //            if (isInfo)
    //            {
    //                OpioEngine.StartOpioCommand(infoSpilt[1]);
    //                OpioClass oc = OpioEngine.GiveOpioObject() as OpioClass;
    //                foreach (string objKey in oc.objects.Keys)
    //                {
    //                    OpioEngine.oc["this"].objects.Add(objKey, oc.objects[objKey]);
    //                }
    //                re = null;
    //            }

    //        }
    //    }
    //    public class OpioNullInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            isInfo = info == "@null";
    //            re = null;
    //        }
    //        public class NullC
    //        {
    //            public static NullC s = new NullC();
    //            public override string ToString()
    //            {
    //                return "Null";
    //            }
    //        }
    //    }
    //    public class OpioNoteInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            try
    //            {
    //                var itca = info.ToCharArray();
    //                isInfo = itca[0] == '/' && itca[1] == '/';
    //                bool iI2 = isInfo || itca[0] == '>';
    //                if (iI2)
    //                {
    //                    isInfo = true;
    //                    info = info.Remove(0, 1);
    //                    try
    //                    {
    //                        if (OpioEngine.oc[info].Administrative)
    //                        {
    //                            re = new OpioError { error = "target admin?", code = "0006" };
    //                        }
    //                        else
    //                        {
    //                            OpioEngine.oc["this"] = OpioEngine.oc[info];
    //                        }
    //                    }
    //                    catch
    //                    {

    //                        try
    //                        {
    //                            if (!OpioEngine.oc.ContainsKey("this"))
    //                            {
    //                                OpioEngine.oc.Add("this", OpioEngine.oc[info]);
    //                            }
    //                        }
    //                        catch (Exception e)
    //                        {
    //                            OpioEngine.oo.Add(new OpioError { error = e.Message, code = "1001" });
    //                        }
    //                    }
    //                }
    //            }
    //            catch
    //            {
    //                isInfo = false;
    //            }
    //        }
    //    }
    //    public class OpioStringInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {

    //            isInfo = info.First() == '\"' && info.Last() == '\"';
    //            if (isInfo)
    //            {
    //                info = info.Substring(1, info.Length - 2);
    //                string[] rlin = info.Split("\\\\");

    //                int i = rlin.Length;
    //                string r = "";
    //                if (rlin.Length > 0)
    //                {
    //                    while (i > 0)
    //                    {
    //                        foreach (reeS ree in reeSs)
    //                        {
    //                            rlin[i - 1] = rlin[i - 1].Replace("\\" + ree.a, ree.b);
    //                        }
    //                        if (i != 1)
    //                        {
    //                            r += "\\" + rlin[i - 1];
    //                        }
    //                        else
    //                        {
    //                            r += rlin[i - 1];
    //                        }
    //                        i--;
    //                    }
    //                    re = r;
    //                }
    //                else
    //                {
    //                    re = info;
    //                }
    //            }
    //        }
    //        public static List<reeS> reeSs = new List<reeS>();
    //        public class reeS
    //        {
    //            public string a;
    //            public string b;
    //            public reeS(string A, string B) { a = A; b = B; reeSs.Add(this); }
    //        }
    //    }
    //    public class OpioCharInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {

    //            isInfo = info.First() == '\'' && info.Last() == '\'';
    //            if (isInfo)
    //            {
    //                info = info.Substring(1, info.Length - 2);
    //                re = info.ToCharArray()[0];
    //            }
    //        }
    //    }
    //    public class OpioHomeVarInfo : OpioInfo
    //    {
    //        DataTable table = new DataTable();
    //        public override void ApplyInfo(string info)
    //        {
    //            Dictionary<string, string> vii = new Dictionary<string, string>();
    //            foreach (string oc in OpioEngine.oc.Keys)
    //            {
    //                foreach (string obj in OpioEngine.oc[oc].objects.Keys)
    //                {
    //                    vii.Add(oc + "." + obj, OpioEngine.oc[oc].objects[obj].ToString());
    //                }
    //            }
    //            foreach (string v in vii.Keys)
    //            {
    //                info = info.Replace(v, vii[v]);
    //            }
    //            List<string> numstar = new List<string> { "OR", "AND", "NOT", "=", "<>", "=<", "=>", "<", ">", "!=", "true", "false", ".", " ", "(", ")", "-", "+", "/", "*", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
    //            string ii = info;
    //            foreach (string s in numstar)
    //            {
    //                ii = ii.Replace(s, "");
    //            }
    //            isInfo = ii == "";

    //            if (isInfo)
    //            {
    //                re = table.Compute(info, "");
    //            }
    //        }
    //    }
    //    public class OpioGetObjectInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            isInfo = info.First() == '@';
    //            if (isInfo)
    //            {
    //                info.Remove(1);
    //                string[] ff = info.Split('.');
    //                OpioClass oc = OpioEngine.oc[ff[0]];
    //                re = oc.objects[ff[1]];
    //            }
    //        }
    //    }
    //    public class OpioActionInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            char[] chars = info.ToCharArray();
    //            if (chars.Length == 0) return;
    //            isInfo = chars[0] == '{' && chars[chars.Length - 1] == '}';
    //            if (isInfo)
    //            {
    //                info = info.Remove(0, 1);
    //                info = info.Remove(info.Length - 1, 1);
    //                re = new OpioAction { OpioCommands = info };
    //            }
    //        }
    //    }
    //    public class OpioClassInfo : OpioInfo
    //    {
    //        public override void ApplyInfo(string info)
    //        {
    //            List<string> spiltSpace = info.Split(" ").ToList();
    //            if (spiltSpace.Count == 0) return;
    //            bool CA = spiltSpace[0] == "admin" && spiltSpace[1] == "class";
    //            if (CA)
    //            {
    //                spiltSpace.RemoveAt(0);
    //            }
    //            isInfo = spiltSpace[0] == "class" || CA;
    //            if (isInfo)
    //            {
    //                spiltSpace.RemoveAt(0);
    //                string className = spiltSpace[0];
    //                spiltSpace.RemoveAt(0);
    //                OpioClass oc = new OpioClass { Name = className };
    //                OpioEngine.oc.Add(className, oc);
    //                OpioEngine.oc["this"] = oc;
    //            }
    //        }
    //    }

    //}
    public class OpioSystem
    {
        public static string SystemFilePoint;
        public static OpioClass SystemOC = new OpioClass() { Name = "System" };
        public static void Using()
        {
            OpioEngine.oc.Add("System", SystemOC);
            SystemOC.Add("Start", Start);
        }
        public static void Run()
        {
            OpioEngine.StartOpioCommands(OpioEngine.GiveOpioObject() as string);
        }
        public static void Start()
        {
            Process.Start(OpioEngine.GiveOpioObject(1) as string, OpioEngine.GiveOpioObject(1) as string);
        }
        public static async Task ExtractWithProgress(string zipPath, string extractPath)
        {
            await Task.Run(() =>
            {
                using (ZipArchive archive = ZipFile.OpenRead(zipPath))
                {
                    int totalEntries = archive.Entries.Count;
                    int completedEntries = 0;

                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        // Create the directory if it does not exist.
                        string fullPath = Path.Combine(extractPath, entry.FullName);
                        string directoryName = Path.GetDirectoryName(fullPath);
                        if (!Directory.Exists(directoryName))
                        {
                            Directory.CreateDirectory(directoryName);
                        }

                        // Extract each entry to the specified directory.
                        entry.ExtractToFile(fullPath);

                        // Update the progress.
                        completedEntries++;
                        double progressPercentage = (double)completedEntries / totalEntries * 100;

                        // Draw the progress bar.
                        DrawProgressBar(progressPercentage, 50);
                    }
                }
            });
        }
        public static void DrawProgressBar(double percent, int barSize)
        {
            Console.CursorVisible = false;

            int progress = (int)Math.Round(barSize * percent / 100.0);
            int empty = barSize - progress;

            Console.Write("[");
            Console.Write(new string('#', progress));
            Console.Write(new string('-', empty));
            Console.Write("] ");
            Console.Write("{0,3:##0}%", percent);
            Console.Write("\r");

            if (percent == 100)
            {
                Console.WriteLine();
                Console.CursorVisible = true;
            }
        }
    }
    public class OpioTranslator
    {
        public static Dictionary<string, string> ENT = new Dictionary<string, string>();
        public static string Translate(string text)
        {
            foreach (string s in ENT.Values)
            {
                string pattern = @"(?<=^|\s)" + Regex.Escape(s) + @"(?=\s|$)";
                text = Regex.Replace(text, pattern, m => m.Value.Contains("\"") ? m.Value : ENT.FirstOrDefault(x => x.Value == s).Key);

                //text = text.Replace(s, ENT.FirstOrDefault(x => x.Value == s).Key);
            }
            return text;
        }
        public static void ApplyARE()
        {
            ENT = new Dictionary<string, string>()
            {
                { "public", "عام"},
                { "private", "خاص"},
                { "System", "النظام"},
                { "rOP", "تشعيل_كود"},
                { "Console", "وحدة_التحكم"},
                { "Print", "طباعة"},
                { "Line", "خط"},
                { "Clear", "مسح"},
                { "UserWrite", "كتابة_المستخدم"},
                { "BackgroundColor", "لون الخلفية"},
                { "TextColor", "لون_النص"},
                { "Magenta", "ارجواني"},
                { "Red", "احمر"},
                { "Gray", "رمادي"},
                { "Yellow", "اصفر"},
                { "Green", "اخضر"},
                { "Black", "اسود"},
                { "Blue", "ازرق"},
                { "Cyan", "سمائي"},
                { "DarkBlue", "ازرق_غامق"},
                { "DarkGray", "رمادي_غامق"},
                { "DarkGreen", "اخضر_غامق"},
                { "DarkMagenta", "ارجواني_غامق"},
                { "DarkRed", "احمر_غامق"},
                { "DarkYellow", "اصفر_غامق"},
                { "class", "الفصل"},
                { "true", "صحيح"},
                { "false", "خطأ"},
                { "OR", "او"},
                { "AND", "و"},
                { "NOT", "ليس"},
                { "this", "هذا"},
                { "while", "حتى"},
                { "if", "اذا"},
                { "import", "استيراد"},
                { "@null", "@دون"},

            };
        }
    }
}

namespace opio.packages
{
    public class OpioMath
    {
        public static void Using()
        {
            OpioEngine.oc.Add("Math", new OpioClass { Name = "Math" });
            OpioClass oc = OpioEngine.oc["Math"] as OpioClass;

            oc.Add("plus", plus);
            oc.Add("minus", minus);
            oc.Add("multiply", multiply);
            oc.Add("divide", divide);
        }
        public static void plus()
        {
            var v1 = OpioEngine.GiveOpioObject(2);
            var v2 = OpioEngine.GiveOpioObject(1);
            if (v1 is float && v2 is float)
            {
                OpioEngine.oo.Add((float)v1 + (float)v2);
            }
            else if (v1 is float && v2 is int)
            {
                OpioEngine.oo.Add((float)v1 + (int)v2);
            }
            else if (v1 is int && v2 is float)
            {
                OpioEngine.oo.Add((int)v1 + (float)v2);
            }
            else
            {
                OpioEngine.oo.Add((int)v1 + (int)v2);
            }
        }
        public static void minus()
        {
            var v1 = OpioEngine.GiveOpioObject(2);
            var v2 = OpioEngine.GiveOpioObject(1);
            if (v1 is float && v2 is float)
            {
                OpioEngine.oo.Add((float)v1 - (float)v2);
            }
            else if (v1 is float && v2 is int)
            {
                OpioEngine.oo.Add((float)v1 - (int)v2);
            }
            else if (v1 is int && v2 is float)
            {
                OpioEngine.oo.Add((int)v1 - (float)v2);
            }
            else
            {
                OpioEngine.oo.Add((int)v1 - (int)v2);
            }
        }
        public static void multiply()
        {
            var v1 = OpioEngine.GiveOpioObject(2);
            var v2 = OpioEngine.GiveOpioObject(1);
            if (v1 is float && v2 is float)
            {
                OpioEngine.oo.Add((float)v1 * (float)v2);
            }
            else if (v1 is float && v2 is int)
            {
                OpioEngine.oo.Add((float)v1 * (int)v2);
            }
            else if (v1 is int && v2 is float)
            {
                OpioEngine.oo.Add((int)v1 * (float)v2);
            }
            else
            {
                OpioEngine.oo.Add((int)v1 * (int)v2);
            }
        }
        public static void divide()
        {
            var v1 = OpioEngine.GiveOpioObject(2);
            var v2 = OpioEngine.GiveOpioObject(1);
            if (v1 is float && v2 is float)
            {
                OpioEngine.oo.Add((float)v1 / (float)v2);
            }
            else if (v1 is float && v2 is int)
            {
                OpioEngine.oo.Add((float)v1 / (int)v2);
            }
            else if (v1 is int && v2 is float)
            {
                OpioEngine.oo.Add((int)v1 / (float)v2);
            }
            else
            {
                OpioEngine.oo.Add((int)v1 / (int)v2);
            }
        }
    }
    public class OpioConsole
    {
        public static void Using()
        {
            OpioEngine.oc.Add("Console", new OpioClass { Name = "Console" });
            OpioClass opioClass = OpioEngine.oc["Console"] as OpioClass;
            opioClass.Add("Print", new OpioSystemAction(Print));
            opioClass.Add("Line", Line);
            opioClass.Add("Clear", Clear);
            opioClass.Add("UserWrite", UserWrite);
            opioClass.Add("BackgroundColor", BackgroundColor);
            opioClass.Add("TextColor", TextColor);

            opioClass.Add("Magenta", ConsoleColor.Magenta);
            opioClass.Add("Red", ConsoleColor.Red);
            opioClass.Add("Gray", ConsoleColor.Gray);
            opioClass.Add("Yellow", ConsoleColor.Yellow);
            opioClass.Add("Green", ConsoleColor.Green);
            opioClass.Add("Black", ConsoleColor.Black);
            opioClass.Add("Blue", ConsoleColor.Blue);
            opioClass.Add("Cyan", ConsoleColor.Cyan);
            opioClass.Add("DarkBlue", ConsoleColor.DarkBlue);
            opioClass.Add("DarkCyan", ConsoleColor.DarkCyan);
            opioClass.Add("DarkGray", ConsoleColor.DarkGray);
            opioClass.Add("DarkGreen", ConsoleColor.DarkGreen);
            opioClass.Add("DarkMagenta", ConsoleColor.DarkMagenta);
            opioClass.Add("DarkRed", ConsoleColor.DarkRed);
            opioClass.Add("DarkYellow", ConsoleColor.DarkYellow);
        }
        public static void Print(object[] objects)
        {
            Console.Write(objects[0]);
        }
        public static void Line()
        {
            Console.WriteLine("");
        }
        public static void Clear()
        {
            Console.Clear();
        }
        public static void UserWrite()
        {
            OpioEngine.oo.Add(Console.ReadLine());
        }
        public static void UserKey()
        {
            OpioEngine.oo.Add(Console.ReadKey());
        }
        public static void TextColor()
        {
            Console.ForegroundColor = (ConsoleColor)OpioEngine.GiveOpioObject();
        }
        public static void BackgroundColor()
        {
            Console.BackgroundColor = (ConsoleColor)OpioEngine.GiveOpioObject();
        }
    }
    public class OpioFiles
    {
        public static void Using()
        {
            OpioEngine.oc.Add("Files", new OpioClass { Name = "Files" });
            OpioClass opioClass = OpioEngine.oc["Files"] as OpioClass;
            opioClass.Add("RunApp", RunApp);
            opioClass.Add("NewFile", NewFile);
            opioClass.Add("SetFile", SetFile);
            opioClass.Add("GetFile", GetFile);
            opioClass.Add("NewDirectory", NewDirectory);
        }

        public static void RunApp()
        {
            Process.Start((string)OpioEngine.GiveOpioObject());
        }
        public static void NewFile()
        {
            string path = OpioEngine.GiveOpioObject() as string;
            using (FileStream fs = File.Create(path)) ;
        }
        public static void SetFile()
        {
            string path = OpioEngine.GiveOpioObject(2) as string;
            string set = OpioEngine.GiveOpioObject(1) as string;
            if (File.Exists(path))
            {
                File.WriteAllText(path, set);
            }
        }
        public static void GetFile()
        {
            string path = OpioEngine.GiveOpioObject() as string;
            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);
                OpioEngine.oo.Add(text);
            }
        }
        public static void NewDirectory()
        {
            string dir = OpioEngine.GiveOpioObject() as string;
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
        }

    }
}
