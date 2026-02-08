<script setup lang="ts">
import { ref, onMounted, nextTick } from 'vue'

const props = defineProps<{
  loading?: boolean
  error?: string | null
}>()

const emit = defineEmits<{
  search: [value: string]
}>()

const inputValue = ref('')
const inputRef = ref<HTMLInputElement | null>(null)

function handleSubmit() {
  const value = inputValue.value.trim()
  if (value) {
    emit('search', value)
  }
}

function setInput(value: string) {
  inputValue.value = value
}

onMounted(() => {
  nextTick(() => {
    inputRef.value?.focus()
  })
})

defineExpose({ setInput })
</script>

<template>
  <section class="gh-search-card">
    <div class="gh-search-header">
      <div>
        <h1 class="main-title">GitHub README Link Checker</h1>
        <p class="subtitle">Check for broken links in your GitHub repository README files</p>
      </div>
    </div>
    <form class="gh-search-form" @submit.prevent="handleSubmit">
      <div class="input-group">
        <input
          ref="inputRef"
          v-model="inputValue"
          type="text"
          class="form-control gh-input"
          placeholder="GitHub username or org"
          aria-label="GitHub username or org"
          autocomplete="off"
          spellcheck="false"
          required
        />
        <button type="submit" class="gh-btn gh-btn-primary" :disabled="loading">
          <span v-if="!loading" class="btn-text">Check repos</span>
          <span v-else class="btn-spinner">
            <span class="mini-spinner"></span> Searching...
          </span>
        </button>
      </div>
      <Transition name="fade">
        <div v-if="error" class="error-message">
          <span class="error-icon">⚠️</span>
          {{ error }}
        </div>
      </Transition>
    </form>
  </section>
</template>

<style scoped>
.gh-search-card {
  background: var(--gh-surface);
  border: 1px solid var(--gh-border);
  border-radius: var(--radius);
  padding: 1.5rem;
  margin-bottom: 1.5rem;
  box-shadow: var(--shadow);
}

.gh-search-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  margin-bottom: 1rem;
}

.main-title {
  margin: 0 0 0.25rem 0;
  font-size: 1.6rem;
  font-weight: 700;
  color: var(--gh-text);
}

.subtitle {
  margin: 0;
  color: var(--gh-muted);
  font-size: 0.95rem;
}

.gh-search-form .input-group {
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.gh-input {
  flex: 1;
  padding: 0.7rem 0.85rem;
  border: 1px solid var(--gh-border);
  border-radius: 6px;
  font-size: 0.95rem;
  background: var(--gh-surface);
}

.gh-input:focus {
  outline: 2px solid rgba(9, 105, 218, 0.25);
  border-color: var(--gh-accent);
}

.gh-btn {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  gap: 0.4rem;
  padding: 0.55rem 1rem;
  font-size: 0.95rem;
  font-weight: 600;
  border-radius: 6px;
  border: 1px solid var(--gh-border);
  background: var(--gh-surface);
  color: var(--gh-text);
  cursor: pointer;
  transition: background 0.15s ease, border-color 0.15s ease;
  white-space: nowrap;
}

.gh-btn-primary {
  background: var(--gh-accent);
  color: #fff;
  border-color: var(--gh-accent);
}

.gh-btn-primary:hover:not(:disabled) {
  background: var(--gh-accent-hover);
}

.gh-btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error-message {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--gh-danger);
  background: #ffebeb;
  padding: 0.75rem 0.9rem;
  border-radius: 6px;
  margin-top: 0.75rem;
  border: 1px solid #ffdede;
}

.error-icon {
  font-size: 1.1rem;
  flex-shrink: 0;
}

@media (max-width: 540px) {
  .gh-search-form .input-group {
    flex-direction: column;
  }

  .gh-btn {
    width: 100%;
  }
}
</style>
