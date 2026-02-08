<script setup lang="ts">
defineProps<{
  modelValue?: string
}>()

const emit = defineEmits<{
  search: [value: string]
}>()

function handleSubmit(e: Event) {
  e.preventDefault()
  const form = e.target as HTMLFormElement
  const input = form.querySelector('input') as HTMLInputElement
  const value = input.value.trim()
  if (value) {
    emit('search', value)
  }
}
</script>

<template>
  <header class="gh-global-header">
    <div class="gh-header-inner gh-header-compact">
      <a class="gh-logo" href="/" aria-label="GitHub README Link Checker">
        <span class="gh-logo-mark" aria-hidden="true">
          <svg viewBox="0 0 16 16" width="20" height="20" aria-hidden="true">
            <path fill="currentColor" d="M8 0C3.58 0 0 3.58 0 8c0 3.54 2.29 6.533 5.47 7.59.4.075.547-.173.547-.385 0-.19-.007-.693-.01-1.36-2.226.483-2.695-1.073-2.695-1.073-.364-.924-.89-1.17-.89-1.17-.727-.497.055-.487.055-.487.803.056 1.226.825 1.226.825.715 1.225 1.874.87 2.33.665.072-.518.28-.87.508-1.07-1.777-.202-3.644-.888-3.644-3.953 0-.873.312-1.587.823-2.147-.083-.202-.357-1.017.078-2.12 0 0 .67-.215 2.2.82a7.67 7.67 0 0 1 2-.27c.68.003 1.37.092 2 .27 1.53-1.035 2.2-.82 2.2-.82.435 1.103.162 1.918.08 2.12.513.56.822 1.274.822 2.147 0 3.073-1.87 3.748-3.65 3.947.287.246.543.734.543 1.48 0 1.068-.01 1.93-.01 2.193 0 .213.146.463.55.384A8.013 8.013 0 0 0 16 8c0-4.42-3.58-8-8-8Z" />
          </svg>
        </span>
        <span class="gh-logo-text">README Link Checker</span>
      </a>

      <form class="gh-header-search" @submit="handleSubmit" role="search">
        <input
          type="search"
          :value="modelValue"
          placeholder="Search for a GitHub user or organization..."
          aria-label="Search GitHub user or org"
          autocomplete="off"
          spellcheck="false"
        />
      </form>
    </div>
  </header>
</template>

<style scoped>
.gh-global-header {
  background: var(--gh-header-bg);
  color: var(--gh-header-text);
  position: sticky;
  top: 0;
  z-index: 1000;
  box-shadow: var(--shadow);
}

.gh-header-inner {
  max-width: 1140px;
  margin: 0 auto;
  padding: 0.5rem 1rem;
  display: grid;
  grid-template-columns: auto 1fr;
  align-items: center;
  gap: 1rem;
}

.gh-header-compact {
  display: flex;
  align-items: center;
  gap: 1rem;
  justify-content: space-between;
}

.gh-logo {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--gh-header-text);
  font-weight: 600;
  font-size: 0.95rem;
  text-decoration: none;
  white-space: nowrap;
}

.gh-logo:hover {
  color: var(--gh-header-text);
  opacity: 0.9;
  text-decoration: none;
}

.gh-logo-mark {
  display: inline-flex;
  align-items: center;
  justify-content: center;
}

.gh-header-search {
  width: 100%;
}

.gh-header-search input {
  width: 100%;
  background: #0d1117;
  border: 1px solid #30363d;
  color: var(--gh-header-text);
  padding: 0.4rem 0.65rem;
  border-radius: 6px;
  font-size: 0.9rem;
}

.gh-header-search input::placeholder {
  color: #8b949e;
}

.gh-header-search input:focus {
  outline: none;
  border-color: #58a6ff;
  box-shadow: 0 0 0 3px rgba(88, 166, 255, 0.3);
}

@media (max-width: 768px) {
  .gh-header-inner {
    grid-template-columns: 1fr auto;
  }
}

@media (max-width: 540px) {
  .gh-header-inner {
    flex-direction: column;
    align-items: flex-start;
  }
}
</style>
