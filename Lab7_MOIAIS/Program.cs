using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab7_MOIAIS
{
    public enum Institute { ФЭИ, ИФК, ИМиКН, ИнЗем, ИФиЖ, ИПиП, ИГиП, СоцГум };
    // названия институтов можно посмотреть на сайте ТюмГУ
    struct Student : IComparable
    {
        public string fam;
        public Institute inst;
        public string dat_r;
        public double sr_ball;
        public double vozrast
        {
            get
            {
                return (DateTime.Now.Year - Convert.ToDateTime(dat_r).Year);
                // возраст лучше вычислить с учётом месяца и дня
            }
        }
        public int CompareTo(Object obj)
        {
            Student st = (Student)obj;
            if (vozrast > st.vozrast) return 1;
            else
            { if (vozrast < st.vozrast) return -1; else return 0; }
        }
        public override string ToString()
        {
            return $"Имя: {fam}, \n" +
                   $"Институт: {Enum.GetName(typeof(Institute), inst)}, \n" +
                   $"Средний балл: {sr_ball}, \n" +
                   $"Возраст: {vozrast}\n"; 
        }
    }

    // нужен для InstTop и NameChastota
    class Data
    {
        public Data(int v, string n)
        {
            value = v;
            name = n;
        }
        public int value;
        public string name;
    }
    // Нужен для расчета средних значений (наследуется от класса выше)
    class SredBall: Data
    {
        public double ball;
        public double age;

        public SredBall(double ball, double age, int c, string n) : base( c, n )
        {
            this.ball = ball;
            this.age = age;
        }
    }
    class Program
    {
        //список студентов (всех)
        private static Student[] students;
        //генератор имен
        private static string randName()
        {
            string[] maleNames = new string[] { "aaron", "abdul", "abe", "abel", "abraham", "adam", "adan", "adolfo", "adolph", "adrian" };
            string[] femaleNames = new string[] { "abby", "abigail", "adele", "adrian" };
            string[] lastNames = new string[] { "abbott", "acosta", "adams", "adkins", "aguilar" };

            Random rand = new ();
            string res = "";
            if (rand.Next(1, 2) == 1)
            {
                res = maleNames[rand.Next(0, maleNames.Length - 1)];
            }
            else
            {
                res = femaleNames[rand.Next(0, femaleNames.Length - 1)];
            }
            res += $" {lastNames[rand.Next(0, lastNames.Length)]}";
            return res;
        }
        //генератор исходного файла
        static void Gen(string path)
        {
            Random random = new Random();
            StreamWriter file = new(path);
            for (int i = 0; i < Enum.GetNames(typeof(Institute)).Length*random.Next(3,10); i++)
            {
                int c = random.Next(0, 10);
                file.WriteLine($"{randName()};" +
                    $"{Enum.GetNames(typeof(Institute))[random.Next(Enum.GetNames(typeof(Institute)).Length)]};" +
                    $"{random.Next(1,28)}/{random.Next(1,12)}/{random.Next(1990,2004)};" +
                    $"{Math.Round(c<10?c+random.NextDouble():c,2)}");
            }
            file.Close();
        }
        //универсальное меню
        //(принимает на вход список команд, при выборе элемента возвращает индекс данной команды)
        static int GetMenu(string[] F, string title)
        {
            Console.Title = title;
            Console.Clear();
            int v = -1, p = 0;
            for (int i = 0; i < F.Length; i++) Console.WriteLine($"{i + 1}) {F[i]}");
            bool ff = true;
            while (ff)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                if (!(v == 0 && p == 0))
                {
                    Console.SetCursorPosition(3, p);
                    Console.WriteLine(F[p]);
                }
                Console.SetCursorPosition(0, F.Length);
                ConsoleKeyInfo k = Console.ReadKey();
                if (v == -1) p = 0; else p = v;
                if (k.Key == ConsoleKey.Enter) ff = false;
                else
                {
                    if (Char.GetNumericValue(k.KeyChar) > 0 &&
                                           Char.GetNumericValue(k.KeyChar) <= F.Length)
                    {
                        v = Convert.ToInt32(Char.GetNumericValue(k.KeyChar)) - 1;
                        Console.SetCursorPosition(3, v);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(F[v]);
                    }
                    else v = -1;
                }
            }
            return v;
        }
        //сохранение в файл
        static void SaveFile()
        {
            Array.Sort(students);
            StreamWriter f1 = new("baza1.txt");
            string[] F = Enum.GetNames(typeof(Institute));
            for (int ii = 0; ii < F.Length; ii++)
            {
                f1.WriteLine("\tИнститут: " + F[ii]);
                int q = 0;
                for (int i = 0; i < students.Length; i++)
                {
                    if (students[i].inst.ToString() == F[ii])
                    {
                        q++;
                        f1.WriteLine("║ {0,2} ║ {1,15} ║ {2,4} ║ {3,4} ║", q, students[i].fam, students[i].sr_ball, students[i].vozrast);
                    }
                }
                if (q == 0)
                    f1.WriteLine("\tДанных нет.");
                f1.WriteLine();
            }
            f1.Close();
            Console.WriteLine("Файл baza1.txt сожранен");
        }
        //средний балл
        static void SredBal()
        {
            List<SredBall> sr = new();
            foreach (var i in Enum.GetNames(typeof(Institute)))
                sr.Add(new(0, 0, 0, i));
            foreach (var j in sr)
            {
                foreach (var i in students)
                    if (Enum.GetName(typeof(Institute), i.inst) == j.name)
                    {
                        j.value++;
                        j.age += i.vozrast;
                        j.ball += i.sr_ball;
                    }
                Console.WriteLine(j.name);
                Console.WriteLine($"Средний возраст: {Math.Round(j.age/j.value,2)}");
                Console.WriteLine($"Средний балл:    {Math.Round(j.ball/j.value, 2)}");
                Console.WriteLine();
            }

        }
        //топ 10 самых старших студентов
        static void Age10()
        {
            Array.Sort(students);
            Array.Reverse(students);
            for (int i = 0; i < (students.Length < 10 ? students.Length : 10); i++)
                Console.WriteLine(students[i]);
        }
        //частота имен
        static void NameChastota()
        {
            Dictionary<string, int> dict = new();
            foreach (var student in students)
                if (dict.ContainsKey(student.fam))
                    dict[student.fam]++;
                else dict.Add(student.fam, 1);

            List<Data> f = new();
            foreach (var i in dict)
            {
                f.Add(new(i.Value, i.Key));
            }
            Data temp;
            for (int i = 0; i < f.Count; i++)
            {
                for (int j = i + 1; j < f.Count; j++)
                {
                    if (f[i].value > f[j].value)
                    {
                        temp = f[i];
                        f[i] = f[j];
                        f[j] = temp;
                    }
                }
            }
            foreach (var i in f)
                Console.WriteLine($"{i.name} - {i.value}");

        }
        //институты по количеству студентов
        static void InstTop()
        {
            Dictionary<string, int> e = new();
            foreach (var i in Enum.GetNames(typeof(Institute)))
                e.Add(i, 0);
            foreach (var j in e)
                foreach (var i in students)
                    if (Enum.GetName(typeof(Institute), i.inst) == j.Key)
                        e[j.Key]++;

            List<Data> f = new();
            foreach (var i in e)
            {
                f.Add(new (i.Value, i.Key));
            }
            Data temp;
            for (int i = 0; i < f.Count; i++)
            {
                for (int j = i + 1; j < f.Count; j++)
                {
                    if (f[i].value > f[j].value)
                    {
                        temp = f[i];
                        f[i] = f[j];
                        f[j] = temp;
                    }
                }
            }
            foreach (var i in f)
                Console.WriteLine($"{i.name} - {i.value}");
        }
        //вывод информации о студентах по институту
        static void PrintInfo()
        {
            string[] F = Enum.GetNames(typeof(Institute));
            int v = GetMenu(F, "Студенты");
            if (v != -1)
            {
                Console.Clear();
                Console.WriteLine("\tИнститут: " + F[v]);
                // шапка таблицы…..
                int j = 0;
                for (int i = 0; i < students.Length; i++)
                {
                    if (Enum.GetName(typeof(Institute), students[i].inst) == F[v])
                    {
                        j++;
                        Console.WriteLine("║ {0,2} ║ {1,15} ║ {2,11} ║ {3,4} ║", j, students[i].fam, students[i].dat_r, students[i].sr_ball);
                    }
                }
            }
            else
                Console.WriteLine(" Институт не выбран ");

        }
        static void Main(string[] args)
        {
            string path = "baza.dat";
            Gen(path); // Генерация исходного файла
            //считывание данных из файла
            StreamReader f = new StreamReader(path);
            string s = f.ReadLine(); int j = 0;
            while (s != null)
            {
                s = f.ReadLine();
                j++;
            }
            f.Close();
            students = new Student[j];
            string[] d = new string[4];
            f = new StreamReader(path);
            s = f.ReadLine(); j = 0;
            while (s != null)
            {
                d = s.Split(';');
                students[j].fam = d[0];
                students[j].inst = (Institute)Enum.Parse(typeof(Institute), d[1]);
                students[j].dat_r = d[2];
                students[j].sr_ball = Convert.ToDouble(d[3]);
                s = f.ReadLine();
                j++;
            }
            f.Close();
            //список команд для меню
            string[] M = new string[]
            {
            "Вывести на экран информацию о студентах",
            "Сохранить в файл",
            "Получить список институтов в порядке численности студентов",
            "По каждому институту получить средний балл и средний возраст студентов",
            "Вывести на экран 10 самых взрослых студентов",
            "Вывести на экран имена в порядке уменьшения их численности",
            "Выйти"
            };
            while (true)
            {
                switch (GetMenu(M, "Меню") + 1)
                {
                    case 7:
                        {
                            Console.Clear();
                            Environment.Exit(0);
                            break;
                        }
                    case 1:
                        {
                            Console.Clear();
                            PrintInfo();
                            break;
                        }
                    case 2:
                        {
                            Console.Clear();
                            SaveFile();
                            break;
                        }
                    case 3:
                        {
                            Console.Clear();
                            InstTop();
                            break;
                        }
                    case 4:
                        {
                            Console.Clear();
                            SredBal();
                            break;
                        }
                    case 5:
                        {
                            Console.Clear();
                            Age10();
                            break;
                        }
                    case 6:
                        {
                            Console.Clear();
                            NameChastota();
                            break;
                        }
                    default:
                        {
                            Console.Clear();
                            Console.WriteLine("Неверное действие");
                            break;
                        }
                }
                Console.WriteLine("Для возврата в меню нажмите любую клавишу...");
                Console.ReadKey();
            }
        }
    }
}
