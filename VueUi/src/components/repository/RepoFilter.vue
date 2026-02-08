<script setup lang="ts">
const props = defineProps<{
  modelValue: string
}>()

const emit = defineEmits<{
  'update:modelValue': [value: string]
}>()

function onInput(e: Event) {
  const target = e.target as HTMLInputElement
  emit('update:modelValue', target.value)
}
</script>

<template>
  <div class="repo-search-row">
    <input
      type="search"
      class="repo-search-input"
      placeholder="Find a repository..."
      aria-label="Find a repository"
      :value="modelValue"
      @input="onInput"
    />
  </div>
</template>

<style scoped>
.repo-search-row {
  margin-bottom: 8px;
}

.repo-search-input {
  width: 100%;
  height: 36px;
  border-radius: 6px;
  border: 1px solid #d0d7de;
  padding: 0 10px;
  font-size: 14px;
  font-family: inherit;
  background: #ffffff;
}

.repo-search-input:focus {
  outline: none;
  border-color: #0969da;
  box-shadow: 0 0 0 3px rgba(9, 105, 218, 0.15);
}

.repo-search-input::placeholder {
  color: #8b949e;
}
</style>
