# GitHub Repo README.md Dead Link Finder

[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

> Automatically discover and report broken links in GitHub repository README files

## Overview

Many GitHub repositories contain README files with broken or outdated links. Repository owners and contributors are often unaware of these issues, as manually checking links across multiple repositories can be tedious and time-consuming.

This project provides both a **web interface** and a **console application** to automatically scan GitHub repositories and identify broken links in their README files.

**📢 Found this tool through an issue we created?** Please consider giving this repo a ⭐ or suggest improvements by opening an issue!

## Features

- 🔍 **Automated Scanning** - Searches GitHub for active repositories with README files
- 🌐 **Web Interface** - User-friendly web UI for on-demand repository checks
- 💻 **Console Application** - Batch processing tool for systematic scanning
- ⭐ **Smart Filtering** - Focuses on repositories with 100+ stars and recent commits
- 🔗 **Link Validation** - Comprehensive checking of all links in README files
- 📊 **Detailed Reports** - Saves findings with actionable information
- 🔐 **GitHub API Integration** - Optional Personal Access Token support for higher rate limits

## Web UI

Access the web interface at: **[https://GitHubReadMeChecker.com](https://GitHubReadMeChecker.com)**

## Console UI

The console application runs as a batch process and can find repositories with broken links in approximately 30 seconds.

![Console Demo](deadlink-finder-example.gif)

### How It Works

1. **Search Criteria**: Scans GitHub repositories with:
   - 100+ stars
   - Commits within the last hour
   
2. **Batch Processing**: Checks 25 repositories per run (configurable)

3. **Results**: Broken link information is saved to a file for manual review and reporting

## Installation

### Prerequisites

- .NET SDK (version specified in project files)
- GitHub account (optional, for Personal Access Token)

### Setup

```bash
# Clone the repository
git clone https://github.com/yourusername/GitHub-Repo-ReadMe-Dead-Link-Finder.git

# Navigate to the project directory
cd GitHub-Repo-ReadMe-Dead-Link-Finder

# Build the project
dotnet build

# Run the console application
dotnet run
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
2. Enter a repository URL or username
3. View the results and broken links found

### Console Application

```bash
dotnet run
```

Follow the on-screen prompts to:
- Enter your GitHub Personal Access Token (optional)
- Configure the number of repositories to check
- View real-time scanning results

## Contributing

Contributions are welcome! Here's how you can help:

1. 🐛 **Report Bugs** - Open an issue describing the problem
2. 💡 **Suggest Features** - Share your ideas for improvements
3. 🔧 **Submit Pull Requests** - Help fix bugs or add features
4. ⭐ **Star the Repo** - Show your support!

## Related Projects

Other excellent tools for checking broken links via GitHub Actions:

- [Markdown Link Check](https://github.com/marketplace/actions/markdown-link-check)
- [Link Checker](https://github.com/marketplace/actions/link-checker)

## License

This project is open source and available under the MIT License.

## Support

If you encounter any issues or have questions:

1. Check existing [Issues](https://github.com/yourusername/GitHub-Repo-ReadMe-Dead-Link-Finder/issues)
2. Open a new issue with detailed information
3. Provide relevant error messages and screenshots

---

**Made with ❤️ to help keep GitHub documentation clean and maintainable**
