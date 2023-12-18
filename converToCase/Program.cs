using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace converToCase
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("\nУкажите путь к директории и нужный стиль --from=snake/camel --to=camel/snake");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("\tДополнительная информация о converTo.exe: ");
                Console.ResetColor();

                Console.Write("\n\tвведите ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("-h");
                Console.ResetColor();

                Console.Write(", для вызова блока help с инструкцией по использованию; ");
                Console.Write("\n\tвведите ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("-v");
                Console.ResetColor();

                Console.Write(", для просмотра версии программы; ");
                Console.Write("\n\tвведите ");

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("-c");
                Console.ResetColor();

                Console.WriteLine(", для досрочного завершения работы программы;");

                string input = Console.ReadLine();

                if (input.ToLower() == "-h")
                {
                    DisplayHelp();
                    continue;
                }
                else if (input.ToLower() == "-v")
                {
                    DisplayVersion();
                    continue;
                }
                else if (input.ToLower() == "-c" || input.ToLower() == "-с")
                {
                    return;
                }

                string[] inputArgs = input.Split(' ');
                if (inputArgs.Length < 3)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка ввода");
                    Console.ResetColor();
                    continue;
                }

                string path = inputArgs[0];
                string sourceCase = GetCase(inputArgs, 1);
                string targetCase = GetCase(inputArgs, 2);

                if (String.IsNullOrEmpty(sourceCase) || String.IsNullOrEmpty(targetCase) ||
                    (sourceCase != "snake" && sourceCase != "camel") || (targetCase != "snake" && targetCase != "camel"))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ошибка ввода - неверный стиль. Допустимые значения: snake, camel");
                    Console.ResetColor();
                    continue;
                }

                try
                {
                    ProcessFiles(path, sourceCase, targetCase);

                    string continueResponse;
                    bool validResponse = false;
                    do
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGreen;
                        Console.Write("\nПрограмма завершила выполнение. ");
                        Console.ResetColor();

                        Console.Write("Желаете продолжить? ");

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("(да/нет) ");
                        Console.ResetColor();

                        Console.Write("- ");

                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("английский язык ввода можно не переключать");
                        Console.ResetColor();

                        continueResponse = Console.ReadLine().ToLower();
                        switch (continueResponse)
                        {
                            case "да":
                                validResponse = true;
                                break;
                            case "lf":
                                continueResponse = "да";
                                validResponse = true;
                                break;
                            case "нет":
                                validResponse = true;
                                break;
                            case "ytn":
                                continueResponse = "нет";
                                validResponse = true;
                                break;
                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Некорректный ввод. Введите 'да' или 'нет'.");
                                Console.ResetColor();
                                break;
                        }
                    } while (!validResponse);

                    if (continueResponse == "нет")
                    {
                        break;
                    }
                }
                catch (DirectoryNotFoundException)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("В пути к директории ошибка или такой директории нет по данному пути");
                    Console.ResetColor();
                }
            }
        }
        static string GetCase(string[] inputArgs, int index)
        {
            return inputArgs.Length > index ? (inputArgs[index].Split('=').Length > 1 ? inputArgs[index].Split('=')[1] : "") : "";
        }
        static bool ProcessFiles(string path, string sourceCase, string targetCase)
        {
            string[] files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);
            bool filesConverted = false;

            foreach (string file in files)
            {
                string content = File.ReadAllText(file);
                string pattern = GetPattern(sourceCase, targetCase);

                string modifiedContent = Regex.Replace(content, pattern, match =>
                {
                    string word = match.Value;
                    if (sourceCase == "snake" && targetCase == "camel")
                    {
                        return ToCamelCase(word);
                    }
                    else if (sourceCase == "camel" && targetCase == "snake")
                    {
                        return ToSnakeCase(word);
                    }
                    else
                    {
                        return word;
                    }
                });

                if (content != modifiedContent)
                {
                    File.WriteAllText(file, modifiedContent);
                    filesConverted = true;
                }
            }

            if (!filesConverted)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Файлы уже в указанном стиле");
                Console.ResetColor();
            }

            return filesConverted;
        }

        static string GetPattern(string sourceCase, string targetCase)
        {
            if (sourceCase == "snake" && targetCase == "camel")
            {
                return @"(\b\w+_\w+\b)";
            }
            else if (sourceCase == "camel" && targetCase == "snake")
            {
                return @"(\b\w+\b)";
            }
            else
            {
                return "";
            }
        }

        static string ToCamelCase(string input)
        {
            string[] words = input.Split('_');
            for (int i = 1; i < words.Length; i++)
            {
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1);
            }
            return string.Join("", words);
        }

        static string ToSnakeCase(string input)
        {
            return Regex.Replace(input, "([a-z0-9])([A-Z])", "$1_$2").ToLower();
        }

        static void DisplayHelp()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("\tПодсказка по использованию приложения:");
            Console.ResetColor();
            Console.WriteLine("\tИспользуйте формат: путь_к_директории --from=snake или camel --to=camel или snake\n");
            Console.WriteLine("\tПример на Windows: C:\\Users\\nameUser\\folder\\myProject --from=snake --to=camel\n");
            Console.WriteLine("\tПример на Linux: /home/user/MyProject --from=snake --to=camel\n");
            Console.WriteLine("\tПробелов в названии папок быть не должно 'my Project' - нет, обязательно 'myProject'!\n");
        }
        static void DisplayVersion()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.Write("\tВерсия приложения: ");
            Console.ResetColor();
            Console.Write("3.0;\n");
        }
    }
}
