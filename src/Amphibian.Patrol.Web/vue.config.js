module.exports = {
  lintOnSave: false,
  runtimeCompiler: true,
  configureWebpack: {
    //Necessary to run npm link https://webpack.js.org/configuration/resolve/#resolve-symlinks
    resolve: {
       symlinks: false
    },
    devtool: 'eval-source-map'
  },
  transpileDependencies: [
    '@coreui/utils'
  ],
  publicPath: '.',
  devServer: {
    proxy: 'https://localhost:44348',
    publicPath: '/',
    public: 'fdc9cc9eee2d.ngrok.io'
  }
  // use this option for production linking
  // publicPath: process.env.NODE_ENV === 'production' ? '/vue/demo/3.1.0' : '/'
}
