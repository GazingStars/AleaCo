
# AleaCo Mini-Blockchain (C#) — Transactions, Property Registry & WPF Demo

> Учебный проект на C#/.NET, который демонстрирует **минималистичный блокчейн‑движок**, правила валидации, простейший **Proof‑of‑Work**, **RSA‑подписи**, два прикладных домена (денежные **транзакции** и **реестр собственности**), а также WPF‑интерфейс. Проект написан для понимания принципов: как связываются блоки, как проверяются подписи и как из «сырых» строк делаются типизированные блоки.

---

## Основные идеи

- **BasicBlockchain** — простейшая цепочка `BasicBlock` с вычислением хэша блока по данным **+** хэш родителя.
- **IHashFunction**: две реализации — `SHA256Hash` (криптографическая) и `CRC32Hash` (некрипто, но быстрая для демонстраций).
- **Типизированный уровень**: `Blockchain<T>` оборачивает `BasicBlockchain`, сериализует/десериализует полезные данные через `System.Text.Json` и применяет **правила** (`IRule<T>`).
- **Подписи (RSA)**: `RSAEncryptor` генерирует ключи, подписывает и проверяет подписи. Любой блок домена можно сделать «подписанным» через `ISignedBlock<T>`.
- **Proof of Work**: примитивный `RuleOfApprovement<T>` + перебор `Nonce` в `ProofOfWork<T>` до тех пор, пока хэш блока проходит простую проверку.
- **Два домена**:
    1) **TransactionLogic** — переводы условных токенов с контролем баланса и проверки подписи.
    2) **Property** — регистрация и передача «документов» (артефактов собственности).
- **MemPool**: перед майнингом объекты попадают во временный пул, далее PoW собирает пачку (по 5 штук) в блок.
- **WPF**: демонстрационные страницы (`CongratulationsPage`, `HistoryOfTransPage`) для UI‑просмотра и навигации.
- **EmailService**: заготовка для отправки кода доступа через SMTP (Gmail).

---

## Проектные сущности (обзор)

### Низкий уровень (ядро цепочки)
- `record BasicBlock(List<string> Data, string Hash, string ParentHash)` — атом блока в сыром виде.
- `class BasicBlockchain : IEnumerable<BasicBlock>`
    - `BuildBlock(List<string> data)` — формирует **кандидат** на блок: считает хэш по `join(data) + tail.Hash`.
    - `AddBlock(BasicBlock block)` — валидирует `ParentHash` и пересчитывает ожидаемый хэш.
- `IHashFunction` → `SHA256Hash`, `CRC32Hash`.

### Типизированный уровень
- `record Block<T>(string Hash, string ParentHash, List<string> RawData, List<T?> Data)`
    - `FromBasicBlock(BasicBlock)` — парсит `RawData` через `JsonSerializer.Deserialize<T>` (кроме Genesis).
- `class Blockchain<T> : IEnumerable<Block<T>>`
    - Мост между сырыми блоками и типизированными, поддерживает правила `IRule<T>` (на стороне доменов).

### Криптография
- `IEncryptor` и `RSAEncryptor`
    - `GenerateKeys()` → `keyPair(PublicKey, PrivateKey)` с кастомным форматом параметров RSA.
    - `Sign(data, privateKey)` / `VerifySign(publicKey, data, sign)`.
    - `Encrypt/Decrypt` (демонстрационные).

### Правила/консенсус
- `IRule<T>` — интерфейс правил валидации.
- `RuleOfSugnature<TypedBlock, TypedData>` — проверка подписи блока (через `IEncryptor`).
- `RuleOfSufficiency` — контроль достаточности средств (демо‑баланс, см. замечания ниже).
- `RuleOfApprovement<T>` — PoW‑правило:
    - сейчас принимает хэш, начинающийся на `'1'` (сложность = 1 символ).
- `ProofOfWork<T>` — перебирает `Nonce` (через функцию `nextVariant`) до прохождения `RuleOfApprovement`.

### Домен «Транзакции»
- `record Transaction(string From, string To, long Amount)`
- `record TransactionBlock(Transaction Data, string Sign, int Nonce) : ISignedBlock<Transaction>`
- `class TransactionLogic`
    - MemPool `_MemPoolOfTB`, цепочка `_blocks` (`CRC32Hash` по умолчанию), набор `IRule`: подпись + баланс.
    - `PerformTransactionInMem(keyPair from, string toPublicKey, long amount)` — создать и положить в пул.
    - `PerformProofOfWork()` — когда в пуле ≥ 5, майнит и добавляет в цепочку.
    - `ReturnBalance(publicKey)` и `ReturnBalanceImMem(publicKey)` — баланс с учётом цепочки и пула.

### Домен «Собственность»
- `record PropertyTransfer(string Document, string From, string To)`
- `record PropertyBlock(PropertyTransfer Data, string Sign, int Nonce) : ISignedBlock<PropertyTransfer>`
- `class Property`
    - MemPool `_MemPoolOfPB`, цепочка `_blocks`, правило подписи.
    - `RegisterProperty(keyPair from, params string[] docs)` — регистрация документа на себя.
    - `PerformSellPropertyInMem(doc, fromKeyPair, toPublicKey)` — запрос передачи.
    - `AvailableProperty(publicKey)` и `AvailablePropertyInMem(publicKey)` — печать истории по ключу.
    - Внутренние проверки собственности, чтобы нельзя было продать то, чем не владеешь.

### Вспомогательное
- `EmailService` (SMTP Gmail; использовать **app password**).
- Учётные записи: `AccountLogic`, `SimpleAccount` (простая JSON‑персистентность в файлы).
- DTO для UI: `ListBoxData`, `Doc`.
- **WPF** страницы: `CongratulationsPage.xaml`, `HistoryOfTransPage.xaml` и т.п.

---

## Возможная структура репозитория

```
/AleaCo.Blockchain
  /BlockChain        # ядро: BasicBlock, BasicBlockchain, хеши
  /ProjectBlockchain # RSAEncryptor, ключи, подписи
  /Domain
    /Transactions    # TransactionLogic и типы
    /Property        # Property и типы
  /Consensus         # Rules + ProofOfWork
  /Email             # EmailService
  /Accounts          # AccountLogic, SimpleAccount
  /Wpf               # WPF UI (Pages, Images, XAML)
  Program.cs         # Консольные демонстрации/тесты
  AleaCo.sln
```

---

## Быстрый старт

### Требования
- **.NET 7/8 SDK** (Windows для WPF‑проекта).
- **NuGet**: `Force.Crc32` (для `CRC32Hash`).
- (Опционально) **Gmail App Password** для `EmailService`.

### Установка пакета CRC32
```bash
dotnet add package Force.Crc32
```

### Сборка и запуск
```bash
dotnet build
dotnet run --project ./AleaCo.Blockchain
```

Пример «сквозного» теста (фрагмент уже есть в `Program.cs`):
```csharp
var tool = new RSAEncryptor();
var user1 = tool.GenerateKeys();
var user2 = tool.GenerateKeys();

var tx = new TransactionLogic(tool);
tx.PerformTransactionInMem(user1, user2.PublicKey, 10);
tx.PerformTransactionInMem(user1, user2.PublicKey, 10);
tx.PerformTransactionInMem(user1, user2.PublicKey, 10);
tx.PerformTransactionInMem(user1, user2.PublicKey, 10);
tx.PerformTransactionInMem(user1, user2.PublicKey, 10); // >=5 → майнинг

Console.WriteLine($"Balance u1={tx.ReturnBalance(user1.PublicKey)}");
```

Аналогично для реестра собственности:
```csharp
var prop = new Property(tool);
prop.RegisterProperty(user1, "Helicopter: A-001", "Helicopter: A-002", "Helicopter: A-003", "H-A004", "H-A005", "H-A006");
prop.PerformSellPropertyInMem("H-A001", user1, user2.PublicKey); // перейдет в пул, затем в блок после майнинга
```

---

## Как это работает (коротко)

1. **Сырые данные → хэш**: `BuildBlock` конкатенирует `RawData` и хэш «хвоста», считает хэш (SHA‑256/CRC32).
2. **Валидатор**: `AddBlock` пересчитывает ожидаемый хэш и сравнивает.
3. **Типизация**: `Blockchain<T>` сериализует/парсит `RawData` в `T` через `System.Text.Json`.
4. **Правила**: домены перед добавлением в пул гоняют объект через `IRule<T>` (подпись, баланс, владение).
5. **Proof‑of‑Work**: для пачки из 5 объектов изменяем `Nonce` пока `RuleOfApprovement` не вернет `true`.
6. **Майнинг**: успешная пачка превращается в блок и добавляется в `BasicBlockchain`.

---

## ⚠Важно (безопасность и демо‑ограничения)

- **CRC32 не криптографичен** — используйте `SHA256Hash` для реальной устойчивости. В демо выбран CRC32 ради скорости.
- **RuleOfSufficiency** хранит баланс статически и «глобально», считает его при обходе блоков и пула — это **упрощение для демонстрации**. В реальном мире нужна UTXO‑модель/бухгалтерия по адресам + защита от гонок.
- **Genesis**: в `Block<T>.FromBasicBlock` специально возвращается `Data = null` для Блок‑0. Учитывайте это при обходах.
- **Подписи**: формат ключей — **кастомный**, это **учебный** сериализатор параметров RSA. Для продакшена используйте стандарты (PKCS#8/PEM).
- **SMTP (Gmail)**: не храните пароли в коде. Создайте **App Password** и читайте его из переменных окружения/секретов:
    - `GMAIL_USER`, `GMAIL_APP_PASSWORD`.
    - Подключите в `EmailService` чтение из `Environment.GetEnvironmentVariable(...)`.
- **Проверки и исключения**: проект бросает `InvalidOperationException`/`ApplicationException`/`CryptographicException` при нарушении условий — ловите и журналируйте в UI.
- **Proof‑of‑Work сложность = 1 символ** — чисто для демонстрации. Увеличьте проверку (например, 3–4 префиксных символа) для наглядного замедления.

---

## Демонстрационные сценарии в `Program.cs`

- **Test #1**: инициализация ключей/сущностей.
- **Test #2**: регистрация собственности.
- **Test #3–#5**: передачи собственности, проверка истории.
- **Test #6**: попытка «обойти» регистрацию — ожидаемые исключения.
- **Test #7**: вывод списка операций и статусов (цепочка/пул).

Выводы идут в консоль, что удобно для проверки логики без сложного UI.

---

## Полезные классы/API (сигнатуры)

```csharp
// Хеш‑функции
public interface IHashFunction { string GetHash(string data); }
public class SHA256Hash : IHashFunction { /* ... */ }
public class CRC32Hash : IHashFunction { /* ... */ }

// Блокчейн (низкий уровень)
public record class BasicBlock(List<string> Data, string Hash, string ParentHash);
public class BasicBlockchain : IEnumerable<BasicBlock> {
  public BasicBlockchain(IHashFunction hash);
  public BasicBlock BuildBlock(List<string> data);
  public void AddBlock(BasicBlock block);
}

// Блокчейн (типизированный уровень)
public record class Block<T>(string Hash, string ParentHash, List<string> RawData, List<T?> Data) {
  public static Block<T> FromBasicBlock(BasicBlock block);
}
public class Blockchain<T> : IEnumerable<Block<T>> {
  public Blockchain(BasicBlockchain inner, params IRule<T>[] rules);
  public Block<T> BuildBlock(List<T> data);
  public void AddBlock(Block<T> block);
}

// Правила и консенсус
public interface IRule<T> { bool Accomplishment(IEnumerable<Block<T>> previousBlocks, T next); }
public class RuleOfSugnature<TBlock, TData> : IRule<TBlock> where TBlock : ISignedBlock<TData> { /* ... */ }
public class RuleOfSufficiency : IRule<TransactionBlock> { /* ... */ }
public class RuleOfApprovement<T> { /* PoW check */ }
public class ProofOfWork<T> { /* перебор Nonce */ }

// Подписи (RSA)
public record keyPair(string PublicKey, string PrivateKey);
public interface IEncryptor {
  keyPair GenerateKeys();
  string Sign(string data, string privateKey);
  bool VerifySign(string publicKey, string data, string sign);
}

// Домен «Транзакции»
public record Transaction(string From, string To, long Amount);
public record TransactionBlock(Transaction Data, string Sign, int Nonce) : ISignedBlock<Transaction>;
public class TransactionLogic { /* MemPool, PoW, баланс */ }

// Домен «Собственность»
public record PropertyTransfer(string Document, string From, string To);
public record PropertyBlock(PropertyTransfer Data, string Sign, int Nonce) : ISignedBlock<PropertyTransfer>;
public class Property { /* регистрация/передача собственности */ }

// Email
public class EmailService { void SendAccessCodeEmail(string toEmail, string accessCode); }
```

---

## Дорожная карта (что улучшить дальше)

- [ ] Перейти на `SHA256Hash` по умолчанию во всех доменах.
- [ ] Увеличить сложность PoW и сделать её динамической.
- [ ] Экспорт/импорт ключей в стандартных форматах (PEM/PKCS#8).
- [ ] Нормальный менеджмент балансов (UTXO/счетная модель) + параллелизм.
- [ ] Подписи блока на уровне **всего** блока, а не только элементов данных.
- [ ] Журналирование (Serilog) + тесты (xUnit) для правил и потоков.
- [ ] Конфиги (appsettings.json) и секреты (User Secrets/ENV).
- [ ] UI: истории, визуализация цепочки, статус майнинга/пула.
- [ ] gRPC/WebAPI для сетевого обмена между нодами.

---

## Конфигурация `EmailService` (пример)

```csharp
var smtpClient = new SmtpClient("smtp.gmail.com") {
    Port = 587,
    Credentials = new NetworkCredential(
        Environment.GetEnvironmentVariable("GMAIL_USER"),
        Environment.GetEnvironmentVariable("GMAIL_APP_PASSWORD")),
    EnableSsl = true
};
```

В Windows PowerShell:
```powershell
setx GMAIL_USER "you@gmail.com"
setx GMAIL_APP_PASSWORD "your_16_char_app_password"
```
