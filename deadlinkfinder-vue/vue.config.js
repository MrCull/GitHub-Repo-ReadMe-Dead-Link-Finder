const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    proxy: {
      '/api': {
        target: 'http://localhost:5000', // Your ASP.NET Core backend URL
        changeOrigin: true
      }
    }
  }
})
