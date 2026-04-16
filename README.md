# 🌟 Stargazing App

**Stargazing** is a cross-platform .NET MAUI application designed for astronomy enthusiasts. It serves as a comprehensive digital companion, centralizing NASA's daily discoveries, real-time celestial data, and a personal observation journal into a high-performance, "Space-themed" dashboard.

---

## 🚀 Key Features

### 1. NASA Picture of the Day (APOD)
The home dashboard fetches daily imagery from the **NASA Planetary API**, providing users with high-resolution space photography and detailed scientific descriptions.

### 2. Live Sky Dashboard & GPS Integration
* **Location-Aware Data:** Utilizing the device's **GPS**, the app identifies the user's coordinates to provide accurate Sunrise/Sunset times and tonight's visibility conditions.
* **Tonight's Best:** A dynamic list of constellations currently visible from the user's specific location.
* **Moon Phase Tracking:** Real-time lunar cycle data with illumination percentages.

### 3. Constellation Gallery
A rich library of 88+ constellations.
* **Filtering:** Categorize by Northern/Southern hemisphere or view "Favorites."
* **Technical Details:** View abbreviation, best viewing months, and star counts.
* **Search:** Real-time filtering powered by **LINQ**.

### 4. Personal Astronomical Journal
A dedicated space to document observation sessions.
* **CRUD Operations:** Create, Read, Update, and Delete journal entries.
* **Metadata:** Automatically logs the date, time, and weather conditions of the entry.

### 5. Live Sky Map (Webview Integration)
An immersive, interactive sky map implemented via **Stellarium Web** in a custom WebView, allowing users to point their devices at the sky for real-time identification.

---

## 🛠️ Technical Stack

### Core Framework
* **.NET MAUI:** Cross-platform development for Windows and Android.
* **C# / XAML:** Advanced UI styling using `ControlTemplates`, `Styles`, and `Converters`.

### Data Management
* **SQLite:** Local persistence using two relational tables (`Constellations` and `JournalEntries`).
* **LINQ:** Used for complex querying and filtering of local collections.
* **MVVM Architecture:** Clean separation of concerns between Views, ViewModels, and Data Services.

### Connectivity & API
* **RESTful APIs:** Integrated NASA and Weather services using `HttpClient`.
* **JSON Parsing:** Handled via `Newtonsoft.Json` for robust data mapping.

### Quality Assurance
* **xUnit:** A dedicated test project (`StargazingApp.Test`) ensures logic reliability, specifically targeting search algorithms and data converters.

---

## 📁 Project Structure

```text
StargazingApp/
├── StargazingApp/           # Main Project
│   ├── Models/              # SQLite Tables & Data Objects
│   ├── ViewModels/          # Business logic for UI binding
│   ├── Views/               # XAML Pages (Home, Stars, Journal, SkyMap)
│   ├── Services/            # API & Database interaction
│   ├── Platforms/           # Platform-specific configurations (GPS permissions)
│   └── Resources/           # Custom Fonts, Icons, and Raw Assets
└── StargazingApp.Test/      # xUnit Test Suite
```

---

## ⚙️ Setup & Installation

1.  **Clone the Repo:**
    ```bash
    git clone https://github.com/Shprotya/StargazingApp.git
    ```
2.  **Environment:** Open `StargazingApp.sln` in **Visual Studio 2022**.
3.  **Workloads:** Ensure the **.NET Multi-platform App UI development** workload is installed.
4.  **Database:** On the first run, the SQLite database will automatically initialize and seed the constellation data.
5.  **Permissions:** Ensure Location Services are enabled on your emulator or physical device to use the "Live Sky" features.
6.  **API:** Don't forget to generate your own API key from the NASA Open API portal and update the `secrets.config` (or your relevant service file) to avoid rate limiting!

---

## 🧪 Testing
To run the test suite:
1. Open the **Test Explorer** in Visual Studio.
2. Click **Run All** to execute the xUnit tests.
*The current test suite focuses on validating the search logic and service response handling.*
