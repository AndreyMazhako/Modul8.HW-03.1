using System;
using System.IO;


// Для запуска программы, необходимо написать команду в командной строке,
// первоначально перейдя в папку, в которой находится приложение.
// Команда должна содержать имя программы и полный путь до очищаемой папки.
// Пример: Modul8.HW-03.1.exe "C:\Temp\Test_folder".

public class Program
{
    public static void Main(string[] args)
    {
        // Проверяем, был ли передан аргумент 
        if (args.Length != 1)
        {
            Console.WriteLine("Необходимо указать путь к папке в качестве аргумента.");
            return; // Выходим из программы, если аргумент не передан
        }

        string folderPath = args[0]; // Получаем путь к папке из аргумента

        // Проверяем, существует ли папка
        if (!Directory.Exists(folderPath))
        {
            Console.WriteLine($"Папка '{folderPath}' не существует.");
            return;
        }

        // Проверяем права доступа к папке
        try
        {
            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            if (!dirInfo.Exists)
            {
                Console.WriteLine($"Папка '{folderPath}' не существует.");
                return;
            }

            if (dirInfo.Attributes.HasFlag(FileAttributes.ReadOnly))
            {
                Console.WriteLine($"Нет прав доступа к '{folderPath}' для записи.");
                return;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка доступа к папке: {ex.Message}");
            return;
        }

        try
        {
            // Удаляем файлы и папки, не использовавшиеся более 30 минут
            CleanFolderRecursive(folderPath, TimeSpan.FromMinutes(30));

            Console.WriteLine($"Папка '{folderPath}' успешно очищена.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при очистке папки: {ex.Message}");
        }
    }

    private static void CleanFolderRecursive(string folderPath, TimeSpan threshold)
    {
        // Получаем список файлов и папок в текущей папке
        string[] files = Directory.GetFiles(folderPath);
        string[] directories = Directory.GetDirectories(folderPath);

        // Проверяем файлы
        foreach (string file in files)
        {
            // Проверяем, был ли файл изменен более threshold
            FileInfo fileInfo = new FileInfo(file);
            if (DateTime.Now - fileInfo.LastWriteTime > threshold)
            {
                // Удаляем файл
                File.Delete(file);
                Console.WriteLine($"Удален файл: {file}");
            }
        }

        // Проверяем папки
        foreach (string directory in directories)
        {
            // Проверяем, была ли папка изменена более threshold
            DirectoryInfo dirInfo = new DirectoryInfo(directory);
            if (DateTime.Now - dirInfo.LastWriteTime > threshold)
            {
                // Удаляем папку
                Directory.Delete(directory, true); // true - recursively
                Console.WriteLine($"Удалена папка: {directory}");
            }
            else
            {
                // Рекурсивно очищаем подпапки
                CleanFolderRecursive(directory, threshold);
            }
        }
    }
}