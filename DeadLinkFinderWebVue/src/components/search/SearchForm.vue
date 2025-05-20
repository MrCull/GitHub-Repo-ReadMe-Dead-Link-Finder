<script setup lang="ts">
import { ref, watch } from 'vue'
import { useDebounce } from '@/composables/useDebounce'
import { Octokit } from '@octokit/rest'

interface User {
  login: string
  avatar_url: string
  type: 'User' | 'Organization'
}

const props = defineProps<{
  isLoading: boolean
}>()

const emit = defineEmits<{
  (e: 'search', username: string): void
  (e: 'clear'): void
}>()

const searchQuery = ref('')
const error = ref('')
const users = ref<User[]>([])
const selectedUser = ref<User | null>(null)
const isSearching = ref(false)

const debouncedSearch = useDebounce(async (value: string) => {
  if (!value) {
    users.value = []
    return
  }

  isSearching.value = true
  error.value = ''
  
  try {
    const octokit = new Octokit()
    const [usersResponse, orgsResponse] = await Promise.all([
      octokit.search.users({ q: value, per_page: 5 }),
      octokit.search.users({ q: `${value} type:org`, per_page: 5 })
    ])

    users.value = [
      ...usersResponse.data.items.map(user => ({
        login: user.login,
        avatar_url: user.avatar_url,
        type: 'User' as const
      })),
      ...orgsResponse.data.items.map(org => ({
        login: org.login,
        avatar_url: org.avatar_url,
        type: 'Organization' as const
      }))
    ].slice(0, 5)
  } catch (err) {
    error.value = 'Error searching users'
    console.error(err)
  } finally {
    isSearching.value = false
  }
}, 500)

watch(searchQuery, (newValue) => {
  error.value = ''
  if (newValue) {
    debouncedSearch(newValue)
  } else {
    users.value = []
  }
})

const selectUser = (user: User) => {
  selectedUser.value = user
  users.value = []
  searchQuery.value = user.login
  emit('search', user.login)
}

const clearSelection = () => {
  selectedUser.value = null
  searchQuery.value = ''
  users.value = []
  emit('clear')
}

const handleSearchInput = (event: Event) => {
  const value = (event.target as HTMLInputElement).value
  if (selectedUser.value && value !== selectedUser.value.login) {
    clearSelection()
  }
  error.value = ''
  if (value) {
    debouncedSearch(value)
  } else {
    users.value = []
  }
}
</script>

<template>
  <div class="search-form">
    <h2>GitHub Repository Link Checker</h2>
    
    <div class="search-container">
      <div class="search-input">
        <input 
          v-model="searchQuery"
          type="text"
          placeholder="Search GitHub users or organizations..."
          :disabled="isLoading"
          @input="handleSearchInput"
        />
        <button 
          v-if="selectedUser"
          @click="clearSelection"
          class="clear-button"
          :disabled="isLoading"
        >
          Clear
        </button>
      </div>

      <!-- User selection dropdown -->
      <div v-if="users.length > 0 && !selectedUser" class="users-dropdown">
        <div 
          v-for="user in users" 
          :key="user.login"
          class="user-item"
          @click="selectUser(user)"
        >
          <img :src="user.avatar_url" :alt="user.login" class="avatar" />
          <div class="user-info">
            <span class="username">{{ user.login }}</span>
            <span class="type">{{ user.type }}</span>
          </div>
        </div>
      </div>

      <!-- Selected user info -->
      <div v-if="selectedUser" class="selected-user">
        <img :src="selectedUser.avatar_url" :alt="selectedUser.login" class="avatar" />
        <div class="user-info">
          <span class="username">{{ selectedUser.login }}</span>
          <span class="type">{{ selectedUser.type }}</span>
        </div>
      </div>
    </div>

    <div v-if="error" class="error-message">
      {{ error }}
    </div>

    <div v-if="isSearching" class="loading-message">
      Searching...
    </div>
  </div>
</template>

<style scoped>
.search-form {
  width: 100%;
  max-width: 400px;
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.search-container {
  position: relative;
  width: 100%;
}

.search-input {
  display: flex;
  gap: 10px;
  width: 100%;
}

.search-input input {
  flex: 1;
  padding: 12px 16px;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  background-color: var(--input-bg);
  color: var(--text-color);
  font-size: 1.1em;
}

.search-button {
  padding: 12px 20px;
  border: none;
  border-radius: 4px;
  background-color: var(--primary-color);
  color: white;
  font-weight: 500;
  font-size: 1.1em;
  cursor: pointer;
  transition: background-color 0.2s;
}

.search-button:hover {
  background-color: var(--primary-color-dark);
}

.search-button:disabled {
  background-color: var(--disabled-color);
  cursor: not-allowed;
}

.clear-button {
  padding: 12px 20px;
  border: 1px solid var(--border-color);
  border-radius: 4px;
  background-color: var(--button-bg);
  color: var(--text-secondary);
  font-weight: 500;
  font-size: 1.1em;
  cursor: pointer;
  transition: all 0.2s;
}

.clear-button:hover {
  background-color: var(--button-hover-bg);
  color: var(--text-color);
}

.clear-button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.users-dropdown {
  position: absolute;
  top: 100%;
  left: 0;
  right: 0;
  background-color: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 4px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  z-index: 1000;
  max-height: 300px;
  overflow-y: auto;
  margin-top: 5px;
}

.user-item {
  display: flex;
  align-items: center;
  padding: 12px;
  cursor: pointer;
  transition: background-color 0.2s;
}

.user-item:hover {
  background-color: var(--button-hover-bg);
}

.selected-user {
  display: flex;
  align-items: center;
  padding: 12px;
  background-color: var(--card-bg);
  border: 1px solid var(--border-color);
  border-radius: 4px;
  margin-top: 10px;
}

.avatar {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  margin-right: 12px;
}

.user-info {
  display: flex;
  flex-direction: column;
}

.username {
  font-weight: 600;
  font-size: 1.1em;
  color: var(--text-color);
}

.type {
  font-size: 0.9em;
  color: var(--text-secondary);
}

.error-message {
  color: var(--error-color);
  margin-top: 10px;
  font-size: 1em;
}

.loading-message {
  color: var(--text-secondary);
  margin-top: 10px;
  font-size: 1em;
}

.loading-spinner {
  width: 20px;
  height: 20px;
  border: 2px solid var(--border-color);
  border-top-color: var(--primary-color);
  border-radius: 50%;
  animation: spin 1s linear infinite;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}
</style> 