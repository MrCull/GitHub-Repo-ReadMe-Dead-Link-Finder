# GitHub Repo README.md Dead Link Finder

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

> Automatically discover and report broken links in GitHub repository README files

## Overview

Many GitHub repositories contain README files with broken or outdated links. Repository owners and contributors are often unaware of these issues, as manually checking links across multiple repositories can be tedious and time-consuming.

This project provides both a **web interface** and a **console application** to automatically scan GitHub repositories and identify broken links in their README files.

**📢 Found this tool through an issue we created?** Please consider giving this repo a ⭐ or suggest improvements by opening an issue!

## Features

- 🔍 **Automated Scanning** - Searches GitHub for active repositories with README files
- 🌐 **Web Interface** - Modern Vue 3 SPA with a GitHub-inspired UI for on-demand repository checks
- 💻 **Console Application** - Batch processing tool for systematic scanning
- ⭐ **Smart Filtering** - Filter and sort repositories by name, stars, or last pushed date
- 🔗 **Link Validation** - Comprehensive checking of all links in README files
- 📊 **Detailed Reports** - Categorised results for broken, warning, and working links per repository
- 🔐 **GitHub API Integration** - Optional Personal Access Token support for higher rate limits

## Web UI

Access the web interface at: **[https://GitHubReadMeChecker.com](https://GitHubReadMeChecker.com)**

The web UI is built with **Vue 3** and **TypeScript**, served by the ASP.NET Core backend. Enter a GitHub username or organisation to scan all their public repositories for broken README links.

### Web UI Features

- **GitHub-style design** — profile header with avatar, repository cards with metadata (language, license, stars, forks, topics)
- **Live link checking** — each repository's README links are automatically checked and categorised into Broken, Warning, and Working tabs
- **Filter & sort** — search repositories by name and sort by last pushed, name, or stars
- **Pagination** — load more repositories on demand
- **Error handling** — retry failed link checks with a single click
- **Accessible** — ARIA attributes, keyboard navigation, and focus-visible states
- **Animations** — smooth transitions for loading states, search results, and tab content

## Console UI

The console application runs as a batch process and can find repositories with broken links in approximately 30 seconds.

![Console Demo](deadlink-finder-example.gif)

### How It Works

1. **Search Criteria**: Scans GitHub repositories with:
   - 100+ stars
   - Commits within the last hour
   
2. **Batch Processing**: Checks 25 repositories per run (configurable)

3. **Results**: Broken link information is saved to a file for manual review and reporting

## Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | Vue 3, TypeScript, Vite |
| Backend | ASP.NET Core MVC (.NET) |
| API | ASP.NET Core controllers (`/Home/GetUserRepos`, `/Home/CheckRepo`) |
| Build Integration | MSBuild targets run `npm install` and `npm run build` in the `VueUi/` directory, outputting to `wwwroot/` |

## Project Structure

```
├── DeadLinkFinderWeb/          # ASP.NET Core backend
│   ├── Controllers/            # API endpoints
│   ├── wwwroot/                # Built Vue SPA output (generated)
│   └── Startup.cs              # Static file serving & SPA fallback routing
├── VueUi/                      # Vue 3 SPA frontend
│   └── src/
│       ├── components/
│       │   ├── layout/         # AppHeader, AppFooter
│       │   ├── search/         # SearchSection, LoadingState
│       │   ├── profile/        # ProfileHeader
│       │   ├── repository/     # RepoCard, RepoList, RepoFilter, SortDropdown, etc.
│       │   └── links/          # LinkTabs, LinkList, LinkItem
│       ├── composables/        # useGitHubApi (API calls & state management)
│       ├── types/              # TypeScript interfaces
│       └── utils/              # Formatters, language color mapping
├── DeadLinkFinder/             # Console application
└── README.md
```

## Configuration

### GitHub API Rate Limits

The GitHub API has rate limits to prevent abuse. Learn more: [GitHub Rate Limiting](https://developer.github.com/v3/rate_limit/)

**To increase your rate limit:**

1. Generate a Personal Access Token: [GitHub Settings → Tokens](https://github.com/settings/tokens)
2. Enter the token when prompted by the application

⚠️ **Note**: The Personal Access Token is optional. The program will work without it, but you may hit rate limits sooner.

## Usage

### Web Interface

1. Visit [GitHubReadMeChecker.com](https://GitHubReadMeChecker.com)
2. Enter a GitHub username or organisation
3. Browse repositories with filtering and sorting
4. View broken, warning, and working links for each repository

### Console Application

```bash
dotnet run
```

Follow the on-screen prompts to:
- Enter your GitHub Personal Access Token (optional)
- Configure the number of repositories to check
- View real-time scanning results

### Development

To run the Vue frontend in development mode with hot reload:

```bash
cd VueUi
npm install
npm run dev
```

To build the Vue frontend for production (output goes to `DeadLinkFinderWeb/wwwroot/`):

```bash
cd VueUi
npm run build
```

To run the full application (backend + built frontend):

```bash
cd DeadLinkFinderWeb
dotnet run
```

The MSBuild targets in `DeadLinkFinderWeb.csproj` will automatically build the Vue SPA when publishing in Release configuration.

## Contributing

Contributions are welcome! Here's how you can help:

1. 🐛 **Report Bugs** - Open an issue describing the problem
2. 💡 **Suggest Features** - Share your ideas for improvements
3. 🔧 **Submit Pull Requests** - Help fix bugs or add features
4. ⭐ **Star the Repo** - Show your support!

## Related Projects

Other excellent tools for checking broken links via GitHub Actions:

- [Markdown Link Check](https://github.com/marketplace/actions/markdown-link-check)

## License

This project is open source and available under the MIT License.

## Support

If you encounter any issues or have questions:

1. Check existing [Issues](https://github.com/MrCull/GitHub-Repo-ReadMe-Dead-Link-Finder/issues)
2. Open a new issue with detailed information
3. Provide relevant error messages and screenshots

---

**Made with ❤️ to help keep GitHub documentation clean and maintainable**
