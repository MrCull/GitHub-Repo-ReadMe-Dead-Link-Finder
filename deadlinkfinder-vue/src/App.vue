<template>
  <div id="app" class="container">
    <h1 class="my-4">GitHub "readme" bad link checker</h1>
    <RepoSearch @search="handleSearch" />
    <RepoList :repos="repos" />
  </div>
</template>

<script>
</script>