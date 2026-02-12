# 🌟 Stargazing App

**Stargazing** is a modern digital companion designed for astronomy enthusiasts. Instead of checking multiple websites to plan a night under the stars, this app centralizes NASA's daily discoveries, live sky conditions, and a personal astronomical journal into one sleek, "Space-themed" dashboard.

---

## 🚀 Features

### 1. NASA Picture of the Day (APOD)
Every day starts with a new perspective of our universe. The app integrates directly with the **NASA Planetary API** to fetch the daily image, title, and a detailed scientific explanation.

### 2. Live Sky Dashboard
* **Moon Phase Tracking:** Real-time data on the current lunar cycle.
* **Sky Visibility:** Live weather integration to let you know if conditions are clear enough for stargazing.

### 3. Constellation Library & Favorites
A curated database of constellations (e.g., Orion, Ursa Major). 
* **Search & Filter:** Quickly find specific celestial bodies.
* **Observation Logger:** "Favorite" the constellations you have successfully spotted in the night sky.

### 4. Astronomy Event Calendar & Journal
* **Event Tracking:** Stay updated on meteor showers, eclipses, and planetary alignments.
* **Personal Notes:** A dedicated space to record your observations. Users can save memories of their sessions, stored locally and managed via **LINQ**.

---

## 🛠️ Technical Implementation

This project is built using **.NET MAUI** to demonstrate advanced C# development and UI design principles:

* **XAML & Custom Styling:** A hand-coded, dark-themed interface using custom `Styles` and `Frames` to create a premium "Space" aesthetic.
* **API Integration:** Consumption of RESTful services (NASA API & Weather APIs) using `HttpClient` and `Newtonsoft.Json` for dynamic data parsing.
* **Database Management:** Local data persistence for storing constellation facts and user-generated notes.
* **Architecture:** Implementation of Services and Models to ensure the code is scalable and maintainable.
* **Defensive Programming:** Robust error handling (Try-Catch blocks) to ensure the app remains functional even when offline.
* **LINQ:** Utilized for efficient searching and filtering within the constellation library.

---

## 📝 Project Identity
* **Developer:** Yelyzaveta Kareieva
* **Student Number:** S00267553
* **Platform:** .NET MAUI (C# / XAML)

---

## ⚙️ Setup
1. Clone this repository.
2. Open `StargazingApp.sln` in **Visual Studio 2022**.
3. Ensure you have the **.NET MAUI workload** installed.
4. Run the project on **Windows Machine** or an **Android Emulator**.