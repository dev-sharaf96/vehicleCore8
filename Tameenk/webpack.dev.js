const merge = require('webpack-merge');
const common = require('./webpack.common.js');
const { AngularCompilerPlugin } = require('@ngtools/webpack');
const path = require('path');

module.exports = merge(common, {
  mode: 'development',
  devtool: 'inline-source-map',
  devServer: {
    contentBase: './'
  },
  plugins: [
	new AngularCompilerPlugin({
        tsConfigPath: './tsconfig.json',
        entryModule: './src/app/app.module#AppModule',
        sourceMap: true
    })
  ],
  output: {
    filename: '[name].js',
    path: path.resolve(__dirname, 'dist')
}
});