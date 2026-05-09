# 🛡️ ASVS Security Auditor

Développé par **Soukaina Hessain** dans le cadre d'un projet de fin de module.

## Description
Application web ASP.NET Core MVC pour l'audit de sécurité des applications
web basé sur le standard OWASP ASVS (Application Security Verification Standard).

## Fonctionnalités
- 📥 Import du checklist OWASP ASVS via fichier CSV
- ✅ Évaluation des exigences (Valide / Non Valide / N/A / En attente)
- 📊 Rapport benchmark avec pourcentage de conformité
- 📈 Dashboard avec graphiques et score de sécurité
- 🤖 Assistant IA (DeepSeek) pour expliquer chaque exigence
- 🖨️ Export PDF des rapports
- 🔐 Authentification avec rôles Admin / Utilisateur
- 🌐 Interface en anglais et français

## Technologies utilisées
- ASP.NET Core 8 MVC
- Entity Framework Core + SQLite
- ASP.NET Core Identity
- Bootstrap 5 + Chart.js
- CsvHelper
- DeepSeek AI API

## Installation locale

```bash
git clone https://github.com/Soukaina260699/AVSSecurityAuditor.git
cd AVSSecurityAuditor
dotnet restore
dotnet run
```

Ouvrir : `https://localhost:5001`

## Compte administrateur par défaut
| Champ | Valeur |
|-------|--------|
| Email | admin@asvs.local |
| Mot de passe | Admin@123 |

## Importer le checklist ASVS
1. Télécharger le CSV officiel OWASP ASVS :
   👉 https://github.com/OWASP/ASVS/tree/master/4.0/docs_en
2. Se connecter en admin
3. Admin Panel → Import CSV → Uploader le fichier

## Déploiement
Déployé gratuitement sur Railway :
👉 https://votre-app.up.railway.app

## Auteure
**Soukaina Hessain**
GitHub : [@Soukaina260699](https://github.com/Soukaina260699)
