# 📁 FileOrganizer

[![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![WPF](https://img.shields.io/badge/WPF-5C2D91?style=for-the-badge&logo=.net&logoColor=white)](https://docs.microsoft.com/en-us/dotnet/desktop/wpf/)
[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=for-the-badge)](https://opensource.org/licenses/MIT)

> **FileOrganizer** — это десктопное приложение на C# WPF, которое автоматически сортирует файлы в выбранной папке по категориям. Программа анализирует расширения файлов, создаёт соответствующие папки и перемещает файлы, поддерживая пользовательские настройки категорий через JSON-конфигурацию.

---

## 🚀 Основные возможности

- ✅ **Сортировка файлов** — автоматическое распределение файлов по категориям (например, `Images`, `Documents`, `Archives` и т.д.)
- ✅ **Пользовательские категории** — добавляйте, редактируйте или удаляйте категории через удобный интерфейс настроек
- ✅ **Сохранение настроек** — все категории хранятся в `categories.json`, который легко редактировать вручную
- ✅ **Запоминание последней папки** — при следующем запуске программа автоматически предлагает последнюю использованную папку
- ✅ **Логирование** — каждый сеанс сортировки сохраняется в `log.txt` с детальной информацией о перемещённых файлах и дате выполнения

---

## 🖼️ Скриншоты

| Главное окно | Окно настроек |
|--------------|---------------|
| *(вставьте сюда скриншот главного окна)* | *(вставьте сюда скриншот окна настроек)* |

---

## 📦 Установка

### Требования
- Windows 10 / 11
- [.NET 6.0+](https://dotnet.microsoft.com/en-us/download) (или .NET Core 3.1)

### Скачать
1. Перейдите в раздел [Releases](https://github.com/ваш_ник/FileOrganizer/releases)
2. Скачайте последнюю версию `FileOrganizer.zip`
3. Распакуйте архив в любую папку
4. Запустите `FileOrganizer.exe`

### Сборка из исходников
git clone https://github.com/ваш_ник/FileOrganizer.git
cd FileOrganizer
dotnet restore
dotnet build --configuration Release