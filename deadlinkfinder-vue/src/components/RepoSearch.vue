<template>
    <div class="repo-search">
      <h4>Repo Search</h4>
      <form @submit.prevent="searchRepos">
        <div class="form-group">
          <label for="singleRepoUri">Single Repo URI</label>
          <input
            id="singleRepoUri"
            v-model="searchForm.singleRepoUri"
            type="text"
            class="form-control"
            placeholder="e.g. https://github.com/MrCull/GitHub-Repo-ReadMe-Dead-Link-Finder"
          >
        </div>
        <div class="form-group">
          <label for="user">User</label>
          <input
            id="user"
            v-model="searchForm.user"
            type="text"
            class="form-control"
            placeholder="e.g. google"
          >
        </div>
        <div class="form-group">
          <label for="numberOfReposToSearchFor">Number of Repos to Search For</label>
          <input
            id="numberOfReposToSearchFor"
            v-model.number="searchForm.numberOfReposToSearchFor"
            type="number"
            class="form-control"
            placeholder="5"
          >
        </div>
        <button type="submit" class="btn btn-primary">Search</button>
      </form>
    </div>
  </template>
  
  <script>
  export default {
    name: 'RepoSearch',
    data() {
      return {
        searchForm: {
          singleRepoUri: '',
          user: '',
          numberOfReposToSearchFor: 5
        }
      }
    },
    methods: {
      searchRepos() {
        this.$emit('search', this.searchForm)
      }
    }
  }
  </script>