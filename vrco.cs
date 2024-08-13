
/**************************************************************************/
/*  vrco.cs                                                               */
/*                         This code is part of:                          */
/*                             VRCO: Project                              */
/*                  "Virtual reality command operator"                    */
/*                      vrco 1.0 from MantiqStudio                        */
/**************************************************************************/

using System.Reflection;
using System.Reflection.Emit;

namespace vrco
{
    public class Engine : Dictionary<string, Action<string, Engine>>
    {
        public Type TargetType = null;
        public Class TargetClass = null;
        public Dictionary<string, Class> StaticClasses = new Dictionary<string, Class>();
        public Dictionary<string, Type> Types = new Dictionary<string, Type>();
        public List<Object> Objects = new List<Object>();

        public Object Target<T>(int i = 1)
        {
            Object obj = Objects[Objects.Count - i];
            Objects.RemoveAt(Objects.Count - i);
            return obj;
        }

        public Object Target(int i = 1) => Target<Object>(i);

        public void Drop(Object obj)
        {
            Objects.Add(obj);
        }

        public void Single(string code)
        {
            string act = code.Split('>')[0];
            string mod = code.Remove(0, act.Length + 1);
            this[act].Invoke(mod, this);
        }

        private bool stopping = false;
        public void Stop() => stopping = true;
        public void Start()
        {
            stopping = false;
            while (Commands.Count > 0 && !stopping)
            {
                Single(Commands[0]);
                if (Commands.Count != 0) Commands.RemoveAt(0);
            }
        }

        public List<string> Commands = new List<string>();
        public List<string> WaitCommands = new List<string>();
        public async void Multi(string codes)
        {
            if (Commands.Count > 0) WaitCommands = Commands;
            Commands = codes.Split('\n').ToList();
            Start();
            Commands = WaitCommands;
        }
        public void FixedMulti(string codes) => Multi(codes.Replace("\n\n", "\n"));
    }

    public class Type : Object
    {
        public string @key;
        public Dictionary<string, Object> Public = new Dictionary<string, Object>();
        public Dictionary<string, Object> Private = new Dictionary<string, Object>();
    }

    public class Class : Type
    {
        public string @key;
        public Type @type;
        public Dictionary<string, Object> Public = new Dictionary<string, Object>();
        public Dictionary<string, Object> Private = new Dictionary<string, Object>();

        public Object ppget(string key)
        {
            if (Private.ContainsKey(key)) return Private[key];
            else return Public[key];
        }

        public Class() { }
        public Class(string key, Type type, Dictionary<string, Object> @public, Dictionary<string, Object> @private)
        {
            this.key = key;
            this.type = type;
            Public = @public;
            Private = @private;
        }
        public Class(string name, Type type)
        {
            key = name;
            Public = type.Public.ToDictionary();
            Private = type.Private.ToDictionary();
        }
    }

    public class Object
    {
        public object value;
        public Object() { }
        public Object(object value)
        {
            this.value = value;
        }
        public override string ToString()
        {
            return value.ToString();
        }
    }

    public class oAction : Object
    {
        public string[] types;
        public virtual async void Invoke() { }
    }

    public class OebCodeAction : oAction
    {
        public string code;
        public Engine engine;

        public override void Invoke()
        {
            engine.Multi(code);
            actvar.Reimport(engine);
        }
    }

    public class SystemAction : oAction
    {
        public override void Invoke()
        {
            ((Action)value).Invoke();
        }

        public SystemAction(Action action)
        {
            value = action;
        }
    }

    public class KernelAction : oAction
    {
        public Engine engine;
        public string library;
        public string path = "IN";
        public string key = "IN";
        public override void Invoke()
        {
            engine.Multi($"LOAD>{library}:{path}-{key}");
        }
    }


    public class actvar
    {
        public static void Install(Engine engine)
        {
            Class @class = new Class(":ACTVAR", engine.Types[":INADDON"]);
            engine.StaticClasses.Add(":ACTVAR", @class);
        }
        public static void Reimport(Engine engine)
        {
            engine.StaticClasses[":ACTVAR"].Public.Clear();
        }
    }
    public class Kernel
    {
        public static void LoadPackage(string assemblyText)
        {
            System.Type[] Types = Assembly.Load(assemblyText).GetTypes();
            foreach (System.Type type in Types) LoadType(type);
        }
        public static void LoadType(System.Type type)
        {
            Type t = new Type();
            
        }
    }

    namespace Core
    {
        public class CoreObjects
        {
            public static void Install(Engine engine)
            {
                engine.Add("V:NULL", GNULL);
                engine.Add("T:AFTER", AfterAction);
                engine.Add("T:BREAK", BreakAction);

                engine.Add("V:[8]I", Integer8Action);
                engine.Add("V:[16]I", Integer16Action);
                engine.Add("V:[32]I", Integer32Action);
                engine.Add("V:[64]I", Integer64Action);
                engine.Add("V:[A]I", IntegerAutoAction);

                engine.Add("V:[B8]I", IntegerB8Action);
                engine.Add("V:[B16]I", IntegerB16Action);
                engine.Add("V:[B32]I", IntegerB32Action);
                engine.Add("V:[B64]I", IntegerB64Action);
                engine.Add("V:[BA]I", IntegerBAutoAction);

                engine.Add("V:[4]D", Decimal4Action);
                engine.Add("V:[8]D", Decimal8Action);
                engine.Add("V:[16]D", Decimal16Action);


                engine.Add("S:[S]TYPE", StartTypeAction);
                engine.Add("S:[E]TYPE", EndTypeAction);

                engine.Add("S:[U]OBJECT", uObjectAction);
                engine.Add("S:[R]OBJECT", rObjectAction);
                engine.Add("S:[Q]OBJECT", scuObjectAction);

                engine.Add("S:[L]GET", uGetAction);
                engine.Add("S:[P]GET", upGetAction);
                engine.Add("S:[O]GET", StaticGetAction);

                engine.Add("S:NEW", NewAction);
                engine.Add("A:PRINT", PrintAction);
                engine.Add("V:STRING", StringAction);
                engine.Add("V:BOOL", BoolAction);
                engine.Add("V:ACTION", ActionAction);
                engine.Add("T:INVOKE", InvokeAction);
                engine.Add("T:IF", IfAction);
                engine.Add("V:[I]BOOL", IBoolAction);

                //INADDON installer
                engine.Types.Add(":INADDON", new Type { key = ":INADDON", value = "::ID0"});
            }
            //Objects Action

            public static Object NULL = new Object("-null");
            public static void GNULL(string mode, Engine engine)
            {
                engine.Drop(NULL);
            }

            public static List<string> CommandsAfter;
            public static async void AfterAction(string mode, Engine engine)
            {
                engine.Stop();
                CommandsAfter = engine.Commands.ToList();
                engine.Commands.Clear();
                ((oAction)engine.Target()).Invoke();
            }
            public static void StartAfter(Engine engine)
            {
                engine.Commands = CommandsAfter;
                engine.Start();
            }

            //small task
            public static void IBoolAction(string mode, Engine engine)
            {
                bool boolen = (bool)engine.Target().value;
                if (boolen) engine.Drop(False);
                else engine.Drop(True);
            }

            //numbers
            //Integer
            public static void Integer8Action(string mode, Engine engine) => engine.Drop(new Object { value = sbyte.Parse(mode) });
            public static void Integer16Action(string mode, Engine engine) => engine.Drop(new Object { value = short.Parse(mode) });
            public static void Integer32Action(string mode, Engine engine) => engine.Drop(new Object { value = int.Parse(mode) });
            public static void Integer64Action(string mode, Engine engine) => engine.Drop(new Object { value = long.Parse(mode) });
            public static void IntegerAutoAction(string mode, Engine engine) => engine.Drop(new Object { value = nint.Parse(mode) });
            //basic integer
            public static void IntegerB8Action(string mode, Engine engine) => engine.Drop(new Object { value = byte.Parse(mode) });
            public static void IntegerB16Action(string mode, Engine engine) => engine.Drop(new Object { value = ushort.Parse(mode) });
            public static void IntegerB32Action(string mode, Engine engine) => engine.Drop(new Object { value = uint.Parse(mode) });
            public static void IntegerB64Action(string mode, Engine engine) => engine.Drop(new Object { value = ulong.Parse(mode) });
            public static void IntegerBAutoAction(string mode, Engine engine) => engine.Drop(new Object { value = nuint.Parse(mode) });
            //decimal
            public static void Decimal4Action(string mode, Engine engine) => engine.Drop(new Object { value = float.Parse(mode) });
            public static void Decimal8Action(string mode, Engine engine) => engine.Drop(new Object { value = double.Parse(mode) });
            public static void Decimal16Action(string mode, Engine engine) => engine.Drop(new Object { value = decimal.Parse(mode) });


            private static Object True = new Object { value = true };
            private static Object False = new Object { value = false };
            public static void IfAction(string mode, Engine engine)
            {
                bool boolen = (bool)engine.Target().value;
                string action = (string)engine.Target().value;
                if (boolen) engine.Multi(action);
            }
            public static void BoolAction(string mode, Engine engine)
            {
                if (mode == "") engine.Drop(True);
                else engine.Drop(False);
            }
            public static void uObjectAction(string mode, Engine engine)
            {
                engine.TargetType.Public.Add(mode, engine.Target());
            }

            public static void rObjectAction(string mode, Engine engine)
            {
                engine.TargetType.Private.Add(mode, engine.Target());
            }

            public static void scuObjectAction(string mode, Engine engine)
            {
                string CLASS = mode.Split('|')[0];
                string KEY = mode.Split('|')[1];
                engine.StaticClasses[CLASS].Public.Add(KEY, engine.Target());
            }

            public static void BreakAction(string mode, Engine engine)
            {
                engine.Stop();
            }

            public static void StartTypeAction(string mode, Engine engine)
            {
                Type type = new Type { key = mode, value = mode };
                engine.TargetType = type;
                engine.Types.Add(mode, type);
            }

            public static void EndTypeAction(string mode, Engine engine)
            {
                engine.TargetType = null;
            }

            public static void NewAction(string mode, Engine engine)
            {
                engine.TargetClass = new Class(mode.Split('|')[1], engine.Types[mode.Split('|')[0]]);
                engine.Drop(engine.TargetType);
            }

            public static void ActionAction(string mode, Engine engine)
            {
                engine.Drop(new OebCodeAction() { engine = engine, code = (engine.Target()).ToString(), types = mode.Split("|") });
            }

            public static void upGetAction(string mode, Engine engine)
            {
                string[] ModeSplit = mode.Split('/');
                string Root = ModeSplit[0];
                List<string> Path = ModeSplit.ToList();
                string Key = ModeSplit[ModeSplit.Length - 1];

                Path.RemoveAt(0);
                Path.RemoveAt(Path.Count - 1);
                Class Target = engine.TargetClass;
                foreach (string path in Path) Target = (Class)Target.ppget(path);
                engine.Drop(Target.ppget(Key));
            }

            public static void uGetAction(string mode, Engine engine)
            {
                engine.Drop(engine.TargetClass.ppget(mode));
            }

            public static void StaticGetAction(string mode, Engine engine)
            {
                string[] ModeSplit = mode.Split('/');
                string Root = ModeSplit[0];
                List<string> Path = ModeSplit.ToList();
                string Key = ModeSplit[ModeSplit.Length - 1];

                Path.RemoveAt(0);
                Path.RemoveAt(Path.Count - 1);
                Class Target = engine.StaticClasses[Root];
                foreach (string path in Path) Target = (Class)Target.Public[path];
                engine.Drop(Target.Public[Key]);
            }

            public static void PrintAction(string mode, Engine engine)
            {
                Console.WriteLine(engine.Target());
            }

            public static void InvokeAction(string mode, Engine engine)
            {
                oAction act = (oAction)engine.Target();
                act.Invoke();
            }
            public static void StringAction(string mode, Engine engine)
            {
                string[] modes = mode.Split("\\\\");
                int i = 0;
                while (i < modes.Length)
                {
                    modes[i] = modes[i].Replace("\\", "\n");
                    i++;
                }
                mode = "";
                foreach (var m in modes)
                {
                    mode += "\\" + m;
                }
                mode = mode.Remove(0, 1);
                engine.Drop(new Object(mode));
            }
        }
        public class KernelObjects
        {
            public static void Install(Engine engine)
            {
                engine.Add("LOAD", Load);
            }

            public static void Load(string mode, Engine engine)
            {
                string[] modes = mode.Split(':');

                Assembly library = Assembly.LoadFrom(modes[0].Replace("[@S]", Environment.SystemDirectory));
                MethodInfo method = library.GetType(modes[1]).GetMethod(modes[2]);

                List<object> parameters = new List<object>();
                int parametersNumber = method.GetParameters().Length;
                while(parametersNumber > 0)
                {
                    parametersNumber--;
                    parameters.Add(engine.Target().value);
                }

                object result = method.Invoke(null, parameters.ToArray());
                engine.Drop(new Object(result));
            }
        }
    }
}
