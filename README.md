# CatFactFetcher 🐱

Projekt zrealizowany w ramach zadania rekrutacyjnego do **Szkółki .NET (Netwise S.A.)**. 

Głównym celem aplikacji jest pobieranie losowych ciekawostek o kotach z zewnętrznego API (`https://catfact.ninja/fact`) oraz ich trwały zapis w formacie tekstowym, linia po linii.

Aplikacja jest w pełni zhostowana w chmurze **Microsoft Azure**:
* 🌐 **Frontend (Blazor WASM):** `https://orange-water-05813ae0f.3.azurestaticapps.net/` (Azure Static Web Apps)
* ⚡ **Backend (Azure Functions):** `https://catfactapp-bbd5afgbfpezbah0.polandcentral-01.azurewebsites.net/`
* ☁️ **Storage:** Azure Blob Storage
---

## 🏛️ Architektura i decyzje projektowe

Mimo że wymagania wstępne były stosunkowo proste, postawiłem na architekturę, która pozwala zaprezentować dobre praktyki inżynierii oprogramowania, elastyczność oraz łatwość testowania.

### 1. Vertical Slice Architecture (VSA)
Dla tak małej aplikacji tradycyjna architektura warstwowa  wprowadziłaby niepotrzebny narzut kodu i niepotrzebne abstrakcje. Zamiast dzielić kod na warstwy, zastosowałem **Vertical Slice Architecture**. 

Kod jest podzielony według funkcji biznesowych (*Features*):
* `FetchAndSaveCatFact` – pobranie faktu i jego zapis.
* `GetStoredCatFacts` – odczyt i przetworzenie historii faktów.

Każdy wycinek biznesowy zawiera wszystko, czego potrzebuje do działania, co czyni kod czytelnym i łatwym w utrzymaniu.

### 2. Abstrakcja Storage i Dependency Injection (DI)
Zgodnie z wymaganiami zadania, dane zapisywane są w formacie tekstowym `.txt`. Aby zaprezentować możliwości kontenera Dependency Injection w .NET, przygotowałem interfejs `IFileStorage` oraz **dwie niezależne implementacje**:
* **`LocalStorage`** – zapis i odczyt z lokalnego systemu plików (z użyciem `SemaphoreSlim` dla bezpieczeństwa wielowątkowego).
* **`AzureBlobStorage`** – zapis do chmury Azure z wykorzystaniem mechanizmu `AppendBlob`.

Dane są odczytywane asynchronicznie za pomocą **`Stream`** dla wydajności.

Przełączanie dostawcy odbywa się w konfiguracji (`appsettings.json`) poprzez wzorzec **Keyed Services** w DI.

### 3. Luźne powiązanie (Decoupled Presentation Layer)
Całość logiki biznesowej została zamknięta w bibliotece `CatFactApp.Core`. Dzięki temu warstwa prezentacji jest całkowicie odizolowana i wymienna. W projekcie zademonstrowałem to poprzez dwa różne punkty wejścia:
* **Minimal API (`CatFactApp.WebApi`)** – lekki endpoint HTTP.
* **Azure Functions (`CatFactApp.Functions`)** – podejście bezserwerowe (Serverless).

### 4. Optymalizacja i Caching
Aby uniknąć ciągłego otwierania pliku, pobierania go z Azure Blob Storage oraz ciągłego parsowania tekstu (`Split("|")`) przy każdym zapytaniu `GET`, wdrożyłem pamięć podręczną **`IMemoryCache`**. Po zapisie nowego faktu cache jest inwalidowany, co zapewnia wysokie bezpieczeństwo i natychmiastowy czas odpowiedzi.

---

## 🛠️ Technologie i narzędzia

* **Platforma:** .NET 10 / C#
* **Frameworki:** Azure Functions (Isolated Worker), ASP.NET Core Minimal API, Blazor WebAssembly
* **Storage:** Local File System, Azure Blob Storage SDK
* **Testowanie:** xUnit, Moq, FluentAssertions, Testcontainers (Azurite)
* **CI/CD:** GitHub Actions (automatyczne testy oraz deploy na Azure Functions i Azure Static Web Apps)

---

## 🧪 Testy i jakość kodu

Projekt zawiera zestaw testów automatycznych pisanych z asystą agentów do kodowania w projekcie `CatFactApp.Core.Tests`:
1. **Testy jednostkowe (Unit Tests):** Testują logikę biznesową handlerów (`FetchAndSaveCatFactHandler`, `GetStoredCatFactsHandler`), włączając w to parsowanie błędnych linii oraz inwalidację pamięci cache.
2. **Testy integracyjne (Integration Tests):** Testy dla `AzureBlobStorage` wykorzystują bibliotekę **Testcontainers** do uruchomienia prawdziwego emulatora **Azurite** w kontenerze Docker. Dzięki temu testujemy rzeczywistą komunikację z Azure Blob Storage bez konieczności mockowania SDK.

---
