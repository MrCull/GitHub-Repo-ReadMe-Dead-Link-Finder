<template>
  <div id="app" class="container">
    <h1 class="my-4">GitHub "readme" bad link checker</h1>
    <RepoSearch @search="handleSearch" />
    <RepoList :repos="repos" />
  </div>
</template>

<script>
import axios from 'axios'
import RepoSearch from './components/RepoSearch.vue'
import RepoList from './components/RepoList.vue'

export default {
  name: 'App',
  components: {
    RepoSearch,
    RepoList
  },
  data() {
    return {
      repos: []
    }
  },
  methods: {
    async handleSearch(searchForm) {
      try {
        const response = await axios.get('/api/Home/Search', {
          params: {
            SingleRepoUri: searchForm.singleRepoUri,
            User: searchForm.user,
            NumberOfReposToSearchFor: searchForm.numberOfReposToSearchFor
          }
        })
        
        this.repos = response.data.repoUrlsAndDefaultBranch.map(repo => ({
          repoUri: repo.repoUri,
          branch: repo.branch
        }))
      } catch (error) {
        console.error('Error fetching repos:', error)
        // You might want to add error handling here, such as displaying an error message to the user
      }
    }
  }
}
</script>