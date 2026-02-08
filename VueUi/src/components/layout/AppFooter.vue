<script setup lang="ts">
const currentYear = new Date().getFullYear()
</script>

<template>
  <footer class="gh-footer">
    <div class="gh-footer-inner">
      <div class="gh-footer-meta">
        <span>&copy; {{ currentYear }}</span>
        <span class="gh-footer-title">GitHub README Link Checker</span>
      </div>
      <div class="gh-footer-links">
        <a href="https://github.com/MrCull/GitHub-Repo-ReadMe-Dead-Link-Finder/issues" target="_blank" rel="noopener">Report issue</a>
        <a href="https://github.com/MrCull/GitHub-Repo-ReadMe-Dead-Link-Finder/discussions" target="_blank" rel="noopener">Discuss</a>
        <a href="https://github.com/MrCull/GitHub-Repo-ReadMe-Dead-Link-Finder" target="_blank" rel="noopener">Repository</a>
      </div>
    </div>
  </footer>
</template>

<style scoped>
.gh-footer {
  border-top: 1px solid var(--gh-border);
  background: var(--gh-surface);
  padding: 1.25rem 1rem;
  margin-top: 2.5rem;
}

.gh-footer-inner {
  max-width: 1140px;
  margin: 0 auto;
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  color: var(--gh-muted);
  font-size: 0.9rem;
  flex-wrap: wrap;
}

.gh-footer-links {
  display: flex;
  gap: 1rem;
  flex-wrap: wrap;
}

.gh-footer-links a {
  color: var(--gh-muted);
}

.gh-footer-links a:hover {
  color: var(--gh-accent);
  text-decoration: underline;
}

.gh-footer-meta {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.gh-footer-title {
  font-weight: 600;
}

@media (max-width: 540px) {
  .gh-footer-inner {
    flex-direction: column;
    text-align: center;
  }

  .gh-footer-links {
    justify-content: center;
  }
}
</style>
