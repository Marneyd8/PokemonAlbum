# Pokémon Album App

A Pokémon card management application built using the **MVVM** pattern in **WPF**. The app fetches Pokémon data from an external **API** and stores the Pokémon album data in **JSON** files for persistent storage.

## Features

- **MVVM Pattern**: The app follows the **Model-View-ViewModel (MVVM)** architecture for clean separation of concerns.
- **Persistent Storage**: Pokémon album data is saved in **JSON files** (`Album.json`, `PokemonNames.json`, `PokemonSets.json`).
- **API Integration**: Pokémon names and sets are fetched dynamically from the **Pokémon API**.
- **Card Management**: Users can add, remove, and view Pokémon cards in their album.
- **Filtering & Sorting**: Filter and sort Pokémon cards by various attributes (rarity, type, set, etc.).
- **Real-time Updates**: The Pokémon data is automatically updated by fetching the latest information from the API.

## Technologies

- **WPF**: For building the user interface.
- **MVVM**: To maintain separation of concerns.
- **JSON**: Used for persistent data storage (album data, Pokémon names, and sets).
- **Pokémon API**: For fetching Pokémon names and sets.
