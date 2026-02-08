<script setup lang="ts">
import { onMounted, ref, watch } from 'vue'
import AppHeader from '@/components/layout/AppHeader.vue'
import AppFooter from '@/components/layout/AppFooter.vue'
import SearchSection from '@/components/search/SearchSection.vue'
import LoadingState from '@/components/search/LoadingState.vue'
import ProfileHeader from '@/components/profile/ProfileHeader.vue'
import ResultsSection from '@/components/repository/ResultsSection.vue'
import { useGitHubApi } from '@/composables/useGitHubApi'

const {
  currentUserOrOrg,
  loading,
  error,
  filterTerm,
  sortBy,
  sortDir,
  repoLinkResults,
  checkingRepos,
  selectedTabs,
  visibleRepos,
  totalCount,
  showingCount,
  remainingCount,
  hasMore,
  hasResults,
  searchUser,
  checkRepo,
  loadMore,
  setFilter,
  setSort,
  setSortDir,
  setSelectedTab,
  getLastSearchValue,
} = useGitHubApi()

const searchSectionRef = ref<InstanceType<typeof SearchSection> | null>(null)

function handleSearch(value: string) {
  searchUser(value)
}

function handleHeaderSearch(value: string) {
  searchUser(value)
}

function handleRetry(repo: any) {
  checkRepo(repo, true)
}

// Auto-search on load if there's a last search saved
onMounted(() => {
  const lastSearch = getLastSearchValue()
  if (lastSearch) {
    if (searchSectionRef.value) {
      searchSectionRef.value.setInput(lastSearch)
    }
    searchUser(lastSearch)
  }
})
</script>

<template>
  <AppHeader
    :modelValue="currentUserOrOrg"
    @search="handleHeaderSearch"
  />

  <main class="gh-main">
    <div class="gh-page">
      <div class="gh-page-container">
        <!-- Profile Header -->
        <ProfileHeader
          v-if="hasResults || loading"
          :userOrOrg="currentUserOrOrg"
        />

        <!-- Search Section (initial state) -->
        <SearchSection
          v-if="!hasResults && !loading"
          ref="searchSectionRef"
          :loading="loading"
          :error="error"
          @search="handleSearch"
        />

        <!-- Loading State -->
        <LoadingState v-if="loading" />

        <!-- Error while results are shown -->
        <div v-if="error && hasResults" class="error-message">
          {{ error }}
        </div>

        <!-- Results Section -->
        <ResultsSection
          v-if="hasResults && !loading"
          :visibleRepos="visibleRepos"
          :totalCount="totalCount"
          :showingCount="showingCount"
          :remainingCount="remainingCount"
          :hasMore="hasMore"
          :filterTerm="filterTerm"
          :sortBy="sortBy"
          :sortDir="sortDir"
          :repoLinks="repoLinkResults"
          :checkingRepos="checkingRepos"
          :selectedTabs="selectedTabs"
          @update:filterTerm="setFilter"
          @update:sortBy="setSort"
          @update:sortDir="setSortDir"
          @check="checkRepo"
          @retry="handleRetry"
          @tabChange="setSelectedTab"
          @loadMore="loadMore"
        />
      </div>
    </div>
  </main>

  <AppFooter />
</template>

<style scoped>
.gh-page {
  padding: 32px 16px 48px;
}

.gh-page-container {
  max-width: 980px;
  margin: 0 auto;
  padding: 16px;
}

.error-message {
  color: var(--gh-danger);
  background: #ffebeb;
  padding: 0.75rem 0.9rem;
  border-radius: 6px;
  margin-bottom: 1rem;
  border: 1px solid #ffdede;
}

@media (max-width: 768px) {
  .gh-page {
    padding: 20px 12px 36px;
  }
}
</style>
