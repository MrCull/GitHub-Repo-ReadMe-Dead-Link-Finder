<template>
  <section class="gh-loading">
    <div class="loading-content">
      <div class="loading-spinner"></div>
      <p class="loading-text">Fetching repositories...</p>
      <div class="loading-dots">
        <span class="dot"></span>
        <span class="dot"></span>
        <span class="dot"></span>
      </div>
    </div>
  </section>
</template>

<style scoped>
.gh-loading {
  text-align: center;
  padding: 3rem 1rem;
  color: var(--gh-muted);
}

.loading-content {
  display: flex;
  flex-direction: column;
  align-items: center;
}

.loading-spinner {
  display: inline-block;
  width: 40px;
  height: 40px;
  border: 3px solid var(--gh-border);
  border-top-color: var(--gh-accent);
  border-radius: 50%;
  animation: gh-spin 0.8s linear infinite;
  margin-bottom: 1rem;
}

.loading-text {
  font-size: 0.95rem;
  margin: 0 0 0.5rem;
}

.loading-dots {
  display: flex;
  gap: 4px;
}

.dot {
  width: 6px;
  height: 6px;
  border-radius: 50%;
  background: var(--gh-accent);
  animation: dot-pulse 1.4s infinite ease-in-out both;
}

.dot:nth-child(1) { animation-delay: 0s; }
.dot:nth-child(2) { animation-delay: 0.16s; }
.dot:nth-child(3) { animation-delay: 0.32s; }

@keyframes gh-spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes dot-pulse {
  0%, 80%, 100% { transform: scale(0.6); opacity: 0.4; }
  40% { transform: scale(1); opacity: 1; }
}
</style>
