const { defineConfig } = require('@vue/cli-service')
module.exports = defineConfig({
  transpileDependencies: true,
  devServer: {
    proxy: {
      '/api': {
        target: 'https://localhost:44389/', // Your ASP.NET Core backend URL
        changeOrigin: true
      }
    }
  }
})
