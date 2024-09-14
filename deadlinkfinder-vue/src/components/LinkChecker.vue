<template>
    <div class="link-checker">
      <div v-if="loading">
        <img src="@/assets/checking-icon.gif" alt="Checking" width="25" height="25">
        Checking links...
      </div>
      <div v-else-if="error">
        Error checking repo: {{ error }}
      </div>
      <div v-else>
        <div class="mb-2">
          <img :src="statusIcon" alt="Status" width="25" height="25">
          Bad[{{ badCount }}] Warning[{{ warningCount }}] Ok[{{ okCount }}]
        </div>
        <table class="table table-striped">
          <thead>
            <tr>
              <th>Status</th>
              <th>URL</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="link in links" :key="link.uriText">
              <td>
                <span :class="getStatusClass(link.httpStatusCode)">
                  {{ link.httpStatusCode }}
                </span>
              </td>
              <td>
                <a :href="link.uriText" target="_blank">{{ link.uriText }}</a>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </template>
  
  <script>
  import axios from 'axios'
  
  export default {
    name: 'LinkChecker',
    props: {
      repoUri: {
        type: String,
        required: true
      },
      branch: {
        type: String,
        required: true
      }
    },
    data() {
      return {
        loading: true,
        error: null,
        links: []
      }
    },
    computed: {
      badCount() {
        return this.links.filter(link => link.httpStatusCode >= 400).length
      },
      warningCount() {
        return this.links.filter(link => link.httpStatusCode >= 300 && link.httpStatusCode < 400).length
      },
      okCount() {
        return this.links.filter(link => link.httpStatusCode < 300).length
      },
      statusIcon() {
        if (this.badCount > 0) return require('@/assets/error-icon.png')
        if (this.warningCount > 0) return require('@/assets/warning-icon.jpg')
        return require('@/assets/ok-icon.png')
      }
    },
    mounted() {
      this.checkLinks()
    },
    methods: {
      async checkLinks() {
        try {
          const response = await axios.get('/api/CheckRepo', {
            params: {
              projectBaseUrl: this.repoUri,
              branch: this.branch
            }
          })
          this.links = response.data
        } catch (error) {
          this.error = error.message
        } finally {
          this.loading = false
        }
      },
      getStatusClass(statusCode) {
        if (statusCode >= 400) return 'text-danger'
        if (statusCode >= 300) return 'text-warning'
        return 'text-success'
      }
    }
  }
  </script>