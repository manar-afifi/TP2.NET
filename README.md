# Gauniv - Plateforme de Distribution de Jeux Vidéo

## 📌 Description du Projet

Gauniv est une plateforme de distribution de contenu pour les jeux vidéo, comprenant :

* Un backend en ASP.NET Core pour gérer les jeux, les utilisateurs et les transactions.

- Un client Windows (WPF/MAUI) permettant aux utilisateurs de parcourir, acheter, télécharger et lancer des jeux.

- Une API REST permettant aux clients externes de consulter la bibliothèque.

# 🏗️ Technologies Utilisées

## Backend (ASP.NET Core)

- ASP.NET Core Web API pour la gestion des jeux et des utilisateurs.

- Entity Framework Core pour l'interaction avec la base de données PostgreSQL.

- JWT Authentication pour l’authentification des utilisateurs.

- SignalR pour la gestion des statuts en temps réel des joueurs.

- Stockage externe des fichiers de jeux avec gestion du streaming.

- Gestion des rôles (Admin, Utilisateur).

## Frontend (MAUI/WPF/WinUI)

- MAUI pour une application multi-plateforme.

- Binding MVVM pour séparer la logique métier de l'interface.

- Navigation avec Shell.

- Affichage des jeux avec pagination et filtres.

- Gestion des téléchargements et de l’état des jeux.

- Connexion avec le backend via HTTPClient.

# 📂 Structure du Projet

Gauniv/
│── Backend/
│   ├── Controllers/
│   ├── Data/
│   ├── Dtos/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│
│── Client/
│   ├── Pages/
│   ├── ViewModel/
│   ├── Services/
│   ├── Models/
│   ├── AppShell.xaml
│   ├── MainPage.xaml

# 📡 Backend - API REST

## Endpoints Disponibles : 

📌 Authentification

POST /api/1.0.0/auth/login → Connexion utilisateur.

POST /api/1.0.0/auth/register → Inscription utilisateur.

📌 Gestion des Jeux

GET /api/1.0.0/games → Lister tous les jeux (filtrage, pagination inclus).

GET /api/1.0.0/games/{id} → Détails d’un jeu spécifique.

POST /api/1.0.0/games → Ajouter un jeu (Admin).

PUT /api/1.0.0/games/{id} → Modifier un jeu (Admin).

DELETE /api/1.0.0/games/{id} → Supprimer un jeu (Admin).

GET /api/1.0.0/games/download/{id} → Télécharger un jeu.

📌 Gestion des Utilisateurs

GET /api/1.0.0/users → Lister tous les utilisateurs (Admin).

GET /api/1.0.0/users/{id} → Détails d’un utilisateur.

PUT /api/1.0.0/users/{id} → Modifier un utilisateur.

DELETE /api/1.0.0/users/{id} → Supprimer un utilisateur (Admin).

POST /api/1.0.0/users/{userId}/games/{gameId} → Acheter un jeu.

📌 Statistiques

GET /api/1.0.0/stats → Nombre total de jeux, moyenne de jeux par utilisateur, temps de jeu moyen.

# Frontend - Application WPF/MAUI

📌 Fonctionnalités Principales

✅ Lister les jeux disponibles avec pagination et filtres.
✅ Voir les jeux possédés par l’utilisateur.
✅ Acheter, télécharger, lancer et supprimer un jeu.
✅ Affichage dynamique des boutons en fonction de l’état du jeu.
✅ Gestion du profil utilisateur (identifiants, dossier d’installation).
✅ Connexion et authentification.
✅ Interface moderne avec Shell pour la navigation.

📌 Pages Principales

GamesPage.xaml → Liste des jeux.

MyGamesPage.xaml → Liste des jeux possédés.

ProfilePage.xaml → Modification des informations utilisateur.

LoginPage.xaml → Connexion.

# 🚀 Déploiement & Exécution

🔧 Prérequis

.NET 9 installé.

PostgreSQL pour la base de données.

📄 Licence

Projet développé par Manar AFIFI et Yassine IJJA © 2025. Tous droits réservés.



